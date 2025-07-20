using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a service category in the platform
/// </summary>
public class ServiceCategory
{
    /// <summary>
    /// Gets or sets the unique identifier for the service category
    /// </summary>
    [Key]
    public Guid ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the service category
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the service category
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CSS icon class for the service category (typically Font Awesome classes)
    /// </summary>
    [MaxLength(50)]
    public string IconClass { get; set; } = string.Empty; // Font Awesome classes
    
    /// <summary>
    /// Gets or sets the color code for the service category (typically Tailwind color classes)
    /// </summary>
    [MaxLength(20)]
    public string ColorCode { get; set; } = string.Empty; // Tailwind color classes
    
    /// <summary>
    /// Gets or sets a value indicating whether the service category is active and available
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the display order for sorting service categories in the user interface
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the service category was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the service category was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Gets or sets the collection of sub-services associated with this service category (navigation property)
    /// </summary>
    public List<SubService> SubServices { get; set; } = new();
    /// <summary>
    /// Gets or sets the collection of provider services associated with this service category (navigation property)
    /// </summary>
    public List<ProviderService> ProviderServices { get; set; } = new();
}