using Microsoft.AspNetCore.Identity;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a role in the MeAndMyDog application, extending ASP.NET Core Identity
/// </summary>
public class ApplicationRole : IdentityRole<string>
{
    /// <summary>
    /// Role description for administrative purposes
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Indicates if this is a system-defined role that cannot be deleted
    /// </summary>
    public bool IsSystemRole { get; set; } = false;
    
    /// <summary>
    /// When the role was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the role was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property for role permissions
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}