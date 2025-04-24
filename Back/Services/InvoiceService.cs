using Microsoft.EntityFrameworkCore;
using Back.Models;
using Back.Data;
using Back.DTOs;
using System.Text.RegularExpressions;

namespace Back.Services;

public class InvoiceService
{
    private readonly AppDbContext _context;

    public InvoiceService(AppDbContext context)
    {
        _context = context;
    }

public async Task<bool> ProcessInvoiceAsync(InvoiceDto dto)
{   
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // 1. Procesar cliente
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerRun == dto.Customer.Run);
        if (customer == null)
        {
            customer = new Customer
            {
                CustomerRun = dto.Customer.Run,
                CustomerName = dto.Customer.Name,
                CustomerEmail = dto.Customer.Email
            };
            await _context.Customers.AddAsync(customer);
        }

        // 2. Validación previa: verificar si total es consistente con subtotales
        var calculatedTotal = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
        if (Math.Abs(calculatedTotal - dto.TotalAmount) > 0.01m)
        {
            Console.WriteLine($"Factura {dto.InvoiceNumber} rechazada: total declarado {dto.TotalAmount} no coincide con subtotales {calculatedTotal}");
            return false; // No procesa esta factura
        }
        if (!IsValidRutFormat(dto.Customer.Run))
        {
            Console.WriteLine($"Factura {dto.InvoiceNumber} rechazada: RUT inválido ({dto.Customer.Run})");
            return false;
        }
        // 2. Crear factura
        var invoice = new Invoice
        {
            InvoiceNumber = dto.InvoiceNumber,
            InvoiceDate = dto.InvoiceDate,
            DaysToDue = dto.DaysToDue,
            PaymentStatus = dto.PaymentStatus,
            CustomerRun = customer.CustomerRun,
            TotalAmount = dto.TotalAmount
        };
        await _context.Invoices.AddAsync(invoice);

        // 3. Procesar detalles
        foreach (var item in dto.Items)
        {
            var detail = new InvoiceDetail
            {
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                InvoiceNumber = invoice.InvoiceNumber
            };
            await _context.InvoiceDetails.AddAsync(detail);
        }

        // 4. Procesar pago si es válido
        if (dto.Payment != null &&
            (dto.Payment.PaymentMethod != null || dto.Payment.PaymentDate != null))
        {
            var payment = new InvoicePayment
            {
                PaymentMethod = dto.Payment.PaymentMethod,
                PaymentDate = dto.Payment.PaymentDate,
                InvoiceNumber = invoice.InvoiceNumber
            };
            await _context.InvoicePayments.AddAsync(payment);
        }

        // 5. Procesar notas de crédito si existen
        List<decimal> creditNoteAmounts = new();
        if (dto.CreditNotes != null && dto.CreditNotes.Any())
        {
            foreach (var note in dto.CreditNotes)
            {
                creditNoteAmounts.Add(note.CreditNoteAmount);

                var creditNote = new CreditNote
                {
                    CreditNoteNumber = note.CreditNoteNumber,
                    CreditNoteDate = note.CreditNoteDate,
                    CreditNoteAmount = note.CreditNoteAmount,
                    InvoiceNumber = invoice.InvoiceNumber
                };
                await _context.CreditNotes.AddAsync(creditNote);
            }
        }

        // 6. Calcular y asignar estado de factura
        invoice.InvoiceStatus = CalculateInvoiceStatus(invoice.TotalAmount, creditNoteAmounts);

        // 7. Calcular y asignar estado de pago
        invoice.PaymentStatus = CalculatePaymentStatus(invoice, dto.Payment?.PaymentDate);

        // Guardar todo
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

public async Task<List<InvoiceSummaryDto>> GetInvoiceSummariesAsync()
{
    var query = await (
        from invoice in _context.Invoices
        join customer in _context.Customers on invoice.CustomerRun equals customer.CustomerRun
        join creditNote in _context.CreditNotes on invoice.InvoiceNumber equals creditNote.InvoiceNumber into creditNoteGroup
        select new InvoiceSummaryDto
        {
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerRun = invoice.CustomerRun,
            CustomerName = customer.CustomerName,
            DaysToDue = invoice.DaysToDue,
            InvoiceStatus = invoice.InvoiceStatus,
            PaymentStatus = invoice.PaymentStatus,
            TotalAmount = invoice.TotalAmount,
            InvoiceDate = invoice.InvoiceDate,
            CreditNoteAmount = creditNoteGroup.Sum(cn => (decimal?)cn.CreditNoteAmount) ?? 0
        }
    ).OrderBy(i => i.InvoiceNumber).ToListAsync();

    return query;
}
public async Task<bool> AddCreditNoteAsync(CreditNoteDto creditNoteDto)
{
    // Buscar la factura con sus notas de crédito asociadas
    var invoice = await _context.Invoices
        .Include(i => i.CreditNotes)
        .FirstOrDefaultAsync(i => i.InvoiceNumber == creditNoteDto.InvoiceNumber);

    if (invoice == null)
    {
        return false; // Factura no encontrada
    }

    var existingCreditNote = await _context.CreditNotes
        .FirstOrDefaultAsync(cn => cn.CreditNoteNumber == creditNoteDto.CreditNoteNumber);
    
    if (existingCreditNote != null)
    {
        throw new InvalidOperationException($"Ya existe una nota de crédito con el número {creditNoteDto.CreditNoteNumber}.");
    }
    
        if (creditNoteDto.CreditNoteAmount <= 0)
    {
        throw new InvalidOperationException("El monto de la nota de crédito debe ser positivo.");
    }


    // Obtener el total actual de notas de crédito
    decimal currentCreditNotesTotal = invoice.CreditNotes.Sum(cn => cn.CreditNoteAmount);

    // Validar que la nueva nota de crédito no exceda el total permitido
    if (currentCreditNotesTotal + creditNoteDto.CreditNoteAmount > invoice.TotalAmount)
    {
        throw new InvalidOperationException("El monto total de las notas de crédito excede el valor de la factura.");
    }

    // Crear y agregar la nueva nota de crédito
    var creditNote = new CreditNote
    {
        InvoiceNumber = creditNoteDto.InvoiceNumber,
        CreditNoteNumber = creditNoteDto.CreditNoteNumber,
        CreditNoteAmount = creditNoteDto.CreditNoteAmount,
        CreditNoteDate = DateTime.UtcNow
    };

    _context.CreditNotes.Add(creditNote);

    // Calcular nuevo total después de agregar la nueva nota
    decimal newTotalCreditNotesAmount = currentCreditNotesTotal + creditNoteDto.CreditNoteAmount;

    // Actualizar InvoiceStatus basado en las notas de crédito
    if (newTotalCreditNotesAmount == invoice.TotalAmount)
    {
        invoice.InvoiceStatus = "Cancelled";
    }
    else if (newTotalCreditNotesAmount > 0)
    {
        invoice.InvoiceStatus = "Partial";
    }
    else
    {
        invoice.InvoiceStatus = "Issued";
    }
    // Actualizar solo la factura (el PaymentStatus lo manejará el trigger)
    _context.Invoices.Update(invoice);

    // Guardar cambios en base de datos
    await _context.SaveChangesAsync();

    return true;
}


// Función de cálculo de estado de factura
private string CalculateInvoiceStatus(decimal invoiceTotal, List<decimal> creditNoteAmounts)
{
    var totalCreditNotes = creditNoteAmounts.Sum();

    if (totalCreditNotes == 0)
        return "Issued";
    else if (totalCreditNotes >= invoiceTotal)
        return "Cancelled";
    else
        return "Partial";
}

// Función de cálculo de estado de pago
private string CalculatePaymentStatus(Invoice invoice, DateTime? paymentDate)
{
    // Verificar si ya se ha registrado un pago
    if (paymentDate != null)
        return "Paid";

    // Verificar si la factura está vencida
    var dueDate = invoice.InvoiceDate.AddDays(invoice.DaysToDue);
    if (dueDate < DateTime.Now)
        return "Overdue";

    // Si el pago está pendiente dentro del plazo
    return "Pending";
}

private bool IsValidRutFormat(string rut)
{
    return Regex.IsMatch(rut, @"^\d{7,8}-[0-9kK]{1}$");
}

}