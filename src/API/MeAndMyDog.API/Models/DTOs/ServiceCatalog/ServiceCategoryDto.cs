namespace MeAndMyDog.API.Models.DTOs.ServiceCatalog;

/// <summary>
/// Data transfer object for service category information
/// </summary>
public class ServiceCategoryDto
{
    /// <summary>
    /// Unique identifier for the service category
    /// </summary>
    public Guid ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Name of the service category
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the service category
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// CSS icon class for display
    /// </summary>
    public string IconClass { get; set; } = string.Empty;
    
    /// <summary>
    /// Color code for category theming
    /// </summary>
    public string ColorCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// List of sub-services within this category
    /// </summary>
    public List<SubServiceDto> SubServices { get; set; } = new();
}

