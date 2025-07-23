namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Data transfer object for the service provider dashboard overview
/// </summary>
public class ProviderDashboardDto
{
    /// <summary>
    /// Provider business information
    /// </summary>
    public ProviderInfoDto Provider { get; set; } = new();
    
    /// <summary>
    /// Current business metrics
    /// </summary>
    public BusinessMetricsDto Metrics { get; set; } = new();
    
    /// <summary>
    /// Today's scheduled bookings
    /// </summary>
    public List<TodaysBookingDto> TodaysBookings { get; set; } = new();
    
    /// <summary>
    /// Recent invoices
    /// </summary>
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    
    /// <summary>
    /// Quick statistics for the current period
    /// </summary>
    public QuickStatsDto QuickStats { get; set; } = new();
    
    /// <summary>
    /// Recent client messages
    /// </summary>
    public List<RecentMessageDto> RecentMessages { get; set; } = new();
    
    /// <summary>
    /// Revenue trend data for charts
    /// </summary>
    public RevenueTrendDto RevenueTrend { get; set; } = new();
}