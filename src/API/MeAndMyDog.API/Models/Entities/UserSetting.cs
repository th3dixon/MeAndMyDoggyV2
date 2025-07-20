namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// User-specific configuration settings
/// </summary>
public class UserSetting
{
    /// <summary>
    /// Unique identifier for the setting
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Setting key/name
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Category grouping for settings
    /// </summary>
    public string Category { get; set; } = "General";
    
    /// <summary>
    /// Data type of the value (string, int, bool, decimal, etc.)
    /// </summary>
    public string DataType { get; set; } = "string";
    
    /// <summary>
    /// When the setting was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the setting was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}