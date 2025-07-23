namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Recent invoice information
/// </summary>
public class RecentInvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
}