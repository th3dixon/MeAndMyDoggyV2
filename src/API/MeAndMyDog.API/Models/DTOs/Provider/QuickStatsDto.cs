namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Quick statistics for current period
/// </summary>
public class QuickStatsDto
{
    public decimal WeekRevenue { get; set; }
    public decimal PendingPayments { get; set; }
    public int CompletedServices { get; set; }
    public decimal ResponseRate { get; set; }
}