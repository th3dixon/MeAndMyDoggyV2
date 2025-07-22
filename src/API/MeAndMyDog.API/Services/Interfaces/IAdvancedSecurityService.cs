using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for advanced message security features
/// </summary>
public interface IAdvancedSecurityService
{
    #region Self-Destruct Messages

    /// <summary>
    /// Configure self-destruct for a message
    /// </summary>
    /// <param name="userId">User configuring the self-destruct</param>
    /// <param name="request">Self-destruct configuration</param>
    /// <returns>Self-destruct response</returns>
    Task<SelfDestructResponse> ConfigureSelfDestructAsync(string userId, ConfigureSelfDestructRequest request);

    /// <summary>
    /// Update self-destruct configuration
    /// </summary>
    /// <param name="userId">User updating the configuration</param>
    /// <param name="selfDestructId">Self-destruct ID to update</param>
    /// <param name="request">Updated configuration</param>
    /// <returns>Self-destruct response</returns>
    Task<SelfDestructResponse> UpdateSelfDestructAsync(string userId, string selfDestructId, UpdateSelfDestructRequest request);

    /// <summary>
    /// Get self-destruct configuration for a message
    /// </summary>
    /// <param name="userId">User requesting the configuration</param>
    /// <param name="messageId">Message ID</param>
    /// <returns>Self-destruct configuration or null if not found</returns>
    Task<SelfDestructMessageDto?> GetSelfDestructConfigAsync(string userId, string messageId);

    /// <summary>
    /// Manually trigger self-destruct for a message
    /// </summary>
    /// <param name="userId">User triggering the destruction</param>
    /// <param name="messageId">Message ID to destroy</param>
    /// <param name="reason">Reason for manual destruction</param>
    /// <returns>True if successful</returns>
    Task<bool> TriggerSelfDestructAsync(string userId, string messageId, string? reason = null);

    /// <summary>
    /// Cancel self-destruct for a message
    /// </summary>
    /// <param name="userId">User cancelling the self-destruct</param>
    /// <param name="messageId">Message ID</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelSelfDestructAsync(string userId, string messageId);

    /// <summary>
    /// Track message view for self-destruct purposes
    /// </summary>
    /// <param name="userId">User viewing the message</param>
    /// <param name="messageId">Message ID being viewed</param>
    /// <param name="viewDurationMs">How long the message was viewed</param>
    /// <param name="clientInfo">Client information</param>
    /// <returns>View tracking result</returns>
    Task<MessageViewTrackingDto> TrackMessageViewAsync(string userId, string messageId, long viewDurationMs, ClientInformationDto? clientInfo = null);

    /// <summary>
    /// Get pending self-destruct messages that need to be destroyed
    /// </summary>
    /// <param name="beforeTime">Messages that should be destroyed before this time</param>
    /// <returns>List of messages to destroy</returns>
    Task<List<SelfDestructMessageDto>> GetPendingDestructionAsync(DateTimeOffset beforeTime);

    /// <summary>
    /// Execute destruction for expired messages
    /// </summary>
    /// <returns>Number of messages destroyed</returns>
    Task<int> ExecutePendingDestructionAsync();

    #endregion

    #region Message Security

    /// <summary>
    /// Configure advanced security for a message
    /// </summary>
    /// <param name="userId">User configuring security</param>
    /// <param name="request">Security configuration</param>
    /// <returns>Security configuration response</returns>
    Task<MessageSecurityResponse> ConfigureMessageSecurityAsync(string userId, ConfigureMessageSecurityRequest request);

    /// <summary>
    /// Get security configuration for a message
    /// </summary>
    /// <param name="userId">User requesting the configuration</param>
    /// <param name="messageId">Message ID</param>
    /// <returns>Security configuration or null if not found</returns>
    Task<MessageSecurityDto?> GetMessageSecurityAsync(string userId, string messageId);

    /// <summary>
    /// Validate access to a secure message
    /// </summary>
    /// <param name="userId">User requesting access</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="clientInfo">Client information for verification</param>
    /// <param name="verificationCode">Optional verification code</param>
    /// <returns>Access validation result</returns>
    Task<MessageAccessValidationResult> ValidateMessageAccessAsync(string userId, string messageId, ClientInformationDto clientInfo, string? verificationCode = null);

    /// <summary>
    /// Log message access for audit purposes
    /// </summary>
    /// <param name="userId">User accessing the message</param>
    /// <param name="messageId">Message ID being accessed</param>
    /// <param name="accessType">Type of access (view, download, print, etc.)</param>
    /// <param name="clientInfo">Client information</param>
    /// <param name="accessGranted">Whether access was granted</param>
    /// <param name="denialReason">Reason if access was denied</param>
    /// <returns>Access log entry</returns>
    Task<MessageAccessLogDto> LogMessageAccessAsync(string userId, string messageId, string accessType, ClientInformationDto clientInfo, bool accessGranted, string? denialReason = null);

    /// <summary>
    /// Get access logs for a message
    /// </summary>
    /// <param name="userId">User requesting logs</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="limit">Maximum number of logs to return</param>
    /// <returns>List of access logs</returns>
    Task<List<MessageAccessLogDto>> GetMessageAccessLogsAsync(string userId, string messageId, int limit = 50);

    /// <summary>
    /// Calculate risk score for message access
    /// </summary>
    /// <param name="userId">User accessing the message</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="clientInfo">Client information</param>
    /// <returns>Risk score (0-100)</returns>
    Task<double> CalculateAccessRiskScoreAsync(string userId, string messageId, ClientInformationDto clientInfo);

    /// <summary>
    /// Generate watermark for a message
    /// </summary>
    /// <param name="userId">User viewing the message</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="watermarkText">Custom watermark text</param>
    /// <returns>Watermark configuration</returns>
    Task<WatermarkDto> GenerateWatermarkAsync(string userId, string messageId, string? watermarkText = null);

    #endregion

    #region Security Incidents

    /// <summary>
    /// Create a security incident
    /// </summary>
    /// <param name="request">Incident creation request</param>
    /// <returns>Created incident</returns>
    Task<SecurityIncidentResponse> CreateSecurityIncidentAsync(CreateSecurityIncidentRequest request);

    /// <summary>
    /// Update a security incident
    /// </summary>
    /// <param name="userId">User updating the incident</param>
    /// <param name="incidentId">Incident ID to update</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated incident</returns>
    Task<SecurityIncidentResponse> UpdateSecurityIncidentAsync(string userId, string incidentId, UpdateSecurityIncidentRequest request);

    /// <summary>
    /// Get security incident by ID
    /// </summary>
    /// <param name="userId">User requesting the incident</param>
    /// <param name="incidentId">Incident ID</param>
    /// <returns>Incident details or null if not found</returns>
    Task<SecurityIncidentDto?> GetSecurityIncidentAsync(string userId, string incidentId);

    /// <summary>
    /// Search security incidents
    /// </summary>
    /// <param name="userId">User performing the search</param>
    /// <param name="request">Search criteria</param>
    /// <returns>Search results</returns>
    Task<SearchSecurityIncidentsResponse> SearchSecurityIncidentsAsync(string userId, SearchSecurityIncidentsRequest request);

    /// <summary>
    /// Automatically detect and create security incidents
    /// </summary>
    /// <param name="userId">User involved in potential incident</param>
    /// <param name="messageId">Related message ID</param>
    /// <param name="clientInfo">Client information</param>
    /// <param name="accessAttempt">Access attempt details</param>
    /// <returns>True if incident was created</returns>
    Task<bool> DetectSecurityIncidentAsync(string userId, string? messageId, ClientInformationDto clientInfo, string accessAttempt);

    /// <summary>
    /// Get security incident statistics
    /// </summary>
    /// <param name="userId">User requesting statistics (admin only)</param>
    /// <param name="fromDate">From date</param>
    /// <param name="toDate">To date</param>
    /// <returns>Security statistics</returns>
    Task<SecurityStatsDto> GetSecurityStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

    /// <summary>
    /// Resolve a security incident
    /// </summary>
    /// <param name="userId">User resolving the incident</param>
    /// <param name="incidentId">Incident ID to resolve</param>
    /// <param name="resolutionSummary">Resolution summary</param>
    /// <returns>True if resolved successfully</returns>
    Task<bool> ResolveSecurityIncidentAsync(string userId, string incidentId, string resolutionSummary);

    #endregion

    #region Security Analytics

    /// <summary>
    /// Analyze user behavior for security anomalies
    /// </summary>
    /// <param name="userId">User to analyze</param>
    /// <param name="timeWindow">Time window for analysis</param>
    /// <returns>Security analysis result</returns>
    Task<UserSecurityAnalysisDto> AnalyzeUserSecurityAsync(string userId, TimeSpan timeWindow);

    /// <summary>
    /// Generate security compliance report
    /// </summary>
    /// <param name="userId">User generating report (admin only)</param>
    /// <param name="fromDate">Report from date</param>
    /// <param name="toDate">Report to date</param>
    /// <returns>Compliance report</returns>
    Task<SecurityComplianceReportDto> GenerateComplianceReportAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate);

    /// <summary>
    /// Check if user has required security clearance
    /// </summary>
    /// <param name="userId">User to check</param>
    /// <param name="requiredLevel">Required clearance level</param>
    /// <returns>True if user has required clearance</returns>
    Task<bool> CheckSecurityClearanceAsync(string userId, string requiredLevel);

    /// <summary>
    /// Verify user location against restrictions
    /// </summary>
    /// <param name="userId">User to verify</param>
    /// <param name="clientInfo">Client information with location</param>
    /// <param name="restrictions">Geographic restrictions</param>
    /// <returns>True if location is allowed</returns>
    Task<bool> VerifyGeographicRestrictionsAsync(string userId, ClientInformationDto clientInfo, List<string> restrictions);

    /// <summary>
    /// Verify access time against restrictions
    /// </summary>
    /// <param name="userId">User to verify</param>
    /// <param name="timeRestrictions">Time-based restrictions</param>
    /// <returns>True if current time is allowed</returns>
    Task<bool> VerifyTimeRestrictionsAsync(string userId, TimeRestrictionDto timeRestrictions);

    #endregion
}