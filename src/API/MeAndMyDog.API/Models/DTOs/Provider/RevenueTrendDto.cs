namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Revenue trend data for charts
/// </summary>
public class RevenueTrendDto
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> Data { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public decimal AverageMonthlyGrowth { get; set; }
}