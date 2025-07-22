# Premium Video Calling - Technical Specification

## Overview

This technical specification details the implementation of premium video calling features for the MeAndMyDoggy messaging system, including API design, database schema enhancements, WebRTC integration, and premium subscription validation.

## Database Schema Enhancements

### Enhanced VideoCallSession Entity

```csharp
// Enhanced VideoCallSession.cs
namespace MeAndMyDog.API.Models.Entities;

public class VideoCallSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ConversationId { get; set; } = string.Empty;
    public string InitiatorId { get; set; } = string.Empty;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    public VideoCallStatus Status { get; set; } = VideoCallStatus.Pending;
    public string? RoomId { get; set; }
    
    // Premium Features (NEW)
    public VideoCallQuality QualityTier { get; set; } = VideoCallQuality.Standard;
    public bool IsRecordingEnabled { get; set; } = false;
    public string? RecordingUrl { get; set; }
    public bool IsScreenSharingEnabled { get; set; } = false;
    public int MaxParticipants { get; set; } = 2;
    public VideoCallType CallType { get; set; } = VideoCallType.Standard;
    public string? EmergencyLevel { get; set; } // HIGH, MEDIUM, LOW
    public string? ProfessionalFeatures { get; set; } // JSON array of enabled features
    public decimal? CallCost { get; set; } // For premium consultations
    public string? PaymentIntentId { get; set; } // Stripe payment reference
    
    // Quality Metrics
    public string? ConnectionMetrics { get; set; } // JSON object
    public int? AverageLatency { get; set; }
    public decimal? PacketLossPercentage { get; set; }
    public string? QualityFeedback { get; set; } // JSON user feedback
    
    // Navigation Properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser Initiator { get; set; } = null!;
    public virtual ICollection<VideoCallParticipant> Participants { get; set; } = new List<VideoCallParticipant>();
    public virtual ICollection<VideoCallRecording> Recordings { get; set; } = new List<VideoCallRecording>();
}

public enum VideoCallStatus
{
    Pending = 0,
    Connecting = 1,
    Active = 2,
    Ended = 3,
    Cancelled = 4,
    Failed = 5,
    EmergencyQueue = 6
}

public enum VideoCallQuality
{
    Audio = 0,      // Voice only
    Standard = 1,   // 720p, 30fps
    HD = 2,         // 1080p, 30fps
    UltraHD = 3     // 1080p, 60fps + enhanced features
}

public enum VideoCallType
{
    Standard = 0,
    Professional = 1,
    Emergency = 2,
    Group = 3
}
```

### New Premium-Specific Entities

```csharp
// VideoCallParticipant.cs - Track individual participants
public class VideoCallParticipant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string VideoCallSessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }
    public VideoCallRole Role { get; set; } = VideoCallRole.Participant;
    public bool IsMuted { get; set; } = false;
    public bool IsVideoEnabled { get; set; } = true;
    public string? ConnectionId { get; set; } // SignalR connection
    
    // Navigation Properties
    public virtual VideoCallSession VideoCallSession { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public enum VideoCallRole
{
    Participant = 0,
    Moderator = 1,
    Observer = 2 // For emergency consultations
}

// VideoCallRecording.cs - Premium recording features
public class VideoCallRecording
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string VideoCallSessionId { get; set; } = string.Empty;
    public string RecordingUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public long FileSizeBytes { get; set; }
    public int DurationSeconds { get; set; }
    public VideoCallRecordingStatus Status { get; set; } = VideoCallRecordingStatus.Processing;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; } // Auto-deletion date
    public bool IsTranscribed { get; set; } = false;
    public string? TranscriptionText { get; set; }
    public string? RecordingMetadata { get; set; } // JSON
    
    // Access Control
    public bool IsShared { get; set; } = false;
    public string? SharedWithUserIds { get; set; } // JSON array
    public string? DownloadToken { get; set; } // Secure download token
    
    // Navigation Properties
    public virtual VideoCallSession VideoCallSession { get; set; } = null!;
}

public enum VideoCallRecordingStatus
{
    Processing = 0,
    Available = 1,
    Failed = 2,
    Deleted = 3
}

// PremiumFeatureUsage.cs - Track premium feature consumption
public class PremiumFeatureUsage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty; // "video_call", "recording", "group_call"
    public int UsageCount { get; set; } = 0;
    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }
    public int LimitCount { get; set; } = 0; // Monthly limit
    public string? Metadata { get; set; } // JSON additional data
    
    // Navigation Properties
    public virtual ApplicationUser User { get; set; } = null!;
}

// EmergencyConsultationQueue.cs - Premium+ emergency feature
public class EmergencyConsultationQueue
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequesterId { get; set; } = string.Empty;
    public string? AssignedExpertId { get; set; }
    public string EmergencyLevel { get; set; } = "MEDIUM"; // HIGH, MEDIUM, LOW
    public string PetType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; } // JSON coordinates
    public EmergencyStatus Status { get; set; } = EmergencyStatus.Queued;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public int EstimatedWaitMinutes { get; set; } = 0;
    public string? VideoCallSessionId { get; set; }
    
    // Navigation Properties
    public virtual ApplicationUser Requester { get; set; } = null!;
    public virtual ApplicationUser? AssignedExpert { get; set; }
    public virtual VideoCallSession? VideoCallSession { get; set; }
}

public enum EmergencyStatus
{
    Queued = 0,
    Assigned = 1,
    InProgress = 2,
    Resolved = 3,
    Cancelled = 4
}
```

## API Specification

### Premium Video Call Endpoints

```yaml
# Video Call Management API
/api/v1/video-calls:
  /:
    POST:
      summary: Initiate a new video call
      security:
        - BearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                conversationId:
                  type: string
                  description: Target conversation ID
                participantIds:
                  type: array
                  items:
                    type: string
                  description: Additional participant user IDs
                callType:
                  type: string
                  enum: [Standard, Professional, Emergency, Group]
                requestedQuality:
                  type: string
                  enum: [Audio, Standard, HD, UltraHD]
                enableRecording:
                  type: boolean
                  default: false
                professionalFeatures:
                  type: array
                  items:
                    type: string
                  description: ["whiteboard", "screen_share", "payment", "forms"]
                emergencyLevel:
                  type: string
                  enum: [HIGH, MEDIUM, LOW]
                  description: Required for emergency calls
      responses:
        201:
          description: Video call initiated
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    $ref: '#/components/schemas/VideoCallSession'
                  connectionInfo:
                    type: object
                    properties:
                      roomId: 
                        type: string
                      iceServers:
                        type: array
                      signalRConnectionId:
                        type: string
        402:
          description: Premium subscription required
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                    example: false
                  error: 
                    type: string
                    example: "Premium subscription required for video calls"
                  upgradeUrl:
                    type: string
                    example: "/upgrade"
        429:
          description: Monthly limit exceeded
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                    example: false
                  error: 
                    type: string
                    example: "Monthly video call limit exceeded"
                  currentUsage:
                    type: integer
                  monthlyLimit:
                    type: integer
                  resetDate:
                    type: string
                    format: date-time

  /{callId}/join:
    POST:
      summary: Join an existing video call
      security:
        - BearerAuth: []
      parameters:
        - name: callId
          in: path
          required: true
          schema:
            type: string
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                deviceCapabilities:
                  type: object
                  properties:
                    hasCamera: 
                      type: boolean
                    hasMicrophone: 
                      type: boolean
                    maxVideoQuality: 
                      type: string
                connectionType:
                  type: string
                  enum: [wifi, cellular, ethernet]
                bandwidthMbps:
                  type: number
      responses:
        200:
          description: Successfully joined call
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    type: object
                    properties:
                      participantId: 
                        type: string
                      roomId: 
                        type: string
                      participants: 
                        type: array
                        items:
                          $ref: '#/components/schemas/CallParticipant'
                      allowedFeatures:
                        type: array
                        items:
                          type: string

  /{callId}/end:
    POST:
      summary: End video call
      security:
        - BearerAuth: []
      responses:
        200:
          description: Call ended successfully

  /{callId}/recording:
    POST:
      summary: Start/stop call recording (Premium only)
      security:
        - BearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                action:
                  type: string
                  enum: [start, stop, pause, resume]
      responses:
        200:
          description: Recording action completed
        403:
          description: Recording not allowed for subscription tier

    GET:
      summary: Get call recordings
      security:
        - BearerAuth: []
      responses:
        200:
          description: List of recordings
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    type: array
                    items:
                      $ref: '#/components/schemas/VideoCallRecording'

/api/v1/emergency-consultation:
  /:
    POST:
      summary: Request emergency consultation (Premium+ only)
      security:
        - BearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [emergencyLevel, petType, description]
              properties:
                emergencyLevel:
                  type: string
                  enum: [HIGH, MEDIUM, LOW]
                petType:
                  type: string
                  description: Type of pet needing help
                description:
                  type: string
                  maxLength: 500
                location:
                  type: object
                  properties:
                    latitude: 
                      type: number
                    longitude: 
                      type: number
                images:
                  type: array
                  items:
                    type: string
                  description: Base64 encoded images
      responses:
        201:
          description: Emergency consultation queued
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    $ref: '#/components/schemas/EmergencyConsultationQueue'
                  estimatedWaitTime:
                    type: integer
                    description: Estimated wait time in minutes

  /queue:
    GET:
      summary: Get emergency consultation queue status
      security:
        - BearerAuth: []
      responses:
        200:
          description: Queue status
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    type: object
                    properties:
                      position: 
                        type: integer
                      estimatedWaitTime: 
                        type: integer
                      averageConsultationTime: 
                        type: integer
                      availableExperts: 
                        type: integer

/api/v1/premium-features:
  /usage:
    GET:
      summary: Get current premium feature usage
      security:
        - BearerAuth: []
      parameters:
        - name: period
          in: query
          schema:
            type: string
            enum: [current_month, last_month, last_3_months]
            default: current_month
      responses:
        200:
          description: Usage statistics
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  data:
                    type: object
                    properties:
                      videoCalls:
                        type: object
                        properties:
                          used: 
                            type: integer
                          limit: 
                            type: integer
                          remainingMinutes: 
                            type: integer
                      recordings:
                        type: object
                        properties:
                          used: 
                            type: integer
                          limit: 
                            type: integer
                      groupCalls:
                        type: object
                        properties:
                          used: 
                            type: integer
                          limit: 
                            type: integer
                      emergencyConsultations:
                        type: object
                        properties:
                          used: 
                            type: integer
                          limit: 
                            type: integer

  /validate:
    POST:
      summary: Validate premium feature access
      security:
        - BearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                featureName:
                  type: string
                  enum: [video_call, group_call, recording, emergency_consultation, professional_tools]
                additionalParams:
                  type: object
      responses:
        200:
          description: Feature validation result
          content:
            application/json:
              schema:
                type: object
                properties:
                  success: 
                    type: boolean
                  allowed: 
                    type: boolean
                  reason: 
                    type: string
                  upgradeRequired: 
                    type: boolean
                  currentTier: 
                    type: string
                  requiredTier: 
                    type: string
```

## SignalR Hub Enhancement

```csharp
// Enhanced MessagingHub.cs with premium features
[Authorize]
public class MessagingHub : Hub
{
    private readonly IPremiumFeatureService _premiumService;
    private readonly IVideoCallService _videoCallService;
    private readonly IEmergencyConsultationService _emergencyService;

    // Premium Video Call Methods
    public async Task StartPremiumVideoCall(StartVideoCallRequest request)
    {
        var userId = Context.UserIdentifier;
        
        // Validate premium access
        var validation = await _premiumService.ValidateFeatureAccess(
            userId, "video_call", request);
            
        if (!validation.IsAllowed)
        {
            await Clients.Caller.SendAsync("VideoCallError", new
            {
                Error = validation.Reason,
                UpgradeRequired = validation.UpgradeRequired,
                RequiredTier = validation.RequiredTier
            });
            return;
        }

        try
        {
            var callSession = await _videoCallService.InitiateCall(userId, request);
            
            // Get optimal server configuration based on subscription
            var serverConfig = await _premiumService.GetVideoServerConfiguration(userId);
            
            // Notify all participants
            var participantIds = await _videoCallService.GetParticipantIds(request.ConversationId);
            
            await Clients.Users(participantIds).SendAsync("IncomingVideoCall", new
            {
                CallId = callSession.Id,
                InitiatorName = Context.User?.Identity?.Name,
                CallType = request.CallType,
                QualityTier = callSession.QualityTier,
                ServerConfig = serverConfig,
                AllowedFeatures = await _premiumService.GetAllowedFeatures(userId)
            });

            // Analytics tracking
            await _analyticsService.TrackEvent("video_call_initiated", new
            {
                UserId = userId,
                CallType = request.CallType,
                QualityTier = callSession.QualityTier,
                SubscriptionTier = await _premiumService.GetUserSubscriptionTier(userId)
            });
        }
        catch (PremiumFeatureLimitExceededException ex)
        {
            await Clients.Caller.SendAsync("VideoCallLimitExceeded", new
            {
                CurrentUsage = ex.CurrentUsage,
                MonthlyLimit = ex.MonthlyLimit,
                ResetDate = ex.ResetDate,
                UpgradeOptions = await _premiumService.GetUpgradeOptions(userId)
            });
        }
    }

    public async Task JoinVideoCall(string callId, JoinCallRequest request)
    {
        var userId = Context.UserIdentifier;
        
        try
        {
            var participantInfo = await _videoCallService.JoinCall(callId, userId, request);
            
            // Add to SignalR group for real-time updates
            await Groups.AddToGroupAsync(Context.ConnectionId, $"videocall_{callId}");
            
            // Notify existing participants
            await Clients.Group($"videocall_{callId}")
                .SendAsync("ParticipantJoined", participantInfo);
                
            // Send current call state to new participant
            var callState = await _videoCallService.GetCallState(callId);
            await Clients.Caller.SendAsync("CallStateUpdate", callState);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("JoinCallError", new { Error = ex.Message });
        }
    }

    public async Task UpdateCallQuality(string callId, CallQualityMetrics metrics)
    {
        await _videoCallService.UpdateQualityMetrics(callId, Context.UserIdentifier, metrics);
        
        // Provide quality feedback to premium users
        var userId = Context.UserIdentifier;
        var isPremium = await _premiumService.IsUserPremium(userId);
        
        if (isPremium)
        {
            var qualityFeedback = await _videoCallService.AnalyzeCallQuality(metrics);
            await Clients.Caller.SendAsync("QualityFeedback", qualityFeedback);
        }
    }

    public async Task RequestEmergencyConsultation(EmergencyConsultationRequest request)
    {
        var userId = Context.UserIdentifier;
        
        // Validate Premium+ subscription for emergency features
        var hasPremiumPlus = await _premiumService.HasPremiumPlusSubscription(userId);
        if (!hasPremiumPlus)
        {
            await Clients.Caller.SendAsync("EmergencyConsultationError", new
            {
                Error = "Emergency consultation requires Premium+ subscription",
                UpgradeRequired = true
            });
            return;
        }

        try
        {
            var consultation = await _emergencyService.QueueEmergencyConsultation(userId, request);
            
            await Clients.Caller.SendAsync("EmergencyConsultationQueued", new
            {
                QueueId = consultation.Id,
                Position = await _emergencyService.GetQueuePosition(consultation.Id),
                EstimatedWaitTime = consultation.EstimatedWaitMinutes
            });

            // Notify available emergency experts
            var availableExperts = await _emergencyService.GetAvailableExperts(request.PetType);
            await Clients.Users(availableExperts)
                .SendAsync("EmergencyConsultationAvailable", consultation);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("EmergencyConsultationError", new { Error = ex.Message });
        }
    }

    // Professional Tools for Premium+ users
    public async Task StartScreenShare(string callId)
    {
        var userId = Context.UserIdentifier;
        var canUseFeature = await _premiumService.CanUseFeature(userId, "screen_sharing");
        
        if (!canUseFeature)
        {
            await Clients.Caller.SendAsync("FeatureNotAllowed", new
            {
                Feature = "screen_sharing",
                RequiredTier = "Premium"
            });
            return;
        }

        await Clients.Group($"videocall_{callId}")
            .SendAsync("ScreenShareStarted", new { UserId = userId });
    }

    public async Task SendWhiteboardData(string callId, WhiteboardData data)
    {
        var userId = Context.UserIdentifier;
        var canUseFeature = await _premiumService.CanUseFeature(userId, "whiteboard");
        
        if (canUseFeature)
        {
            await Clients.OthersInGroup($"videocall_{callId}")
                .SendAsync("WhiteboardUpdate", data);
        }
    }

    public async Task ProcessPaymentRequest(string callId, PaymentRequest request)
    {
        var userId = Context.UserIdentifier;
        var canUseFeature = await _premiumService.CanUseFeature(userId, "payment_processing");
        
        if (!canUseFeature)
        {
            await Clients.Caller.SendAsync("FeatureNotAllowed", new
            {
                Feature = "payment_processing",
                RequiredTier = "Premium+"
            });
            return;
        }

        try
        {
            var paymentIntent = await _paymentService.CreatePaymentIntent(request);
            
            // Send payment request to client
            await Clients.Group($"videocall_{callId}")
                .SendAsync("PaymentRequested", new
                {
                    PaymentIntentId = paymentIntent.Id,
                    Amount = request.Amount,
                    Description = request.Description,
                    ServiceProviderId = userId
                });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("PaymentError", new { Error = ex.Message });
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        
        // Handle user disconnect from active video calls
        var activeCalls = await _videoCallService.GetActiveCallsForUser(userId);
        
        foreach (var call in activeCalls)
        {
            await _videoCallService.HandleParticipantDisconnect(call.Id, userId);
            
            await Clients.Group($"videocall_{call.Id}")
                .SendAsync("ParticipantDisconnected", new { UserId = userId });
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
```

## Service Layer Implementation

```csharp
// Premium Feature Service
public interface IPremiumFeatureService
{
    Task<bool> IsUserPremium(string userId);
    Task<bool> HasPremiumPlusSubscription(string userId);
    Task<string> GetUserSubscriptionTier(string userId);
    Task<FeatureValidationResult> ValidateFeatureAccess(string userId, string featureName, object? parameters = null);
    Task<bool> CanUseFeature(string userId, string featureName);
    Task<VideoServerConfiguration> GetVideoServerConfiguration(string userId);
    Task<string[]> GetAllowedFeatures(string userId);
    Task<PremiumFeatureUsage> GetMonthlyUsage(string userId, string featureName);
    Task<bool> IncrementFeatureUsage(string userId, string featureName);
    Task<UpgradeOption[]> GetUpgradeOptions(string userId);
}

public class PremiumFeatureService : IPremiumFeatureService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    
    public async Task<FeatureValidationResult> ValidateFeatureAccess(
        string userId, string featureName, object? parameters = null)
    {
        var user = await _context.Users
            .Include(u => u.ServiceProvider)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.ServiceProvider == null)
        {
            return new FeatureValidationResult
            {
                IsAllowed = false,
                Reason = "Service provider account required",
                UpgradeRequired = true,
                RequiredTier = "Premium"
            };
        }

        var serviceProvider = user.ServiceProvider;
        var subscriptionTier = GetSubscriptionTier(serviceProvider);

        return featureName switch
        {
            "video_call" => await ValidateVideoCallAccess(userId, subscriptionTier, parameters),
            "group_call" => ValidateGroupCallAccess(subscriptionTier, parameters),
            "recording" => await ValidateRecordingAccess(userId, subscriptionTier),
            "emergency_consultation" => ValidateEmergencyConsultationAccess(subscriptionTier),
            "screen_sharing" => ValidateScreenSharingAccess(subscriptionTier),
            "whiteboard" => ValidateWhiteboardAccess(subscriptionTier),
            "payment_processing" => ValidatePaymentProcessingAccess(subscriptionTier),
            _ => new FeatureValidationResult { IsAllowed = false, Reason = "Unknown feature" }
        };
    }

    private async Task<FeatureValidationResult> ValidateVideoCallAccess(
        string userId, string subscriptionTier, object? parameters)
    {
        // Free users cannot use video calls
        if (subscriptionTier == "Free")
        {
            return new FeatureValidationResult
            {
                IsAllowed = false,
                Reason = "Video calls require Premium subscription",
                UpgradeRequired = true,
                RequiredTier = "Premium"
            };
        }

        // Check monthly limits for Premium users
        if (subscriptionTier == "Premium")
        {
            var usage = await GetMonthlyUsage(userId, "video_call");
            var limit = _configuration.GetValue<int>("Premium:VideoCall:MonthlyMinuteLimit", 300); // 5 hours

            if (usage.UsageCount >= limit)
            {
                return new FeatureValidationResult
                {
                    IsAllowed = false,
                    Reason = $"Monthly video call limit exceeded ({usage.UsageCount}/{limit} minutes)",
                    UpgradeRequired = true,
                    RequiredTier = "Premium+",
                    CurrentUsage = usage.UsageCount,
                    MonthlyLimit = limit
                };
            }
        }

        // Premium+ has unlimited access
        return new FeatureValidationResult { IsAllowed = true };
    }

    private FeatureValidationResult ValidateGroupCallAccess(string subscriptionTier, object? parameters)
    {
        if (subscriptionTier == "Free")
        {
            return new FeatureValidationResult
            {
                IsAllowed = false,
                Reason = "Group calls require Premium subscription",
                UpgradeRequired = true,
                RequiredTier = "Premium"
            };
        }

        // Check participant limits
        if (parameters is GroupCallParameters groupParams)
        {
            var maxParticipants = subscriptionTier switch
            {
                "Premium" => 4,
                "Premium+" => 8,
                _ => 0
            };

            if (groupParams.ParticipantCount > maxParticipants)
            {
                return new FeatureValidationResult
                {
                    IsAllowed = false,
                    Reason = $"Group calls limited to {maxParticipants} participants for {subscriptionTier}",
                    UpgradeRequired = subscriptionTier != "Premium+",
                    RequiredTier = "Premium+"
                };
            }
        }

        return new FeatureValidationResult { IsAllowed = true };
    }

    private string GetSubscriptionTier(ServiceProvider serviceProvider)
    {
        if (!serviceProvider.IsPremium || 
            serviceProvider.PremiumEndDate < DateTimeOffset.UtcNow)
        {
            return "Free";
        }

        // Determine tier based on subscription metadata or pricing
        // This could be enhanced with proper subscription management
        var subscriptionId = serviceProvider.PremiumSubscriptionId;
        
        // For now, simple logic based on subscription ID patterns
        if (subscriptionId?.Contains("premium_plus") == true)
        {
            return "Premium+";
        }
        else if (serviceProvider.IsPremium)
        {
            return "Premium";
        }

        return "Free";
    }
}

// Video Call Service
public interface IVideoCallService
{
    Task<VideoCallSession> InitiateCall(string initiatorId, StartVideoCallRequest request);
    Task<CallParticipant> JoinCall(string callId, string userId, JoinCallRequest request);
    Task<CallState> GetCallState(string callId);
    Task<bool> EndCall(string callId, string userId);
    Task<VideoCallRecording> StartRecording(string callId, string userId);
    Task<VideoCallRecording> StopRecording(string callId, string userId);
    Task<string[]> GetParticipantIds(string conversationId);
    Task UpdateQualityMetrics(string callId, string userId, CallQualityMetrics metrics);
    Task<QualityFeedback> AnalyzeCallQuality(CallQualityMetrics metrics);
    Task HandleParticipantDisconnect(string callId, string userId);
    Task<VideoCallSession[]> GetActiveCallsForUser(string userId);
}

public class VideoCallService : IVideoCallService
{
    private readonly ApplicationDbContext _context;
    private readonly IPremiumFeatureService _premiumService;
    private readonly IWebRTCService _webRtcService;
    private readonly IRecordingService _recordingService;

    public async Task<VideoCallSession> InitiateCall(string initiatorId, StartVideoCallRequest request)
    {
        // Create video call session
        var callSession = new VideoCallSession
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = request.ConversationId,
            InitiatorId = initiatorId,
            StartTime = DateTimeOffset.UtcNow,
            Status = VideoCallStatus.Pending,
            CallType = request.CallType,
            QualityTier = await DetermineQualityTier(initiatorId, request.RequestedQuality),
            IsRecordingEnabled = request.EnableRecording,
            MaxParticipants = await DetermineMaxParticipants(initiatorId, request),
            ProfessionalFeatures = request.ProfessionalFeatures != null 
                ? JsonSerializer.Serialize(request.ProfessionalFeatures) 
                : null
        };

        // Generate room ID with WebRTC service
        callSession.RoomId = await _webRtcService.CreateRoom(callSession.Id, new RoomConfig
        {
            MaxParticipants = callSession.MaxParticipants,
            QualityTier = callSession.QualityTier,
            RecordingEnabled = callSession.IsRecordingEnabled
        });

        _context.VideoCallSessions.Add(callSession);

        // Add initiator as first participant
        var initiatorParticipant = new VideoCallParticipant
        {
            VideoCallSessionId = callSession.Id,
            UserId = initiatorId,
            JoinedAt = DateTimeOffset.UtcNow,
            Role = VideoCallRole.Moderator
        };

        _context.VideoCallParticipants.Add(initiatorParticipant);
        await _context.SaveChangesAsync();

        // Track usage for premium features
        await _premiumService.IncrementFeatureUsage(initiatorId, "video_call");

        return callSession;
    }

    private async Task<VideoCallQuality> DetermineQualityTier(string userId, VideoCallQuality requestedQuality)
    {
        var subscriptionTier = await _premiumService.GetUserSubscriptionTier(userId);

        return subscriptionTier switch
        {
            "Free" => VideoCallQuality.Audio, // Voice only for free users
            "Premium" => requestedQuality > VideoCallQuality.HD ? VideoCallQuality.HD : requestedQuality,
            "Premium+" => requestedQuality, // Full access to all quality levels
            _ => VideoCallQuality.Standard
        };
    }

    private async Task<int> DetermineMaxParticipants(string userId, StartVideoCallRequest request)
    {
        var subscriptionTier = await _premiumService.GetUserSubscriptionTier(userId);

        var maxAllowed = subscriptionTier switch
        {
            "Free" => 2,
            "Premium" => 4,
            "Premium+" => 8,
            _ => 2
        };

        return Math.Min(request.ParticipantIds?.Length + 1 ?? 2, maxAllowed);
    }
}
```

## WebRTC Integration

```typescript
// Enhanced WebRTC client for premium features
export class PremiumWebRTCClient {
  private peerConnection: RTCPeerConnection;
  private localStream: MediaStream | null = null;
  private remoteStreams: Map<string, MediaStream> = new Map();
  private subscriptionTier: string = 'Free';
  private allowedFeatures: string[] = [];

  constructor(
    private callId: string,
    private userId: string,
    private signalRConnection: HubConnection
  ) {
    this.initializePeerConnection();
  }

  private initializePeerConnection() {
    const config: RTCConfiguration = {
      iceServers: this.getIceServers(),
      bundlePolicy: 'max-bundle',
      rtcpMuxPolicy: 'require'
    };

    this.peerConnection = new RTCPeerConnection(config);
    this.setupEventHandlers();
  }

  private getIceServers(): RTCIceServer[] {
    // Premium users get access to dedicated TURN servers
    if (this.subscriptionTier === 'Premium+') {
      return [
        { urls: 'stun:premium-stun.meandmydoggy.com' },
        { 
          urls: 'turn:premium-turn.meandmydoggy.com',
          username: 'premium-user',
          credential: 'premium-secret'
        }
      ];
    } else if (this.subscriptionTier === 'Premium') {
      return [
        { urls: 'stun:premium-stun.meandmydoggy.com' },
        { 
          urls: 'turn:shared-turn.meandmydoggy.com',
          username: 'premium-user',
          credential: 'premium-secret'
        }
      ];
    }

    // Free users get basic STUN servers
    return [
      { urls: 'stun:stun.l.google.com:19302' }
    ];
  }

  async startCall(options: CallStartOptions) {
    try {
      // Get media with quality constraints based on subscription
      const constraints = this.getMediaConstraints(options.requestedQuality);
      this.localStream = await navigator.mediaDevices.getUserMedia(constraints);

      // Add tracks to peer connection
      this.localStream.getTracks().forEach(track => {
        if (this.localStream) {
          this.peerConnection.addTrack(track, this.localStream);
        }
      });

      // Premium features initialization
      if (this.allowedFeatures.includes('screen_sharing')) {
        this.initializeScreenSharing();
      }

      if (this.allowedFeatures.includes('recording')) {
        this.initializeRecording();
      }

      // Start quality monitoring for premium users
      if (this.subscriptionTier !== 'Free') {
        this.startQualityMonitoring();
      }

      return {
        success: true,
        localStream: this.localStream,
        features: this.allowedFeatures
      };

    } catch (error) {
      console.error('Failed to start call:', error);
      throw new Error(`Call initialization failed: ${error.message}`);
    }
  }

  private getMediaConstraints(requestedQuality: string): MediaStreamConstraints {
    const baseConstraints: MediaStreamConstraints = {
      audio: {
        echoCancellation: true,
        noiseSuppression: this.subscriptionTier !== 'Free',
        autoGainControl: true
      },
      video: false
    };

    // Video constraints based on subscription tier
    if (this.subscriptionTier !== 'Free') {
      const videoConstraints: MediaTrackConstraints = {
        width: { ideal: 1280 },
        height: { ideal: 720 },
        frameRate: { ideal: 30 }
      };

      if (this.subscriptionTier === 'Premium+') {
        switch (requestedQuality) {
          case 'UltraHD':
            videoConstraints.width = { ideal: 1920 };
            videoConstraints.height = { ideal: 1080 };
            videoConstraints.frameRate = { ideal: 60 };
            break;
          case 'HD':
            videoConstraints.width = { ideal: 1920 };
            videoConstraints.height = { ideal: 1080 };
            videoConstraints.frameRate = { ideal: 30 };
            break;
        }
      }

      baseConstraints.video = videoConstraints;
    }

    return baseConstraints;
  }

  private startQualityMonitoring() {
    setInterval(async () => {
      const stats = await this.peerConnection.getStats();
      const metrics = this.extractQualityMetrics(stats);
      
      // Send metrics to SignalR hub
      await this.signalRConnection.invoke('UpdateCallQuality', this.callId, metrics);
    }, 5000); // Every 5 seconds
  }

  private extractQualityMetrics(stats: RTCStatsReport): CallQualityMetrics {
    const metrics: CallQualityMetrics = {
      timestamp: Date.now(),
      bitrate: 0,
      packetLoss: 0,
      jitter: 0,
      roundTripTime: 0
    };

    stats.forEach(stat => {
      if (stat.type === 'inbound-rtp' && stat.mediaType === 'video') {
        metrics.bitrate = stat.bytesReceived || 0;
        metrics.packetsLost = stat.packetsLost || 0;
        metrics.jitter = stat.jitter || 0;
      } else if (stat.type === 'candidate-pair' && stat.state === 'succeeded') {
        metrics.roundTripTime = stat.currentRoundTripTime || 0;
      }
    });

    return metrics;
  }

  // Premium feature: Screen sharing
  async startScreenShare(): Promise<void> {
    if (!this.allowedFeatures.includes('screen_sharing')) {
      throw new Error('Screen sharing not available for current subscription');
    }

    try {
      const screenStream = await navigator.mediaDevices.getDisplayMedia({
        video: { mediaSource: 'screen' },
        audio: true
      });

      // Replace video track with screen share
      const videoTrack = screenStream.getVideoTracks()[0];
      const sender = this.peerConnection.getSenders().find(s => 
        s.track && s.track.kind === 'video'
      );

      if (sender) {
        await sender.replaceTrack(videoTrack);
      }

      // Notify other participants via SignalR
      await this.signalRConnection.invoke('StartScreenShare', this.callId);

    } catch (error) {
      throw new Error(`Screen sharing failed: ${error.message}`);
    }
  }

  // Premium feature: Call recording
  async startRecording(): Promise<void> {
    if (!this.allowedFeatures.includes('recording')) {
      throw new Error('Recording not available for current subscription');
    }

    await this.signalRConnection.invoke('StartRecording', this.callId);
  }

  // Premium+ feature: Virtual backgrounds
  async enableVirtualBackground(backgroundType: 'blur' | 'custom', backgroundData?: string): Promise<void> {
    if (!this.allowedFeatures.includes('virtual_backgrounds')) {
      throw new Error('Virtual backgrounds not available for current subscription');
    }

    // Implementation would integrate with a background replacement library
    // like @mediapipe/selfie_segmentation or similar
    console.log('Virtual background feature would be implemented here');
  }

  setSubscriptionInfo(tier: string, features: string[]) {
    this.subscriptionTier = tier;
    this.allowedFeatures = features;
  }
}
```

## Implementation Summary

This technical specification provides:

1. **Enhanced Database Schema** with premium-specific entities and fields
2. **Comprehensive API Design** with proper premium validation and error handling
3. **Enhanced SignalR Hub** with premium feature support
4. **Service Layer Implementation** with subscription tier validation
5. **WebRTC Client Integration** with quality tiers and premium features

The implementation ensures that:
- ✅ Premium features are properly gated behind subscription tiers
- ✅ Video calls have quality differentiation based on subscription
- ✅ Emergency consultation system is exclusive to Premium+
- ✅ Professional tools enhance service provider capabilities
- ✅ Usage tracking prevents abuse and enforces limits
- ✅ Proper error handling guides users to upgrade paths

This specification complements your existing comprehensive messaging system documentation and provides the missing implementation details needed to build a world-class premium video calling system for pet service providers and owners.