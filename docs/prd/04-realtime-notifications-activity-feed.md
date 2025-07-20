# Story 4: Real-time Notifications & Activity Feed

## Overview
This section defines the requirements for implementing real-time dashboard updates using the existing SignalR infrastructure to provide immediate notifications for booking changes, messages, and system activities while enhancing the current notification framework without creating conflicts.

## Story Definition
**As a pet owner,**  
**I want my dashboard widgets to update automatically when important information changes,**  
**so that I always have current information about my bookings, messages, and notifications without manually refreshing.**

## Technical Context

### Existing Real-time Infrastructure
- **SignalR Implementation**: Established hubs for messaging and notifications
- **Connection Management**: User-specific connection handling with authentication
- **Message Types**: UnifiedMessage system with multiple communication channels
- **Notification System**: Email, push notification, and in-app notification support
- **Performance**: Current SignalR optimization patterns and connection pooling

### Integration Requirements
- Enhance existing SignalR hubs without breaking current messaging functionality
- Implement dashboard-specific real-time channels
- Maintain existing notification preferences and delivery mechanisms
- Optimize real-time update performance for multiple concurrent widgets

## Functional Requirements

### FR4.1: Widget Real-time Updates
- **Requirement**: Dashboard widgets receive automatic updates for relevant data changes
- **Integration Point**: Existing SignalR hubs and notification system
- **Implementation**: Extend current SignalR implementation with dashboard-specific channels
- **Success Criteria**: Widgets update within 2 seconds of data changes without manual refresh

### FR4.2: Notification Aggregation and Prioritization
- **Requirement**: Smart notification system that reduces noise while ensuring important updates reach users
- **Integration Point**: Current notification preferences and delivery channels
- **Implementation**: Intelligent notification filtering and batching system
- **Success Criteria**: Notification volume reduced by 40% while maintaining 100% delivery of critical updates

### FR4.3: Activity Feed Real-time Streaming
- **Requirement**: Live activity feed showing real-time events across user's pet care ecosystem
- **Integration Point**: Existing activity logging and user interaction tracking
- **Implementation**: Real-time event streaming with intelligent filtering and aggregation
- **Success Criteria**: Activity feed updates in real-time with sub-second latency

### FR4.4: Cross-Device Synchronization
- **Requirement**: Real-time updates sync across all user devices and sessions
- **Integration Point**: Existing user session management and authentication
- **Implementation**: Multi-device SignalR connection management with synchronized state
- **Success Criteria**: Updates appear simultaneously across all active user sessions

## Real-time Update Specifications

### Dashboard SignalR Hub Extension
```csharp
// Dashboard Hub extending existing functionality
[Authorize]
public class DashboardHub : Hub
{
    private readonly IUserConnectionManager _connectionManager;
    private readonly INotificationService _notificationService;
    
    public async Task JoinDashboardGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"dashboard_{userId}");
        await _connectionManager.RegisterDashboardConnectionAsync(userId, Context.ConnectionId);
    }
    
    public async Task LeaveDashboardGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"dashboard_{userId}");
        await _connectionManager.UnregisterDashboardConnectionAsync(userId, Context.ConnectionId);
    }
    
    public async Task SubscribeToWidget(string widgetType, string userId)
    {
        var channelName = $"widget_{widgetType}_{userId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await _connectionManager.UnregisterDashboardConnectionAsync(userId, Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}

// Real-time update service
public interface IDashboardUpdateService
{
    Task SendWidgetUpdateAsync(string userId, string widgetType, object data);
    Task SendNotificationAsync(string userId, DashboardNotification notification);
    Task SendActivityUpdateAsync(string userId, ActivityEvent activity);
    Task BroadcastSystemUpdateAsync(SystemUpdate update);
}

public class DashboardUpdateService : IDashboardUpdateService
{
    private readonly IHubContext<DashboardHub> _dashboardHub;
    private readonly INotificationAggregator _notificationAggregator;
    
    public async Task SendWidgetUpdateAsync(string userId, string widgetType, object data)
    {
        var updateMessage = new WidgetUpdate
        {
            WidgetType = widgetType,
            Data = data,
            Timestamp = DateTime.UtcNow,
            UserId = userId
        };
        
        await _dashboardHub.Clients.Group($"widget_{widgetType}_{userId}")
            .SendAsync("WidgetDataUpdate", updateMessage);
    }
    
    public async Task SendNotificationAsync(string userId, DashboardNotification notification)
    {
        // Check if notification should be aggregated
        var aggregatedNotification = await _notificationAggregator
            .ProcessNotificationAsync(userId, notification);
        
        if (aggregatedNotification != null)
        {
            await _dashboardHub.Clients.Group($"dashboard_{userId}")
                .SendAsync("NotificationUpdate", aggregatedNotification);
        }
    }
}
```

### Notification Aggregation System
```csharp
// Notification aggregation and prioritization
public interface INotificationAggregator
{
    Task<AggregatedNotification> ProcessNotificationAsync(string userId, DashboardNotification notification);
    Task<List<AggregatedNotification>> GetPendingNotificationsAsync(string userId);
    Task MarkNotificationAsSeenAsync(string userId, string notificationId);
}

public class NotificationAggregator : INotificationAggregator
{
    private readonly IMemoryCache _cache;
    private readonly INotificationPreferencesService _preferencesService;
    
    public async Task<AggregatedNotification> ProcessNotificationAsync(string userId, DashboardNotification notification)
    {
        var userPreferences = await _preferencesService.GetPreferencesAsync(userId);
        
        // Check if notification should be filtered
        if (!ShouldNotify(notification, userPreferences))
        {
            return null;
        }
        
        // Check for aggregation opportunities
        var cacheKey = $"notifications_{userId}";
        var pendingNotifications = _cache.Get<List<DashboardNotification>>(cacheKey) ?? new List<DashboardNotification>();
        
        // Aggregate similar notifications
        var existingSimilar = pendingNotifications
            .FirstOrDefault(n => CanAggregate(n, notification));
        
        if (existingSimilar != null)
        {
            return CreateAggregatedNotification(existingSimilar, notification);
        }
        
        // Check if notification is critical (immediate delivery)
        if (notification.Priority == NotificationPriority.Critical)
        {
            return new AggregatedNotification
            {
                Notifications = new[] { notification },
                DeliveryMode = DeliveryMode.Immediate,
                Priority = NotificationPriority.Critical
            };
        }
        
        // Add to pending for batching
        pendingNotifications.Add(notification);
        _cache.Set(cacheKey, pendingNotifications, TimeSpan.FromMinutes(5));
        
        return null; // Will be delivered in next batch
    }
    
    private bool CanAggregate(DashboardNotification existing, DashboardNotification incoming)
    {
        return existing.Type == incoming.Type &&
               existing.EntityId == incoming.EntityId &&
               (DateTime.UtcNow - existing.CreatedAt).TotalMinutes < 10;
    }
}
```

### Frontend Real-time Integration
```vue
<!-- Real-time Dashboard Component -->
<template>
  <div class="dashboard-realtime">
    <!-- Connection Status Indicator -->
    <div class="connection-status" :class="connectionStatusClass">
      <div class="status-indicator"></div>
      <span class="status-text">{{ connectionStatusText }}</span>
    </div>
    
    <!-- Real-time Notifications Panel -->
    <NotificationPanel
      v-if="showNotifications"
      :notifications="realtimeNotifications"
      @dismiss="dismissNotification"
      @mark-read="markNotificationRead"
    />
    
    <!-- Activity Feed Widget -->
    <ActivityFeedWidget
      :activities="realtimeActivities"
      :loading="activitiesLoading"
      @load-more="loadMoreActivities"
    />
    
    <!-- Real-time Widget Updates -->
    <div class="widgets-container">
      <component
        v-for="widget in widgets"
        :key="widget.id"
        :is="widget.component"
        :data="getWidgetData(widget.id)"
        :last-updated="getWidgetLastUpdated(widget.id)"
        @refresh="refreshWidget"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useSignalR } from '@/composables/useSignalR';
import { useNotifications } from '@/composables/useNotifications';
import { useAuth } from '@/composables/useAuth';

const { user } = useAuth();
const { connection, isConnected, connect, disconnect } = useSignalR();
const { notifications, addNotification, removeNotification } = useNotifications();

const realtimeNotifications = ref<DashboardNotification[]>([]);
const realtimeActivities = ref<ActivityEvent[]>([]);
const widgetData = ref(new Map<string, any>());
const widgetLastUpdated = ref(new Map<string, Date>());
const activitiesLoading = ref(false);

const connectionStatusClass = computed(() => ({
  'connected': isConnected.value,
  'disconnected': !isConnected.value,
  'reconnecting': connection.value?.state === 'Reconnecting'
}));

const connectionStatusText = computed(() => {
  if (isConnected.value) return 'Connected';
  if (connection.value?.state === 'Reconnecting') return 'Reconnecting...';
  return 'Disconnected';
});

const showNotifications = computed(() => realtimeNotifications.value.length > 0);

// Setup SignalR event handlers
const setupSignalRHandlers = () => {
  if (!connection.value) return;
  
  // Widget data updates
  connection.value.on('WidgetDataUpdate', (update: WidgetUpdate) => {
    widgetData.value.set(update.WidgetType, update.Data);
    widgetLastUpdated.value.set(update.WidgetType, new Date(update.Timestamp));
  });
  
  // Notification updates
  connection.value.on('NotificationUpdate', (notification: AggregatedNotification) => {
    realtimeNotifications.value.unshift(notification);
    
    // Limit notification display count
    if (realtimeNotifications.value.length > 10) {
      realtimeNotifications.value = realtimeNotifications.value.slice(0, 10);
    }
    
    // Show browser notification if page is not visible
    if (document.hidden && notification.Priority >= NotificationPriority.High) {
      showBrowserNotification(notification);
    }
  });
  
  // Activity feed updates
  connection.value.on('ActivityUpdate', (activity: ActivityEvent) => {
    realtimeActivities.value.unshift(activity);
    
    // Limit activity feed size
    if (realtimeActivities.value.length > 50) {
      realtimeActivities.value = realtimeActivities.value.slice(0, 50);
    }
  });
  
  // System updates
  connection.value.on('SystemUpdate', (update: SystemUpdate) => {
    handleSystemUpdate(update);
  });
};

const joinDashboardGroups = async () => {
  if (!connection.value || !user.value) return;
  
  try {
    await connection.value.invoke('JoinDashboardGroup', user.value.id);
    
    // Subscribe to specific widget channels
    const activeWidgets = ['appointments', 'messages', 'health', 'activity'];
    for (const widgetType of activeWidgets) {
      await connection.value.invoke('SubscribeToWidget', widgetType, user.value.id);
    }
  } catch (error) {
    console.error('Failed to join dashboard groups:', error);
  }
};

const getWidgetData = (widgetId: string) => {
  return widgetData.value.get(widgetId) || null;
};

const getWidgetLastUpdated = (widgetId: string) => {
  return widgetLastUpdated.value.get(widgetId) || null;
};

const dismissNotification = (notificationId: string) => {
  realtimeNotifications.value = realtimeNotifications.value
    .filter(n => n.id !== notificationId);
};

const markNotificationRead = async (notificationId: string) => {
  try {
    await fetch(`/api/v1/notifications/${notificationId}/read`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' }
    });
    
    dismissNotification(notificationId);
  } catch (error) {
    console.error('Failed to mark notification as read:', error);
  }
};

const refreshWidget = async (widgetId: string) => {
  try {
    const response = await fetch(`/api/v1/dashboard/widgets/${widgetId}/data`);
    const data = await response.json();
    
    widgetData.value.set(widgetId, data);
    widgetLastUpdated.value.set(widgetId, new Date());
  } catch (error) {
    console.error(`Failed to refresh widget ${widgetId}:`, error);
  }
};

const showBrowserNotification = (notification: AggregatedNotification) => {
  if (Notification.permission === 'granted') {
    new Notification(notification.title, {
      body: notification.message,
      icon: '/images/logo.png',
      tag: notification.id
    });
  }
};

const handleSystemUpdate = (update: SystemUpdate) => {
  switch (update.type) {
    case 'maintenance':
      // Show maintenance notice
      break;
    case 'feature':
      // Show new feature announcement
      break;
    case 'security':
      // Handle security updates
      break;
  }
};

onMounted(async () => {
  await connect();
  setupSignalRHandlers();
  await joinDashboardGroups();
  
  // Request browser notification permission
  if (Notification.permission === 'default') {
    await Notification.requestPermission();
  }
});

onUnmounted(async () => {
  if (connection.value && user.value) {
    await connection.value.invoke('LeaveDashboardGroup', user.value.id);
  }
  await disconnect();
});
</script>

<style scoped>
.connection-status {
  @apply flex items-center gap-2 px-3 py-1 rounded-full text-sm;
}

.connection-status.connected {
  @apply bg-green-100 text-green-800;
}

.connection-status.disconnected {
  @apply bg-red-100 text-red-800;
}

.connection-status.reconnecting {
  @apply bg-yellow-100 text-yellow-800;
}

.status-indicator {
  @apply w-2 h-2 rounded-full;
}

.connected .status-indicator {
  @apply bg-green-500;
}

.disconnected .status-indicator {
  @apply bg-red-500;
}

.reconnecting .status-indicator {
  @apply bg-yellow-500 animate-pulse;
}

.widgets-container {
  @apply grid gap-4;
}

@media (min-width: 768px) {
  .widgets-container {
    @apply grid-cols-2;
  }
}

@media (min-width: 1024px) {
  .widgets-container {
    @apply grid-cols-3;
  }
}
</style>
```

### Notification Composable
```typescript
// Real-time notifications composable
export function useRealtimeNotifications() {
  const notifications = ref<DashboardNotification[]>([]);
  const unreadCount = ref(0);
  
  const addNotification = (notification: DashboardNotification) => {
    notifications.value.unshift(notification);
    if (!notification.isRead) {
      unreadCount.value++;
    }
    
    // Auto-dismiss non-critical notifications after 10 seconds
    if (notification.priority < NotificationPriority.High) {
      setTimeout(() => {
        removeNotification(notification.id);
      }, 10000);
    }
  };
  
  const removeNotification = (notificationId: string) => {
    const index = notifications.value.findIndex(n => n.id === notificationId);
    if (index !== -1) {
      const notification = notifications.value[index];
      if (!notification.isRead) {
        unreadCount.value--;
      }
      notifications.value.splice(index, 1);
    }
  };
  
  const markAsRead = async (notificationId: string) => {
    const notification = notifications.value.find(n => n.id === notificationId);
    if (notification && !notification.isRead) {
      notification.isRead = true;
      unreadCount.value--;
      
      try {
        await fetch(`/api/v1/notifications/${notificationId}/read`, {
          method: 'POST'
        });
      } catch (error) {
        // Revert on error
        notification.isRead = false;
        unreadCount.value++;
        throw error;
      }
    }
  };
  
  const markAllAsRead = async () => {
    const unreadNotifications = notifications.value.filter(n => !n.isRead);
    
    // Optimistically update UI
    unreadNotifications.forEach(n => n.isRead = true);
    unreadCount.value = 0;
    
    try {
      await fetch('/api/v1/notifications/mark-all-read', {
        method: 'POST'
      });
    } catch (error) {
      // Revert on error
      unreadNotifications.forEach(n => n.isRead = false);
      unreadCount.value = unreadNotifications.length;
      throw error;
    }
  };
  
  const clearAll = () => {
    notifications.value = [];
    unreadCount.value = 0;
  };
  
  return {
    notifications: readonly(notifications),
    unreadCount: readonly(unreadCount),
    addNotification,
    removeNotification,
    markAsRead,
    markAllAsRead,
    clearAll
  };
}
```

## Acceptance Criteria

### AC4.1: Real-time Widget Updates
- **Given**: A user has the dashboard open and a booking status changes
- **When**: The booking status is updated in the system
- **Then**: The appointments widget updates automatically within 2 seconds
- **And**: The user sees a subtle visual indication of the update

### AC4.2: Notification Aggregation
- **Given**: Multiple similar notifications occur within a short timeframe
- **When**: The notification aggregation system processes them
- **Then**: Similar notifications are combined into a single aggregated notification
- **And**: Critical notifications are never delayed by aggregation

### AC4.3: Cross-Device Synchronization
- **Given**: A user has the dashboard open on multiple devices
- **When**: An update occurs that affects their data
- **Then**: All devices receive the update simultaneously
- **And**: The update is reflected consistently across all sessions

### AC4.4: Connection Management
- **Given**: A user's internet connection is temporarily interrupted
- **When**: The connection is restored
- **Then**: The SignalR connection automatically reconnects
- **And**: Any missed updates are retrieved and applied to the dashboard

### AC4.5: Performance Impact
- **Given**: Real-time updates are active for multiple widgets
- **When**: Updates are being processed and displayed
- **Then**: The dashboard remains responsive with no noticeable lag
- **And**: Memory usage remains within acceptable limits during extended use

## Integration Verification Points

### IV4.1: SignalR Infrastructure Compatibility
- Verify real-time widget updates do not increase SignalR connection overhead beyond the 15% threshold
- Confirm existing messaging performance remains unaffected by dashboard real-time features
- Ensure SignalR connection management follows existing patterns for authentication and error recovery

### IV4.2: Notification System Integration
- Verify existing notification delivery mechanisms continue to work properly alongside dashboard updates
- Confirm notification preferences are respected for dashboard real-time notifications
- Ensure no duplicate notifications are created between existing and dashboard notification systems

### IV4.3: Performance and Scalability
- Verify real-time update processing does not impact existing application performance metrics
- Confirm dashboard real-time features scale appropriately with increased user concurrent connections
- Ensure real-time data processing maintains existing database performance baselines

## Non-Functional Requirements

### NFR4.1: Performance
- Real-time update delivery must occur within 2 seconds of source event
- Notification aggregation processing must complete within 100ms
- Connection establishment must complete within 5 seconds

### NFR4.2: Reliability
- SignalR connection must automatically reconnect within 30 seconds of disconnection
- Critical notifications must have 99.9% delivery success rate
- System must handle graceful degradation when real-time features are unavailable

### NFR4.3: Scalability
- System must support concurrent real-time connections equal to 150% of current peak user load
- Notification aggregation must handle burst loads of 1000 notifications per minute per user
- Real-time updates must maintain performance with up to 10 active widgets per user

## Testing Strategy

### Unit Tests
- SignalR hub connection and group management
- Notification aggregation logic and prioritization
- Real-time update processing and delivery
- Connection state management and error handling

### Integration Tests
- End-to-end real-time update delivery from source to dashboard
- Cross-device synchronization verification
- Notification aggregation and delivery workflows
- Performance impact measurement on existing systems

### Performance Tests
- SignalR connection load testing with simulated user loads
- Real-time update throughput and latency measurement
- Memory usage monitoring during extended real-time sessions
- Network bandwidth usage optimization verification

## Dependencies

### Existing Systems
- SignalR infrastructure and hub implementations
- User authentication and session management
- Existing notification framework and preferences
- Database change tracking and event sourcing

### Required Integrations
- Dashboard widget framework for update reception
- Notification aggregation and prioritization engine
- Cross-device session synchronization
- Performance monitoring for real-time metrics

## Success Metrics

### Functional Metrics
- 98% of real-time updates delivered within 2-second target
- 40% reduction in notification volume through intelligent aggregation
- 99.9% success rate for critical notification delivery

### Performance Metrics
- SignalR connection overhead increase <15% of baseline
- Real-time update processing latency <100ms
- Cross-device synchronization delay <3 seconds

### User Experience Metrics
- Real-time feature adoption rate >75% of active dashboard users
- User satisfaction with notification relevance >4.0/5.0
- Dashboard responsiveness rating maintained at current levels