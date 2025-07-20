# Administrative Control Panel - Technical Specification

## Component Overview
The Administrative Control Panel provides comprehensive platform management capabilities including user management, content moderation, financial oversight, system monitoring, security management, and business intelligence tools for platform administrators.

## Database Schema

### Administrative Tables

```sql
-- AdminRoles
CREATE TABLE [dbo].[AdminRoles] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RoleName] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [Permissions] NVARCHAR(MAX) NOT NULL, -- JSON array
    [Priority] INT NOT NULL DEFAULT 0, -- Higher priority overrides lower
    [IsSystem] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- AdminUserRoles
CREATE TABLE [dbo].[AdminUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] INT NOT NULL,
    [AssignedBy] NVARCHAR(450) NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NULL,
    PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AdminUserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_AdminUserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [AdminRoles]([Id]),
    CONSTRAINT [FK_AdminUserRoles_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [AspNetUsers]([Id])
);

-- AdminActions
CREATE TABLE [dbo].[AdminActions] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AdminUserId] NVARCHAR(450) NOT NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [Category] NVARCHAR(50) NOT NULL,
    [EntityType] NVARCHAR(100) NULL,
    [EntityId] NVARCHAR(100) NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [OldValues] NVARCHAR(MAX) NULL, -- JSON
    [NewValues] NVARCHAR(MAX) NULL, -- JSON
    [Reason] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Result] INT NOT NULL, -- 0: Success, 1: Failed, 2: Partial
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [Duration] INT NULL, -- Milliseconds
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_AdminActions_AdminUserId] ([AdminUserId]),
    INDEX [IX_AdminActions_EntityType_EntityId] ([EntityType], [EntityId]),
    INDEX [IX_AdminActions_CreatedAt] ([CreatedAt] DESC),
    CONSTRAINT [FK_AdminActions_AdminUsers] FOREIGN KEY ([AdminUserId]) REFERENCES [AspNetUsers]([Id])
);

-- PlatformSettings
CREATE TABLE [dbo].[PlatformSettings] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Category] NVARCHAR(100) NOT NULL,
    [Key] NVARCHAR(100) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [ValueType] NVARCHAR(50) NOT NULL, -- string, int, bool, json, decimal
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [ValidationRules] NVARCHAR(MAX) NULL, -- JSON
    [IsEditable] BIT NOT NULL DEFAULT 1,
    [IsVisible] BIT NOT NULL DEFAULT 1,
    [IsSensitive] BIT NOT NULL DEFAULT 0,
    [RequiredPermission] NVARCHAR(100) NULL,
    [LastModifiedBy] NVARCHAR(450) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE ([Category], [Key]),
    CONSTRAINT [FK_PlatformSettings_ModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [AspNetUsers]([Id])
);

-- UserSuspensions
CREATE TABLE [dbo].[UserSuspensions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [SuspendedBy] NVARCHAR(450) NOT NULL,
    [Reason] NVARCHAR(MAX) NOT NULL,
    [Evidence] NVARCHAR(MAX) NULL, -- JSON array of links/references
    [StartDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [EndDate] DATETIME2 NULL, -- NULL = permanent
    [AppealStatus] INT NOT NULL DEFAULT 0, -- 0: None, 1: Pending, 2: Approved, 3: Denied
    [AppealNotes] NVARCHAR(MAX) NULL,
    [LiftedBy] NVARCHAR(450) NULL,
    [LiftedAt] DATETIME2 NULL,
    [LiftReason] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserSuspensions_UserId] ([UserId]),
    INDEX [IX_UserSuspensions_EndDate] ([EndDate]),
    CONSTRAINT [FK_UserSuspensions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserSuspensions_SuspendedBy] FOREIGN KEY ([SuspendedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserSuspensions_LiftedBy] FOREIGN KEY ([LiftedBy]) REFERENCES [AspNetUsers]([Id])
);

-- ContentModerationQueue
CREATE TABLE [dbo].[ContentModerationQueue] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentType] NVARCHAR(50) NOT NULL, -- Review, Message, Profile, Photo
    [ContentId] NVARCHAR(100) NOT NULL,
    [ReportedBy] NVARCHAR(450) NULL,
    [Reason] INT NOT NULL, -- 0: Spam, 1: Inappropriate, 2: Fake, 3: Harassment, 4: Other
    [Details] NVARCHAR(MAX) NULL,
    [Priority] INT NOT NULL DEFAULT 0, -- Higher = more urgent
    [Status] INT NOT NULL DEFAULT 0, -- 0: Pending, 1: InReview, 2: Approved, 3: Removed, 4: Dismissed
    [AssignedTo] NVARCHAR(450) NULL,
    [AssignedAt] DATETIME2 NULL,
    [Decision] NVARCHAR(MAX) NULL,
    [ActionTaken] NVARCHAR(500) NULL,
    [ReviewedBy] NVARCHAR(450) NULL,
    [ReviewedAt] DATETIME2 NULL,
    [AutoFlagged] BIT NOT NULL DEFAULT 0,
    [ConfidenceScore] DECIMAL(5, 2) NULL, -- AI confidence 0-100
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_ContentModerationQueue_Status_Priority] ([Status], [Priority] DESC),
    INDEX [IX_ContentModerationQueue_ContentType_ContentId] ([ContentType], [ContentId]),
    CONSTRAINT [FK_ContentModerationQueue_ReportedBy] FOREIGN KEY ([ReportedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_ContentModerationQueue_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_ContentModerationQueue_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [AspNetUsers]([Id])
);

-- PlatformMetrics
CREATE TABLE [dbo].[PlatformMetrics] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MetricDate] DATE NOT NULL,
    [MetricHour] INT NULL, -- 0-23 for hourly metrics
    [MetricType] NVARCHAR(50) NOT NULL,
    [MetricName] NVARCHAR(100) NOT NULL,
    [Value] DECIMAL(18, 4) NOT NULL,
    [Dimension1] NVARCHAR(100) NULL, -- e.g., UserType, Region
    [Dimension2] NVARCHAR(100) NULL, -- e.g., Service, Platform
    [CalculatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_PlatformMetrics_MetricDate_MetricType] ([MetricDate], [MetricType]),
    INDEX [IX_PlatformMetrics_MetricName] ([MetricName])
);

-- SystemAlerts
CREATE TABLE [dbo].[SystemAlerts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AlertType] NVARCHAR(50) NOT NULL,
    [Severity] INT NOT NULL, -- 0: Info, 1: Warning, 2: Error, 3: Critical
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [Details] NVARCHAR(MAX) NULL, -- JSON
    [AffectedSystem] NVARCHAR(100) NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0: Active, 1: Acknowledged, 2: Resolved
    [AcknowledgedBy] NVARCHAR(450) NULL,
    [AcknowledgedAt] DATETIME2 NULL,
    [ResolvedBy] NVARCHAR(450) NULL,
    [ResolvedAt] DATETIME2 NULL,
    [Resolution] NVARCHAR(MAX) NULL,
    [AutoResolved] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_SystemAlerts_Status_Severity] ([Status], [Severity] DESC),
    CONSTRAINT [FK_SystemAlerts_AcknowledgedBy] FOREIGN KEY ([AcknowledgedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_SystemAlerts_ResolvedBy] FOREIGN KEY ([ResolvedBy]) REFERENCES [AspNetUsers]([Id])
);

-- FeatureFlags
CREATE TABLE [dbo].[FeatureFlags] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FeatureName] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 0,
    [EnabledFor] NVARCHAR(MAX) NULL, -- JSON (user groups, percentages, conditions)
    [Configuration] NVARCHAR(MAX) NULL, -- JSON
    [StartDate] DATETIME2 NULL,
    [EndDate] DATETIME2 NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_FeatureFlags_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- AdminNotifications
CREATE TABLE [dbo].[AdminNotifications] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RecipientId] NVARCHAR(450) NULL, -- NULL = all admins
    [Type] NVARCHAR(50) NOT NULL,
    [Priority] INT NOT NULL, -- 0: Low, 1: Normal, 2: High, 3: Urgent
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [ActionRequired] BIT NOT NULL DEFAULT 0,
    [ActionUrl] NVARCHAR(500) NULL,
    [Data] NVARCHAR(MAX) NULL, -- JSON
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    [ExpiresAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_AdminNotifications_RecipientId_IsRead] ([RecipientId], [IsRead]),
    CONSTRAINT [FK_AdminNotifications_Recipients] FOREIGN KEY ([RecipientId]) REFERENCES [AspNetUsers]([Id])
);

-- SecurityEvents
CREATE TABLE [dbo].[SecurityEvents] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [EventType] NVARCHAR(50) NOT NULL, -- Login, FailedLogin, PermissionDenied, DataAccess
    [Severity] INT NOT NULL, -- 0: Info, 1: Warning, 2: High, 3: Critical
    [UserId] NVARCHAR(450) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Resource] NVARCHAR(200) NULL,
    [Action] NVARCHAR(100) NULL,
    [Result] NVARCHAR(50) NULL,
    [Details] NVARCHAR(MAX) NULL, -- JSON
    [GeoLocation] NVARCHAR(200) NULL,
    [ThreatScore] DECIMAL(5, 2) NULL, -- 0-100
    [IsInvestigated] BIT NOT NULL DEFAULT 0,
    [InvestigationNotes] NVARCHAR(MAX) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_SecurityEvents_EventType_Timestamp] ([EventType], [Timestamp] DESC),
    INDEX [IX_SecurityEvents_UserId] ([UserId]),
    INDEX [IX_SecurityEvents_Severity_IsInvestigated] ([Severity], [IsInvestigated])
);
```

## API Endpoints

### User Management

```yaml
/api/v1/admin/users:
  /:
    GET:
      description: Get users with advanced filtering
      auth: required (admin.users.read)
      parameters:
        search: string
        userType: enum [all, petOwner, serviceProvider, both]
        status: enum [active, suspended, deleted]
        registeredFrom: date
        registeredTo: date
        hasBookings: boolean
        verified: boolean
        page: number
        pageSize: number
        sort: string
      responses:
        200:
          users: array[UserAdminView]
          totalCount: number
          statistics: UserStatistics

  /{userId}:
    GET:
      description: Get detailed user information
      auth: required (admin.users.read)
      responses:
        200:
          user: UserDetail
          activity: UserActivity
          bookings: BookingSummary
          financial: FinancialSummary
          flags: array[UserFlag]
          notes: array[AdminNote]

    PUT:
      description: Update user
      auth: required (admin.users.write)
      body:
        status: enum
        verified: boolean
        userType: enum
        notes: string
      responses:
        200: Updated user

  /{userId}/suspend:
    POST:
      description: Suspend user
      auth: required (admin.users.suspend)
      body:
        reason: string
        evidence: array[string]
        duration: number # Days, null = permanent
        notifyUser: boolean
      responses:
        200:
          suspensionId: number
          endDate: datetime

  /{userId}/unsuspend:
    POST:
      description: Lift suspension
      auth: required (admin.users.suspend)
      body:
        reason: string
      responses:
        200: Suspension lifted

  /{userId}/impersonate:
    POST:
      description: Impersonate user (with audit)
      auth: required (admin.users.impersonate)
      responses:
        200:
          token: string
          expiresIn: number

  /{userId}/data:
    GET:
      description: Export user data (GDPR)
      auth: required (admin.users.data)
      parameters:
        format: enum [json, csv, pdf]
      responses:
        200: File download

    DELETE:
      description: Delete user data (GDPR)
      auth: required (admin.users.delete)
      body:
        confirmation: string
        reason: string
      responses:
        200: Data deletion report
```

### Content Moderation

```yaml
/api/v1/admin/moderation:
  /queue:
    GET:
      description: Get moderation queue
      auth: required (admin.moderation.read)
      parameters:
        status: enum [pending, inReview, all]
        contentType: string
        priority: enum [all, high, normal]
        assignedTo: string
        autoFlagged: boolean
      responses:
        200:
          items: array[ModerationItem]
          statistics: ModerationStats

  /queue/{itemId}:
    GET:
      description: Get moderation item details
      auth: required (admin.moderation.read)
      responses:
        200:
          item: ModerationDetail
          content: ContentDetail
          history: array[ModerationHistory]
          similarCases: array[SimilarCase]

    PUT:
      description: Update moderation item
      auth: required (admin.moderation.write)
      body:
        status: enum
        decision: string
        actionTaken: string
        notes: string
      responses:
        200: Updated item

  /queue/{itemId}/assign:
    POST:
      description: Assign to moderator
      auth: required (admin.moderation.assign)
      body:
        moderatorId: string
      responses:
        200: Assigned

  /rules:
    GET:
      description: Get auto-moderation rules
      auth: required (admin.moderation.rules)
      responses:
        200:
          rules: array[ModerationRule]

    POST:
      description: Create moderation rule
      auth: required (admin.moderation.rules)
      body:
        name: string
        conditions: object
        actions: array[string]
        severity: number
      responses:
        201:
          ruleId: number

  /reports:
    GET:
      description: Get user reports
      auth: required (admin.moderation.reports)
      parameters:
        startDate: date
        endDate: date
        reportType: enum
      responses:
        200:
          reports: array[Report]
          trends: ReportTrends
```

### Financial Oversight

```yaml
/api/v1/admin/financial:
  /overview:
    GET:
      description: Platform financial overview
      auth: required (admin.financial.read)
      parameters:
        period: enum [day, week, month, quarter, year]
      responses:
        200:
          revenue: RevenueBreakdown
          transactions: TransactionSummary
          providers: ProviderFinancials
          forecasts: FinancialForecasts

  /transactions:
    GET:
      description: Get all transactions
      auth: required (admin.financial.read)
      parameters:
        startDate: date
        endDate: date
        type: enum
        status: enum
        minAmount: number
        maxAmount: number
        providerId: number
        customerId: string
      responses:
        200:
          transactions: array[Transaction]
          summary: TransactionSummary

  /transactions/{transactionId}:
    GET:
      description: Get transaction details
      auth: required (admin.financial.read)
      responses:
        200:
          transaction: TransactionDetail
          relatedTransactions: array[Transaction]
          audit: array[AuditEntry]

  /refunds:
    POST:
      description: Process refund
      auth: required (admin.financial.refund)
      body:
        transactionId: string
        amount: number
        reason: string
        notifyParties: boolean
      responses:
        200:
          refundId: string
          status: string

  /payouts:
    GET:
      description: Manage provider payouts
      auth: required (admin.financial.payouts)
      parameters:
        status: enum
        providerId: number
      responses:
        200:
          payouts: array[Payout]
          pending: PayoutSummary

    POST:
      description: Trigger manual payout
      auth: required (admin.financial.payouts)
      body:
        providerId: number
        amount: number
        reason: string
      responses:
        200:
          payoutId: string

  /reports/financial:
    POST:
      description: Generate financial report
      auth: required (admin.financial.reports)
      body:
        reportType: enum [revenue, tax, compliance, audit]
        startDate: date
        endDate: date
        format: enum [pdf, excel, csv]
      responses:
        200:
          reportId: string
          downloadUrl: string
```

### System Management

```yaml
/api/v1/admin/system:
  /settings:
    GET:
      description: Get platform settings
      auth: required (admin.system.read)
      parameters:
        category: string
      responses:
        200:
          settings: array[SettingGroup]

    PUT:
      description: Update settings
      auth: required (admin.system.write)
      body:
        settings: array[{
          category: string
          key: string
          value: any
        }]
      responses:
        200:
          updated: array[Setting]
          errors: array[SettingError]

  /health:
    GET:
      description: System health check
      auth: required (admin.system.monitor)
      responses:
        200:
          status: enum [healthy, degraded, unhealthy]
          components: array[ComponentHealth]
          metrics: SystemMetrics

  /alerts:
    GET:
      description: Get system alerts
      auth: required (admin.system.monitor)
      parameters:
        severity: enum
        status: enum
        system: string
      responses:
        200:
          alerts: array[SystemAlert]
          summary: AlertSummary

  /alerts/{alertId}/acknowledge:
    POST:
      description: Acknowledge alert
      auth: required (admin.system.monitor)
      body:
        notes: string
      responses:
        200: Alert acknowledged

  /maintenance:
    GET:
      description: Get maintenance windows
      auth: required (admin.system.maintenance)
      responses:
        200:
          scheduled: array[MaintenanceWindow]
          history: array[MaintenanceHistory]

    POST:
      description: Schedule maintenance
      auth: required (admin.system.maintenance)
      body:
        startTime: datetime
        duration: number # minutes
        affectedSystems: array[string]
        description: string
        notifyUsers: boolean
      responses:
        201:
          maintenanceId: number

  /features:
    GET:
      description: Manage feature flags
      auth: required (admin.system.features)
      responses:
        200:
          features: array[FeatureFlag]

    PUT:
      description: Update feature flag
      auth: required (admin.system.features)
      body:
        featureName: string
        enabled: boolean
        enabledFor: object
        configuration: object
      responses:
        200: Updated feature

  /cache:
    DELETE:
      description: Clear cache
      auth: required (admin.system.cache)
      parameters:
        pattern: string
        type: enum [all, redis, memory, cdn]
      responses:
        200:
          cleared: number
          remaining: number

  /jobs:
    GET:
      description: Background job management
      auth: required (admin.system.jobs)
      parameters:
        status: enum [pending, running, completed, failed]
        type: string
      responses:
        200:
          jobs: array[BackgroundJob]
          queues: array[QueueStatus]

    POST:
      description: Trigger job
      auth: required (admin.system.jobs)
      body:
        jobType: string
        parameters: object
        priority: enum
      responses:
        201:
          jobId: string
          estimatedTime: number
```

### Analytics & Reporting

```yaml
/api/v1/admin/analytics:
  /dashboard:
    GET:
      description: Admin analytics dashboard
      auth: required (admin.analytics.read)
      parameters:
        period: enum
        compare: boolean
      responses:
        200:
          kpis: array[KPI]
          charts: array[ChartData]
          insights: array[Insight]

  /metrics:
    GET:
      description: Platform metrics
      auth: required (admin.analytics.read)
      parameters:
        metrics: array[string]
        dimensions: array[string]
        startDate: date
        endDate: date
        granularity: enum [hour, day, week, month]
      responses:
        200:
          data: array[MetricData]
          aggregations: object

  /cohorts:
    GET:
      description: Cohort analysis
      auth: required (admin.analytics.cohorts)
      parameters:
        cohortType: enum [registration, firstBooking]
        metric: enum [retention, revenue, activity]
      responses:
        200:
          cohorts: array[CohortData]
          visualization: ChartConfig

  /funnels:
    GET:
      description: Conversion funnels
      auth: required (admin.analytics.funnels)
      parameters:
        funnelType: enum [registration, booking, provider]
        segment: object
      responses:
        200:
          steps: array[FunnelStep]
          dropoff: array[DropoffAnalysis]

  /reports/generate:
    POST:
      description: Generate custom report
      auth: required (admin.analytics.reports)
      body:
        name: string
        type: enum
        metrics: array[string]
        filters: object
        schedule: object # Optional
        recipients: array[string]
      responses:
        201:
          reportId: string
          status: enum [generating, scheduled]

  /exports:
    POST:
      description: Export analytics data
      auth: required (admin.analytics.export)
      body:
        dataType: enum
        format: enum [csv, excel, json]
        filters: object
      responses:
        200:
          exportId: string
          downloadUrl: string
```

### Security Management

```yaml
/api/v1/admin/security:
  /events:
    GET:
      description: Security event log
      auth: required (admin.security.read)
      parameters:
        eventType: string
        severity: enum
        userId: string
        startDate: datetime
        endDate: datetime
      responses:
        200:
          events: array[SecurityEvent]
          threats: ThreatSummary

  /events/{eventId}/investigate:
    POST:
      description: Mark event as investigated
      auth: required (admin.security.investigate)
      body:
        notes: string
        action: enum [dismiss, escalate, block]
      responses:
        200: Event updated

  /threats:
    GET:
      description: Active threats
      auth: required (admin.security.threats)
      responses:
        200:
          threats: array[Threat]
          riskScore: number
          recommendations: array[SecurityRecommendation]

  /blocks:
    GET:
      description: IP/User blocks
      auth: required (admin.security.blocks)
      responses:
        200:
          ipBlocks: array[IPBlock]
          userBlocks: array[UserBlock]

    POST:
      description: Add block
      auth: required (admin.security.blocks)
      body:
        type: enum [ip, user, pattern]
        value: string
        reason: string
        duration: number # hours
      responses:
        201:
          blockId: string

  /audit:
    GET:
      description: Security audit log
      auth: required (admin.security.audit)
      parameters:
        action: string
        adminId: string
        entityType: string
        startDate: datetime
        endDate: datetime
      responses:
        200:
          entries: array[AuditEntry]
          summary: AuditSummary
```

## Frontend Components

### Admin Dashboard Components (Vue.js)

```typescript
// AdminDashboard.vue
interface AdminDashboardProps {
  user: AdminUser
  permissions: string[]
}

// Layout components:
// - AdminHeader.vue (notifications, profile, search)
// - AdminSidebar.vue (navigation, quick stats)
// - AdminContent.vue (main content area)
// - AdminFooter.vue (system status)

// Dashboard widgets:
// - SystemHealthWidget.vue
// - RealtimeMetricsWidget.vue
// - AlertsWidget.vue
// - QuickActionsWidget.vue
```

### User Management Components

```typescript
// UserManagementTable.vue
interface UserManagementTableProps {
  filters: UserFilters
  onUserSelect: (user: User) => void
}

// Features:
// - Advanced filtering
// - Bulk actions
// - Inline editing
// - Export functionality

// UserDetailModal.vue
interface UserDetailModalProps {
  userId: string
  mode: 'view' | 'edit'
}

// Tabs:
// - Profile information
// - Activity history
// - Financial summary
// - Messages/Reviews
// - Admin notes
// - Suspension history
```

### Moderation Components

```typescript
// ModerationQueue.vue
interface ModerationQueueProps {
  contentType?: string
  autoAssign: boolean
}

// Features:
// - Priority sorting
// - Quick actions
// - Bulk moderation
// - AI suggestions

// ContentReviewModal.vue
interface ContentReviewModalProps {
  moderationItem: ModerationItem
  onDecision: (decision: ModerationDecision) => void
}

// Sections:
// - Original content
// - Report details
// - User history
// - Similar cases
// - Action tools
```

### Analytics Components

```typescript
// AnalyticsDashboard.vue
interface AnalyticsDashboardProps {
  timeRange: DateRange
  compareWith?: DateRange
}

// Chart components:
// - MetricsChart.vue (line, bar, area)
// - HeatmapChart.vue
// - FunnelChart.vue
// - CohortChart.vue
// - GeographicMap.vue

// InsightsPanel.vue
interface InsightsPanelProps {
  insights: Insight[]
  onAction: (insight: Insight, action: string) => void
}
```

## Technical Implementation Details

### Role-Based Access Control (RBAC)

```csharp
public class AdminAuthorizationService
{
    private readonly IPermissionService _permissionService;
    private readonly IAuditService _auditService;
    
    public async Task<bool> AuthorizeAsync(
        ClaimsPrincipal user, 
        string resource, 
        string action)
    {
        var userId = user.GetUserId();
        var permissions = await _permissionService.GetUserPermissions(userId);
        
        var requiredPermission = $"{resource}.{action}";
        var hasPermission = permissions.Contains(requiredPermission) || 
                           permissions.Contains($"{resource}.*") ||
                           permissions.Contains("*");
        
        // Audit the authorization check
        await _auditService.LogAuthorizationCheck(new AuthorizationAudit
        {
            UserId = userId,
            Resource = resource,
            Action = action,
            Result = hasPermission,
            Timestamp = DateTime.UtcNow
        });
        
        return hasPermission;
    }
    
    public async Task<List<string>> GetEffectivePermissions(string userId)
    {
        var roles = await _permissionService.GetUserRoles(userId);
        var permissions = new HashSet<string>();
        
        foreach (var role in roles.OrderByDescending(r => r.Priority))
        {
            foreach (var permission in role.Permissions)
            {
                if (permission.StartsWith("-"))
                {
                    // Negative permission (deny)
                    permissions.Remove(permission.Substring(1));
                }
                else
                {
                    permissions.Add(permission);
                }
            }
        }
        
        return permissions.ToList();
    }
}

// Permission-based attribute
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireAdminPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;
    
    public RequireAdminPermissionAttribute(string permission)
    {
        _permission = permission;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var authService = context.HttpContext.RequestServices
            .GetService<AdminAuthorizationService>();
        
        var parts = _permission.Split('.');
        var authorized = authService.AuthorizeAsync(
            user, 
            parts[0], 
            parts.Length > 1 ? parts[1] : "*"
        ).Result;
        
        if (!authorized)
        {
            context.Result = new ForbidResult();
        }
    }
}
```

### Advanced Monitoring System

```csharp
public class PlatformMonitoringService
{
    private readonly IMetricsCollector _metrics;
    private readonly IAlertingService _alerting;
    private readonly IHealthCheckService _healthCheck;
    
    public async Task<SystemHealth> GetSystemHealth()
    {
        var healthChecks = await Task.WhenAll(
            CheckDatabaseHealth(),
            CheckRedisHealth(),
            CheckStorageHealth(),
            CheckAPIHealth(),
            CheckPaymentSystemHealth()
        );
        
        var overallStatus = healthChecks.All(h => h.Status == HealthStatus.Healthy) 
            ? HealthStatus.Healthy 
            : healthChecks.Any(h => h.Status == HealthStatus.Unhealthy)
                ? HealthStatus.Unhealthy
                : HealthStatus.Degraded;
        
        return new SystemHealth
        {
            Status = overallStatus,
            Components = healthChecks,
            Metrics = await GetSystemMetrics(),
            Alerts = await GetActiveAlerts()
        };
    }
    
    private async Task<ComponentHealth> CheckDatabaseHealth()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Check connection
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // Check query performance
            var queryTime = await MeasureQueryTime(connection);
            
            // Check replication lag (if applicable)
            var replicationLag = await CheckReplicationLag(connection);
            
            return new ComponentHealth
            {
                Component = "Database",
                Status = queryTime < 100 && replicationLag < 1000 
                    ? HealthStatus.Healthy 
                    : HealthStatus.Degraded,
                ResponseTime = stopwatch.ElapsedMilliseconds,
                Details = new
                {
                    QueryTime = queryTime,
                    ReplicationLag = replicationLag,
                    ConnectionPoolSize = GetConnectionPoolSize()
                }
            };
        }
        catch (Exception ex)
        {
            await _alerting.RaiseAlert(new SystemAlert
            {
                Component = "Database",
                Severity = AlertSeverity.Critical,
                Message = "Database health check failed",
                Exception = ex
            });
            
            return new ComponentHealth
            {
                Component = "Database",
                Status = HealthStatus.Unhealthy,
                Error = ex.Message
            };
        }
    }
    
    public async Task<PlatformMetrics> CollectPlatformMetrics()
    {
        var metrics = new PlatformMetrics
        {
            Timestamp = DateTime.UtcNow,
            
            // User metrics
            ActiveUsers = await CountActiveUsers(TimeSpan.FromMinutes(15)),
            DailyActiveUsers = await CountActiveUsers(TimeSpan.FromDays(1)),
            MonthlyActiveUsers = await CountActiveUsers(TimeSpan.FromDays(30)),
            NewRegistrations = await CountNewRegistrations(TimeSpan.FromDays(1)),
            
            // Business metrics
            BookingsToday = await CountBookings(DateTime.Today),
            RevenueToday = await CalculateRevenue(DateTime.Today),
            AverageBookingValue = await CalculateAverageBookingValue(),
            
            // Performance metrics
            ApiResponseTime = await GetAverageResponseTime(),
            ErrorRate = await CalculateErrorRate(),
            ThroughputRps = await GetRequestsPerSecond(),
            
            // Infrastructure metrics
            CpuUsage = await GetCpuUsage(),
            MemoryUsage = await GetMemoryUsage(),
            DiskUsage = await GetDiskUsage(),
            DatabaseConnections = await GetDatabaseConnections()
        };
        
        // Store metrics
        await StoreMetrics(metrics);
        
        // Check for anomalies
        await DetectAnomalies(metrics);
        
        return metrics;
    }
}
```

### Content Moderation Engine

```csharp
public class ContentModerationEngine
{
    private readonly IAIContentAnalyzer _aiAnalyzer;
    private readonly IPatternMatcher _patternMatcher;
    private readonly IModerationRulesEngine _rulesEngine;
    
    public async Task<ModerationResult> AnalyzeContent(
        string contentType, 
        object content)
    {
        var result = new ModerationResult
        {
            ContentType = contentType,
            Timestamp = DateTime.UtcNow
        };
        
        // AI analysis
        var aiAnalysis = await _aiAnalyzer.AnalyzeContent(content);
        result.AIScore = aiAnalysis.ConfidenceScore;
        result.AIFlags = aiAnalysis.Flags;
        
        // Pattern matching
        var patternMatches = await _patternMatcher.FindMatches(content);
        result.PatternMatches = patternMatches;
        
        // Rules engine
        var ruleViolations = await _rulesEngine.EvaluateRules(content, contentType);
        result.RuleViolations = ruleViolations;
        
        // Calculate overall risk score
        result.RiskScore = CalculateRiskScore(aiAnalysis, patternMatches, ruleViolations);
        
        // Determine action
        result.RecommendedAction = DetermineAction(result.RiskScore);
        
        // Auto-action for high-risk content
        if (result.RiskScore > 0.9 && _config.EnableAutoModeration)
        {
            await AutoModerateContent(contentType, content, result);
        }
        
        return result;
    }
    
    private ModerationAction DetermineAction(double riskScore)
    {
        return riskScore switch
        {
            > 0.9 => ModerationAction.Remove,
            > 0.7 => ModerationAction.Review,
            > 0.5 => ModerationAction.Flag,
            _ => ModerationAction.Approve
        };
    }
    
    public async Task<BatchModerationResult> BatchModerate(
        List<ModerationItem> items)
    {
        var results = new ConcurrentBag<ModerationResult>();
        
        await Parallel.ForEachAsync(items, new ParallelOptions
        {
            MaxDegreeOfParallelism = 10
        }, async (item, ct) =>
        {
            var result = await AnalyzeContent(item.ContentType, item.Content);
            result.ItemId = item.Id;
            results.Add(result);
        });
        
        return new BatchModerationResult
        {
            TotalItems = items.Count,
            Results = results.ToList(),
            AutoActioned = results.Count(r => r.AutoActioned),
            RequiringReview = results.Count(r => r.RecommendedAction == ModerationAction.Review)
        };
    }
}
```

### Financial Oversight System

```csharp
public class FinancialOversightService
{
    private readonly ITransactionMonitor _transactionMonitor;
    private readonly IFraudDetector _fraudDetector;
    private readonly IComplianceChecker _complianceChecker;
    
    public async Task<FinancialHealthReport> GenerateFinancialHealthReport(
        DateTime startDate, 
        DateTime endDate)
    {
        var report = new FinancialHealthReport
        {
            Period = new DateRange(startDate, endDate),
            GeneratedAt = DateTime.UtcNow
        };
        
        // Revenue analysis
        report.Revenue = await AnalyzeRevenue(startDate, endDate);
        
        // Transaction analysis
        report.Transactions = await AnalyzeTransactions(startDate, endDate);
        
        // Provider financials
        report.ProviderMetrics = await AnalyzeProviderFinancials(startDate, endDate);
        
        // Risk assessment
        report.RiskAssessment = await AssessFinancialRisks();
        
        // Compliance check
        report.ComplianceStatus = await CheckCompliance(startDate, endDate);
        
        // Anomaly detection
        report.Anomalies = await DetectFinancialAnomalies(startDate, endDate);
        
        return report;
    }
    
    public async Task<List<FinancialAnomaly>> DetectFinancialAnomalies(
        DateTime startDate, 
        DateTime endDate)
    {
        var anomalies = new List<FinancialAnomaly>();
        
        // Unusual transaction patterns
        var transactions = await GetTransactions(startDate, endDate);
        
        // Statistical anomaly detection
        var stats = CalculateTransactionStatistics(transactions);
        
        foreach (var transaction in transactions)
        {
            var zScore = (transaction.Amount - stats.Mean) / stats.StandardDeviation;
            
            if (Math.Abs(zScore) > 3)
            {
                anomalies.Add(new FinancialAnomaly
                {
                    Type = AnomalyType.UnusualAmount,
                    Transaction = transaction,
                    Score = Math.Abs(zScore),
                    Description = $"Transaction amount ${transaction.Amount} is {Math.Abs(zScore):F1} standard deviations from mean"
                });
            }
        }
        
        // Velocity checks
        var velocityAnomalies = await CheckVelocityAnomalies(transactions);
        anomalies.AddRange(velocityAnomalies);
        
        // Pattern anomalies
        var patternAnomalies = await _fraudDetector.DetectPatternAnomalies(transactions);
        anomalies.AddRange(patternAnomalies);
        
        return anomalies.OrderByDescending(a => a.Score).ToList();
    }
    
    public async Task<ComplianceReport> CheckCompliance(
        DateTime startDate, 
        DateTime endDate)
    {
        var report = new ComplianceReport();
        
        // KYC compliance
        report.KYCCompliance = await CheckKYCCompliance();
        
        // AML checks
        report.AMLStatus = await CheckAMLCompliance(startDate, endDate);
        
        // Tax compliance
        report.TaxCompliance = await CheckTaxCompliance(startDate, endDate);
        
        // Data protection compliance
        report.DataProtection = await CheckDataProtectionCompliance();
        
        // Generate required reports
        if (report.RequiresReporting())
        {
            await GenerateComplianceReports(report);
        }
        
        return report;
    }
}
```

### System Configuration Management

```csharp
public class PlatformConfigurationService
{
    private readonly IConfigRepository _repository;
    private readonly IConfigValidator _validator;
    private readonly IConfigCache _cache;
    private readonly IConfigAuditor _auditor;
    
    public async Task<T> GetSetting<T>(string category, string key)
    {
        var cacheKey = $"config:{category}:{key}";
        
        var cached = await _cache.GetAsync<T>(cacheKey);
        if (cached != null) return cached;
        
        var setting = await _repository.GetSetting(category, key);
        if (setting == null)
        {
            throw new ConfigurationException($"Setting {category}.{key} not found");
        }
        
        var value = ConvertValue<T>(setting.Value, setting.ValueType);
        
        await _cache.SetAsync(cacheKey, value, TimeSpan.FromMinutes(5));
        
        return value;
    }
    
    public async Task UpdateSetting(
        string category, 
        string key, 
        object value, 
        string userId)
    {
        var setting = await _repository.GetSetting(category, key);
        if (setting == null)
        {
            throw new ConfigurationException($"Setting {category}.{key} not found");
        }
        
        // Validate new value
        var validation = await _validator.ValidateSetting(setting, value);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        // Store old value for audit
        var oldValue = setting.Value;
        
        // Update setting
        setting.Value = SerializeValue(value);
        setting.LastModifiedBy = userId;
        setting.UpdatedAt = DateTime.UtcNow;
        
        await _repository.UpdateSetting(setting);
        
        // Clear cache
        await _cache.RemoveAsync($"config:{category}:{key}");
        await _cache.RemoveAsync($"config:{category}:*");
        
        // Audit the change
        await _auditor.AuditConfigChange(new ConfigAudit
        {
            UserId = userId,
            Category = category,
            Key = key,
            OldValue = oldValue,
            NewValue = setting.Value,
            Timestamp = DateTime.UtcNow
        });
        
        // Notify dependent services
        await NotifyConfigChange(category, key, value);
    }
    
    public async Task<Dictionary<string, object>> ExportConfiguration()
    {
        var settings = await _repository.GetAllSettings();
        var export = new Dictionary<string, object>();
        
        foreach (var categoryGroup in settings.GroupBy(s => s.Category))
        {
            var categorySettings = new Dictionary<string, object>();
            
            foreach (var setting in categoryGroup)
            {
                if (!setting.IsSensitive)
                {
                    categorySettings[setting.Key] = ConvertValue<object>(
                        setting.Value, 
                        setting.ValueType);
                }
            }
            
            export[categoryGroup.Key] = categorySettings;
        }
        
        return export;
    }
}
```

## Security Considerations

### Admin Access Security
1. **Multi-Factor Authentication**: Required for all admin accounts
2. **IP Whitelisting**: Restrict admin access by IP
3. **Session Management**: Short session timeouts, secure tokens
4. **Audit Trail**: Every admin action logged

### Data Protection
```csharp
[ApiController]
[Route("api/v1/admin")]
[RequireAdminPermission("admin.access")]
public class AdminBaseController : ControllerBase
{
    protected readonly IAdminAuditService _auditService;
    
    protected async Task<IActionResult> ExecuteAdminAction<T>(
        Func<Task<T>> action,
        string actionName,
        object actionData = null)
    {
        var userId = User.GetUserId();
        var requestId = Guid.NewGuid();
        
        try
        {
            // Log action start
            await _auditService.LogActionStart(new AdminAction
            {
                RequestId = requestId,
                UserId = userId,
                Action = actionName,
                Data = actionData,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"]
            });
            
            // Execute action
            var result = await action();
            
            // Log success
            await _auditService.LogActionComplete(requestId, result);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log failure
            await _auditService.LogActionFailed(requestId, ex);
            
            throw;
        }
    }
}
```

## Performance Optimization

### Admin Dashboard Performance
```csharp
public class AdminDashboardCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<AdminDashboardData> GetDashboardData(AdminUser user)
    {
        var cacheKey = $"admin:dashboard:{user.Id}";
        
        // Try memory cache
        if (_memoryCache.TryGetValue(cacheKey, out AdminDashboardData cached))
            return cached;
        
        // Try distributed cache
        var distributedCached = await _distributedCache.GetAsync<AdminDashboardData>(cacheKey);
        if (distributedCached != null)
        {
            _memoryCache.Set(cacheKey, distributedCached, TimeSpan.FromMinutes(1));
            return distributedCached;
        }
        
        // Generate dashboard data
        var data = await GenerateDashboardData(user);
        
        // Cache with different TTLs based on data volatility
        await CacheDashboardData(cacheKey, data);
        
        return data;
    }
    
    private async Task CacheDashboardData(string key, AdminDashboardData data)
    {
        // Memory cache - very short TTL
        _memoryCache.Set(key, data, TimeSpan.FromSeconds(30));
        
        // Distributed cache - longer TTL
        await _distributedCache.SetAsync(key, data, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        });
    }
}
```

### Batch Operations
```csharp
public class AdminBatchOperationService
{
    public async Task<BatchOperationResult> ProcessBatchOperation(
        BatchOperation operation,
        IProgress<BatchOperationProgress> progress = null)
    {
        var result = new BatchOperationResult
        {
            OperationId = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            TotalItems = operation.Items.Count
        };
        
        var completed = 0;
        var errors = new ConcurrentBag<BatchOperationError>();
        
        await Parallel.ForEachAsync(
            operation.Items.Batch(operation.BatchSize ?? 100),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = operation.MaxParallelism ?? 5
            },
            async (batch, ct) =>
            {
                foreach (var item in batch)
                {
                    try
                    {
                        await ProcessItem(operation.Action, item);
                        Interlocked.Increment(ref completed);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new BatchOperationError
                        {
                            ItemId = item.Id,
                            Error = ex.Message
                        });
                    }
                    
                    // Report progress
                    progress?.Report(new BatchOperationProgress
                    {
                        Completed = completed,
                        Total = operation.Items.Count,
                        Errors = errors.Count
                    });
                }
            });
        
        result.CompletedAt = DateTime.UtcNow;
        result.SuccessCount = completed - errors.Count;
        result.Errors = errors.ToList();
        
        return result;
    }
}
```

## Testing Strategy

### Admin Feature Testing
```csharp
[TestClass]
public class AdminAuthorizationTests
{
    [TestMethod]
    public async Task AdminAction_WithoutPermission_ReturnsForbidden()
    {
        // Arrange
        var user = CreateAdminUser("admin.users.read"); // Has read, not write
        
        // Act
        var response = await _client
            .WithAuth(user)
            .PutAsync("/api/v1/admin/users/123", new { status = "suspended" });
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [TestMethod]
    public async Task AdminAction_WithPermission_LogsAudit()
    {
        // Arrange
        var user = CreateAdminUser("admin.users.write");
        
        // Act
        await _client
            .WithAuth(user)
            .PutAsync("/api/v1/admin/users/123", new { status = "active" });
        
        // Assert
        var audit = await _auditRepository.GetLatestByUser(user.Id);
        Assert.IsNotNull(audit);
        Assert.AreEqual("UpdateUser", audit.Action);
    }
}
```