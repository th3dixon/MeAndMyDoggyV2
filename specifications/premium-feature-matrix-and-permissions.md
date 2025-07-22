# Premium Feature Matrix and User Permissions Specification

## Overview

This document defines the comprehensive premium feature matrix, user permission system, and access control logic for the MeAndMyDoggy messaging and video calling system. It establishes clear boundaries between subscription tiers and ensures proper feature gating.

## User Types and Subscription Tiers

### User Types

1. **Pet Owners** - Individual users seeking pet services
2. **Service Providers** - Professionals offering pet services
3. **Emergency Experts** - Veterinarians and emergency pet care specialists
4. **Community Moderators** - Platform moderators and support staff

### Subscription Tiers

| Tier | Monthly Cost | Target Users | Primary Benefits |
|------|-------------|--------------|------------------|
| **Free** | £0 | All Users | Basic messaging, profile viewing, service discovery |
| **Premium** | £9.99 | Service Providers | Video calls, basic professional tools, priority support |
| **Premium+** | £19.99 | Professional Service Providers | Unlimited video, emergency access, advanced tools |
| **Enterprise** | £49.99 | Large Service Providers | White-label options, API access, advanced analytics |

## Comprehensive Feature Matrix

### Core Messaging Features

| Feature | Free | Premium | Premium+ | Enterprise |
|---------|------|---------|----------|------------|
| **Text Messaging** | ✅ Unlimited | ✅ Unlimited | ✅ Unlimited | ✅ Unlimited |
| **Photo Sharing** | ✅ 10/day | ✅ 50/day | ✅ Unlimited | ✅ Unlimited |
| **Video Sharing** | ❌ | ✅ 5/day (30s max) | ✅ Unlimited (5min max) | ✅ Unlimited |
| **File Attachments** | ❌ | ✅ 5MB max, 10/day | ✅ 25MB max, unlimited | ✅ 100MB max, unlimited |
| **Voice Messages** | ✅ 1min max, 5/day | ✅ 5min max, unlimited | ✅ Unlimited | ✅ Unlimited |
| **Message History** | ✅ 30 days | ✅ 1 year | ✅ Unlimited | ✅ Unlimited |
| **Message Search** | ❌ | ✅ Basic | ✅ Advanced + AI | ✅ Advanced + AI |
| **Message Export** | ❌ | ❌ | ✅ Available | ✅ Available |
| **Read Receipts** | ✅ Basic | ✅ Advanced | ✅ Advanced | ✅ Advanced |
| **Typing Indicators** | ✅ Available | ✅ Available | ✅ Available | ✅ Available |
| **Message Reactions** | ✅ Basic emojis | ✅ Extended set | ✅ Custom reactions | ✅ Custom reactions |

### Video & Voice Calling Features

| Feature | Free | Premium | Premium+ | Enterprise |
|---------|------|---------|----------|------------|
| **Voice Calls** | ✅ 10min/month | ✅ Unlimited | ✅ Unlimited | ✅ Unlimited |
| **Video Calls** | ❌ Upgrade Required | ✅ 5hrs/month, 30min/session | ✅ Unlimited | ✅ Unlimited |
| **Group Video Calls** | ❌ | ✅ Up to 4 people | ✅ Up to 8 people | ✅ Up to 25 people |
| **Call Recording** | ❌ | ✅ 10 recordings/month | ✅ Unlimited | ✅ Unlimited |
| **Call Transcription** | ❌ | ❌ | ✅ AI-powered | ✅ AI-powered |
| **Screen Sharing** | ❌ | ✅ Available | ✅ Available + annotation | ✅ Available + annotation |
| **Video Quality** | N/A | 720p HD | 1080p HD, 60fps option | 4K option |
| **Virtual Backgrounds** | ❌ | ✅ 5 preset backgrounds | ✅ Unlimited + custom upload | ✅ Unlimited + branding |
| **Noise Cancellation** | ❌ | ✅ Basic | ✅ Advanced AI | ✅ Advanced AI |
| **Call Analytics** | ❌ | ❌ | ✅ Basic stats | ✅ Detailed reports |
| **Priority Connection** | ❌ | ✅ Available | ✅ Highest priority | ✅ Dedicated servers |

### Professional Service Features

| Feature | Free | Premium | Premium+ | Enterprise |
|---------|------|---------|----------|------------|
| **Digital Whiteboard** | ❌ | ❌ | ✅ Available | ✅ Available |
| **Document Sharing** | ❌ | ✅ Basic | ✅ Advanced + markup | ✅ Advanced + markup |
| **Pet Health Forms** | ❌ | ❌ | ✅ Pre-built templates | ✅ Custom forms + API |
| **Appointment Scheduling** | ❌ | ✅ Basic calendar | ✅ Advanced + integrations | ✅ Full CRM integration |
| **Payment Processing** | ❌ | ❌ | ✅ In-call payments | ✅ Full payment suite |
| **Session Notes** | ❌ | ❌ | ✅ AI-generated | ✅ AI + custom templates |
| **Client Management** | ❌ | ✅ Basic profiles | ✅ Advanced CRM | ✅ Full business suite |
| **Custom Branding** | ❌ | ❌ | ✅ Logo overlay | ✅ Full white-label |

### Emergency & Specialized Features

| Feature | Free | Premium | Premium+ | Enterprise |
|---------|------|---------|----------|------------|
| **Emergency Consultation** | ❌ | ❌ | ✅ 24/7 access | ✅ Priority queue |
| **Emergency Queue Priority** | N/A | N/A | Standard | Highest |
| **Multi-Expert Conference** | ❌ | ❌ | ✅ Up to 3 experts | ✅ Up to 5 experts |
| **Emergency Recording** | ❌ | ❌ | ✅ Automatic | ✅ Automatic + transcription |
| **Location Services** | Basic | ✅ Enhanced | ✅ Emergency sharing | ✅ Advanced location |
| **Specialist Matching** | ❌ | ✅ Basic | ✅ AI-powered | ✅ AI + predictive |

### Business & Analytics Features

| Feature | Free | Premium | Premium+ | Enterprise |
|---------|------|---------|----------|------------|
| **Usage Analytics** | ❌ | ✅ Basic stats | ✅ Detailed insights | ✅ Custom dashboards |
| **Revenue Tracking** | ❌ | ❌ | ✅ Built-in | ✅ Advanced reporting |
| **Client Retention Metrics** | ❌ | ❌ | ✅ Basic | ✅ Predictive analytics |
| **API Access** | ❌ | ❌ | ❌ | ✅ Full REST API |
| **Webhook Integration** | ❌ | ❌ | ✅ Basic | ✅ Advanced |
| **Export Capabilities** | ❌ | ❌ | ✅ CSV/PDF | ✅ All formats + API |
| **Custom Reports** | ❌ | ❌ | ❌ | ✅ Available |

## Permission Matrix Implementation

### Database Schema for Permissions

```csharp
// FeaturePermission.cs - Define what features are available to which tiers
public class FeaturePermission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FeatureName { get; set; } = string.Empty; // "video_call", "group_call", etc.
    public string SubscriptionTier { get; set; } = string.Empty; // "Free", "Premium", etc.
    public bool IsEnabled { get; set; } = false;
    public string? Limitations { get; set; } // JSON object with limits
    public int Priority { get; set; } = 0; // For feature ordering
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

// UserPermissionOverride.cs - Allow custom permissions for specific users
public class UserPermissionOverride
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public string? CustomLimitations { get; set; } // JSON object
    public string Reason { get; set; } = string.Empty; // Why override was granted
    public string GrantedBy { get; set; } = string.Empty; // Admin user ID
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser GrantedByUser { get; set; } = null!;
}

// FeatureUsageLimit.cs - Track usage limits by tier
public class FeatureUsageLimit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FeatureName { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
    public string LimitType { get; set; } = string.Empty; // "monthly", "daily", "per_session", "total"
    public int LimitValue { get; set; } = 0;
    public string? LimitUnit { get; set; } // "minutes", "count", "mb", etc.
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
}
```

### Permission Service Implementation

```csharp
public interface IPermissionService
{
    Task<bool> HasPermission(string userId, string featureName);
    Task<PermissionResult> CheckPermission(string userId, string featureName, object? context = null);
    Task<Dictionary<string, bool>> GetUserPermissions(string userId);
    Task<UsageLimitResult> CheckUsageLimit(string userId, string featureName, int requestedUsage = 1);
    Task<bool> IncrementUsage(string userId, string featureName, int amount = 1);
    Task<Dictionary<string, int>> GetCurrentUsage(string userId, string period = "monthly");
    Task<bool> GrantPermissionOverride(string userId, string featureName, string reason, string grantedBy, DateTimeOffset? expiresAt = null);
    Task<bool> RevokePermissionOverride(string userId, string featureName, string revokedBy);
}

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PermissionService> _logger;

    public async Task<PermissionResult> CheckPermission(string userId, string featureName, object? context = null)
    {
        var cacheKey = $"permission_{userId}_{featureName}";
        
        if (_cache.TryGetValue(cacheKey, out PermissionResult? cachedResult))
        {
            return cachedResult!;
        }

        var result = await CheckPermissionInternal(userId, featureName, context);
        
        // Cache for 5 minutes
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        
        return result;
    }

    private async Task<PermissionResult> CheckPermissionInternal(string userId, string featureName, object? context)
    {
        try
        {
            // 1. Check for user-specific permission overrides first
            var userOverride = await _context.UserPermissionOverrides
                .Where(o => o.UserId == userId && o.FeatureName == featureName)
                .Where(o => o.ExpiresAt == null || o.ExpiresAt > DateTimeOffset.UtcNow)
                .FirstOrDefaultAsync();

            if (userOverride != null)
            {
                return new PermissionResult
                {
                    IsAllowed = userOverride.IsEnabled,
                    Reason = userOverride.IsEnabled ? "Permission override granted" : "Permission override denied",
                    Source = "UserOverride",
                    Limitations = ParseLimitations(userOverride.CustomLimitations)
                };
            }

            // 2. Get user's subscription tier
            var user = await _context.Users
                .Include(u => u.ServiceProvider)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new PermissionResult
                {
                    IsAllowed = false,
                    Reason = "User not found",
                    Source = "System"
                };
            }

            var subscriptionTier = GetUserSubscriptionTier(user);

            // 3. Check feature permission for subscription tier
            var featurePermission = await _context.FeaturePermissions
                .FirstOrDefaultAsync(fp => fp.FeatureName == featureName && fp.SubscriptionTier == subscriptionTier);

            if (featurePermission == null || !featurePermission.IsEnabled)
            {
                return new PermissionResult
                {
                    IsAllowed = false,
                    Reason = $"Feature '{featureName}' not available for {subscriptionTier} tier",
                    Source = "SubscriptionTier",
                    RequiredTier = GetRequiredTierForFeature(featureName),
                    CurrentTier = subscriptionTier,
                    UpgradeRequired = true
                };
            }

            // 4. Check usage limits
            var usageLimitResult = await CheckUsageLimit(userId, featureName, 1);
            if (!usageLimitResult.IsAllowed)
            {
                return new PermissionResult
                {
                    IsAllowed = false,
                    Reason = usageLimitResult.Reason,
                    Source = "UsageLimit",
                    CurrentUsage = usageLimitResult.CurrentUsage,
                    Limit = usageLimitResult.Limit,
                    ResetDate = usageLimitResult.ResetDate,
                    UpgradeRequired = usageLimitResult.UpgradeRequired
                };
            }

            // 5. Apply contextual restrictions
            var contextualResult = await ApplyContextualRestrictions(userId, featureName, subscriptionTier, context);
            if (!contextualResult.IsAllowed)
            {
                return contextualResult;
            }

            // Permission granted
            return new PermissionResult
            {
                IsAllowed = true,
                Reason = $"Feature '{featureName}' allowed for {subscriptionTier} tier",
                Source = "SubscriptionTier",
                Limitations = ParseLimitations(featurePermission.Limitations),
                CurrentTier = subscriptionTier
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission for user {UserId}, feature {FeatureName}", userId, featureName);
            
            // Fail-safe: deny access on error
            return new PermissionResult
            {
                IsAllowed = false,
                Reason = "Internal error occurred while checking permissions",
                Source = "System"
            };
        }
    }

    private async Task<PermissionResult> ApplyContextualRestrictions(
        string userId, string featureName, string subscriptionTier, object? context)
    {
        switch (featureName)
        {
            case "video_call":
                return await CheckVideoCallRestrictions(userId, subscriptionTier, context);
            
            case "group_call":
                return await CheckGroupCallRestrictions(userId, subscriptionTier, context);
            
            case "emergency_consultation":
                return await CheckEmergencyConsultationRestrictions(userId, subscriptionTier, context);
            
            case "recording":
                return await CheckRecordingRestrictions(userId, subscriptionTier, context);
            
            default:
                return new PermissionResult { IsAllowed = true, Source = "NoRestrictions" };
        }
    }

    private async Task<PermissionResult> CheckVideoCallRestrictions(
        string userId, string subscriptionTier, object? context)
    {
        // Check for concurrent call limits
        var activeCalls = await _context.VideoCallSessions
            .Where(vc => vc.InitiatorId == userId && vc.Status == VideoCallStatus.Active)
            .CountAsync();

        var maxConcurrentCalls = subscriptionTier switch
        {
            "Free" => 0, // No video calls for free users
            "Premium" => 1,
            "Premium+" => 3,
            "Enterprise" => 10,
            _ => 0
        };

        if (activeCalls >= maxConcurrentCalls)
        {
            return new PermissionResult
            {
                IsAllowed = false,
                Reason = $"Maximum concurrent video calls reached ({activeCalls}/{maxConcurrentCalls})",
                Source = "ConcurrencyLimit"
            };
        }

        // Check session duration limits for Premium tier
        if (subscriptionTier == "Premium" && context is VideoCallContext videoContext)
        {
            var maxSessionMinutes = 30;
            if (videoContext.RequestedDurationMinutes > maxSessionMinutes)
            {
                return new PermissionResult
                {
                    IsAllowed = false,
                    Reason = $"Video call duration limited to {maxSessionMinutes} minutes for Premium tier",
                    Source = "SessionLimit",
                    UpgradeRequired = true,
                    RequiredTier = "Premium+"
                };
            }
        }

        return new PermissionResult { IsAllowed = true, Source = "VideoCallRestrictions" };
    }

    private async Task<PermissionResult> CheckGroupCallRestrictions(
        string userId, string subscriptionTier, object? context)
    {
        if (context is not GroupCallContext groupContext)
        {
            return new PermissionResult { IsAllowed = true, Source = "NoContext" };
        }

        var maxParticipants = subscriptionTier switch
        {
            "Free" => 0,
            "Premium" => 4,
            "Premium+" => 8,
            "Enterprise" => 25,
            _ => 0
        };

        if (groupContext.ParticipantCount > maxParticipants)
        {
            return new PermissionResult
            {
                IsAllowed = false,
                Reason = $"Group call limited to {maxParticipants} participants for {subscriptionTier} tier",
                Source = "ParticipantLimit",
                UpgradeRequired = true,
                RequiredTier = GetTierForParticipantCount(groupContext.ParticipantCount)
            };
        }

        return new PermissionResult { IsAllowed = true, Source = "GroupCallRestrictions" };
    }

    private async Task<PermissionResult> CheckEmergencyConsultationRestrictions(
        string userId, string subscriptionTier, object? context)
    {
        // Emergency consultation is Premium+ only
        if (subscriptionTier != "Premium+" && subscriptionTier != "Enterprise")
        {
            return new PermissionResult
            {
                IsAllowed = false,
                Reason = "Emergency consultation requires Premium+ subscription",
                Source = "SubscriptionTier",
                RequiredTier = "Premium+",
                UpgradeRequired = true
            };
        }

        // Check for abuse (too many emergency requests)
        var recentEmergencies = await _context.EmergencyConsultationQueues
            .Where(eq => eq.RequesterId == userId)
            .Where(eq => eq.CreatedAt > DateTimeOffset.UtcNow.AddHours(-24))
            .CountAsync();

        var maxDailyEmergencies = subscriptionTier == "Enterprise" ? 10 : 3;
        
        if (recentEmergencies >= maxDailyEmergencies)
        {
            return new PermissionResult
            {
                IsAllowed = false,
                Reason = $"Daily emergency consultation limit reached ({recentEmergencies}/{maxDailyEmergencies})",
                Source = "UsageLimit",
                ResetDate = DateTimeOffset.UtcNow.Date.AddDays(1)
            };
        }

        return new PermissionResult { IsAllowed = true, Source = "EmergencyRestrictions" };
    }

    public async Task<UsageLimitResult> CheckUsageLimit(string userId, string featureName, int requestedUsage = 1)
    {
        var user = await _context.Users.Include(u => u.ServiceProvider).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return new UsageLimitResult
            {
                IsAllowed = false,
                Reason = "User not found"
            };
        }

        var subscriptionTier = GetUserSubscriptionTier(user);
        
        // Get usage limits for this tier
        var usageLimits = await _context.FeatureUsageLimits
            .Where(ful => ful.FeatureName == featureName && ful.SubscriptionTier == subscriptionTier && ful.IsActive)
            .ToListAsync();

        foreach (var limit in usageLimits)
        {
            var currentUsage = await GetCurrentUsageForLimit(userId, featureName, limit.LimitType);
            
            if (currentUsage + requestedUsage > limit.LimitValue)
            {
                return new UsageLimitResult
                {
                    IsAllowed = false,
                    Reason = $"{limit.LimitType} limit exceeded for {featureName}",
                    CurrentUsage = currentUsage,
                    Limit = limit.LimitValue,
                    LimitType = limit.LimitType,
                    ResetDate = GetResetDateForLimitType(limit.LimitType),
                    UpgradeRequired = !IsHighestTier(subscriptionTier)
                };
            }
        }

        return new UsageLimitResult
        {
            IsAllowed = true,
            CurrentUsage = await GetCurrentUsageForLimit(userId, featureName, "monthly"), // Default to monthly for reporting
            Limit = usageLimits.FirstOrDefault(l => l.LimitType == "monthly")?.LimitValue ?? -1 // -1 for unlimited
        };
    }

    private string GetUserSubscriptionTier(ApplicationUser user)
    {
        if (user.ServiceProvider == null || !user.ServiceProvider.IsPremium || 
            user.ServiceProvider.PremiumEndDate < DateTimeOffset.UtcNow)
        {
            return "Free";
        }

        // Determine tier based on subscription metadata
        var subscriptionId = user.ServiceProvider.PremiumSubscriptionId;
        
        if (subscriptionId?.Contains("enterprise") == true)
            return "Enterprise";
        else if (subscriptionId?.Contains("premium_plus") == true)
            return "Premium+";
        else if (user.ServiceProvider.IsPremium)
            return "Premium";

        return "Free";
    }

    private string GetRequiredTierForFeature(string featureName)
    {
        return featureName switch
        {
            "video_call" => "Premium",
            "group_call" => "Premium",
            "recording" => "Premium",
            "emergency_consultation" => "Premium+",
            "whiteboard" => "Premium+",
            "payment_processing" => "Premium+",
            "api_access" => "Enterprise",
            "custom_branding" => "Premium+",
            _ => "Premium"
        };
    }

    private string GetTierForParticipantCount(int participantCount)
    {
        return participantCount switch
        {
            <= 4 => "Premium",
            <= 8 => "Premium+",
            <= 25 => "Enterprise",
            _ => "Enterprise"
        };
    }
}

// Result objects
public class PermissionResult
{
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty; // "UserOverride", "SubscriptionTier", "UsageLimit", etc.
    public string? CurrentTier { get; set; }
    public string? RequiredTier { get; set; }
    public bool UpgradeRequired { get; set; }
    public Dictionary<string, object>? Limitations { get; set; }
    public int? CurrentUsage { get; set; }
    public int? Limit { get; set; }
    public DateTimeOffset? ResetDate { get; set; }
}

public class UsageLimitResult
{
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int CurrentUsage { get; set; }
    public int Limit { get; set; }
    public string? LimitType { get; set; }
    public DateTimeOffset? ResetDate { get; set; }
    public bool UpgradeRequired { get; set; }
}

// Context objects for different features
public class VideoCallContext
{
    public int RequestedDurationMinutes { get; set; }
    public string QualityLevel { get; set; } = string.Empty;
    public bool RecordingRequested { get; set; }
    public string[] ProfessionalFeatures { get; set; } = Array.Empty<string>();
}

public class GroupCallContext
{
    public int ParticipantCount { get; set; }
    public string CallType { get; set; } = string.Empty;
    public bool RequiresModeration { get; set; }
}
```

## Feature Flag System

```csharp
// Feature flags for gradual rollout and A/B testing
public class FeatureFlag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public string? TargetAudience { get; set; } // JSON: user IDs, subscription tiers, etc.
    public decimal RolloutPercentage { get; set; } = 0; // 0-100
    public string? Configuration { get; set; } // JSON configuration for the feature
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EnabledAt { get; set; }
    public string? Description { get; set; }
}

public interface IFeatureFlagService
{
    Task<bool> IsFeatureEnabled(string featureName, string userId);
    Task<T?> GetFeatureConfiguration<T>(string featureName, string userId);
    Task<Dictionary<string, bool>> GetEnabledFeatures(string userId);
}
```

## Authorization Attributes

```csharp
// Custom authorization attributes for controllers
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequiresPremiumFeatureAttribute : Attribute, IAuthorizationRequirement
{
    public string FeatureName { get; }
    public string? MinimumTier { get; }
    
    public RequiresPremiumFeatureAttribute(string featureName, string? minimumTier = null)
    {
        FeatureName = featureName;
        MinimumTier = minimumTier;
    }
}

public class PremiumFeatureAuthorizationHandler : AuthorizationHandler<RequiresPremiumFeatureAttribute>
{
    private readonly IPermissionService _permissionService;
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequiresPremiumFeatureAttribute requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            context.Fail();
            return;
        }

        var permissionResult = await _permissionService.CheckPermission(userId, requirement.FeatureName);
        
        if (permissionResult.IsAllowed)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

// Usage in controllers:
[ApiController]
[Route("api/v1/video-calls")]
public class VideoCallController : ControllerBase
{
    [HttpPost]
    [RequiresPremiumFeature("video_call", "Premium")]
    public async Task<IActionResult> StartVideoCall([FromBody] StartVideoCallRequest request)
    {
        // Implementation
    }
    
    [HttpPost("{callId}/recording")]
    [RequiresPremiumFeature("recording", "Premium")]
    public async Task<IActionResult> StartRecording(string callId)
    {
        // Implementation
    }
}
```

## Summary

This specification provides:

✅ **Comprehensive Feature Matrix** - Clear boundaries between all subscription tiers
✅ **Permission System** - Robust authorization with user overrides and contextual restrictions  
✅ **Usage Limit Tracking** - Monthly, daily, and session-based limits with proper reset logic
✅ **Feature Flag Support** - Gradual rollout and A/B testing capabilities
✅ **Authorization Attributes** - Easy-to-use controller decorators for feature gating
✅ **Contextual Restrictions** - Smart limits based on call type, participant count, duration, etc.
✅ **Upgrade Guidance** - Clear messaging about what tier is required for each feature
✅ **Audit Trail** - Full tracking of permission grants, usage, and overrides

The system ensures that premium features are properly monetized while providing clear upgrade paths for users who hit limitations, creating a sustainable business model for the MeAndMyDoggy platform.