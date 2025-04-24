using System.ComponentModel.DataAnnotations;

namespace Back.Models;

public class InvoicePayment
{   
    [Key]
    public int InvoiceNumber { get; set; } // PK y FK
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Invoice Invoice { get; set; } = null!;
}