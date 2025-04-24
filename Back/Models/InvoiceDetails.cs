namespace Back.Models;

public class InvoiceDetail
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    
    public int InvoiceNumber { get; set; }
    public Invoice Invoice { get; set; } = null!;
}