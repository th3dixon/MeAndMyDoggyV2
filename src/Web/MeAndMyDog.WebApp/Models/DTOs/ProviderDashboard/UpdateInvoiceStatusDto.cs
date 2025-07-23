namespace MeAndMyDog.WebApp.Models.DTOs.ProviderDashboard;

/// <summary>
/// DTO for updating invoice status
/// </summary>
public class UpdateInvoiceStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
}