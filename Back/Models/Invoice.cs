using System.ComponentModel.DataAnnotations;

namespace Back.Models;

public class Invoice
{   
    [Key]
    public int InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string InvoiceStatus { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public int DaysToDue { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Relación con Customer
    public string CustomerRun { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    
    // Relación 1:1 con Payment
    public InvoicePayment? Payment { get; set; }
    
    // Relación 1:1 con CreditNote
    public List<CreditNote> CreditNotes { get; set; } = new();
    
    // Relación 1:N con Details (la única de este tipo)
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}