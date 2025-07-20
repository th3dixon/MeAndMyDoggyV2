namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// System-wide configuration settings
/// </summary>
public class SystemSetting
{
    /// <summary>
    /// Unique identifier for the setting
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Setting key/name
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Setting description for administrative purposes
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Category grouping for settings
    /// </summary>
    public string Category { get; set; } = "General";
    
    /// <summary>
    /// Data type of the value (string, int, bool, decimal, etc.)
    /// </summary>
    public string DataType { get; set; } = "string";
    
    /// <summary>
    /// Whether this setting can be modified
    /// </summary>
    public bool IsReadOnly { get; set; } = false;
    
    /// <summary>
    /// When the setting was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the setting was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}