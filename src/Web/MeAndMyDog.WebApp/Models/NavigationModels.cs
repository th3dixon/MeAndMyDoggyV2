namespace MeAndMyDog.WebApp.Models;

/// <summary>
/// Represents the navigation context for a user
/// </summary>
public class UserNavigationContext
{
    /// <summary>
    /// User's available roles
    /// </summary>
    public List<UserRole> AvailableRoles { get; set; } = new();
    
    /// <summary>
    /// Currently active role
    /// </summary>
    public string? CurrentRole { get; set; }
    
    /// <summary>
    /// Whether user can switch between roles
    /// </summary>
    public bool CanSwitchRoles { get; set; }
    
    /// <summary>
    /// Default dashboard URL
    /// </summary>
    public string DefaultDashboardUrl { get; set; } = "/Dashboard";
    
    /// <summary>
    /// User display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional user context data
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
}

/// <summary>
/// Represents a user role with navigation information
/// </summary>
public class UserRole
{
    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the role
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Icon for the role
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// Dashboard URL for this role
    /// </summary>
    public string DashboardUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the role
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this role is currently active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Additional role-specific data
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents a navigation menu item
/// </summary>
public class NavigationMenuItem
{
    /// <summary>
    /// Menu item ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Display text
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// URL or route
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// Icon class or name
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// Required role to access this item
    /// </summary>
    public string? RequiredRole { get; set; }
    
    /// <summary>
    /// Whether this item is currently active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Sub-menu items
    /// </summary>
    public List<NavigationMenuItem> SubItems { get; set; } = new();
    
    /// <summary>
    /// Display order
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Whether to open in new window/tab
    /// </summary>
    public bool OpenInNewWindow { get; set; }
    
    /// <summary>
    /// Badge text (e.g., for notifications)
    /// </summary>
    public string? Badge { get; set; }
    
    /// <summary>
    /// Badge color class
    /// </summary>
    public string? BadgeColor { get; set; }
    
    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }
}