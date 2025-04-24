public class InvoiceSummaryDto
{
    public int InvoiceNumber { get; set; }
    public required string CustomerRun { get; set; }
    public required string CustomerName { get; set; }
    public int DaysToDue { get; set; }
    public required string InvoiceStatus { get; set; }
    public required string PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal CreditNoteAmount { get; set; }
}