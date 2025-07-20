namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a permission in the role-based access control system
/// </summary>
public class Permission
{
    /// <summary>
    /// Unique identifier for the permission
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Permission name (e.g., "kyc.review", "ai.access")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Human-readable description of the permission
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Category grouping for permissions (e.g., "Security", "AI", "Messaging")
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// When the permission was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Navigation property for role permissions
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}