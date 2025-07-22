using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.DTOs.ProviderSearch;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;
using System.Data;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Implementation of provider search service with caching and optimization
/// </summary>
public class ProviderSearchService : IProviderSearchService
{
    private readonly ApplicationDbContext _context;
    private readonly ILocationService _locationService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProviderSearchService> _logger;
    
    private const int DEFAULT_CACHE_MINUTES = 10;
    private const int MAX_SEARCH_RESULTS = 100;
    private const int MAX_RADIUS_MILES = 50;

    public ProviderSearchService(
        ApplicationDbContext context,
        ILocationService locationService,
        IMemoryCache cache,
        ILogger<ProviderSearchService> logger)
    {
        _context = context;
        _locationService = locationService;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ProviderSearchResponseDto> SearchProvidersAsync(
        ProviderSearchFilterDto filters, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting provider search with filters: {@Filters}", filters);
            
            // Validate and normalize inputs
            var searchRadius = Math.Min(filters.RadiusMiles, MAX_RADIUS_MILES);
            var pageSize = Math.Min(filters.PageSize, MAX_SEARCH_RESULTS);
            var pageNumber = Math.Max(filters.Page, 1);

            // Get coordinates for location search - use provided coordinates or geocode location
            LocationCoordinates? coordinates = null;
            
            if (filters.Latitude.HasValue && filters.Longitude.HasValue)
            {
                // Use provided coordinates directly
                coordinates = new LocationCoordinates
                {
                    Latitude = (decimal)filters.Latitude.Value,
                    Longitude = (decimal)filters.Longitude.Value
                };
                _logger.LogInformation("Using provided coordinates: Lat={Latitude}, Lng={Longitude}", 
                    coordinates.Latitude, coordinates.Longitude);
            }
            else
            {
                // Fallback to geocoding the location string
                coordinates = await _locationService.GetCoordinatesFromPostCodeAsync(filters.Location ?? "");
                _logger.LogInformation("Geocoded location '{Location}' to coordinates: Lat={Latitude}, Lng={Longitude}", 
                    filters.Location, coordinates?.Latitude, coordinates?.Longitude);
            }
            
            if (coordinates == null)
            {
                _logger.LogWarning("Could not resolve location: {Location}, Lat={Latitude}, Lng={Longitude}", 
                    filters.Location, filters.Latitude, filters.Longitude);
                return new ProviderSearchResponseDto
                {
                    Results = new List<ProviderSearchResultDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Message = "Could not find the specified location"
                };
            }

            // Build cache key for this search
            var cacheKey = BuildSearchCacheKey(filters, (double)coordinates.Latitude, (double)coordinates.Longitude);
            
            // Check cache first
            if (_cache.TryGetValue(cacheKey, out ProviderSearchResponseDto? cachedResult))
            {
                _logger.LogInformation("Returning cached search results for key: {CacheKey}", cacheKey);
                return cachedResult!;
            }

            // Execute search via stored procedure for optimal performance
            var results = await ExecuteProviderSearchAsync(
                (double)coordinates.Latitude, 
                (double)coordinates.Longitude, 
                searchRadius,
                filters,
                pageNumber,
                pageSize,
                cancellationToken);

            // Build response
            var response = new ProviderSearchResponseDto
            {
                Results = results.providers,
                TotalCount = results.totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                // TotalPages is a calculated property, no need to set it
                // HasNextPage is a calculated property, no need to set it
                Filters = filters
            };

            // Cache results for future searches
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES));
            
            _logger.LogInformation("Provider search completed. Found {Count} providers", results.totalCount);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing provider search");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProviderSearchResultDto?> GetProviderDetailsAsync(
        string providerId, 
        bool includeAvailability = false,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting provider details for ID: {ProviderId}", providerId);

            // Get real provider from database
            var provider = await _context.ServiceProviders
                .AsNoTracking() // Read-only query optimization
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Id == providerId && sp.IsActive, cancellationToken);

            if (provider == null)
            {
                _logger.LogWarning("Provider not found: {ProviderId}", providerId);
                return null;
            }

            // Get provider's services using ProviderService junction table
            // Parse provider ID outside the query
            var providerGuid = Guid.Parse(provider.Id);
            
            var providerServices = await _context.ProviderService
                .AsNoTracking() // Read-only query optimization
                .Include(ps => ps.ServiceCategory)
                .Include(ps => ps.Pricing)
                    .ThenInclude(p => p.SubService)
                .Where(ps => ps.ProviderId == providerGuid && ps.IsOffered)
                .ToListAsync(cancellationToken);

            var services = new List<ProviderServiceDto>();
            foreach (var ps in providerServices)
            {
                // Get sub-services from already loaded data
                var subServices = ps.Pricing
                    .Where(psp => psp.IsAvailable)
                    .Select(psp => new ProviderSubServiceDto
                    {
                        SubServiceId = psp.SubServiceId.ToString(),
                        Name = psp.SubService.Name,
                        Price = psp.Price,
                        PricingType = psp.PricingType.ToString(),
                        DurationMinutes = psp.SubService.DurationMinutes,
                        IsAvailable = psp.IsAvailable
                    })
                    .ToList();

                services.Add(new ProviderServiceDto
                {
                    ServiceId = ps.ServiceCategoryId.ToString(),
                    ServiceName = ps.ServiceCategory.Name,
                    SubServiceName = subServices.FirstOrDefault()?.Name ?? "",
                    BasePrice = subServices.FirstOrDefault()?.Price ?? 0,
                    Currency = "GBP",
                    DurationMinutes = 60, // Default duration
                    MaxPets = 1, // Default max pets
                    IsEmergencyAvailable = ps.OffersEmergencyService,
                    HasWeekendSurcharge = false, // Default
                    HasEveningSurcharge = false, // Default
                    Description = ps.SpecialNotes
                });
            }

            // Get price range from already loaded pricing data
            var pricingData = providerServices
                .SelectMany(ps => ps.Pricing)
                .ToList();

            var priceRange = new PriceRangeDto();
            if (pricingData.Any())
            {
                priceRange.MinPrice = pricingData.Min(p => p.Price);
                priceRange.MaxPrice = pricingData.Max(p => p.Price);
                priceRange.CommonPricingType = "per service";
            }

            // Get the most recent completed job date for this provider
            var lastCompletedJob = await _context.Bookings
                .AsNoTracking() // Read-only query optimization
                .Where(b => b.ServiceProviderId == provider.Id && 
                           b.Status == "Completed" && 
                           b.CompletedAt.HasValue)
                .OrderByDescending(b => b.CompletedAt)
                .Select(b => b.CompletedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var result = new ProviderSearchResultDto
            {
                Id = provider.Id,
                BusinessName = provider.BusinessName,
                ProviderName = $"{provider.User.FirstName} {provider.User.LastName}",
                Description = provider.BusinessDescription,
                Location = new ProviderLocationDto
                {
                    Postcode = provider.User.PostCode ?? "",
                    City = provider.User.City,
                    County = provider.User.County,
                    ServiceRadiusMiles = 10,
                    Latitude = (double?)provider.User.Latitude,
                    Longitude = (double?)provider.User.Longitude
                },
                Rating = provider.Rating,
                ReviewCount = 0,
                IsVerified = provider.IsVerified,
                Services = services,
                PriceRange = priceRange,
                ResponseTimeHours = 2.0m,
                ReliabilityScore = 0.95m,
                YearsOfExperience = provider.YearsOfExperience,
                LastJobCompletedDate = lastCompletedJob,
                Specializations = provider.Specializations?.Split(',').ToList() ?? new List<string>(),
                ProfileImageUrl = provider.User.ProfileImageUrl,
                Website = provider.BusinessWebsite,
                OffersEmergencyService = providerServices.Any(ps => ps.OffersEmergencyService),
                OffersWeekendService = providerServices.Any(ps => ps.OffersWeekendService),
                OffersEveningService = providerServices.Any(ps => ps.OffersEveningService)
            };

            if (includeAvailability && startDate.HasValue && endDate.HasValue)
            {
                var availability = await CheckProviderAvailabilityAsync(
                    providerId, startDate.Value, endDate.Value, cancellationToken: cancellationToken);
                result.Availability = availability;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider details for ID: {ProviderId}", providerId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<ProviderSearchResultDto>> GetNearbyProvidersAsync(
        double latitude, 
        double longitude, 
        int radiusMiles = 10,
        List<string>? serviceCategories = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting nearby providers at {Lat},{Lng} within {Radius} miles", 
                latitude, longitude, radiusMiles);

            // Use the main search method with simplified filters
            var filters = new ProviderSearchFilterDto
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusMiles = radiusMiles,
                ServiceCategoryIds = serviceCategories,
                PageSize = 20
            };

            var searchResult = await SearchProvidersAsync(filters, cancellationToken);
            return searchResult.Results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearby providers");
            return new List<ProviderSearchResultDto>();
        }
    }

    /// <inheritdoc />
    public async Task<ProviderAvailabilityDto> CheckProviderAvailabilityAsync(
        string providerId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string? serviceId = null,
        int petCount = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking availability for provider {ProviderId} from {Start} to {End}", 
                providerId, startDate, endDate);

            // Check if provider exists and is active
            var provider = await _context.ServiceProviders
                .AsNoTracking() // Read-only query optimization
                .FirstOrDefaultAsync(sp => sp.Id == providerId && sp.IsActive, cancellationToken);

            if (provider == null)
            {
                return new ProviderAvailabilityDto 
                { 
                    ProviderId = providerId,
                    IsAvailable = false, 
                    Message = "Provider not found or inactive" 
                };
            }

            // Check for existing bookings that would conflict with the requested time slot
            // For now, assume providers are generally available
            var conflictingBookings = 0;
            var dayOfWeek = startDate.DayOfWeek;
            
            // Simplified availability logic - assume providers are available during business hours
            bool isWithinOperatingHours = startDate.Hour >= 8 && endDate.Hour <= 18;
            bool isAvailable = conflictingBookings == 0 && isWithinOperatingHours;
            
            // Calculate available slots (simplified - assuming 2-hour slots)
            int availableSlots = 0;
            if (isAvailable)
            {
                var totalMinutes = (18 - 8) * 60; // 8 AM to 6 PM = 10 hours
                availableSlots = Math.Max(0, (int)(totalMinutes / 120) - conflictingBookings); // 2-hour slots
            }

            // Find next available slot if current time is not available
            DateTimeOffset nextAvailableSlot = startDate;
            if (!isAvailable)
            {
                // Simplified logic: next business day at 9 AM
                nextAvailableSlot = startDate.AddDays(1).Date.AddHours(9);
                // Skip weekends
                while (nextAvailableSlot.DayOfWeek == DayOfWeek.Saturday || nextAvailableSlot.DayOfWeek == DayOfWeek.Sunday)
                {
                    nextAvailableSlot = nextAvailableSlot.AddDays(1);
                }
            }
            
            return new ProviderAvailabilityDto
            {
                ProviderId = providerId,
                IsAvailable = isAvailable,
                AvailableSlots = availableSlots,
                NextAvailableSlot = nextAvailableSlot,
                Message = isAvailable ? "Available for booking" : 
                         !isWithinOperatingHours ? "Outside operating hours" :
                         "Time slot already booked"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking provider availability");
            return new ProviderAvailabilityDto { IsAvailable = false, Message = "Error checking availability" };
        }
    }

    /// <inheritdoc />
    public async Task<List<AvailabilitySlotDto>> GetAvailableTimeSlotsAsync(
        string providerId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        int serviceDurationMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting time slots for provider {ProviderId}", providerId);

            // Check if provider exists and is active
            var provider = await _context.ServiceProviders
                .AsNoTracking() // Read-only query optimization
                .FirstOrDefaultAsync(sp => sp.Id == providerId && sp.IsActive, cancellationToken);

            if (provider == null)
            {
                return new List<AvailabilitySlotDto>();
            }

            var slots = new List<AvailabilitySlotDto>();
            var current = startDate.Date;

            // For now, return simplified availability slots
            var timeSlots = new List<AvailabilitySlotDto>();

            // Get pricing information for the provider
            // Parse provider ID outside the query
            var providerGuid = Guid.Parse(providerId);
            
            var pricing = await _context.ProviderServicePricing
                .AsNoTracking() // Read-only query optimization
                .Include(psp => psp.ProviderService)
                .Where(psp => psp.ProviderService.ProviderId == providerGuid)
                .FirstOrDefaultAsync(cancellationToken);

            decimal pricePerSlot = pricing?.Price ?? 25.00m; // Default price if not found

            // Simplified availability logic since ProviderAvailability DbSet not implemented
            while (current.Date <= endDate.Date)
            {
                var dayOfWeek = current.DayOfWeek;
                
                // Skip weekends for now (can be made configurable later)
                if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
                {
                    // Generate time slots for business hours (9 AM to 5 PM)
                    var slotStart = current.Date.AddHours(9); // 9 AM
                    var dayEnd = current.Date.AddHours(17); // 5 PM

                    while (slotStart.Add(TimeSpan.FromMinutes(serviceDurationMinutes)) <= dayEnd)
                    {
                        var slotEnd = slotStart.AddMinutes(serviceDurationMinutes);

                        // Simplified conflict check - assume no conflicts for now
                        bool isConflicted = false;

                        // Only include slots that are in the future and within the requested range
                        if (slotStart >= startDate && slotEnd <= endDate && slotStart >= DateTimeOffset.Now && !isConflicted)
                        {
                            slots.Add(new AvailabilitySlotDto
                            {
                                StartTime = slotStart,
                                EndTime = slotEnd,
                                IsAvailable = true,
                                MaxPets = 3, // Default max pets per slot
                                PricePerPet = pricePerSlot
                            });
                        }

                        // Move to next slot (allowing some buffer time between bookings)
                        slotStart = slotStart.AddMinutes(serviceDurationMinutes + 30); // 30-minute buffer
                    }
                }

                current = current.AddDays(1);
            }

            return slots.OrderBy(s => s.StartTime).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time slots");
            return new List<AvailabilitySlotDto>();
        }
    }

    /// <inheritdoc />
    public async Task<ServicePricingDto> CalculateServicePricingAsync(
        string providerId,
        string subServiceId,
        DateTimeOffset startDate,
        int durationMinutes,
        int petCount = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calculating pricing for provider {ProviderId}, service {ServiceId}", 
                providerId, subServiceId);

            // Validate inputs
            if (!Guid.TryParse(subServiceId, out var subServiceGuid))
            {
                throw new InvalidOperationException("Invalid sub-service ID format");
            }

            // Get the specific pricing for this provider and sub-service
            // Parse provider ID outside the query
            var providerGuid = Guid.Parse(providerId);
            
            var providerPricing = await _context.ProviderServicePricing
                .AsNoTracking() // Read-only query optimization
                .Include(psp => psp.SubService)
                .Include(psp => psp.ProviderService)
                .FirstOrDefaultAsync(psp => 
                    psp.ProviderService.ProviderId == providerGuid &&
                    psp.SubServiceId == subServiceGuid &&
                    psp.IsAvailable, cancellationToken);

            if (providerPricing == null)
            {
                throw new InvalidOperationException("Pricing not found for the specified provider and service");
            }

            var basePrice = providerPricing.Price;
            var pricing = new ServicePricingDto
            {
                BasePrice = basePrice,
                Currency = "GBP",
                Breakdown = new List<PricingBreakdownItem>()
            };

            // Add base price to breakdown
            pricing.Breakdown.Add(new PricingBreakdownItem
            {
                Description = $"Base Price - {providerPricing.SubService.Name}",
                Amount = basePrice,
                Type = "base"
            });

            // Calculate duration-based pricing if service is charged hourly
            if (providerPricing.PricingType == PricingType.PerHour && durationMinutes != 60)
            {
                var hourlyRate = basePrice;
                var actualHours = (decimal)durationMinutes / 60;
                basePrice = hourlyRate * actualHours;
                
                pricing.Breakdown.Clear();
                pricing.Breakdown.Add(new PricingBreakdownItem
                {
                    Description = $"{providerPricing.SubService.Name} ({actualHours:F1} hours @ £{hourlyRate}/hour)",
                    Amount = basePrice,
                    Type = "base"
                });
            }

            // Apply weekend surcharge
            if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                var weekendSurchargeRate = 0.2m; // 20% default, but could be provider-specific
                pricing.WeekendSurcharge = basePrice * weekendSurchargeRate;
                pricing.Breakdown.Add(new PricingBreakdownItem
                {
                    Description = $"Weekend Surcharge ({weekendSurchargeRate:P0})",
                    Amount = pricing.WeekendSurcharge,
                    Type = "surcharge"
                });
            }

            // Apply evening/early morning surcharge
            if (startDate.Hour >= 18 || startDate.Hour <= 6)
            {
                var eveningSurchargeRate = 0.15m; // 15% default
                pricing.EveningSurcharge = basePrice * eveningSurchargeRate;
                pricing.Breakdown.Add(new PricingBreakdownItem
                {
                    Description = $"Evening/Early Morning Surcharge ({eveningSurchargeRate:P0})",
                    Amount = pricing.EveningSurcharge,
                    Type = "surcharge"
                });
            }

            // Apply multiple pet surcharge
            if (petCount > 1)
            {
                var additionalPets = petCount - 1;
                var petSurchargeRate = 0.5m; // 50% per additional pet
                pricing.MultiplePetSurcharge = basePrice * petSurchargeRate * additionalPets;
                pricing.Breakdown.Add(new PricingBreakdownItem
                {
                    Description = $"Additional Pet Surcharge ({additionalPets} pet{(additionalPets > 1 ? "s" : "")} @ {petSurchargeRate:P0} each)",
                    Amount = pricing.MultiplePetSurcharge,
                    Type = "surcharge"
                });
            }

            // Check if this is an emergency booking (short notice)
            var hoursUntilService = (startDate - DateTimeOffset.Now).TotalHours;
            if (hoursUntilService < 24) // Less than 24 hours notice
            {
                var emergencyRate = 0.25m; // 25% emergency surcharge
                pricing.EmergencySurcharge = basePrice * emergencyRate;
                pricing.Breakdown.Add(new PricingBreakdownItem
                {
                    Description = $"Short Notice Surcharge ({emergencyRate:P0})",
                    Amount = pricing.EmergencySurcharge,
                    Type = "surcharge"
                });
            }

            // Calculate total price
            pricing.TotalPrice = basePrice + pricing.WeekendSurcharge + 
                               pricing.EveningSurcharge + pricing.EmergencySurcharge + 
                               pricing.MultiplePetSurcharge;

            // Update base price in the DTO to reflect any duration adjustments
            pricing.BasePrice = basePrice;

            return pricing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating service pricing");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<ProviderSearchResultDto>> GetTrendingProvidersAsync(
        string postcode,
        int radiusMiles = 10,
        string? serviceCategory = null,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting trending providers near {Postcode}", postcode);

            var coordinates = await _locationService.GetCoordinatesFromPostCodeAsync(postcode);
            if (coordinates == null) return new List<ProviderSearchResultDto>();

            // Use regular search but sort by popularity/rating
            var filters = new ProviderSearchFilterDto
            {
                Latitude = (double)coordinates.Latitude,
                Longitude = (double)coordinates.Longitude,
                RadiusMiles = radiusMiles,
                ServiceCategoryIds = serviceCategory != null ? new List<string> { serviceCategory } : null,
                PageSize = limit,
                SortBy = "popularity" // This would be implemented in the actual search
            };

            var searchResult = await SearchProvidersAsync(filters, cancellationToken);
            return searchResult.Results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending providers");
            return new List<ProviderSearchResultDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<ProviderSearchResultDto>> SearchEmergencyProvidersAsync(
        double latitude,
        double longitude,
        List<string> serviceCategories,
        int radiusMiles = 25,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching emergency providers at {Lat},{Lng}", latitude, longitude);

            var filters = new ProviderSearchFilterDto
            {
                Location = $"{latitude},{longitude}",
                RadiusMiles = radiusMiles,
                ServiceCategoryIds = serviceCategories,
                EmergencyServiceOnly = true,
                PageSize = 20
            };

            var searchResult = await SearchProvidersAsync(filters, cancellationToken);
            
            // Filter and prioritize emergency providers
            return searchResult.Results
                .Where(p => p.OffersEmergencyService)
                .OrderBy(p => p.ResponseTimeHours)
                .ThenBy(p => p.DistanceMiles)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching emergency providers");
            return new List<ProviderSearchResultDto>();
        }
    }

    /// <inheritdoc />
    public async Task InvalidateSearchCacheAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogInformation("Cache invalidated for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error invalidating cache for key: {Key}", key);
        }
        
        await Task.CompletedTask;
    }

    #region Private Methods

    private async Task<(List<ProviderSearchResultDto> providers, int totalCount)> ExecuteProviderSearchAsync(
        double latitude,
        double longitude,
        int radiusMiles,
        ProviderSearchFilterDto filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Executing real provider search at {Lat},{Lng} within {Radius} miles", 
                latitude, longitude, radiusMiles);

            // Query service providers from database using geographic calculation
            var query = _context.ServiceProviders
                .AsNoTracking() // Read-only query optimization
                .Include(sp => sp.User)
                .Where(sp => sp.IsActive && sp.User.IsActive)
                .AsQueryable();

            // Apply service category filters if specified
            if (filters.ServiceCategoryIds?.Any() == true)
            {
                var serviceGuids = filters.ServiceCategoryIds
                    .Where(sc => Guid.TryParse(sc, out _))
                    .Select(sc => Guid.Parse(sc))
                    .ToList();

                if (serviceGuids.Any())
                {
                    // Get provider IDs that offer the requested services
                    var providerIdsWithServices = await _context.ProviderService
                        .Where(ps => serviceGuids.Contains(ps.ServiceCategoryId) && ps.IsOffered)
                        .Select(ps => ps.ProviderId.ToString())
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    if (providerIdsWithServices.Any())
                    {
                        query = query.Where(sp => providerIdsWithServices.Contains(sp.Id));
                    }
                    else
                    {
                        // No providers offer the requested services, return empty result
                        query = query.Where(sp => false);
                    }
                }
            }

            // Calculate distance and filter by radius
            // Using Haversine formula approximation for SQL Server
            var providers = await query
                .Where(sp => sp.User.Latitude.HasValue && sp.User.Longitude.HasValue)
                .Select(sp => new
                {
                    Provider = sp,
                    User = sp.User,
                    Distance = (3959 * Math.Acos(
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos((double)(sp.User.Latitude ?? 0) * Math.PI / 180) *
                        Math.Cos(((double)(sp.User.Longitude ?? 0) - longitude) * Math.PI / 180) +
                        Math.Sin(latitude * Math.PI / 180) *
                        Math.Sin((double)(sp.User.Latitude ?? 0) * Math.PI / 180)))
                })
                .Where(p => p.Distance <= radiusMiles)
                .OrderBy(p => p.Distance)
                .ToListAsync(cancellationToken);

            var totalCount = providers.Count;

            // Apply pagination
            var skip = (pageNumber - 1) * pageSize;
            var paginatedProviders = providers.Skip(skip).Take(pageSize);

            var results = new List<ProviderSearchResultDto>();

            foreach (var item in paginatedProviders)
            {
                var provider = item.Provider;
                var user = item.User;

                // Get provider's services from ProviderService junction table
                // Parse provider ID outside the query
                var providerGuid = Guid.Parse(provider.Id);
                
                var providerServices = await _context.ProviderService
                    .AsNoTracking() // Read-only query optimization
                    .Include(ps => ps.ServiceCategory)
                    .Where(ps => ps.ProviderId == providerGuid && ps.IsOffered)
                    .ToListAsync(cancellationToken);

                var services = providerServices.Select(ps => new ProviderServiceDto
                {
                    ServiceId = ps.ServiceCategoryId.ToString(),
                    ServiceName = ps.ServiceCategory.Name,
                    SubServiceName = "", // Default empty
                    BasePrice = 0, // Will be set from pricing data
                    Currency = "GBP",
                    DurationMinutes = 60,
                    MaxPets = 1,
                    IsEmergencyAvailable = ps.OffersEmergencyService,
                    HasWeekendSurcharge = false,
                    HasEveningSurcharge = false,
                    Description = ps.SpecialNotes
                }).ToList();

                // Get price range from provider service pricing
                var pricingData = await _context.ProviderServicePricing
                    .AsNoTracking() // Read-only query optimization
                    .Where(psp => providerServices.Select(ps => ps.ProviderServiceId).Contains(psp.ProviderServiceId))
                    .ToListAsync(cancellationToken);

                var priceRange = new PriceRangeDto();
                if (pricingData.Any())
                {
                    priceRange.MinPrice = pricingData.Min(p => p.Price);
                    priceRange.MaxPrice = pricingData.Max(p => p.Price);
                    priceRange.CommonPricingType = "per service";
                }

                // Get the most recent completed job date for this provider
                var lastCompletedJob = await _context.Bookings
                    .AsNoTracking() // Read-only query optimization
                    .Where(b => b.ServiceProviderId == provider.Id && 
                               b.Status == "Completed" && 
                               b.CompletedAt.HasValue)
                    .OrderByDescending(b => b.CompletedAt)
                    .Select(b => b.CompletedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                results.Add(new ProviderSearchResultDto
                {
                    Id = provider.Id,
                    BusinessName = provider.BusinessName,
                    ProviderName = $"{user.FirstName} {user.LastName}",
                    Description = provider.BusinessDescription,
                    Location = new ProviderLocationDto
                    {
                        Postcode = user.PostCode ?? "",
                        City = user.City,
                        County = user.County,
                        ServiceRadiusMiles = 10, // Default service radius
                        Latitude = (double?)user.Latitude,
                        Longitude = (double?)user.Longitude
                    },
                    DistanceMiles = item.Distance,
                    Rating = provider.Rating,
                    ReviewCount = 0,
                    IsVerified = provider.IsVerified,
                    Services = services,
                    PriceRange = priceRange,
                    ResponseTimeHours = 2.0m, // Default response time
                    ReliabilityScore = 0.95m, // Default reliability score
                    YearsOfExperience = provider.YearsOfExperience,
                    LastJobCompletedDate = lastCompletedJob,
                    Specializations = provider.Specializations?.Split(',').ToList() ?? new List<string>(),
                    ProfileImageUrl = user.ProfileImageUrl,
                    Website = provider.BusinessWebsite,
                    OffersEmergencyService = providerServices.Any(ps => ps.OffersEmergencyService),
                    OffersWeekendService = providerServices.Any(ps => ps.OffersWeekendService),
                    OffersEveningService = providerServices.Any(ps => ps.OffersEveningService)
                });
            }

            _logger.LogInformation("Found {Count} real providers within {Radius} miles", totalCount, radiusMiles);
            return (results, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing real provider search");
            // Return empty results instead of throwing
            return (new List<ProviderSearchResultDto>(), 0);
        }
    }


    private string BuildSearchCacheKey(ProviderSearchFilterDto filters, double latitude, double longitude)
    {
        var key = $"search_{latitude:F4}_{longitude:F4}_{filters.RadiusMiles}";
        
        if (filters.ServiceCategoryIds?.Any() == true)
        {
            key += $"_services_{string.Join(",", filters.ServiceCategoryIds.OrderBy(s => s))}";
        }
        
        if (filters.StartDate.HasValue)
        {
            key += $"_date_{filters.StartDate.Value:yyyyMMdd}";
        }
        
        if (filters.PetCount > 0)
        {
            key += $"_pets_{filters.PetCount}";
        }

        return key;
    }

    #endregion
}