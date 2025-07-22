# Phase 2: Premium Video Calling (Weeks 5-8)

## Overview

This phase implements premium video calling features with subscription-based access control, usage tracking, and quality tier differentiation. It builds upon the messaging foundation from Phase 1 to provide premium users with advanced communication capabilities that justify subscription costs.

## Goals

- ✅ Launch premium video calling with subscription gating
- ✅ Implement usage limits and quality tiers
- ✅ Create premium feature validation system
- ✅ Build WebRTC integration for video calls
- ✅ Establish upgrade paths for free users

## Week 5-6: Premium Infrastructure (High Priority)

### Epic 2.1: Premium Feature Services
**Estimated Effort:** 18 hours  
**Dependencies:** Phase 1 completion, ServiceProvider premium fields  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Services/IPremiumFeatureService.cs`
- `src/API/MeAndMyDog.API/Services/PremiumFeatureService.cs`
- `src/API/MeAndMyDog.API/Services/IPermissionService.cs`
- `src/API/MeAndMyDog.API/Services/PermissionService.cs`

#### Tasks:

- [ ] **Task 2.1.1: Create IPremiumFeatureService Interface**
  ```csharp
  // Key methods to define:
  - Task<bool> IsUserPremium(string userId)
  - Task<bool> HasPremiumPlusSubscription(string userId)
  - Task<string> GetUserSubscriptionTier(string userId)
  - Task<FeatureValidationResult> ValidateFeatureAccess(string userId, string featureName, object? parameters = null)
  - Task<bool> CanUseFeature(string userId, string featureName)
  - Task<VideoServerConfiguration> GetVideoServerConfiguration(string userId)
  - Task<string[]> GetAllowedFeatures(string userId)
  - Task<PremiumFeatureUsage> GetMonthlyUsage(string userId, string featureName)
  - Task<bool> IncrementFeatureUsage(string userId, string featureName, int amount = 1)
  ```

- [ ] **Task 2.1.2: Implement PremiumFeatureService**
  - [ ] Add subscription tier validation logic
  - [ ] Implement feature access checking
  - [ ] Create usage tracking functionality
  - [ ] Add premium server configuration logic
  - [ ] Include upgrade path recommendations
  - [ ] Add comprehensive error handling and logging

- [ ] **Task 2.1.3: Create IPermissionService Interface**
  ```csharp
  // Key methods to define:
  - Task<bool> HasPermission(string userId, string featureName)
  - Task<PermissionResult> CheckPermission(string userId, string featureName, object? context = null)
  - Task<Dictionary<string, bool>> GetUserPermissions(string userId)
  - Task<UsageLimitResult> CheckUsageLimit(string userId, string featureName, int requestedUsage = 1)
  - Task<bool> IncrementUsage(string userId, string featureName, int amount = 1)
  ```

- [ ] **Task 2.1.4: Implement PermissionService with Caching**
  - [ ] Add Redis caching for permission checks
  - [ ] Implement contextual permission validation
  - [ ] Create usage limit tracking
  - [ ] Add user override support
  - [ ] Include audit logging for permission checks

**Acceptance Criteria:**
- Permission checks are cached and performant (<50ms response time)
- Subscription tier validation is accurate
- Usage limits are properly enforced
- Clear error messages guide users to upgrade paths
- All permission decisions are logged for audit

### Epic 2.2: Video Call Database Enhancement
**Estimated Effort:** 10 hours  
**Dependencies:** Existing VideoCallSession entity  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Models/Entities/VideoCallParticipant.cs`
- `src/API/MeAndMyDog.API/Models/Entities/VideoCallRecording.cs`
- `src/API/MeAndMyDog.API/Models/Entities/PremiumFeatureUsage.cs`
- Database migration files

#### Tasks:

- [ ] **Task 2.2.1: Create VideoCallParticipant Entity**
  ```csharp
  // Key properties:
  - string Id (PK)
  - string VideoCallSessionId (FK)
  - string UserId (FK)
  - DateTimeOffset JoinedAt
  - DateTimeOffset? LeftAt
  - VideoCallRole Role (Participant, Moderator, Observer)
  - bool IsMuted
  - bool IsVideoEnabled
  - string? ConnectionId (SignalR connection)
  ```

- [ ] **Task 2.2.2: Create VideoCallRecording Entity**
  ```csharp
  // Key properties:
  - string Id (PK)
  - string VideoCallSessionId (FK)
  - string RecordingUrl
  - string? ThumbnailUrl
  - long FileSizeBytes
  - int DurationSeconds
  - VideoCallRecordingStatus Status
  - DateTimeOffset StartedAt
  - DateTimeOffset? CompletedAt
  - DateTimeOffset ExpiresAt
  - string? TranscriptionText
  - bool IsShared
  ```

- [ ] **Task 2.2.3: Create PremiumFeatureUsage Entity**
  ```csharp
  // Key properties:
  - string Id (PK)
  - string UserId (FK)
  - string FeatureName
  - int UsageCount
  - DateTimeOffset PeriodStart
  - DateTimeOffset PeriodEnd
  - int LimitCount
  - string? Metadata (JSON)
  ```

- [ ] **Task 2.2.4: Enhance VideoCallSession Entity**
  - [ ] Add premium-specific fields (QualityTier, IsRecordingEnabled, MaxParticipants)
  - [ ] Add call cost and payment integration fields
  - [ ] Include connection metrics and quality tracking
  - [ ] Add emergency call support fields
  - [ ] Update relationships with new entities

- [ ] **Task 2.2.5: Create Database Migration**
  - [ ] Generate EF Core migration for new entities
  - [ ] Add proper indexes for performance
  - [ ] Include foreign key constraints
  - [ ] Add seed data for testing
  - [ ] Verify migration rollback capability

**Acceptance Criteria:**
- All new entities are properly configured with EF Core
- Database relationships are correctly established
- Migration applies successfully to development environment
- Proper indexes exist for query performance
- Seed data supports testing scenarios

### Epic 2.3: WebRTC Service Integration
**Estimated Effort:** 14 hours  
**Dependencies:** Premium services implementation  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Services/IVideoCallService.cs`
- `src/API/MeAndMyDog.API/Services/VideoCallService.cs`
- `src/API/MeAndMyDog.API/Services/IWebRTCService.cs`
- `src/API/MeAndMyDog.API/Services/WebRTCService.cs`

#### Tasks:

- [ ] **Task 2.3.1: Choose WebRTC Provider**
  - [ ] Evaluate Twilio Video vs Agora.io vs custom WebRTC
  - [ ] Set up development accounts and API keys
  - [ ] Compare pricing and feature sets
  - [ ] Make final provider selection
  - [ ] Document integration approach

- [ ] **Task 2.3.2: Create IWebRTCService Interface**
  ```csharp
  // Key methods to define:
  - Task<string> CreateRoom(string callId, RoomConfig config)
  - Task<RoomInfo> GetRoomInfo(string roomId)
  - Task<AccessToken> GenerateAccessToken(string roomId, string userId, string role)
  - Task<bool> EndRoom(string roomId)
  - Task<RecordingInfo> StartRecording(string roomId)
  - Task<RecordingInfo> StopRecording(string recordingId)
  ```

- [ ] **Task 2.3.3: Implement WebRTCService**
  - [ ] Integrate with chosen video calling provider
  - [ ] Implement room creation and management
  - [ ] Add access token generation with proper permissions
  - [ ] Configure different server tiers for subscription levels
  - [ ] Include recording functionality
  - [ ] Add comprehensive error handling

- [ ] **Task 2.3.4: Create IVideoCallService Interface**
  ```csharp
  // Key methods to define:
  - Task<VideoCallSession> InitiateCall(string initiatorId, StartVideoCallRequest request)
  - Task<CallParticipant> JoinCall(string callId, string userId, JoinCallRequest request)
  - Task<CallState> GetCallState(string callId)
  - Task<bool> EndCall(string callId, string userId)
  - Task<VideoCallRecording> StartRecording(string callId, string userId)
  - Task<VideoCallRecording> StopRecording(string callId, string userId)
  ```

- [ ] **Task 2.3.5: Implement VideoCallService**
  - [ ] Implement all interface methods
  - [ ] Add database operations for call management
  - [ ] Integrate with WebRTC service
  - [ ] Include premium feature validation
  - [ ] Add usage tracking integration
  - [ ] Implement call quality monitoring

**Acceptance Criteria:**
- WebRTC integration works with chosen provider
- Video calls can be initiated and joined successfully
- Premium features are properly gated
- Call recording works for premium users
- Quality tiers are enforced based on subscription

### Epic 2.4: Authorization System Enhancement
**Estimated Effort:** 8 hours  
**Dependencies:** Permission service implementation  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Authorization/RequiresPremiumFeatureAttribute.cs`
- `src/API/MeAndMyDog.API/Authorization/PremiumFeatureAuthorizationHandler.cs`
- `src/API/MeAndMyDog.API/Middleware/PremiumFeatureMiddleware.cs`

#### Tasks:

- [ ] **Task 2.4.1: Create Premium Feature Attribute**
  ```csharp
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class RequiresPremiumFeatureAttribute : Attribute, IAuthorizationRequirement
  {
      public string FeatureName { get; }
      public string? MinimumTier { get; }
      public RequiresPremiumFeatureAttribute(string featureName, string? minimumTier = null)
      // Implementation
  }
  ```

- [ ] **Task 2.4.2: Implement Authorization Handler**
  - [ ] Create PremiumFeatureAuthorizationHandler
  - [ ] Integrate with IPermissionService
  - [ ] Add proper error response formatting
  - [ ] Include upgrade URL generation
  - [ ] Add comprehensive logging

- [ ] **Task 2.4.3: Create Feature Gating Middleware**
  - [ ] Implement middleware for global feature checks
  - [ ] Add request context enhancement
  - [ ] Include rate limiting for premium features
  - [ ] Add usage tracking integration
  - [ ] Support for feature flag toggling

- [ ] **Task 2.4.4: Configure Authorization in Program.cs**
  - [ ] Register authorization handlers
  - [ ] Configure authorization policies
  - [ ] Add middleware to pipeline
  - [ ] Set up dependency injection
  - [ ] Configure error handling

**Acceptance Criteria:**
- Authorization attributes work correctly on controllers
- Premium features are properly protected
- Clear error messages guide users to upgrade
- Middleware performs efficiently without blocking
- All authorization decisions are logged

## Week 7-8: Video Calling Features (High Priority)

### Epic 2.5: Core Video Calling Implementation
**Estimated Effort:** 20 hours  
**Dependencies:** Week 5-6 infrastructure completion  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Controllers/VideoCallController.cs`
- Enhancement to MessagingHub.cs

#### Tasks:

- [ ] **Task 2.5.1: Create VideoCallController**
  - [ ] Implement `POST /api/v1/video-calls` (initiate call)
  - [ ] Implement `POST /api/v1/video-calls/{callId}/join` (join call)
  - [ ] Implement `POST /api/v1/video-calls/{callId}/end` (end call)
  - [ ] Implement `GET /api/v1/video-calls/{callId}/state` (get call state)
  - [ ] Add proper premium feature authorization
  - [ ] Include comprehensive error handling

- [ ] **Task 2.5.2: Add Video Call Methods to MessagingHub**
  ```csharp
  // Methods to implement in MessagingHub:
  - Task StartPremiumVideoCall(StartVideoCallRequest request)
  - Task JoinVideoCall(string callId, JoinCallRequest request)
  - Task UpdateCallQuality(string callId, CallQualityMetrics metrics)
  - Task LeaveVideoCall(string callId)
  - Task HandleCallSignaling(string callId, SignalingMessage message)
  ```

- [ ] **Task 2.5.3: Implement Call State Management**
  - [ ] Add call status tracking (Pending, Connecting, Active, Ended)
  - [ ] Implement participant management
  - [ ] Add connection monitoring
  - [ ] Include call duration tracking
  - [ ] Support graceful call termination

- [ ] **Task 2.5.4: Add Premium Feature Validation**
  - [ ] Validate subscription tier before call initiation
  - [ ] Check monthly usage limits for Premium users
  - [ ] Enforce call duration limits
  - [ ] Add quality tier enforcement
  - [ ] Include upgrade prompts for limitations

**Acceptance Criteria:**
- Premium users can initiate video calls successfully
- Call state is managed accurately throughout call lifecycle
- Usage limits are enforced correctly
- Free users see appropriate upgrade prompts
- Call quality matches subscription tier

### Epic 2.6: Premium Feature Implementation
**Estimated Effort:** 12 hours  
**Dependencies:** Task 2.5 completion  
**Files to Create/Modify:**
- Update VideoCallService.cs
- Update MessagingHub.cs

#### Tasks:

- [ ] **Task 2.6.1: Implement Usage Tracking**
  - [ ] Track video call minutes for Premium users
  - [ ] Enforce monthly limits (5 hours for Premium)
  - [ ] Add usage reporting API endpoints
  - [ ] Include usage reset on billing cycle
  - [ ] Support usage notifications

- [ ] **Task 2.6.2: Add Quality Tier Enforcement**
  - [ ] Configure 720p for Premium users
  - [ ] Configure 1080p for Premium+ users
  - [ ] Implement automatic quality degradation
  - [ ] Add bandwidth monitoring
  - [ ] Include quality preference settings

- [ ] **Task 2.6.3: Implement Premium Server Routing**
  - [ ] Route Premium+ users to dedicated servers
  - [ ] Configure priority ICE servers for Premium users
  - [ ] Add connection quality monitoring
  - [ ] Implement fallback server logic
  - [ ] Include server load balancing

- [ ] **Task 2.6.4: Add Call Duration Limits**
  - [ ] Enforce 30-minute limit for Premium users
  - [ ] Add warning notifications before limit
  - [ ] Implement graceful call termination
  - [ ] Include upgrade prompts during extended calls
  - [ ] Support call extension for Premium+ users

**Acceptance Criteria:**
- Usage limits are accurately tracked and enforced
- Video quality matches user subscription tier
- Premium users get better connection quality
- Call duration limits work with proper warnings
- Upgrade prompts appear at appropriate times

### Epic 2.7: Frontend Video Integration
**Estimated Effort:** 16 hours  
**Dependencies:** Backend video calling completion  
**Files to Create/Modify:**
- `src/Web/MeAndMyDog.WebApp/wwwroot/js/video-calling.js`
- `src/Web/MeAndMyDog.WebApp/Views/Messaging/VideoCall.cshtml`
- Update messaging views with video call buttons

#### Tasks:

- [ ] **Task 2.7.1: Create WebRTC Client Implementation**
  - [ ] Implement WebRTC peer connection setup
  - [ ] Add camera and microphone access
  - [ ] Include ICE server configuration based on subscription
  - [ ] Add connection quality monitoring
  - [ ] Implement call state management

- [ ] **Task 2.7.2: Create Video Call UI Components**
  - [ ] Design video call interface
  - [ ] Add participant video displays
  - [ ] Include call control buttons (mute, video toggle, end call)
  - [ ] Add call quality indicators
  - [ ] Design upgrade prompt modals

- [ ] **Task 2.7.3: Integrate Premium Feature UI**
  - [ ] Add subscription tier display
  - [ ] Show usage limit indicators
  - [ ] Include quality settings for Premium+ users
  - [ ] Add upgrade buttons and prompts
  - [ ] Display available features by tier

- [ ] **Task 2.7.4: Add Mobile Optimization**
  - [ ] Optimize video calling for mobile browsers
  - [ ] Add responsive design for different screen sizes
  - [ ] Include touch-friendly controls
  - [ ] Add mobile-specific error handling
  - [ ] Test on major mobile browsers

**Acceptance Criteria:**
- Video calling interface is intuitive and professional
- Camera and microphone work reliably
- Premium features are clearly distinguished
- Mobile experience is smooth and responsive
- Upgrade prompts are compelling and informative

### Epic 2.8: Error Handling and Recovery
**Estimated Effort:** 10 hours  
**Dependencies:** All video calling features  
**Files to Modify:**
- All video calling service files
- Frontend video calling components

#### Tasks:

- [ ] **Task 2.8.1: Add Comprehensive Error Handling**
  - [ ] Handle camera/microphone permission denials
  - [ ] Add network connectivity error handling
  - [ ] Include WebRTC connection failure recovery
  - [ ] Add subscription validation error messages
  - [ ] Implement usage limit exceeded handling

- [ ] **Task 2.8.2: Implement Connection Recovery**
  - [ ] Add automatic reconnection logic
  - [ ] Handle temporary network interruptions
  - [ ] Include call quality degradation handling
  - [ ] Add participant dropout management
  - [ ] Support call resume after brief disconnection

- [ ] **Task 2.8.3: Add User Feedback Systems**
  - [ ] Include call quality feedback prompts
  - [ ] Add error reporting mechanisms
  - [ ] Create helpful error messages with solutions
  - [ ] Include upgrade guidance in error states
  - [ ] Add customer support contact options

- [ ] **Task 2.8.4: Implement Graceful Degradation**
  - [ ] Fall back to audio-only when video fails
  - [ ] Support reduced quality for poor connections
  - [ ] Include alternative communication methods
  - [ ] Add offline mode support
  - [ ] Handle server maintenance gracefully

**Acceptance Criteria:**
- Video calls handle errors gracefully without crashing
- Users receive helpful error messages and solutions
- Connection issues are resolved automatically when possible
- Call quality adapts to network conditions
- Alternative communication methods are available

## Testing Requirements

### Epic 2.9: Video Calling Testing
**Estimated Effort:** 14 hours  
**Dependencies:** All implementation completion  

#### Tasks:

- [ ] **Task 2.9.1: Premium Feature Unit Tests**
  - [ ] Test subscription tier validation
  - [ ] Test usage limit enforcement
  - [ ] Test permission checking logic
  - [ ] Test video call service methods
  - [ ] Achieve 90%+ code coverage

- [ ] **Task 2.9.2: Integration Tests**
  - [ ] Test end-to-end video call workflow
  - [ ] Test premium feature gating
  - [ ] Test WebRTC service integration
  - [ ] Test database operations
  - [ ] Test SignalR video call methods

- [ ] **Task 2.9.3: WebRTC Performance Tests**
  - [ ] Test video call quality across different networks
  - [ ] Test concurrent video call limits
  - [ ] Test server load under multiple calls
  - [ ] Test mobile browser compatibility
  - [ ] Identify performance bottlenecks

- [ ] **Task 2.9.4: User Acceptance Testing**
  - [ ] Test subscription upgrade flow
  - [ ] Test premium feature discovery
  - [ ] Test video call user experience
  - [ ] Test error handling scenarios
  - [ ] Gather user feedback on interface

## Phase 2 Deliverables

### Functional Deliverables
✅ **Premium Video Calling System**
- Video calls with subscription tier validation
- Monthly usage limits and tracking
- Quality tier differentiation (720p Premium, 1080p Premium+)
- Call duration limits with graceful termination

✅ **Premium Feature Infrastructure**
- Comprehensive permission system
- Usage tracking and reporting
- Subscription tier management
- Upgrade path guidance

✅ **WebRTC Integration**
- Reliable video call initiation and joining
- Quality-based server routing
- Connection monitoring and recovery
- Mobile-optimized video calling

✅ **User Experience**
- Intuitive video calling interface
- Clear premium feature distinction
- Compelling upgrade prompts
- Error handling and recovery

### Technical Deliverables
✅ **Permission & Authorization**
- Premium feature authorization attributes
- Cached permission checking
- Usage limit enforcement
- Audit logging

✅ **Database Enhancements**
- Video call participant tracking
- Premium feature usage tables
- Call recording support
- Quality metrics storage

✅ **Service Layer**
- Video call management service
- Premium feature validation
- WebRTC provider integration
- Usage tracking service

## Success Metrics

### Primary Metrics
- [ ] **Premium Conversion Rate**: >15% of users upgrade after video call prompts
- [ ] **Video Call Success Rate**: >90% successful call connections for Premium users
- [ ] **Call Setup Time**: <10 seconds average from initiation to connection
- [ ] **Usage Limit Compliance**: 100% accuracy in enforcing monthly limits

### Secondary Metrics
- [ ] **Call Quality Satisfaction**: >4.0/5.0 average rating from Premium users
- [ ] **Server Performance**: <5% server resource utilization per concurrent call
- [ ] **Error Rate**: <2% error rate across all video calling operations
- [ ] **Mobile Compatibility**: Video calling works on 95%+ of mobile browsers

### Business Metrics
- [ ] **Revenue Impact**: Premium video calling drives 25% increase in Premium subscriptions
- [ ] **User Engagement**: Premium users show 40% higher platform engagement
- [ ] **Churn Reduction**: Premium users have 50% lower churn rate
- [ ] **Support Tickets**: <5% of video calls generate support tickets

## Risk Mitigation

### High-Risk Areas
1. **WebRTC Complexity**: 
   - Mitigation: Use proven service provider (Twilio/Agora)
   - Have fallback to audio-only calls
   - Extensive browser compatibility testing

2. **Premium Feature Performance**:
   - Mitigation: Implement caching for permission checks
   - Use database indexes for usage queries
   - Load test with realistic user volumes

3. **Mobile Browser Issues**:
   - Mitigation: Test on all major mobile browsers
   - Implement progressive web app features
   - Have mobile app fallback strategy

### Contingency Plans
- **Video Service Outage**: Switch to backup provider or audio-only mode
- **Permission Service Issues**: Implement circuit breaker pattern with fallback permissions
- **Usage Tracking Failure**: Continue service with post-processing reconciliation

## Dependencies for Next Phase

Phase 2 completion enables:
- **Phase 3**: Group video calls and advanced features
- **Screen Sharing**: WebRTC foundation supports screen sharing
- **Call Recording**: Premium infrastructure supports recording features
- **Professional Tools**: Premium feature system supports advanced tools

## Post-Phase 2 Preparation

To prepare for Phase 3 (Advanced Features):
- [ ] Ensure video calling infrastructure can handle multiple participants
- [ ] Validate screen sharing capability with current WebRTC setup
- [ ] Test recording service scalability
- [ ] Confirm premium feature system supports advanced tools

This premium video calling implementation will create a significant competitive advantage and provide a strong foundation for monetizing the platform through subscription tiers.