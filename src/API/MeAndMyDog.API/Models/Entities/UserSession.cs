namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents an active user session for security tracking
/// </summary>
public class UserSession
{
    /// <summary>
    /// Unique identifier for the session
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// JWT token for this session
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Refresh token for extending the session
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// When the session expires
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
    
    /// <summary>
    /// IP address from which the session was created
    /// </summary>
    public string? IPAddress { get; set; }
    
    /// <summary>
    /// User agent string from the client
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Device information
    /// </summary>
    public string? DeviceInfo { get; set; }
    
    /// <summary>
    /// Location information (if available)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Whether the session is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the session was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }
    
    /// <summary>
    /// When the session ended (if ended)
    /// </summary>
    public DateTimeOffset? EndedAt { get; set; }
    
    /// <summary>
    /// Reason for session ending
    /// </summary>
    public string? EndReason { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}