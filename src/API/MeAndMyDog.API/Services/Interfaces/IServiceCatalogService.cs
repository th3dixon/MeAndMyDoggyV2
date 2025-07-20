using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.DTOs.ServiceCatalog;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for managing service catalog operations
/// </summary>
public interface IServiceCatalogService
{
    /// <summary>
    /// Retrieves all active service categories with their sub-services
    /// </summary>
    /// <returns>List of service categories</returns>
    Task<ServiceResult<List<ServiceCategoryDto>>> GetServiceCategoriesAsync();
    
    /// <summary>
    /// Retrieves a specific service category by its identifier
    /// </summary>
    /// <param name="categoryId">The service category identifier</param>
    /// <returns>Service category if found, null otherwise</returns>
    Task<ServiceResult<ServiceCategoryDto?>> GetServiceCategoryByIdAsync(Guid categoryId);
    
    /// <summary>
    /// Retrieves all sub-services for a specific category
    /// </summary>
    /// <param name="categoryId">The service category identifier</param>
    /// <returns>List of sub-services for the category</returns>
    Task<ServiceResult<List<SubServiceDto>>> GetSubServicesByCategoryAsync(Guid categoryId);
}