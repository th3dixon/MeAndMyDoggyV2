using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MeAndMyDog.WebApp.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time dashboard updates
    /// </summary>
    [Authorize]
    public class DashboardHub : Hub
    {
        private readonly ILogger<DashboardHub> _logger;

        public DashboardHub(ILogger<DashboardHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the dashboard hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their personal group for targeted updates
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} connected to dashboard hub", userId);
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the dashboard hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} disconnected from dashboard hub", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a specific dashboard room (for future features like shared dashboards)
        /// </summary>
        /// <param name="roomName">Name of the room to join</param>
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            _logger.LogInformation("Connection {ConnectionId} joined room {RoomName}", Context.ConnectionId, roomName);
        }

        /// <summary>
        /// Leave a specific dashboard room
        /// </summary>
        /// <param name="roomName">Name of the room to leave</param>
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            _logger.LogInformation("Connection {ConnectionId} left room {RoomName}", Context.ConnectionId, roomName);
        }

        /// <summary>
        /// Request a refresh of a specific widget
        /// </summary>
        /// <param name="widgetType">Type of widget to refresh</param>
        public async Task RequestWidgetRefresh(string widgetType)
        {
            _logger.LogInformation("User {UserId} requested refresh of widget {WidgetType}", Context.UserIdentifier, widgetType);
            
            // In a full implementation, this would trigger a refresh of the specific widget
            // For now, we'll just acknowledge the request
            await Clients.Caller.SendAsync("WidgetRefreshAcknowledged", widgetType);
        }
    }
}