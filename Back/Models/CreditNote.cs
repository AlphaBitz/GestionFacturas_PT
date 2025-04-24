using System.ComponentModel.DataAnnotations;
namespace Back.Models;

public class CreditNote
{   
    [Key]    
    public int CreditNoteNumber { get; set; }       
    public int InvoiceNumber { get; set; } // PK y FK
    public DateTime CreditNoteDate { get; set; }
    public decimal CreditNoteAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Invoice Invoice { get; set; } = null!;
}