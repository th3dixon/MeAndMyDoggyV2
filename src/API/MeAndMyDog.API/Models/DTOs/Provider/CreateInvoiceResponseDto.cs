namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Response DTO for invoice creation
/// </summary>
public class CreateInvoiceResponseDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}