# Advanced User Account Management - Technical Specification

## Component Overview
The Advanced User Account Management system provides comprehensive privacy controls, account security features, data portability, subscription management, and communication preferences, ensuring GDPR compliance and giving users complete control over their personal data.

## Database Schema

### Account Management Tables

```sql
-- UserPrivacySettings
CREATE TABLE [dbo].[UserPrivacySettings] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [ProfileVisibility] INT NOT NULL DEFAULT 1, -- 0: Private, 1: Connections, 2: Public
    [ShowEmail] BIT NOT NULL DEFAULT 0,
    [ShowPhone] BIT NOT NULL DEFAULT 0,
    [ShowLocation] BIT NOT NULL DEFAULT 1,
    [ShowLastActive] BIT NOT NULL DEFAULT 1,
    [AllowMessagesFrom] INT NOT NULL DEFAULT 1, -- 0: Nobody, 1: Connections, 2: Everyone
    [AllowBookingRequests] BIT NOT NULL DEFAULT 1,
    [ShareDataWithPartners] BIT NOT NULL DEFAULT 0,
    [AllowAnalytics] BIT NOT NULL DEFAULT 1,
    [AllowMarketing] BIT NOT NULL DEFAULT 0,
    [SearchEngineIndexing] BIT NOT NULL DEFAULT 1,
    [ShowInProviderSearch] BIT NOT NULL DEFAULT 1,
    [DataRetentionPeriod] INT NULL, -- Days, NULL = indefinite
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserPrivacySettings_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserSecuritySettings
CREATE TABLE [dbo].[UserSecuritySettings] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [TwoFactorMethod] INT NOT NULL DEFAULT 0, -- 0: None, 1: SMS, 2: Authenticator, 3: Email
    [TwoFactorPhone] NVARCHAR(50) NULL,
    [TwoFactorEmail] NVARCHAR(256) NULL,
    [AuthenticatorKey] NVARCHAR(200) NULL, -- Encrypted
    [RecoveryCodes] NVARCHAR(MAX) NULL, -- Encrypted JSON array
    [LastPasswordChange] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [PasswordExpiryDays] INT NULL,
    [SessionTimeout] INT NOT NULL DEFAULT 30, -- Minutes
    [LoginNotifications] BIT NOT NULL DEFAULT 1,
    [UnknownDeviceAlerts] BIT NOT NULL DEFAULT 1,
    [RequireReauthentication] INT NOT NULL DEFAULT 0, -- 0: Never, 1: Sensitive, 2: Always
    [AllowedIPs] NVARCHAR(MAX) NULL, -- JSON array
    [BlockedIPs] NVARCHAR(MAX) NULL, -- JSON array
    [SecurityQuestions] NVARCHAR(MAX) NULL, -- Encrypted JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserSecuritySettings_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserDevices
CREATE TABLE [dbo].[UserDevices] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [DeviceId] NVARCHAR(200) NOT NULL,
    [DeviceName] NVARCHAR(100) NULL,
    [DeviceType] NVARCHAR(50) NOT NULL,
    [Platform] NVARCHAR(50) NULL,
    [Browser] NVARCHAR(50) NULL,
    [LastActiveAt] DATETIME2 NOT NULL,
    [LastIP] NVARCHAR(45) NULL,
    [LastLocation] NVARCHAR(200) NULL,
    [IsTrusted] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserDevices_UserId] ([UserId]),
    UNIQUE [IX_UserDevices_UserId_DeviceId] ([UserId], [DeviceId]),
    CONSTRAINT [FK_UserDevices_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserSessions
CREATE TABLE [dbo].[UserSessions] (
    [Id] NVARCHAR(128) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [DeviceId] INT NOT NULL,
    [IpAddress] NVARCHAR(45) NOT NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(200) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActivityAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [RevokedAt] DATETIME2 NULL,
    [RevokeReason] NVARCHAR(200) NULL,
    INDEX [IX_UserSessions_UserId] ([UserId]),
    INDEX [IX_UserSessions_ExpiresAt] ([ExpiresAt]),
    CONSTRAINT [FK_UserSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserSessions_Devices] FOREIGN KEY ([DeviceId]) REFERENCES [UserDevices]([Id])
);

-- UserDataExports
CREATE TABLE [dbo].[UserDataExports] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [RequestType] INT NOT NULL, -- 0: DataExport, 1: DataDeletion
    [Status] INT NOT NULL, -- 0: Pending, 1: Processing, 2: Completed, 3: Failed
    [Format] NVARCHAR(20) NULL, -- JSON, CSV, PDF
    [IncludeData] NVARCHAR(MAX) NULL, -- JSON array of data types
    [FileUrl] NVARCHAR(500) NULL,
    [FileSize] BIGINT NULL,
    [ExpiresAt] DATETIME2 NULL,
    [ProcessingStartedAt] DATETIME2 NULL,
    [CompletedAt] DATETIME2 NULL,
    [FailureReason] NVARCHAR(500) NULL,
    [RequestedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserDataExports_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserConnections
CREATE TABLE [dbo].[UserConnections] (
    [UserId] NVARCHAR(450) NOT NULL,
    [ConnectedUserId] NVARCHAR(450) NOT NULL,
    [ConnectionType] INT NOT NULL, -- 0: Favorite, 1: Blocked, 2: Friend
    [ConnectionStatus] INT NOT NULL DEFAULT 0, -- 0: Pending, 1: Accepted, 2: Rejected
    [Notes] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([UserId], [ConnectedUserId], [ConnectionType]),
    CONSTRAINT [FK_UserConnections_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserConnections_ConnectedUser] FOREIGN KEY ([ConnectedUserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserNotificationSettings
CREATE TABLE [dbo].[UserNotificationSettings] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [EmailNotifications] BIT NOT NULL DEFAULT 1,
    [SMSNotifications] BIT NOT NULL DEFAULT 0,
    [PushNotifications] BIT NOT NULL DEFAULT 1,
    [InAppNotifications] BIT NOT NULL DEFAULT 1,
    [NotificationSchedule] NVARCHAR(MAX) NULL, -- JSON (quiet hours, days)
    [DigestFrequency] INT NOT NULL DEFAULT 1, -- 0: Never, 1: Daily, 2: Weekly, 3: Monthly
    [Categories] NVARCHAR(MAX) NOT NULL, -- JSON object of category preferences
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserNotificationSettings_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserSubscriptions
CREATE TABLE [dbo].[UserSubscriptions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [SubscriptionType] INT NOT NULL, -- 0: PremiumProvider, 1: Analytics, 2: Marketing
    [PlanId] NVARCHAR(100) NOT NULL,
    [Status] INT NOT NULL, -- 0: Active, 1: Cancelled, 2: Expired, 3: Paused
    [CurrentPeriodStart] DATETIME2 NOT NULL,
    [CurrentPeriodEnd] DATETIME2 NOT NULL,
    [CancelledAt] DATETIME2 NULL,
    [CancellationReason] NVARCHAR(500) NULL,
    [PausedAt] DATETIME2 NULL,
    [ResumeAt] DATETIME2 NULL,
    [StripeSubscriptionId] NVARCHAR(100) NULL,
    [PaymentMethodId] INT NULL,
    [TrialEndsAt] DATETIME2 NULL,
    [Features] NVARCHAR(MAX) NULL, -- JSON array
    [UsageLimits] NVARCHAR(MAX) NULL, -- JSON object
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserSubscriptions_UserId_Status] ([UserId], [Status]),
    CONSTRAINT [FK_UserSubscriptions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserActivityLog
CREATE TABLE [dbo].[UserActivityLog] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [ActivityType] NVARCHAR(100) NOT NULL,
    [ActivityDescription] NVARCHAR(500) NOT NULL,
    [EntityType] NVARCHAR(50) NULL,
    [EntityId] NVARCHAR(100) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [DeviceId] INT NULL,
    [SessionId] NVARCHAR(128) NULL,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserActivityLog_UserId_Timestamp] ([UserId], [Timestamp] DESC),
    INDEX [IX_UserActivityLog_Timestamp] ([Timestamp]),
    CONSTRAINT [FK_UserActivityLog_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UserActivityLog_Devices] FOREIGN KEY ([DeviceId]) REFERENCES [UserDevices]([Id]),
    CONSTRAINT [FK_UserActivityLog_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [UserSessions]([Id])
);

-- UserConsentLog
CREATE TABLE [dbo].[UserConsentLog] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [ConsentType] NVARCHAR(50) NOT NULL, -- Privacy, Marketing, DataSharing, Cookies
    [ConsentVersion] NVARCHAR(20) NOT NULL,
    [Granted] BIT NOT NULL,
    [ConsentText] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserConsentLog_UserId_ConsentType] ([UserId], [ConsentType]),
    CONSTRAINT [FK_UserConsentLog_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserDeletionRequests
CREATE TABLE [dbo].[UserDeletionRequests] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [Reason] NVARCHAR(500) NULL,
    [ScheduledDeletionDate] DATETIME2 NOT NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0: Pending, 1: Processing, 2: Completed, 3: Cancelled
    [DataRetentionItems] NVARCHAR(MAX) NULL, -- JSON (legal requirements)
    [ConfirmationToken] NVARCHAR(200) NOT NULL,
    [ConfirmedAt] DATETIME2 NULL,
    [ProcessedAt] DATETIME2 NULL,
    [CancelledAt] DATETIME2 NULL,
    [RequestedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserDeletionRequests_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);
```

## API Endpoints

### Account Settings

```yaml
/api/v1/account:
  /profile:
    GET:
      description: Get user profile
      auth: required
      responses:
        200:
          profile: UserProfile
          completeness: number # 0-100
          missingFields: array[string]

    PUT:
      description: Update profile
      auth: required
      body:
        firstName: string
        lastName: string
        dateOfBirth: date
        phone: string
        address: Address
        bio: string
      responses:
        200: Updated profile

    DELETE:
      description: Request account deletion
      auth: required
      body:
        reason: string
        password: string # Confirmation
      responses:
        200:
          deletionToken: string
          scheduledDate: datetime
          instructions: string

  /profile/photo:
    POST:
      description: Upload profile photo
      auth: required
      contentType: multipart/form-data
      body:
        photo: file
      responses:
        200:
          photoUrl: string

    DELETE:
      description: Remove profile photo
      auth: required
      responses:
        204: Photo removed

  /email:
    PUT:
      description: Change email address
      auth: required
      body:
        newEmail: string
        password: string
      responses:
        200:
          verificationRequired: boolean
          verificationSent: boolean

  /password:
    PUT:
      description: Change password
      auth: required
      body:
        currentPassword: string
        newPassword: string
      responses:
        200:
          changed: boolean
          sessionsRevoked: number

  /username:
    PUT:
      description: Change username
      auth: required
      body:
        newUsername: string
        password: string
      responses:
        200:
          changed: boolean
```

### Privacy Settings

```yaml
/api/v1/account/privacy:
  /:
    GET:
      description: Get privacy settings
      auth: required
      responses:
        200:
          settings: PrivacySettings
          recommendations: array[PrivacyRecommendation]

    PUT:
      description: Update privacy settings
      auth: required
      body:
        profileVisibility: enum
        showEmail: boolean
        showPhone: boolean
        showLocation: boolean
        showLastActive: boolean
        allowMessagesFrom: enum
        searchEngineIndexing: boolean
      responses:
        200:
          updated: PrivacySettings
          impact: PrivacyImpact

  /connections:
    GET:
      description: Get connections (friends, blocked)
      auth: required
      parameters:
        type: enum [favorites, blocked, friends]
      responses:
        200:
          connections: array[UserConnection]

    POST:
      description: Add connection
      auth: required
      body:
        userId: string
        type: enum
        notes: string
      responses:
        201:
          connectionId: string

    DELETE:
      description: Remove connection
      auth: required
      parameters:
        userId: string
        type: enum
      responses:
        204: Removed

  /blocked-users:
    GET:
      description: Get blocked users
      auth: required
      responses:
        200:
          blockedUsers: array[BlockedUser]

    POST:
      description: Block user
      auth: required
      body:
        userId: string
        reason: string
      responses:
        200:
          blocked: boolean

    DELETE:
      description: Unblock user
      auth: required
      parameters:
        userId: string
      responses:
        204: Unblocked

  /data-sharing:
    GET:
      description: Get data sharing settings
      auth: required
      responses:
        200:
          partners: array[DataPartner]
          settings: DataSharingSettings

    PUT:
      description: Update data sharing
      auth: required
      body:
        shareWithPartners: boolean
        allowAnalytics: boolean
        allowMarketing: boolean
        specificPartners: object
      responses:
        200: Updated settings
```

### Security Settings

```yaml
/api/v1/account/security:
  /:
    GET:
      description: Get security overview
      auth: required
      responses:
        200:
          settings: SecuritySettings
          score: number # Security score 0-100
          recommendations: array[SecurityRecommendation]
          recentActivity: array[SecurityEvent]

  /two-factor:
    GET:
      description: Get 2FA status
      auth: required
      responses:
        200:
          enabled: boolean
          method: enum
          backupCodesRemaining: number

    POST:
      description: Enable 2FA
      auth: required
      body:
        method: enum [sms, authenticator, email]
        phoneNumber: string # If SMS
      responses:
        200:
          qrCode: string # If authenticator
          secret: string # If authenticator
          verificationRequired: boolean

    PUT:
      description: Verify and enable 2FA
      auth: required
      body:
        verificationCode: string
      responses:
        200:
          enabled: boolean
          backupCodes: array[string]

    DELETE:
      description: Disable 2FA
      auth: required
      body:
        password: string
        code: string # Current 2FA code
      responses:
        200:
          disabled: boolean

  /backup-codes:
    GET:
      description: View backup codes
      auth: required
      body:
        password: string
      responses:
        200:
          codes: array[string]
          generatedAt: datetime

    POST:
      description: Regenerate backup codes
      auth: required
      body:
        password: string
      responses:
        200:
          codes: array[string]

  /sessions:
    GET:
      description: Get active sessions
      auth: required
      responses:
        200:
          currentSession: Session
          otherSessions: array[Session]

    DELETE:
      description: Revoke session(s)
      auth: required
      parameters:
        sessionId: string # Optional, all if not provided
      responses:
        200:
          revokedCount: number

  /devices:
    GET:
      description: Get trusted devices
      auth: required
      responses:
        200:
          devices: array[TrustedDevice]

    PUT:
      description: Update device trust
      auth: required
      body:
        deviceId: string
        trusted: boolean
        name: string
      responses:
        200: Updated device

    DELETE:
      description: Remove device
      auth: required
      parameters:
        deviceId: string
      responses:
        204: Removed

  /security-log:
    GET:
      description: Get security activity log
      auth: required
      parameters:
        startDate: date
        endDate: date
        eventType: string
      responses:
        200:
          events: array[SecurityEvent]
          suspicious: array[SuspiciousActivity]
```

### Data Management

```yaml
/api/v1/account/data:
  /export:
    POST:
      description: Request data export (GDPR)
      auth: required
      body:
        format: enum [json, csv, pdf]
        includeData: array[string] # Data categories
        password: string # Confirmation
      responses:
        201:
          requestId: string
          estimatedTime: number
          notificationMethod: string

  /export/{requestId}:
    GET:
      description: Check export status
      auth: required
      responses:
        200:
          status: enum
          progress: number
          downloadUrl: string # If completed
          expiresAt: datetime

  /deletion:
    POST:
      description: Request account deletion
      auth: required
      body:
        reason: string
        password: string
        feedback: string
      responses:
        200:
          confirmationToken: string
          scheduledDate: datetime
          retentionNotice: string

  /deletion/confirm:
    POST:
      description: Confirm deletion request
      auth: required
      body:
        token: string
      responses:
        200:
          confirmed: boolean
          deletionDate: datetime

  /deletion/cancel:
    POST:
      description: Cancel deletion request
      auth: required
      body:
        reason: string
      responses:
        200:
          cancelled: boolean

  /consent:
    GET:
      description: Get consent history
      auth: required
      responses:
        200:
          consents: array[ConsentRecord]
          current: ConsentStatus

    POST:
      description: Update consent
      auth: required
      body:
        consentType: string
        granted: boolean
        version: string
      responses:
        200:
          recorded: boolean

  /activity:
    GET:
      description: Get activity log
      auth: required
      parameters:
        startDate: date
        endDate: date
        activityType: string
        page: number
        pageSize: number
      responses:
        200:
          activities: array[ActivityEntry]
          summary: ActivitySummary
```

### Notification Preferences

```yaml
/api/v1/account/notifications:
  /:
    GET:
      description: Get notification settings
      auth: required
      responses:
        200:
          settings: NotificationSettings
          channels: array[NotificationChannel]
          categories: array[NotificationCategory]

    PUT:
      description: Update notification settings
      auth: required
      body:
        email: boolean
        sms: boolean
        push: boolean
        inApp: boolean
        digestFrequency: enum
        quietHours: object
        categories: object
      responses:
        200: Updated settings

  /test:
    POST:
      description: Send test notification
      auth: required
      body:
        channel: enum [email, sms, push]
        type: string
      responses:
        200:
          sent: boolean

  /unsubscribe:
    POST:
      description: Unsubscribe from notifications
      auth: required
      body:
        token: string # From email link
        categories: array[string] # Specific categories
      responses:
        200:
          unsubscribed: array[string]
```

### Subscription Management

```yaml
/api/v1/account/subscriptions:
  /:
    GET:
      description: Get subscriptions
      auth: required
      responses:
        200:
          active: array[Subscription]
          cancelled: array[Subscription]
          available: array[SubscriptionPlan]

    POST:
      description: Create subscription
      auth: required
      body:
        planId: string
        paymentMethodId: string
        promoCode: string # Optional
      responses:
        201:
          subscription: Subscription
          invoice: Invoice

  /{subscriptionId}:
    GET:
      description: Get subscription details
      auth: required
      responses:
        200:
          subscription: SubscriptionDetail
          usage: UsageData
          invoices: array[Invoice]

    PUT:
      description: Update subscription
      auth: required
      body:
        action: enum [upgrade, downgrade, pause, resume]
        newPlanId: string # If upgrade/downgrade
        effectiveDate: date
      responses:
        200:
          updated: Subscription
          prorationAmount: number

    DELETE:
      description: Cancel subscription
      auth: required
      body:
        reason: string
        feedback: string
        immediately: boolean
      responses:
        200:
          cancelledAt: datetime
          endsAt: datetime

  /{subscriptionId}/usage:
    GET:
      description: Get usage data
      auth: required
      parameters:
        period: enum [current, previous, all]
      responses:
        200:
          usage: UsageMetrics
          limits: UsageLimits
          overage: OverageDetails
```

## Frontend Components

### Account Settings Components (Vue.js)

```typescript
// AccountSettings.vue
interface AccountSettingsProps {
  user: User
  section?: string
}

// Main sections:
// - ProfileSettings.vue
// - PrivacySettings.vue
// - SecuritySettings.vue
// - NotificationSettings.vue
// - SubscriptionSettings.vue
// - DataManagement.vue

// ProfileSettings.vue
interface ProfileSettingsProps {
  profile: UserProfile
  onUpdate: (profile: Partial<UserProfile>) => void
}

// Features:
// - Form validation
// - Photo upload/crop
// - Address autocomplete
// - Change preview
```

### Privacy Control Components

```typescript
// PrivacyControls.vue
interface PrivacyControlsProps {
  settings: PrivacySettings
  onChange: (settings: PrivacySettings) => void
}

// Controls:
// - VisibilitySelector.vue
// - DataSharingToggle.vue
// - BlockedUsersList.vue
// - ConnectionManager.vue

// PrivacyScoreWidget.vue
interface PrivacyScoreWidgetProps {
  score: number
  recommendations: PrivacyRecommendation[]
}

// Features:
// - Visual score meter
// - Actionable tips
// - Before/after preview
```

### Security Components

```typescript
// SecurityDashboard.vue
interface SecurityDashboardProps {
  settings: SecuritySettings
  events: SecurityEvent[]
}

// Sections:
// - TwoFactorSetup.vue
// - SessionManager.vue
// - DeviceManager.vue
// - SecurityActivityLog.vue

// TwoFactorSetup.vue
interface TwoFactorSetupProps {
  currentMethod?: TwoFactorMethod
  onEnable: (method: TwoFactorMethod) => void
}

// Steps:
// - Method selection
// - QR code/phone verify
// - Backup codes display
// - Test verification
```

### Data Management Components

```typescript
// DataPortability.vue
interface DataPortabilityProps {
  exports: DataExport[]
  onExport: (options: ExportOptions) => void
}

// Features:
// - Export format selection
// - Data category picker
// - Progress tracking
// - Download manager

// AccountDeletion.vue
interface AccountDeletionProps {
  onDelete: (reason: string) => void
}

// Steps:
// - Reason selection
// - Data retention info
// - Alternative options
// - Final confirmation
```

## Technical Implementation Details

### Privacy Engine

```csharp
public class PrivacyEngine
{
    private readonly IPrivacyRepository _repository;
    private readonly IDataMaskingService _maskingService;
    
    public async Task<object> ApplyPrivacyFilter(
        object data, 
        string viewerId, 
        string ownerId)
    {
        if (viewerId == ownerId) return data; // Owner sees everything
        
        var privacySettings = await _repository.GetPrivacySettings(ownerId);
        var relationship = await GetRelationship(viewerId, ownerId);
        
        return ApplyPrivacyRules(data, privacySettings, relationship);
    }
    
    private object ApplyPrivacyRules(
        object data, 
        PrivacySettings settings, 
        UserRelationship relationship)
    {
        var filtered = data.DeepClone();
        
        // Apply visibility rules
        if (settings.ProfileVisibility == ProfileVisibility.Private && 
            relationship != UserRelationship.Connected)
        {
            return null; // No access
        }
        
        // Mask sensitive fields
        if (!settings.ShowEmail && filtered.HasProperty("Email"))
        {
            filtered.Email = _maskingService.MaskEmail(filtered.Email);
        }
        
        if (!settings.ShowPhone && filtered.HasProperty("Phone"))
        {
            filtered.Phone = _maskingService.MaskPhone(filtered.Phone);
        }
        
        if (!settings.ShowLocation && filtered.HasProperty("Location"))
        {
            filtered.Location = _maskingService.GeneralizeLocation(filtered.Location);
        }
        
        if (!settings.ShowLastActive && filtered.HasProperty("LastActive"))
        {
            filtered.LastActive = null;
        }
        
        return filtered;
    }
    
    public async Task<bool> CanUserContact(string senderId, string recipientId)
    {
        var settings = await _repository.GetPrivacySettings(recipientId);
        var relationship = await GetRelationship(senderId, recipientId);
        
        // Check if blocked
        if (await IsBlocked(senderId, recipientId))
            return false;
        
        return settings.AllowMessagesFrom switch
        {
            MessagePermission.Nobody => false,
            MessagePermission.Connections => relationship == UserRelationship.Connected,
            MessagePermission.Everyone => true,
            _ => false
        };
    }
}
```

### Security Service

```csharp
public class AccountSecurityService
{
    private readonly IAuthenticationService _authService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly ISessionManager _sessionManager;
    private readonly ISecurityEventLogger _eventLogger;
    
    public async Task<SecurityScore> CalculateSecurityScore(string userId)
    {
        var score = 100; // Start with perfect score
        var factors = new List<SecurityFactor>();
        
        // Password strength
        var passwordAge = await GetPasswordAge(userId);
        if (passwordAge > TimeSpan.FromDays(180))
        {
            score -= 10;
            factors.Add(new SecurityFactor
            {
                Name = "Password Age",
                Impact = -10,
                Recommendation = "Change your password regularly"
            });
        }
        
        // Two-factor authentication
        var has2FA = await _twoFactorService.IsEnabled(userId);
        if (!has2FA)
        {
            score -= 25;
            factors.Add(new SecurityFactor
            {
                Name = "Two-Factor Authentication",
                Impact = -25,
                Recommendation = "Enable 2FA for better security"
            });
        }
        
        // Suspicious activity
        var suspiciousEvents = await _eventLogger.GetSuspiciousEvents(userId, 30);
        if (suspiciousEvents.Any())
        {
            score -= Math.Min(suspiciousEvents.Count * 5, 20);
            factors.Add(new SecurityFactor
            {
                Name = "Suspicious Activity",
                Impact = -suspiciousEvents.Count * 5,
                Recommendation = "Review your security log"
            });
        }
        
        // Trusted devices
        var trustedDevices = await _sessionManager.GetTrustedDevices(userId);
        if (!trustedDevices.Any())
        {
            score -= 5;
            factors.Add(new SecurityFactor
            {
                Name = "Trusted Devices",
                Impact = -5,
                Recommendation = "Mark your regular devices as trusted"
            });
        }
        
        return new SecurityScore
        {
            Score = Math.Max(0, score),
            Factors = factors,
            LastCalculated = DateTime.UtcNow
        };
    }
    
    public async Task<TwoFactorSetupResult> SetupTwoFactor(
        string userId, 
        TwoFactorMethod method)
    {
        switch (method)
        {
            case TwoFactorMethod.Authenticator:
                return await SetupAuthenticatorTwoFactor(userId);
                
            case TwoFactorMethod.SMS:
                return await SetupSMSTwoFactor(userId);
                
            case TwoFactorMethod.Email:
                return await SetupEmailTwoFactor(userId);
                
            default:
                throw new NotSupportedException($"Method {method} not supported");
        }
    }
    
    private async Task<TwoFactorSetupResult> SetupAuthenticatorTwoFactor(string userId)
    {
        var key = GenerateAuthenticatorKey();
        var user = await _userManager.FindByIdAsync(userId);
        
        await _userManager.SetAuthenticatorKeyAsync(user, key);
        
        var uri = GenerateQrCodeUri(user.Email, key);
        
        return new TwoFactorSetupResult
        {
            Method = TwoFactorMethod.Authenticator,
            SecretKey = FormatKey(key),
            QrCodeUri = uri,
            ManualEntryKey = key
        };
    }
    
    public async Task<SessionInfo[]> GetActiveSessions(string userId)
    {
        var sessions = await _sessionManager.GetUserSessions(userId);
        
        return sessions.Select(s => new SessionInfo
        {
            SessionId = s.Id,
            Device = s.Device,
            Location = s.Location,
            IpAddress = MaskIpAddress(s.IpAddress),
            LastActivity = s.LastActivityAt,
            IsCurrent = s.Id == GetCurrentSessionId(),
            CanRevoke = s.Id != GetCurrentSessionId()
        }).ToArray();
    }
    
    public async Task RevokeSession(string userId, string sessionId)
    {
        var session = await _sessionManager.GetSession(sessionId);
        
        if (session.UserId != userId)
            throw new UnauthorizedException("Cannot revoke another user's session");
        
        if (session.Id == GetCurrentSessionId())
            throw new InvalidOperationException("Cannot revoke current session");
        
        await _sessionManager.RevokeSession(sessionId, "User requested");
        
        await _eventLogger.LogSecurityEvent(new SecurityEvent
        {
            UserId = userId,
            EventType = SecurityEventType.SessionRevoked,
            Details = $"Session {sessionId} revoked by user",
            IpAddress = GetClientIp(),
            Timestamp = DateTime.UtcNow
        });
    }
}
```

### Data Export Service

```csharp
public class UserDataExportService
{
    private readonly IDataCollector _dataCollector;
    private readonly IDataFormatter _formatter;
    private readonly IStorageService _storage;
    private readonly INotificationService _notifications;
    
    public async Task<string> InitiateDataExport(string userId, ExportOptions options)
    {
        var export = new UserDataExport
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            RequestType = DataRequestType.Export,
            Status = ExportStatus.Pending,
            Format = options.Format,
            IncludeData = options.DataCategories,
            RequestedAt = DateTime.UtcNow
        };
        
        await _repository.SaveExport(export);
        
        // Queue background job
        _backgroundJobs.Enqueue(() => ProcessDataExport(export.Id));
        
        return export.Id;
    }
    
    public async Task ProcessDataExport(string exportId)
    {
        var export = await _repository.GetExport(exportId);
        
        try
        {
            export.Status = ExportStatus.Processing;
            export.ProcessingStartedAt = DateTime.UtcNow;
            await _repository.UpdateExport(export);
            
            // Collect all user data
            var userData = await CollectUserData(export.UserId, export.IncludeData);
            
            // Format data
            var formatted = await FormatData(userData, export.Format);
            
            // Store file
            var fileUrl = await StoreExportFile(export.UserId, formatted);
            
            export.Status = ExportStatus.Completed;
            export.FileUrl = fileUrl;
            export.FileSize = formatted.Length;
            export.CompletedAt = DateTime.UtcNow;
            export.ExpiresAt = DateTime.UtcNow.AddDays(7);
            
            await _repository.UpdateExport(export);
            
            // Notify user
            await _notifications.SendDataExportReady(export.UserId, export.Id);
        }
        catch (Exception ex)
        {
            export.Status = ExportStatus.Failed;
            export.FailureReason = ex.Message;
            await _repository.UpdateExport(export);
            
            throw;
        }
    }
    
    private async Task<UserDataPackage> CollectUserData(
        string userId, 
        List<string> categories)
    {
        var package = new UserDataPackage
        {
            ExportDate = DateTime.UtcNow,
            User = await GetUserProfile(userId)
        };
        
        var tasks = new List<Task>();
        
        if (categories.Contains("profile"))
        {
            tasks.Add(Task.Run(async () => 
                package.Profile = await _dataCollector.CollectProfileData(userId)));
        }
        
        if (categories.Contains("bookings"))
        {
            tasks.Add(Task.Run(async () => 
                package.Bookings = await _dataCollector.CollectBookings(userId)));
        }
        
        if (categories.Contains("messages"))
        {
            tasks.Add(Task.Run(async () => 
                package.Messages = await _dataCollector.CollectMessages(userId)));
        }
        
        if (categories.Contains("reviews"))
        {
            tasks.Add(Task.Run(async () => 
                package.Reviews = await _dataCollector.CollectReviews(userId)));
        }
        
        if (categories.Contains("financial"))
        {
            tasks.Add(Task.Run(async () => 
                package.Financial = await _dataCollector.CollectFinancialData(userId)));
        }
        
        if (categories.Contains("activity"))
        {
            tasks.Add(Task.Run(async () => 
                package.ActivityLog = await _dataCollector.CollectActivityLog(userId)));
        }
        
        await Task.WhenAll(tasks);
        
        return package;
    }
}
```

### Account Deletion Service

```csharp
public class AccountDeletionService
{
    private readonly IUserRepository _userRepository;
    private readonly IDataRetentionService _retentionService;
    private readonly INotificationService _notifications;
    
    public async Task<DeletionRequest> RequestDeletion(
        string userId, 
        string reason, 
        string feedback)
    {
        // Check for active subscriptions or pending transactions
        var blockers = await CheckDeletionBlockers(userId);
        if (blockers.Any())
        {
            throw new DeletionBlockedException(
                "Cannot delete account with active obligations", 
                blockers);
        }
        
        var request = new UserDeletionRequest
        {
            UserId = userId,
            Reason = reason,
            Feedback = feedback,
            ScheduledDeletionDate = DateTime.UtcNow.AddDays(30), // Grace period
            Status = DeletionStatus.Pending,
            ConfirmationToken = GenerateConfirmationToken(),
            RequestedAt = DateTime.UtcNow
        };
        
        await _repository.SaveDeletionRequest(request);
        
        // Send confirmation email
        await _notifications.SendDeletionConfirmation(userId, request);
        
        return request;
    }
    
    public async Task ProcessScheduledDeletions()
    {
        var dueDeletions = await _repository.GetDueDeletions();
        
        foreach (var deletion in dueDeletions)
        {
            try
            {
                await ProcessDeletion(deletion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to process deletion for user {UserId}", 
                    deletion.UserId);
            }
        }
    }
    
    private async Task ProcessDeletion(UserDeletionRequest request)
    {
        request.Status = DeletionStatus.Processing;
        await _repository.UpdateDeletionRequest(request);
        
        var user = await _userRepository.GetUser(request.UserId);
        
        // Anonymize data that must be retained
        var retentionItems = await _retentionService.IdentifyRetentionItems(user);
        foreach (var item in retentionItems)
        {
            await AnonymizeData(item);
        }
        
        // Delete user data
        await DeleteUserData(user.Id);
        
        // Delete user account
        await _userRepository.DeleteUser(user.Id);
        
        request.Status = DeletionStatus.Completed;
        request.ProcessedAt = DateTime.UtcNow;
        await _repository.UpdateDeletionRequest(request);
        
        // Send confirmation
        await _notifications.SendDeletionComplete(user.Email);
    }
    
    private async Task DeleteUserData(string userId)
    {
        // Delete in dependency order
        await _repository.DeleteUserMessages(userId);
        await _repository.DeleteUserReviews(userId);
        await _repository.DeleteUserBookings(userId);
        await _repository.DeleteUserPayments(userId);
        await _repository.DeleteUserDogs(userId);
        await _repository.DeleteUserMedia(userId);
        await _repository.DeleteUserSessions(userId);
        await _repository.DeleteUserDevices(userId);
        await _repository.DeleteUserPreferences(userId);
    }
}
```

### Consent Management

```csharp
public class ConsentManagementService
{
    private readonly IConsentRepository _repository;
    private readonly IConsentVersioning _versioning;
    
    public async Task<ConsentStatus> GetConsentStatus(string userId)
    {
        var consents = await _repository.GetUserConsents(userId);
        var currentVersions = await _versioning.GetCurrentVersions();
        
        return new ConsentStatus
        {
            Consents = currentVersions.Select(cv => new ConsentItem
            {
                Type = cv.Type,
                CurrentVersion = cv.Version,
                UserConsent = consents.FirstOrDefault(c => 
                    c.ConsentType == cv.Type && 
                    c.ConsentVersion == cv.Version),
                IsRequired = cv.IsRequired,
                NeedsUpdate = NeedsConsentUpdate(consents, cv)
            }).ToList()
        };
    }
    
    public async Task RecordConsent(
        string userId, 
        string consentType, 
        bool granted, 
        string version)
    {
        var consent = new UserConsentLog
        {
            UserId = userId,
            ConsentType = consentType,
            ConsentVersion = version,
            Granted = granted,
            ConsentText = await _versioning.GetConsentText(consentType, version),
            IpAddress = GetClientIp(),
            Timestamp = DateTime.UtcNow
        };
        
        await _repository.RecordConsent(consent);
        
        // Update user flags based on consent
        await UpdateUserFlags(userId, consentType, granted);
        
        // Track for compliance
        await _complianceTracker.TrackConsentChange(consent);
    }
    
    private async Task UpdateUserFlags(
        string userId, 
        string consentType, 
        bool granted)
    {
        var settings = await _repository.GetUserSettings(userId);
        
        switch (consentType)
        {
            case "marketing":
                settings.AllowMarketing = granted;
                break;
            case "data-sharing":
                settings.ShareDataWithPartners = granted;
                break;
            case "analytics":
                settings.AllowAnalytics = granted;
                break;
        }
        
        await _repository.UpdateUserSettings(settings);
    }
}
```

## Security Considerations

### Account Security
1. **Password Policy**:
   - Minimum 12 characters
   - Complexity requirements
   - Password history (no reuse of last 5)
   - Expiry notifications
2. **Multi-Factor Authentication**:
   - Multiple methods supported
   - Backup codes
   - Device trust
3. **Session Security**:
   - Secure session tokens
   - Activity monitoring
   - Automatic timeout

### Privacy Protection
```csharp
[ApiController]
[Route("api/v1/account")]
[Authorize]
public class AccountController : ControllerBase
{
    [HttpGet("profile")]
    [RateLimit(100, 1)] // 100 requests per minute
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var profile = await _profileService.GetProfile(userId);
        
        // Apply privacy filter for own data (may hide from analytics)
        var filtered = await _privacyEngine.ApplyPrivacyFilter(
            profile, 
            userId, 
            userId);
        
        return Ok(filtered);
    }
    
    [HttpDelete("profile")]
    [RequireRecentAuthentication(30)] // Must have authenticated in last 30 min
    public async Task<IActionResult> RequestDeletion(
        [FromBody] DeletionRequest request)
    {
        // Verify password
        var user = await _userManager.FindByIdAsync(User.GetUserId());
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid password");
        
        var deletion = await _deletionService.RequestDeletion(
            user.Id, 
            request.Reason, 
            request.Feedback);
        
        return Ok(new
        {
            deletion.ConfirmationToken,
            deletion.ScheduledDeletionDate,
            Instructions = "Check your email for confirmation instructions"
        });
    }
}
```

## Performance Optimization

### Settings Caching
```csharp
public class UserSettingsCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<T> GetSettings<T>(string userId) where T : class
    {
        var cacheKey = $"settings:{typeof(T).Name}:{userId}";
        
        // L1 Cache
        if (_cache.TryGetValue(cacheKey, out T cached))
            return cached;
        
        // L2 Cache
        var distributed = await _distributedCache.GetAsync<T>(cacheKey);
        if (distributed != null)
        {
            _cache.Set(cacheKey, distributed, TimeSpan.FromMinutes(5));
            return distributed;
        }
        
        // Load from database
        var settings = await LoadSettings<T>(userId);
        
        // Cache
        await _distributedCache.SetAsync(cacheKey, settings, 
            TimeSpan.FromHours(1));
        _cache.Set(cacheKey, settings, TimeSpan.FromMinutes(5));
        
        return settings;
    }
    
    public async Task InvalidateSettings(string userId)
    {
        var patterns = new[]
        {
            $"settings:*:{userId}",
            $"privacy:{userId}",
            $"security:{userId}"
        };
        
        foreach (var pattern in patterns)
        {
            await _distributedCache.RemoveByPatternAsync(pattern);
        }
    }
}
```

## Testing Strategy

### Privacy Testing
```csharp
[TestClass]
public class PrivacyEngineTests
{
    [TestMethod]
    public async Task ApplyPrivacyFilter_BlockedUser_ReturnsNull()
    {
        // Arrange
        var ownerId = "user1";
        var viewerId = "blocked-user";
        var profile = new UserProfile { Email = "test@example.com" };
        
        _mockRepo.Setup(r => r.IsBlocked(viewerId, ownerId))
            .ReturnsAsync(true);
        
        // Act
        var result = await _privacyEngine.ApplyPrivacyFilter(
            profile, 
            viewerId, 
            ownerId);
        
        // Assert
        Assert.IsNull(result);
    }
}
```

### Security Testing
```csharp
[TestClass]
public class SecurityServiceTests
{
    [TestMethod]
    public async Task EnableTwoFactor_ValidCode_EnablesSuccessfully()
    {
        // Arrange
        var userId = "test-user";
        var setup = await _securityService.SetupTwoFactor(
            userId, 
            TwoFactorMethod.Authenticator);
        
        var code = GenerateCodeFromSecret(setup.SecretKey);
        
        // Act
        var result = await _securityService.VerifyAndEnableTwoFactor(
            userId, 
            code);
        
        // Assert
        Assert.IsTrue(result.Enabled);
        Assert.AreEqual(10, result.BackupCodes.Count);
    }
}
```