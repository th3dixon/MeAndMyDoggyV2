namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents calculated business metrics for a service provider
/// </summary>
public class ProviderBusinessMetrics
{
    /// <summary>
    /// Unique identifier for the metrics record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Year and month for these metrics (YYYY-MM format)
    /// </summary>
    public string MetricsPeriod { get; set; } = string.Empty;
    
    /// <summary>
    /// Total revenue for the period
    /// </summary>
    public decimal TotalRevenue { get; set; }
    
    /// <summary>
    /// Revenue growth percentage compared to previous period
    /// </summary>
    public decimal RevenueGrowthPercentage { get; set; }
    
    /// <summary>
    /// Number of active bookings in the period
    /// </summary>
    public int ActiveBookings { get; set; }
    
    /// <summary>
    /// Number of pending bookings awaiting confirmation
    /// </summary>
    public int PendingBookings { get; set; }
    
    /// <summary>
    /// Number of completed bookings in the period
    /// </summary>
    public int CompletedBookings { get; set; }
    
    /// <summary>
    /// Number of cancelled bookings in the period
    /// </summary>
    public int CancelledBookings { get; set; }
    
    /// <summary>
    /// Total number of unique clients served in the period
    /// </summary>
    public int TotalClients { get; set; }
    
    /// <summary>
    /// Number of new clients acquired in the period
    /// </summary>
    public int NewClients { get; set; }
    
    /// <summary>
    /// Number of returning clients in the period
    /// </summary>
    public int ReturningClients { get; set; }
    
    /// <summary>
    /// Average booking value for the period
    /// </summary>
    public decimal AverageBookingValue { get; set; }
    
    /// <summary>
    /// Average rating received in the period
    /// </summary>
    public decimal AverageRating { get; set; }
    
    /// <summary>
    /// Total number of reviews received in the period
    /// </summary>
    public int TotalReviews { get; set; }
    
    /// <summary>
    /// Average response time to inquiries in hours
    /// </summary>
    public decimal AverageResponseTimeHours { get; set; }
    
    /// <summary>
    /// Booking conversion rate (bookings/inquiries)
    /// </summary>
    public decimal BookingConversionRate { get; set; }
    
    /// <summary>
    /// Client retention rate as a percentage
    /// </summary>
    public decimal ClientRetentionRate { get; set; }
    
    /// <summary>
    /// No-show rate as a percentage
    /// </summary>
    public decimal NoShowRate { get; set; }
    
    /// <summary>
    /// Total amount of pending payments
    /// </summary>
    public decimal PendingPayments { get; set; }
    
    /// <summary>
    /// Total amount of overdue payments
    /// </summary>
    public decimal OverduePayments { get; set; }
    
    /// <summary>
    /// Number of invoices issued in the period
    /// </summary>
    public int InvoicesIssued { get; set; }
    
    /// <summary>
    /// Number of paid invoices in the period
    /// </summary>
    public int InvoicesPaid { get; set; }
    
    /// <summary>
    /// Average days to payment for invoices
    /// </summary>
    public decimal AverageDaysToPayment { get; set; }
    
    /// <summary>
    /// When these metrics were calculated
    /// </summary>
    public DateTimeOffset CalculatedAt { get; set; }
    
    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}