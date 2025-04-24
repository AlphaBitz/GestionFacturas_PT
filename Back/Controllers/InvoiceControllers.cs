using Microsoft.AspNetCore.Mvc;
using Back.DTOs;
using Back.Services;

namespace Back.Controllers;

[ApiController]
[Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

    [HttpPost("batch")]
    public async Task<ActionResult> ProcessInvoices([FromBody] InvoiceWrapperDto wrapper)
    {
        if (wrapper?.Invoices == null || !wrapper.Invoices.Any())
        {
            return BadRequest(new
            {
                error = "No se proporcionaron facturas para procesar"
            });
        }

        try
        {
            int processedCount = 0;
            List<int> failedInvoices = new();

            foreach (var invoiceDto in wrapper.Invoices)
            {
                if (invoiceDto.InvoiceNumber <= 0 || 
                    string.IsNullOrEmpty(invoiceDto.InvoiceStatus) ||
                    invoiceDto.Items == null || !invoiceDto.Items.Any())
                {
                    failedInvoices.Add(invoiceDto.InvoiceNumber);
                    continue;
                }

                bool success = await _invoiceService.ProcessInvoiceAsync(invoiceDto);
                if (success)
                    processedCount++;
                else
                    failedInvoices.Add(invoiceDto.InvoiceNumber);
            }

            int total = wrapper.Invoices.Count;

            if (processedCount == total)
            {
                return Created("", new
                {
                    message = "Todas las facturas fueron procesadas exitosamente.",
                    processedInvoices = processedCount
                });
            }
            else if (processedCount > 0)
            {
                return StatusCode(207, new
                {
                    message = "Algunas facturas fueron procesadas, otras fallaron.",
                    processedInvoices = processedCount,
                    failedInvoices = failedInvoices,
                    totalReceived = total
                });
            }
            else
            {
                return BadRequest(new
                {
                    error = "Ninguna factura fue procesada correctamente.",
                    failedInvoices = failedInvoices
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Error inesperado al procesar facturas.",
                details = ex.Message
            });
        }
    }

    [HttpGet("summaries")]
    public async Task<ActionResult<List<InvoiceSummaryDto>>> GetInvoiceSummaries()
    {
        try
        {
            var summaries = await _invoiceService.GetInvoiceSummariesAsync();
            return Ok(summaries);
        }
        catch (Exception ex)
        {
            // Podrías loguearlo con ILogger si estás usando logs
            return StatusCode(500, $"Ocurrió un error al obtener los datos: {ex.Message}");
        }
    }

    [HttpPost("credit-note")]
    public async Task<ActionResult> AddCreditNote([FromBody] CreditNoteDto creditNoteDto)
    {
        try
        {
            var result = await _invoiceService.AddCreditNoteAsync(creditNoteDto);
            if (!result)
            {
                return BadRequest(new { error = "Factura no encontrada o error al procesar la nota de crédito." });
            }

            return Ok(new { message = "Nota de crédito agregada y estado de pago actualizado exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al agregar la nota de crédito.", details = ex.Message });
        }
    }

}