namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Audit log for tracking system actions and changes
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier for the audit entry
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// User who performed the action (null for system actions)
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Type of action performed
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Entity type that was affected
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the entity that was affected
    /// </summary>
    public string? EntityId { get; set; }
    
    /// <summary>
    /// Old values (JSON format)
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// New values (JSON format)
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// Additional context or metadata
    /// </summary>
    public string? Metadata { get; set; }
    
    /// <summary>
    /// IP address from which the action was performed
    /// </summary>
    public string? IPAddress { get; set; }
    
    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// When the action was performed
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser? User { get; set; }
}