namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Junction entity for many-to-many relationship between roles and permissions
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Foreign key to the role
    /// </summary>
    public string RoleId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the permission
    /// </summary>
    public string PermissionId { get; set; } = string.Empty;
    
    /// <summary>
    /// When this permission was granted to the role
    /// </summary>
    public DateTimeOffset GrantedAt { get; set; }
    
    /// <summary>
    /// User who granted this permission
    /// </summary>
    public string? GrantedBy { get; set; }
    
    /// <summary>
    /// Navigation property to the role
    /// </summary>
    public virtual ApplicationRole Role { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the permission
    /// </summary>
    public virtual Permission Permission { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user who granted the permission
    /// </summary>
    public virtual ApplicationUser? GrantedByUser { get; set; }
}