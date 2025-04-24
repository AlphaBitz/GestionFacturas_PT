using System.Text.Json.Serialization;

namespace Back.DTOs;

public class InvoiceWrapperDto
{
    [JsonPropertyName("invoices")]
    public List<InvoiceDto> Invoices { get; set; } = new List<InvoiceDto>();
}

public class InvoiceDto
{
    [JsonPropertyName("invoice_number")]
    public int InvoiceNumber { get; set; }

    [JsonPropertyName("invoice_date")]
    public DateTime InvoiceDate { get; set; }

    [JsonPropertyName("invoice_status")]
    public string InvoiceStatus { get; set; } = null!;

    [JsonPropertyName("days_to_due")]
    public int DaysToDue { get; set; }

    [JsonPropertyName("total_amount")]
    public int TotalAmount { get; set; }    

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = null!;

    // Datos del cliente (ahora directos en el DTO)
    [JsonPropertyName("customer")]
    public CustomerDataDto Customer { get; set; } = null!;

    [JsonPropertyName("invoice_detail")]
    public List<InvoiceDetailDto> Items { get; set; } = new List<InvoiceDetailDto>();

    [JsonPropertyName("invoice_payment")]
    public InvoicePaymentDto? Payment { get; set; }

[JsonPropertyName("invoice_credit_note")]
public List<CreditNoteDto>? CreditNotes { get; set; }

}

public class CustomerDataDto
{
    [JsonPropertyName("customer_run")]
    public string Run { get; set; } = null!;

    [JsonPropertyName("customer_name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("customer_email")]
    public string Email { get; set; } = null!;
}

public class InvoiceDetailDto
{
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = null!;

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}

public class InvoicePaymentDto
{
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    [JsonPropertyName("payment_date")]
    public DateTime? PaymentDate { get; set; }
}

public class CreditNoteDto
{
    [JsonPropertyName("credit_note_number")]
    public int CreditNoteNumber { get; set; }

    [JsonPropertyName("credit_note_date")]
    public DateTime CreditNoteDate { get; set; }

    [JsonPropertyName("credit_note_amount")]
    public decimal CreditNoteAmount { get; set; }
    
    [JsonPropertyName("invoice_number")]
    public int InvoiceNumber { get; set; }    
}