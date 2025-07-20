# Story 2: Dashboard Layout System & Widget Framework

## Overview
This section defines the requirements for implementing a flexible, widget-based dashboard layout system that integrates with existing data models and provides customizable, responsive widget arrangements for personalized user experiences.

## Story Definition
**As a pet owner,**  
**I want to see my essential pet information organized in customizable widgets on my dashboard,**  
**so that I can quickly access the most relevant information about my dogs and upcoming activities.**

## Technical Context

### Existing Data Infrastructure
- **Database**: SQL Server with comprehensive entities (DogProfile, PetServiceBooking, UserProfile)
- **API Layer**: ASP.NET Core with Entity Framework repositories
- **Frontend**: Vue.js 3 with Composition API and Tailwind CSS
- **Design System**: Established golden yellow theme (#F1C232) with component library

### Widget Framework Requirements
- Modular, reusable widget components
- Configurable widget properties and data sources
- Responsive layout system with drag-and-drop capability
- Integration with existing data access patterns
- Performance-optimized data fetching

## Functional Requirements

### FR2.1: Core Widget System
- **Requirement**: Implement base widget framework with essential pet information widgets
- **Integration Point**: Existing Vue.js component architecture and data models
- **Implementation**: Create widget component library following established patterns
- **Success Criteria**: Core widgets display data from DogProfile, appointments, and activities

### FR2.2: Widget Data Integration
- **Requirement**: Widgets fetch data using existing API endpoints without performance impact
- **Integration Point**: Current repository pattern and API controllers
- **Implementation**: Leverage existing data services with widget-specific aggregation
- **Success Criteria**: Widget data queries maintain existing performance baselines

### FR2.3: Layout Management System
- **Requirement**: Responsive grid system supporting various widget sizes and arrangements
- **Integration Point**: Existing Tailwind CSS responsive utilities
- **Implementation**: CSS Grid/Flexbox with responsive breakpoints
- **Success Criteria**: Widgets adapt seamlessly across mobile, tablet, and desktop

### FR2.4: Widget Configuration Storage
- **Requirement**: User widget preferences persist across sessions and devices
- **Integration Point**: Existing UserProfile database schema
- **Implementation**: JSON configuration storage following established patterns
- **Success Criteria**: Widget arrangements sync across user's devices

## Widget Component Specifications

### Core Widget Types

#### Dog Profile Widget
```typescript
interface DogProfileWidgetConfig {
  dogId: string;
  displayFields: string[];
  showPhoto: boolean;
  showHealthStatus: boolean;
  size: 'small' | 'medium' | 'large';
}
```

#### Appointments Widget
```typescript
interface AppointmentsWidgetConfig {
  timeRange: 'today' | 'week' | 'month';
  maxItems: number;
  showProviderInfo: boolean;
  enableQuickActions: boolean;
  size: 'small' | 'medium' | 'large';
}
```

#### Activity Feed Widget
```typescript
interface ActivityFeedWidgetConfig {
  feedTypes: ('booking' | 'message' | 'health' | 'social')[];
  maxItems: number;
  timeRange: 'day' | 'week' | 'month';
  size: 'small' | 'medium' | 'large';
}
```

#### Quick Stats Widget
```typescript
interface QuickStatsWidgetConfig {
  metrics: ('totalBookings' | 'totalSpent' | 'dogCount' | 'upcomingAppointments')[];
  timeRange: 'month' | 'quarter' | 'year';
  showTrends: boolean;
  size: 'small' | 'medium';
}
```

## Technical Implementation

### Database Schema Extensions
```sql
-- Extend UserProfile for widget configuration
ALTER TABLE UserProfile ADD WidgetConfiguration NVARCHAR(MAX) NULL;

-- Create widget analytics table
CREATE TABLE WidgetAnalytics (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(450) NOT NULL,
    WidgetType NVARCHAR(100) NOT NULL,
    InteractionType NVARCHAR(50) NOT NULL,
    InteractionData NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_WidgetAnalytics_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    INDEX IX_WidgetAnalytics_UserId_CreatedAt (UserId, CreatedAt)
);
```

### API Layer Integration
```csharp
// Dashboard API Controller
[Route("api/v1/dashboard")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    
    [HttpGet("widgets/configuration")]
    public async Task<ActionResult<WidgetConfiguration>> GetWidgetConfiguration()
    {
        var userId = User.GetUserId();
        var configuration = await _dashboardService.GetWidgetConfigurationAsync(userId);
        return Ok(configuration);
    }
    
    [HttpPost("widgets/configuration")]
    public async Task<ActionResult> UpdateWidgetConfiguration([FromBody] WidgetConfiguration config)
    {
        var userId = User.GetUserId();
        await _dashboardService.UpdateWidgetConfigurationAsync(userId, config);
        return Ok();
    }
    
    [HttpGet("widgets/{widgetType}/data")]
    public async Task<ActionResult<WidgetData>> GetWidgetData(string widgetType, [FromQuery] WidgetDataRequest request)
    {
        var userId = User.GetUserId();
        var data = await _dashboardService.GetWidgetDataAsync(userId, widgetType, request);
        return Ok(data);
    }
}

// Widget service interface
public interface IDashboardService
{
    Task<WidgetConfiguration> GetWidgetConfigurationAsync(string userId);
    Task UpdateWidgetConfigurationAsync(string userId, WidgetConfiguration configuration);
    Task<WidgetData> GetWidgetDataAsync(string userId, string widgetType, WidgetDataRequest request);
    Task<DashboardSummary> GetDashboardSummaryAsync(string userId);
}
```

### Frontend Widget Framework
```vue
<!-- Base Widget Component -->
<template>
  <div 
    :class="widgetClasses"
    :style="widgetStyles"
    @dragstart="onDragStart"
    @dragend="onDragEnd"
    draggable="true"
  >
    <div class="widget-header">
      <h3 class="widget-title">{{ title }}</h3>
      <div class="widget-actions">
        <button @click="toggleSettings" class="widget-settings-btn">
          <SettingsIcon />
        </button>
        <button @click="refreshWidget" class="widget-refresh-btn">
          <RefreshIcon />
        </button>
      </div>
    </div>
    
    <div class="widget-content">
      <slot :loading="loading" :error="error" :data="data" />
    </div>
    
    <!-- Widget Settings Modal -->
    <WidgetSettingsModal 
      v-if="showSettings"
      :widget-type="widgetType"
      :configuration="configuration"
      @save="updateConfiguration"
      @close="showSettings = false"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue';
import { useWidgetData } from '@/composables/useWidgetData';
import { useWidgetDragDrop } from '@/composables/useWidgetDragDrop';

interface Props {
  widgetType: string;
  title: string;
  configuration: WidgetConfiguration;
  size: 'small' | 'medium' | 'large';
}

const props = defineProps<Props>();

const { data, loading, error, refresh } = useWidgetData(props.widgetType, props.configuration);
const { onDragStart, onDragEnd } = useWidgetDragDrop();

const showSettings = ref(false);

const widgetClasses = computed(() => [
  'widget',
  `widget-${props.size}`,
  'bg-white',
  'border',
  'border-gray-200',
  'rounded-lg',
  'shadow-sm',
  'hover:shadow-md',
  'transition-shadow',
  'duration-200'
]);

const widgetStyles = computed(() => ({
  gridArea: `span ${getGridSpan(props.size)} / span ${getGridSpan(props.size)}`
}));

const toggleSettings = () => {
  showSettings.value = !showSettings.value;
};

const refreshWidget = () => {
  refresh();
};

const updateConfiguration = (newConfig: WidgetConfiguration) => {
  // Emit configuration update to parent dashboard
  emit('configurationUpdated', newConfig);
  showSettings.value = false;
};

const getGridSpan = (size: string): number => {
  const spanMap = { small: 1, medium: 2, large: 3 };
  return spanMap[size] || 1;
};

onMounted(() => {
  refresh();
});
</script>
```

### Dashboard Layout Component
```vue
<!-- Dashboard Layout -->
<template>
  <div class="dashboard-container">
    <div class="dashboard-header">
      <h1>Welcome back, {{ userProfile.firstName }}!</h1>
      <div class="dashboard-actions">
        <button @click="toggleCustomization" class="customize-btn">
          <CustomizeIcon />
          Customize Dashboard
        </button>
      </div>
    </div>
    
    <div 
      class="dashboard-grid"
      :class="{ 'customization-mode': isCustomizing }"
      @drop="onDrop"
      @dragover="onDragOver"
    >
      <component
        v-for="(widget, index) in widgets"
        :key="widget.id"
        :is="getWidgetComponent(widget.type)"
        :widget-type="widget.type"
        :title="widget.title"
        :configuration="widget.configuration"
        :size="widget.size"
        :data-widget-id="widget.id"
        @configuration-updated="updateWidgetConfiguration"
        @remove="removeWidget"
      />
      
      <!-- Add Widget Button (shown in customization mode) -->
      <div v-if="isCustomizing" class="add-widget-area">
        <button @click="showWidgetLibrary = true" class="add-widget-btn">
          <PlusIcon />
          Add Widget
        </button>
      </div>
    </div>
    
    <!-- Widget Library Modal -->
    <WidgetLibraryModal
      v-if="showWidgetLibrary"
      @add-widget="addWidget"
      @close="showWidgetLibrary = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useDashboardStore } from '@/stores/dashboard';
import { useAuth } from '@/composables/useAuth';

// Widget Components
import DogProfileWidget from '@/components/dashboard/widgets/DogProfileWidget.vue';
import AppointmentsWidget from '@/components/dashboard/widgets/AppointmentsWidget.vue';
import ActivityFeedWidget from '@/components/dashboard/widgets/ActivityFeedWidget.vue';
import QuickStatsWidget from '@/components/dashboard/widgets/QuickStatsWidget.vue';

const { user: userProfile } = useAuth();
const dashboardStore = useDashboardStore();

const isCustomizing = ref(false);
const showWidgetLibrary = ref(false);

const widgets = computed(() => dashboardStore.widgets);

const getWidgetComponent = (type: string) => {
  const componentMap = {
    'dog-profile': DogProfileWidget,
    'appointments': AppointmentsWidget,
    'activity-feed': ActivityFeedWidget,
    'quick-stats': QuickStatsWidget,
  };
  return componentMap[type] || DogProfileWidget;
};

const toggleCustomization = () => {
  isCustomizing.value = !isCustomizing.value;
};

const onDrop = (event: DragEvent) => {
  event.preventDefault();
  const widgetId = event.dataTransfer?.getData('text/widget-id');
  const dropTarget = event.target as HTMLElement;
  // Handle widget reordering logic
};

const onDragOver = (event: DragEvent) => {
  event.preventDefault();
};

const updateWidgetConfiguration = (widgetId: string, configuration: WidgetConfiguration) => {
  dashboardStore.updateWidgetConfiguration(widgetId, configuration);
};

const addWidget = (widgetType: string) => {
  dashboardStore.addWidget(widgetType);
  showWidgetLibrary.value = false;
};

const removeWidget = (widgetId: string) => {
  dashboardStore.removeWidget(widgetId);
};

onMounted(async () => {
  await dashboardStore.loadDashboardConfiguration();
});
</script>

<style scoped>
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1rem;
  padding: 1rem;
}

@media (min-width: 768px) {
  .dashboard-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

@media (min-width: 1024px) {
  .dashboard-grid {
    grid-template-columns: repeat(6, 1fr);
  }
}

.customization-mode {
  border: 2px dashed #F1C232;
  border-radius: 8px;
}

.widget {
  cursor: move;
}

.widget:hover {
  transform: translateY(-2px);
}
</style>
```

## Acceptance Criteria

### AC2.1: Widget Data Display
- **Given**: An authenticated user with existing pet and booking data
- **When**: They access the dashboard
- **Then**: Core widgets display accurate data from DogProfile, appointments, and activities
- **And**: Data loading states and error handling follow established UX patterns

### AC2.2: Widget Configuration
- **Given**: A user wants to customize their dashboard
- **When**: They enter customization mode
- **Then**: They can resize, reorder, and configure widgets through intuitive controls
- **And**: Configuration changes persist across browser sessions

### AC2.3: Responsive Layout
- **Given**: Users accessing dashboard from different screen sizes
- **When**: The viewport changes from mobile to tablet to desktop
- **Then**: Widget layout adapts appropriately while maintaining functionality
- **And**: Touch interactions work seamlessly on mobile devices

### AC2.4: Performance Standards
- **Given**: A dashboard with multiple widgets
- **When**: The page loads or widgets refresh
- **Then**: Widget data fetching does not exceed existing performance baselines
- **And**: Each widget loads independently without blocking others

### AC2.5: Default Configuration
- **Given**: A new user accessing the dashboard for the first time
- **When**: They view their dashboard
- **Then**: A sensible default widget layout is provided based on their profile
- **And**: They receive guidance on how to customize their dashboard

## Integration Verification Points

### IV2.1: Data Performance Impact
- Ensure widget data queries do not exceed existing database performance baselines
- Verify that multiple concurrent widget requests don't impact current application response times
- Confirm that widget data caching integrates with existing Redis infrastructure

### IV2.2: Authorization and Privacy
- Verify widget data access respects all existing authorization rules for user data
- Confirm that dog profiles and appointment information maintain established privacy settings
- Ensure widget configuration changes follow existing user preference synchronization

### IV2.3: Component Integration
- Confirm widget error states and loading patterns maintain consistency with existing application
- Verify that widget components follow established Vue.js component architecture
- Ensure widget styling aligns with existing Tailwind CSS design system

## Non-Functional Requirements

### NFR2.1: Performance
- Widget initial load time must not exceed 1.5 seconds per widget
- Widget data refresh must complete within 500ms for cached data
- Dashboard layout rendering must maintain 60fps during interactions

### NFR2.2: Scalability
- Widget framework must support addition of new widget types without core changes
- Configuration system must handle complex widget arrangements efficiently
- Data fetching must support lazy loading and pagination for large datasets

### NFR2.3: Accessibility
- All widgets must maintain WCAG 2.1 AA compliance
- Drag-and-drop interactions must have keyboard alternatives
- Widget content must be screen reader accessible

## Testing Strategy

### Unit Tests
- Widget component rendering and data binding
- Configuration persistence and retrieval
- Data service integration for widget content
- Layout calculation and responsive behavior

### Integration Tests
- End-to-end widget customization workflow
- Multi-widget dashboard performance
- Cross-device configuration synchronization
- Widget data refresh and real-time updates

### Performance Tests
- Dashboard load time with varying numbers of widgets
- Concurrent widget data fetching impact
- Memory usage during extended dashboard usage
- Mobile device performance optimization

## Dependencies

### Existing Systems
- Vue.js 3 component architecture
- Entity Framework repositories and data models
- Tailwind CSS responsive utilities
- User preference management system

### Required Integrations
- Dashboard API endpoints for widget data
- Widget configuration storage in UserProfile
- Real-time update integration with SignalR
- Performance monitoring for widget analytics

## Success Metrics

### Functional Metrics
- 95% of users can successfully customize their dashboard layout
- Widget data accuracy matches existing API endpoints (100%)
- Configuration persistence success rate >99%

### Performance Metrics
- Average widget load time <1.5 seconds
- Dashboard customization interaction response time <200ms
- Zero performance regression in existing application features

### User Experience Metrics
- Dashboard customization completion rate >80%
- Average time to complete dashboard setup <5 minutes
- User satisfaction with widget relevance and utility >4.0/5.0