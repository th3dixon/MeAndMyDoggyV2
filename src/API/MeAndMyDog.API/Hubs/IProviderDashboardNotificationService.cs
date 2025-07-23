namespace MeAndMyDog.API.Hubs;

/// <summary>
/// Service for sending real-time updates to provider dashboards
/// </summary>
public interface IProviderDashboardNotificationService
{
    /// <summary>
    /// Send business metrics update to a specific provider
    /// </summary>
    Task SendBusinessMetricsUpdateAsync(string providerId, object metricsData);
    
    /// <summary>
    /// Send new booking notification to a provider
    /// </summary>
    Task SendNewBookingNotificationAsync(string providerId, object bookingData);
    
    /// <summary>
    /// Send invoice status update to a provider
    /// </summary>
    Task SendInvoiceStatusUpdateAsync(string providerId, object invoiceData);
    
    /// <summary>
    /// Send new message notification to a provider
    /// </summary>
    Task SendNewMessageNotificationAsync(string providerId, object messageData);
    
    /// <summary>
    /// Send general dashboard update to a provider
    /// </summary>
    Task SendDashboardUpdateAsync(string providerId, string updateType, object data);
}