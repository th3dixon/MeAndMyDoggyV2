using Microsoft.AspNetCore.SignalR;

namespace MeAndMyDog.WebApp.Hubs;

/// <summary>
/// Implementation of provider dashboard notification service
/// </summary>
public class ProviderDashboardNotificationService : IProviderDashboardNotificationService
{
    private readonly IHubContext<ProviderDashboardHub> _hubContext;
    private readonly ILogger<ProviderDashboardNotificationService> _logger;

    public ProviderDashboardNotificationService(
        IHubContext<ProviderDashboardHub> hubContext,
        ILogger<ProviderDashboardNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendBusinessMetricsUpdateAsync(string providerId, object metricsData)
    {
        try
        {
            await _hubContext.Clients.Group($"Provider_{providerId}")
                .SendAsync("BusinessMetricsUpdate", metricsData);
            
            _logger.LogInformation("Sent business metrics update to provider {ProviderId}", providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending business metrics update to provider {ProviderId}", providerId);
        }
    }

    public async Task SendNewBookingNotificationAsync(string providerId, object bookingData)
    {
        try
        {
            await _hubContext.Clients.Group($"Provider_{providerId}")
                .SendAsync("NewBookingNotification", bookingData);
            
            _logger.LogInformation("Sent new booking notification to provider {ProviderId}", providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending new booking notification to provider {ProviderId}", providerId);
        }
    }

    public async Task SendInvoiceStatusUpdateAsync(string providerId, object invoiceData)
    {
        try
        {
            await _hubContext.Clients.Group($"Provider_{providerId}")
                .SendAsync("InvoiceStatusUpdate", invoiceData);
            
            _logger.LogInformation("Sent invoice status update to provider {ProviderId}", providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invoice status update to provider {ProviderId}", providerId);
        }
    }

    public async Task SendNewMessageNotificationAsync(string providerId, object messageData)
    {
        try
        {
            await _hubContext.Clients.Group($"Provider_{providerId}")
                .SendAsync("NewMessageNotification", messageData);
            
            _logger.LogInformation("Sent new message notification to provider {ProviderId}", providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending new message notification to provider {ProviderId}", providerId);
        }
    }

    public async Task SendDashboardUpdateAsync(string providerId, string updateType, object data)
    {
        try
        {
            await _hubContext.Clients.Group($"Provider_{providerId}")
                .SendAsync("DashboardUpdate", updateType, data);
            
            _logger.LogInformation("Sent dashboard update '{UpdateType}' to provider {ProviderId}", updateType, providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending dashboard update to provider {ProviderId}", providerId);
        }
    }
}