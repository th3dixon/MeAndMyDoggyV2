using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.DTOs.ServiceCatalog;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Implementation of service catalog operations
/// </summary>
public class ServiceCatalogService : IServiceCatalogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceCatalogService> _logger;
    private readonly IMemoryCache _cache;
    private const string SERVICE_CATEGORIES_CACHE_KEY = "service_categories_all";
    private const int CACHE_DURATION_MINUTES = 60;

    /// <summary>
    /// Initializes a new instance of the ServiceCatalogService
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="cache">Memory cache instance</param>
    public ServiceCatalogService(ApplicationDbContext context, ILogger<ServiceCatalogService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Retrieves all active service categories with their sub-services
    /// </summary>
    /// <returns>Service result containing list of service categories</returns>
    public async Task<ServiceResult<List<ServiceCategoryDto>>> GetServiceCategoriesAsync()
    {
        try
        {
            // Try to get from cache first
            if (_cache.TryGetValue(SERVICE_CATEGORIES_CACHE_KEY, out List<ServiceCategoryDto>? cachedCategories))
            {
                _logger.LogDebug("Returning service categories from cache");
                return ServiceResult<List<ServiceCategoryDto>>.SuccessResult(cachedCategories!);
            }

            // If not in cache, fetch from database
            var categories = await _context.ServiceCategories
                .Where(sc => sc.IsActive)
                .Include(sc => sc.SubServices.Where(ss => ss.IsActive))
                .OrderBy(sc => sc.DisplayOrder)
                .Select(sc => new ServiceCategoryDto
                {
                    ServiceCategoryId = sc.ServiceCategoryId,
                    Name = sc.Name,
                    Description = sc.Description,
                    IconClass = sc.IconClass,
                    ColorCode = sc.ColorCode,
                    DisplayOrder = sc.DisplayOrder,
                    SubServices = sc.SubServices
                        .Where(ss => ss.IsActive)
                        .OrderBy(ss => ss.DisplayOrder)
                        .Select(ss => new SubServiceDto
                        {
                            SubServiceId = ss.SubServiceId,
                            Name = ss.Name,
                            Description = ss.Description,
                            DurationMinutes = ss.DurationMinutes,
                            SuggestedMinPrice = ss.SuggestedMinPrice,
                            SuggestedMaxPrice = ss.SuggestedMaxPrice,
                            PricingType = GetPricingTypeDescription(ss.DefaultPricingType),
                            DisplayOrder = ss.DisplayOrder
                        }).ToList()
                })
                .ToListAsync();

            // Cache the result
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES))
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            _cache.Set(SERVICE_CATEGORIES_CACHE_KEY, categories, cacheEntryOptions);
            _logger.LogDebug("Service categories cached for {Duration} minutes", CACHE_DURATION_MINUTES);

            return ServiceResult<List<ServiceCategoryDto>>.SuccessResult(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving service categories");
            return ServiceResult<List<ServiceCategoryDto>>.FailureResult("Error retrieving service categories");
        }
    }

    /// <summary>
    /// Retrieves a specific service category by its identifier
    /// </summary>
    /// <param name="categoryId">The service category identifier</param>
    /// <returns>Service result containing the service category if found</returns>
    public async Task<ServiceResult<ServiceCategoryDto?>> GetServiceCategoryByIdAsync(Guid categoryId)
    {
        try
        {
            var category = await _context.ServiceCategories
                .Where(sc => sc.ServiceCategoryId == categoryId && sc.IsActive)
                .Include(sc => sc.SubServices.Where(ss => ss.IsActive))
                .Select(sc => new ServiceCategoryDto
                {
                    ServiceCategoryId = sc.ServiceCategoryId,
                    Name = sc.Name,
                    Description = sc.Description,
                    IconClass = sc.IconClass,
                    ColorCode = sc.ColorCode,
                    DisplayOrder = sc.DisplayOrder,
                    SubServices = sc.SubServices
                        .Where(ss => ss.IsActive)
                        .OrderBy(ss => ss.DisplayOrder)
                        .Select(ss => new SubServiceDto
                        {
                            SubServiceId = ss.SubServiceId,
                            Name = ss.Name,
                            Description = ss.Description,
                            DurationMinutes = ss.DurationMinutes,
                            SuggestedMinPrice = ss.SuggestedMinPrice,
                            SuggestedMaxPrice = ss.SuggestedMaxPrice,
                            PricingType = GetPricingTypeDescription(ss.DefaultPricingType),
                            DisplayOrder = ss.DisplayOrder
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return ServiceResult<ServiceCategoryDto?>.SuccessResult(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving service category {CategoryId}", categoryId);
            return ServiceResult<ServiceCategoryDto?>.FailureResult("Error retrieving service category");
        }
    }

    /// <summary>
    /// Retrieves all sub-services for a specific category
    /// </summary>
    /// <param name="categoryId">The service category identifier</param>
    /// <returns>Service result containing list of sub-services for the category</returns>
    public async Task<ServiceResult<List<SubServiceDto>>> GetSubServicesByCategoryAsync(Guid categoryId)
    {
        try
        {
            var subServices = await _context.SubServices
                .Where(ss => ss.ServiceCategoryId == categoryId && ss.IsActive)
                .OrderBy(ss => ss.DisplayOrder)
                .Select(ss => new SubServiceDto
                {
                    SubServiceId = ss.SubServiceId,
                    Name = ss.Name,
                    Description = ss.Description,
                    DurationMinutes = ss.DurationMinutes,
                    SuggestedMinPrice = ss.SuggestedMinPrice,
                    SuggestedMaxPrice = ss.SuggestedMaxPrice,
                    PricingType = GetPricingTypeDescription(ss.DefaultPricingType),
                    DisplayOrder = ss.DisplayOrder
                })
                .ToListAsync();

            return ServiceResult<List<SubServiceDto>>.SuccessResult(subServices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sub-services for category {CategoryId}", categoryId);
            return ServiceResult<List<SubServiceDto>>.FailureResult("Error retrieving sub-services");
        }
    }

    private static string GetPricingTypeDescription(PricingType pricingType)
    {
        return pricingType switch
        {
            PricingType.PerService => "Per Service",
            PricingType.PerHour => "Per Hour",
            PricingType.PerDay => "Per Day",
            PricingType.PerNight => "Per Night",
            PricingType.PerWeek => "Per Week",
            PricingType.PerMonth => "Per Month",
            _ => "Per Service"
        };
    }
}