using System.ComponentModel.DataAnnotations;

namespace Back.Models;

public class Customer
{
    [Key]        
    public string CustomerRun { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
}