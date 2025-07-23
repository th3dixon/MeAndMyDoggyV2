namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Data transfer object for creating a new invoice
/// </summary>
public class CreateInvoiceDto
{
    /// <summary>
    /// Client ID for the invoice
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Related appointment ID (optional)
    /// </summary>
    public string? AppointmentId { get; set; }
    
    /// <summary>
    /// Service description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Base amount before VAT
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// VAT rate percentage
    /// </summary>
    public decimal VatRate { get; set; } = 20m;
    
    /// <summary>
    /// Due date for payment
    /// </summary>
    public DateTime DueDate { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Terms and conditions
    /// </summary>
    public string? Terms { get; set; }
}