using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MeAndMyDog.WebApp.Hubs;

/// <summary>
/// SignalR Hub for real-time provider dashboard updates
/// </summary>
[Authorize]
public class ProviderDashboardHub : Hub
{
    private readonly ILogger<ProviderDashboardHub> _logger;

    public ProviderDashboardHub(ILogger<ProviderDashboardHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a provider connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var providerId = Context.GetHttpContext()?.Request.Query["providerId"].ToString();
        
        if (!string.IsNullOrEmpty(providerId))
        {
            // Add to provider-specific group for targeted updates
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
            _logger.LogInformation("Provider {ProviderId} connected to dashboard hub", providerId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a provider disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var providerId = Context.GetHttpContext()?.Request.Query["providerId"].ToString();
        
        if (!string.IsNullOrEmpty(providerId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
            _logger.LogInformation("Provider {ProviderId} disconnected from dashboard hub", providerId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific provider group for updates
    /// </summary>
    public async Task JoinProviderGroup(string providerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
        _logger.LogInformation("Connection {ConnectionId} joined provider group {ProviderId}", Context.ConnectionId, providerId);
    }

    /// <summary>
    /// Leave a specific provider group
    /// </summary>
    public async Task LeaveProviderGroup(string providerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
        _logger.LogInformation("Connection {ConnectionId} left provider group {ProviderId}", Context.ConnectionId, providerId);
    }
}