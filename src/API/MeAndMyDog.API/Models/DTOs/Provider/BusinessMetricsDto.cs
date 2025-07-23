namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Current business metrics
/// </summary>
public class BusinessMetricsDto
{
    public decimal MonthlyRevenue { get; set; }
    public decimal RevenueGrowth { get; set; }
    public int ActiveBookings { get; set; }
    public int PendingBookings { get; set; }
    public int TotalClients { get; set; }
    public int NewClients { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
}