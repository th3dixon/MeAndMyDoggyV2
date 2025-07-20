# Mobile-First Dashboard - Technical Specification

## Component Overview
The Mobile-First Dashboard serves as the personalized hub for all users, featuring responsive design, PWA capabilities, customizable widgets, and intelligent content recommendations powered by user behavior analytics.

## Database Schema

### Dashboard-Specific Tables

```sql
-- DashboardLayouts
CREATE TABLE [dbo].[DashboardLayouts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [LayoutName] NVARCHAR(100) NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [DeviceType] INT NOT NULL, -- 0: Mobile, 1: Tablet, 2: Desktop
    [Configuration] NVARCHAR(MAX) NOT NULL, -- JSON widget configuration
    [GridColumns] INT NOT NULL DEFAULT 2,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DashboardLayouts_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    INDEX [IX_DashboardLayouts_UserId_IsDefault] ([UserId], [IsDefault])
);

-- DashboardWidgets
CREATE TABLE [dbo].[DashboardWidgets] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [WidgetType] NVARCHAR(50) NOT NULL, -- QuickActions, DogProfiles, UpcomingBookings, etc.
    [Title] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Icon] NVARCHAR(50) NULL,
    [DefaultSettings] NVARCHAR(MAX) NULL, -- JSON
    [MinWidth] INT NOT NULL DEFAULT 1,
    [MinHeight] INT NOT NULL DEFAULT 1,
    [MaxWidth] INT NULL,
    [MaxHeight] INT NULL,
    [RequiredUserType] INT NULL, -- NULL = all, 0 = PetOwner, 1 = ServiceProvider
    [RequiredPermissions] NVARCHAR(MAX) NULL, -- JSON array
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- UserWidgets
CREATE TABLE [dbo].[UserWidgets] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [LayoutId] INT NOT NULL,
    [WidgetId] INT NOT NULL,
    [Position] INT NOT NULL, -- Grid position
    [Width] INT NOT NULL DEFAULT 1,
    [Height] INT NOT NULL DEFAULT 1,
    [Settings] NVARCHAR(MAX) NULL, -- JSON user-specific settings
    [IsCollapsed] BIT NOT NULL DEFAULT 0,
    [DisplayOrder] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserWidgets_Layouts] FOREIGN KEY ([LayoutId]) REFERENCES [DashboardLayouts]([Id]),
    CONSTRAINT [FK_UserWidgets_Widgets] FOREIGN KEY ([WidgetId]) REFERENCES [DashboardWidgets]([Id])
);

-- UserFeed
CREATE TABLE [dbo].[UserFeed] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [FeedType] INT NOT NULL, -- 0: Activity, 1: Recommendation, 2: Alert, 3: News
    [Priority] INT NOT NULL DEFAULT 0, -- Higher = more important
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [ActionType] NVARCHAR(50) NULL, -- ViewBooking, ContactProvider, etc.
    [ActionData] NVARCHAR(MAX) NULL, -- JSON
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    [IsDismissed] BIT NOT NULL DEFAULT 0,
    [ExpiresAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserFeed_UserId_IsRead_Priority] ([UserId], [IsRead], [Priority] DESC),
    INDEX [IX_UserFeed_CreatedAt] ([CreatedAt]),
    CONSTRAINT [FK_UserFeed_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- QuickActions
CREATE TABLE [dbo].[QuickActions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ActionKey] NVARCHAR(50) NOT NULL UNIQUE,
    [Title] NVARCHAR(100) NOT NULL,
    [Icon] NVARCHAR(50) NOT NULL,
    [Color] NVARCHAR(20) NULL,
    [Route] NVARCHAR(200) NOT NULL,
    [RequiredUserType] INT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- UserQuickActions
CREATE TABLE [dbo].[UserQuickActions] (
    [UserId] NVARCHAR(450) NOT NULL,
    [QuickActionId] INT NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [UsageCount] INT NOT NULL DEFAULT 0,
    [LastUsedAt] DATETIME2 NULL,
    [IsPinned] BIT NOT NULL DEFAULT 0,
    PRIMARY KEY ([UserId], [QuickActionId]),
    CONSTRAINT [FK_UserQuickActions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserQuickActions_QuickActions] FOREIGN KEY ([QuickActionId]) REFERENCES [QuickActions]([Id])
);

-- DashboardMetrics
CREATE TABLE [dbo].[DashboardMetrics] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [MetricType] NVARCHAR(50) NOT NULL, -- TotalBookings, SpentThisMonth, etc.
    [Value] DECIMAL(18, 2) NOT NULL,
    [PreviousValue] DECIMAL(18, 2) NULL,
    [PercentageChange] DECIMAL(5, 2) NULL,
    [Period] NVARCHAR(20) NOT NULL, -- Day, Week, Month, Year
    [CalculatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_DashboardMetrics_UserId_MetricType] ([UserId], [MetricType]),
    CONSTRAINT [FK_DashboardMetrics_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- WidgetCache
CREATE TABLE [dbo].[WidgetCache] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [WidgetType] NVARCHAR(50) NOT NULL,
    [CacheKey] NVARCHAR(200) NOT NULL,
    [CachedData] NVARCHAR(MAX) NOT NULL, -- JSON
    [ExpiresAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_WidgetCache_UserId_WidgetType] ([UserId], [WidgetType]),
    INDEX [IX_WidgetCache_ExpiresAt] ([ExpiresAt]),
    CONSTRAINT [FK_WidgetCache_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserInsights
CREATE TABLE [dbo].[UserInsights] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [InsightType] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Data] NVARCHAR(MAX) NULL, -- JSON
    [Score] DECIMAL(5, 2) NULL, -- Relevance score
    [ValidUntil] DATETIME2 NOT NULL,
    [IsActioned] BIT NOT NULL DEFAULT 0,
    [ActionedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserInsights_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);
```

## API Endpoints

### Dashboard Management

```yaml
/api/v1/dashboard:
  /:
    GET:
      description: Get user's dashboard data
      auth: required
      parameters:
        deviceType: enum [mobile, tablet, desktop]
      responses:
        200:
          layout: DashboardLayout
          widgets: array[WidgetData]
          feed: array[FeedItem]
          metrics: DashboardMetrics
          quickActions: array[QuickAction]

  /layouts:
    GET:
      description: Get all user layouts
      auth: required
      responses:
        200:
          layouts: array[DashboardLayout]
    
    POST:
      description: Create custom layout
      auth: required
      body:
        layoutName: string
        deviceType: enum
        configuration: object
      responses:
        201:
          layoutId: number

  /layouts/{layoutId}:
    PUT:
      description: Update layout
      auth: required
      body:
        configuration: object
        isDefault: boolean
      responses:
        200: Updated layout
    
    DELETE:
      description: Delete custom layout
      auth: required
      responses:
        204: Deleted

  /widgets:
    GET:
      description: Get available widgets
      auth: required
      responses:
        200:
          widgets: array[WidgetDefinition]
          userWidgets: array[UserWidget]

  /widgets/{widgetId}/data:
    GET:
      description: Get widget data
      auth: required
      parameters:
        settings: object # Widget-specific settings
        refresh: boolean
      responses:
        200:
          data: object # Widget-specific data
          cacheExpiry: datetime
          lastUpdated: datetime

  /widgets/reorder:
    POST:
      description: Reorder widgets
      auth: required
      body:
        widgets: array[{
          widgetId: number
          position: number
          width: number
          height: number
        }]
      responses:
        200: Layout updated
```

### Feed & Activity

```yaml
/api/v1/dashboard/feed:
  /:
    GET:
      description: Get personalized feed
      auth: required
      parameters:
        feedTypes: array[enum]
        page: number
        pageSize: number
        includeRead: boolean
      responses:
        200:
          items: array[FeedItem]
          unreadCount: number
          hasMore: boolean

  /mark-read:
    POST:
      description: Mark feed items as read
      auth: required
      body:
        itemIds: array[number]
      responses:
        200: Items marked as read

  /dismiss:
    POST:
      description: Dismiss feed items
      auth: required
      body:
        itemIds: array[number]
      responses:
        200: Items dismissed

/api/v1/dashboard/insights:
  /:
    GET:
      description: Get AI-powered insights
      auth: required
      responses:
        200:
          insights: array[{
            type: string
            title: string
            description: string
            actions: array[InsightAction]
            priority: number
          }]

  /{insightId}/action:
    POST:
      description: Take action on insight
      auth: required
      body:
        action: string
      responses:
        200: Action completed
```

### Quick Actions

```yaml
/api/v1/dashboard/quick-actions:
  /:
    GET:
      description: Get user's quick actions
      auth: required
      responses:
        200:
          pinned: array[QuickAction]
          recent: array[QuickAction]
          suggested: array[QuickAction]

  /pin:
    POST:
      description: Pin/unpin quick action
      auth: required
      body:
        actionId: number
        isPinned: boolean
      responses:
        200: Action updated

  /track:
    POST:
      description: Track quick action usage
      auth: required
      body:
        actionId: number
      responses:
        200: Usage tracked
```

### Metrics & Analytics

```yaml
/api/v1/dashboard/metrics:
  /:
    GET:
      description: Get dashboard metrics
      auth: required
      parameters:
        period: enum [day, week, month, year]
        metricTypes: array[string]
      responses:
        200:
          metrics: object
          comparisons: object
          trends: object

  /export:
    GET:
      description: Export dashboard data
      auth: required
      parameters:
        format: enum [pdf, csv, json]
        dateRange: object
      responses:
        200: File download
```

## Frontend Components

### Dashboard Layout Components (Vue.js)

```typescript
// DashboardContainer.vue
interface DashboardContainerProps {
  userId: string
  deviceType: 'mobile' | 'tablet' | 'desktop'
}

// Features:
// - Responsive grid system
// - Widget drag-and-drop
// - Pull-to-refresh
// - Offline support

// DashboardGrid.vue
interface DashboardGridProps {
  layout: DashboardLayout
  widgets: UserWidget[]
  editMode: boolean
}

// Grid system:
// - Mobile: 2 columns
// - Tablet: 4 columns  
// - Desktop: 6 columns
```

### Widget Components

```typescript
// BaseWidget.vue
interface BaseWidgetProps {
  widgetId: number
  title: string
  settings: WidgetSettings
  data: any
  loading: boolean
}

// Widget types:
// - DogProfilesWidget.vue
// - UpcomingBookingsWidget.vue
// - RecentActivityWidget.vue
// - QuickStatsWidget.vue
// - ProviderRecommendationsWidget.vue
// - ExpenseTrackerWidget.vue
// - CalendarWidget.vue
// - WeatherWidget.vue

// DogProfilesWidget.vue
interface DogProfilesWidgetProps extends BaseWidgetProps {
  dogs: DogProfile[]
  showAddButton: boolean
}

// Features:
// - Carousel view
// - Quick actions per dog
// - Health status indicators
// - Activity summary
```

### Feed Components

```typescript
// ActivityFeed.vue
interface ActivityFeedProps {
  feedItems: FeedItem[]
  onLoadMore: () => void
  onItemAction: (item: FeedItem, action: string) => void
}

// FeedItem.vue
interface FeedItemProps {
  item: FeedItem
  compact: boolean
}

// Item types:
// - BookingUpdate
// - ProviderMessage
// - DogHealthReminder
// - SystemNotification
// - ProviderRecommendation
```

### Mobile-Specific Components

```typescript
// MobileNavigation.vue
interface MobileNavigationProps {
  activeSection: string
  quickActions: QuickAction[]
}

// Features:
// - Bottom tab navigation
// - Gesture support
// - Haptic feedback

// PullToRefresh.vue
interface PullToRefreshProps {
  onRefresh: () => Promise<void>
  threshold: number
}

// SwipeableCard.vue
interface SwipeableCardProps {
  onSwipeLeft?: () => void
  onSwipeRight?: () => void
  onTap?: () => void
}
```

## Technical Implementation Details

### Progressive Web App (PWA) Configuration

```javascript
// service-worker.js
const CACHE_NAME = 'meandmydoggy-v1';
const urlsToCache = [
  '/',
  '/dashboard',
  '/manifest.json',
  '/icons/icon-192x192.png',
  '/icons/icon-512x512.png'
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(urlsToCache))
  );
});

self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request)
      .then(response => {
        // Cache first, then network
        if (response) {
          return response;
        }
        
        return fetch(event.request).then(response => {
          // Cache successful responses
          if (!response || response.status !== 200) {
            return response;
          }
          
          const responseToCache = response.clone();
          caches.open(CACHE_NAME).then(cache => {
            cache.put(event.request, responseToCache);
          });
          
          return response;
        });
      })
  );
});

// Background sync for offline actions
self.addEventListener('sync', event => {
  if (event.tag === 'sync-dashboard-actions') {
    event.waitUntil(syncDashboardActions());
  }
});
```

### Widget System Architecture

```csharp
public interface IWidget
{
    string WidgetType { get; }
    Task<WidgetData> GetDataAsync(string userId, WidgetSettings settings);
    Task<bool> ValidateSettingsAsync(WidgetSettings settings);
    TimeSpan CacheDuration { get; }
}

public class DogProfilesWidget : IWidget
{
    public string WidgetType => "DogProfiles";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
    
    public async Task<WidgetData> GetDataAsync(string userId, WidgetSettings settings)
    {
        var dogs = await _dogService.GetUserDogsAsync(userId);
        var limit = settings.GetValue<int>("maxItems", 3);
        
        return new WidgetData
        {
            Items = dogs.Take(limit).Select(d => new
            {
                d.Id,
                d.Name,
                d.ProfileImageUrl,
                Age = d.GetAge(),
                NextAppointment = GetNextAppointment(d.Id),
                RecentActivity = GetRecentActivity(d.Id)
            }),
            Metadata = new
            {
                TotalDogs = dogs.Count,
                HasMore = dogs.Count > limit
            }
        };
    }
}

// Widget factory
public class WidgetFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _widgetTypes;
    
    public IWidget CreateWidget(string widgetType)
    {
        if (!_widgetTypes.TryGetValue(widgetType, out var type))
            throw new WidgetNotFoundException(widgetType);
        
        return (IWidget)_serviceProvider.GetService(type);
    }
}
```

### Personalization Engine

```csharp
public class DashboardPersonalizationService
{
    private readonly IUserBehaviorAnalytics _analytics;
    private readonly IRecommendationEngine _recommendations;
    
    public async Task<PersonalizedDashboard> GetPersonalizedDashboard(string userId)
    {
        var userProfile = await GetUserProfile(userId);
        var behavior = await _analytics.GetUserBehavior(userId);
        
        // Determine optimal widget layout
        var suggestedWidgets = await DetermineOptimalWidgets(userProfile, behavior);
        
        // Generate personalized feed
        var feedItems = await GeneratePersonalizedFeed(userId, behavior);
        
        // Calculate quick actions based on usage
        var quickActions = await OptimizeQuickActions(userId, behavior);
        
        // Generate insights
        var insights = await GenerateInsights(userId, behavior);
        
        return new PersonalizedDashboard
        {
            SuggestedWidgets = suggestedWidgets,
            FeedItems = feedItems,
            QuickActions = quickActions,
            Insights = insights
        };
    }
    
    private async Task<List<WidgetSuggestion>> DetermineOptimalWidgets(
        UserProfile profile, 
        UserBehavior behavior)
    {
        var widgetScores = new Dictionary<string, double>();
        
        // Score based on user type
        if (profile.UserType == UserType.PetOwner)
        {
            widgetScores["DogProfiles"] = 100;
            widgetScores["UpcomingBookings"] = 90;
            widgetScores["ProviderRecommendations"] = 80;
        }
        else if (profile.UserType == UserType.ServiceProvider)
        {
            widgetScores["BookingCalendar"] = 100;
            widgetScores["EarningsTracker"] = 95;
            widgetScores["CustomerMessages"] = 90;
        }
        
        // Adjust based on behavior
        foreach (var action in behavior.RecentActions)
        {
            AdjustWidgetScores(widgetScores, action);
        }
        
        // Return top widgets
        return widgetScores
            .OrderByDescending(w => w.Value)
            .Take(6)
            .Select(w => new WidgetSuggestion
            {
                WidgetType = w.Key,
                Score = w.Value,
                Reason = GetSuggestionReason(w.Key, behavior)
            })
            .ToList();
    }
}
```

### Real-time Updates

```csharp
public class DashboardHub : Hub
{
    private readonly IDashboardService _dashboardService;
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        
        // Send initial dashboard state
        var dashboard = await _dashboardService.GetDashboard(userId);
        await Clients.Caller.SendAsync("DashboardState", dashboard);
        
        await base.OnConnectedAsync();
    }
    
    public async Task RefreshWidget(string widgetType, object settings)
    {
        var userId = Context.UserIdentifier;
        var widgetData = await _dashboardService.GetWidgetData(userId, widgetType, settings);
        
        await Clients.Caller.SendAsync("WidgetData", widgetType, widgetData);
    }
    
    public async Task UpdateWidgetSettings(int userWidgetId, object settings)
    {
        await _dashboardService.UpdateWidgetSettings(userWidgetId, settings);
        await Clients.Caller.SendAsync("WidgetSettingsUpdated", userWidgetId);
    }
}

// Push updates to dashboard
public class DashboardUpdateService
{
    private readonly IHubContext<DashboardHub> _hubContext;
    
    public async Task NotifyBookingUpdate(string userId, BookingUpdate update)
    {
        await _hubContext.Clients.Group($"user-{userId}")
            .SendAsync("BookingUpdate", update);
        
        // Update relevant widgets
        await _hubContext.Clients.Group($"user-{userId}")
            .SendAsync("RefreshWidget", "UpcomingBookings");
    }
}
```

### Performance Optimization

```csharp
public class DashboardCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<T> GetCachedWidgetData<T>(
        string userId, 
        string widgetType, 
        Func<Task<T>> factory)
    {
        var cacheKey = $"widget:{userId}:{widgetType}";
        
        // L1 Cache - Memory
        if (_memoryCache.TryGetValue(cacheKey, out T cached))
            return cached;
        
        // L2 Cache - Redis
        var distributedCached = await _distributedCache.GetAsync<T>(cacheKey);
        if (distributedCached != null)
        {
            _memoryCache.Set(cacheKey, distributedCached, TimeSpan.FromMinutes(1));
            return distributedCached;
        }
        
        // Generate fresh data
        var data = await factory();
        
        // Cache with appropriate TTL
        var ttl = GetWidgetCacheTTL(widgetType);
        await _distributedCache.SetAsync(cacheKey, data, ttl);
        _memoryCache.Set(cacheKey, data, TimeSpan.FromMinutes(1));
        
        return data;
    }
    
    private TimeSpan GetWidgetCacheTTL(string widgetType)
    {
        return widgetType switch
        {
            "DogProfiles" => TimeSpan.FromMinutes(10),
            "UpcomingBookings" => TimeSpan.FromMinutes(2),
            "RecentActivity" => TimeSpan.FromMinutes(5),
            "QuickStats" => TimeSpan.FromMinutes(15),
            _ => TimeSpan.FromMinutes(5)
        };
    }
}
```

### Mobile Optimization

```typescript
// Lazy loading widgets
const WidgetLoader = {
  DogProfiles: () => import('./widgets/DogProfilesWidget.vue'),
  UpcomingBookings: () => import('./widgets/UpcomingBookingsWidget.vue'),
  RecentActivity: () => import('./widgets/RecentActivityWidget.vue'),
  // ... other widgets
};

// Virtual scrolling for feed
import { VirtualList } from '@tanstack/vue-virtual';

export default {
  components: {
    VirtualList
  },
  setup() {
    const feedItems = ref([]);
    const rowVirtualizer = useVirtualizer({
      count: feedItems.value.length,
      getScrollElement: () => scrollElement.value,
      estimateSize: () => 120, // Estimated item height
      overscan: 5
    });
    
    return { feedItems, rowVirtualizer };
  }
};

// Touch gesture handling
import { useSwipe } from '@vueuse/core';

export default {
  setup() {
    const target = ref();
    const { direction, lengthX } = useSwipe(target, {
      threshold: 50,
      onSwipeEnd: (e, direction) => {
        if (direction === 'left') {
          navigateToNext();
        } else if (direction === 'right') {
          navigateToPrevious();
        }
      }
    });
    
    return { target };
  }
};
```

## Security Considerations

### Data Privacy
1. **Widget Data Isolation**: Each widget only accesses permitted data
2. **Feed Privacy**: Personal recommendations never exposed to other users
3. **Cache Security**: User-specific cache keys prevent data leakage

### API Security
```csharp
[Authorize]
[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet("widgets/{widgetId}/data")]
    [RateLimit(100, 1)] // 100 requests per minute
    public async Task<IActionResult> GetWidgetData(
        int widgetId, 
        [FromQuery] WidgetSettings settings)
    {
        var userId = User.GetUserId();
        
        // Verify widget access
        if (!await _widgetService.UserHasAccess(userId, widgetId))
            return Forbid();
        
        // Sanitize settings
        var sanitized = _widgetService.SanitizeSettings(settings);
        
        var data = await _widgetService.GetWidgetData(userId, widgetId, sanitized);
        return Ok(data);
    }
}
```

## Performance Metrics

### Key Performance Indicators
1. **Load Time**: Dashboard initial load < 1s
2. **Widget Refresh**: < 200ms per widget
3. **Feed Updates**: Real-time (< 100ms)
4. **Offline Capability**: Full dashboard available offline

### Monitoring
```csharp
public class DashboardMetricsCollector
{
    public async Task TrackDashboardLoad(string userId, DashboardLoadMetrics metrics)
    {
        await _telemetry.TrackEvent("DashboardLoad", new
        {
            UserId = userId,
            LoadTime = metrics.TotalLoadTime,
            WidgetCount = metrics.WidgetCount,
            CacheHitRate = metrics.CacheHitRate,
            DeviceType = metrics.DeviceType,
            NetworkType = metrics.NetworkType
        });
    }
}
```

## Testing Strategy

### Component Testing
```typescript
// DashboardWidget.spec.ts
describe('DashboardWidget', () => {
  it('should load data on mount', async () => {
    const wrapper = mount(DogProfilesWidget, {
      props: {
        widgetId: 1,
        settings: { maxItems: 3 }
      }
    });
    
    await flushPromises();
    
    expect(wrapper.find('.dog-profiles').exists()).toBe(true);
    expect(wrapper.findAll('.dog-card')).toHaveLength(3);
  });
  
  it('should handle refresh', async () => {
    const wrapper = mount(DogProfilesWidget);
    await wrapper.find('.refresh-button').trigger('click');
    
    expect(wrapper.emitted('refresh')).toBeTruthy();
  });
});
```

### Performance Testing
- Lighthouse PWA audit score > 95
- Time to Interactive < 2s
- First Contentful Paint < 1s
- Cumulative Layout Shift < 0.1