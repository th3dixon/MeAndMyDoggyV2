using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using System.Security.Claims;
using System.Text.Json;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Models.DTOs.Dashboard;

namespace MeAndMyDog.API.Controllers
{
    /// <summary>
    /// Dashboard API controller for pet owner dashboard functionality
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IDashboardCacheService _cacheService;

        public DashboardController(
            ApplicationDbContext context, 
            ILogger<DashboardController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IDashboardCacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get dashboard configuration and widget preferences for the authenticated user (with caching)
        /// </summary>
        [HttpGet("configuration")]
        public async Task<IActionResult> GetConfiguration()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Try cache first
                var cachedConfig = await _cacheService.GetDashboardConfigAsync(userId);
                if (cachedConfig != null)
                {
                    _logger.LogDebug("Dashboard config served from cache for user {UserId}", userId);
                    return Ok(cachedConfig);
                }

                // Get from database if not cached
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var configuration = new DashboardConfigDto
                {
                    WidgetLayout = new[] { "quickStats", "pets", "upcomingServices", "recentActivity", "weather", "healthReminders" },
                    Preferences = new Dictionary<string, string>
                    { 
                        ["theme"] = "light",
                        ["notifications"] = "true",
                        ["timeZone"] = user.TimeZone ?? "Europe/London"
                    },
                    User = new UserInfoDto
                    {
                        FirstName = user.FirstName,
                        DisplayName = user.DisplayName,
                        ProfileImage = user.ProfilePhotoUrl
                    }
                };

                // Cache the configuration
                await _cacheService.SetDashboardConfigAsync(userId, configuration);

                return Ok(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard configuration for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get quick statistics for dashboard overview
        /// </summary>
        [HttpGet("quick-stats")]
        public async Task<IActionResult> GetQuickStats()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Get pet count
                var petCount = await _context.DogProfiles
                    .Where(d => d.OwnerId == userId && d.IsActive)
                    .CountAsync();

                // Get upcoming services count
                var upcomingServicesCount = await _context.Bookings
                    .Where(b => b.CustomerId == userId && 
                              b.StartDateTime > DateTime.UtcNow &&
                              b.Status != "Cancelled")
                    .CountAsync();

                // Get average rating (from reviews the user has given)
                var averageRating = await _context.ServiceProviderReviews
                    .Where(r => r.ReviewerId == userId && r.IsActive)
                    .AverageAsync(r => (double?)r.Rating) ?? 0.0;

                // Get monthly bookings count
                var currentMonth = DateTime.UtcNow.AddMonths(-1);
                var monthlyBookings = await _context.Bookings
                    .Where(b => b.CustomerId == userId && 
                              b.CreatedAt >= currentMonth &&
                              b.Status != "Cancelled")
                    .CountAsync();

                var quickStats = new
                {
                    petCount = petCount,
                    upcomingServices = upcomingServicesCount,
                    averageRating = Math.Round(averageRating, 1),
                    monthlyBookings = monthlyBookings
                };

                return Ok(quickStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching quick stats for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user's pets with health status and upcoming appointments
        /// </summary>
        [HttpGet("pets")]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var petsData = await _context.DogProfiles
                    .Include(d => d.MedicalRecords)
                    .Include(d => d.Appointments)
                    .Include(d => d.AIHealthRecommendations)
                    .Where(d => d.OwnerId == userId && d.IsActive)
                    .ToListAsync();

                var pets = petsData.Select(d => new
                {
                    id = d.Id,
                    name = d.Name,
                    breed = d.Breed + (!string.IsNullOrEmpty(d.SecondaryBreed) ? $" / {d.SecondaryBreed}" : ""),
                    age = d.DateOfBirth.HasValue 
                        ? $"{DateTime.Now.Year - d.DateOfBirth.Value.Year} years"
                        : "Unknown age",
                    image = !string.IsNullOrEmpty(d.ProfileImageUrl) ? d.ProfileImageUrl : "/images/placeholder-dog.jpg",
                    healthStatus = CalculateHealthStatus(d.MedicalRecords, d.AIHealthRecommendations, d.DateOfBirth?.DateTime),
                    lastCheckup = d.MedicalRecords
                        .Where(mr => mr.RecordType == "Checkup")
                        .OrderByDescending(mr => mr.EventDate)
                        .Select(mr => mr.EventDate)
                        .FirstOrDefault(),
                    lastCheckupFormatted = d.MedicalRecords
                        .Where(mr => mr.RecordType == "Checkup")
                        .OrderByDescending(mr => mr.EventDate)
                        .FirstOrDefault()?.EventDate is DateTimeOffset checkupDate
                            ? GetRelativeTime(checkupDate)
                            : "No checkup recorded",
                    nextAppointment = d.Appointments
                        .Where(a => a.StartTime > DateTime.UtcNow && a.Status == "Confirmed")
                        .OrderBy(a => a.StartTime)
                        .Select(a => a.StartTime)
                        .FirstOrDefault(),
                    pendingHealthActions = d.AIHealthRecommendations
                        .Count(ahr => ahr.ImplementationStatus == "Pending"),
                    weight = d.Weight,
                    vaccinationStatus = CalculateVaccinationStatus(d.MedicalRecords),
                    healthScore = CalculateHealthScore(d.MedicalRecords, d.AIHealthRecommendations, d.DateOfBirth?.DateTime)
                }).ToList();

                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pets for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get upcoming services and bookings with full database relationships
        /// </summary>
        [HttpGet("upcoming-services")]
        public async Task<IActionResult> GetUpcomingServices()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var upcomingServices = await _context.Bookings
                    .Include(b => b.Service)
                    .Include(b => b.ServiceProvider)
                        .ThenInclude(sp => sp.User)
                    .Include(b => b.Dog)
                    .Where(b => b.CustomerId == userId && 
                              b.StartDateTime > DateTime.UtcNow &&
                              b.Status != "Cancelled")
                    .OrderBy(b => b.StartDateTime)
                    .Take(5)
                    .Select(b => new
                    {
                        id = b.Id,
                        serviceName = b.Service != null ? b.Service.Name : "General Service",
                        serviceCategory = b.Service != null 
                            ? b.Service.Category : "General",
                        serviceCategoryIcon = "fas fa-paw",
                        providerName = b.ServiceProvider != null && b.ServiceProvider.User != null 
                            ? b.ServiceProvider.User.DisplayName ?? b.ServiceProvider.User.FirstName + " " + b.ServiceProvider.User.LastName
                            : "Service Provider",
                        providerImage = b.ServiceProvider != null && b.ServiceProvider.User != null && !string.IsNullOrEmpty(b.ServiceProvider.User.ProfilePhotoUrl)
                            ? b.ServiceProvider.User.ProfilePhotoUrl 
                            : "/images/placeholder-provider.jpg",
                        providerRating = b.ServiceProvider != null ? Math.Round((double)b.ServiceProvider.Rating, 1) : 0.0,
                        date = b.StartDateTime.Date == DateTime.UtcNow.Date ? "Today" :
                               b.StartDateTime.Date == DateTime.UtcNow.Date.AddDays(1) ? "Tomorrow" :
                               b.StartDateTime.ToString("MMM dd"),
                        time = b.StartDateTime.ToString("h:mm tt"),
                        duration = b.Service != null ? b.Service.DurationMinutes ?? 60 : 60,
                        pet = b.Dog != null ? b.Dog.Name : "General",
                        petBreed = b.Dog != null ? b.Dog.Breed : null,
                        status = b.Status,
                        price = b.TotalPrice,
                        location = b.ServiceLocation,
                        notes = !string.IsNullOrEmpty(b.SpecialInstructions) ? b.SpecialInstructions : null
                    })
                    .ToListAsync();

                return Ok(upcomingServices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching upcoming services for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get comprehensive recent activity timeline from multiple sources
        /// </summary>
        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var activities = new List<object>();

                // Recent completed bookings with service details
                var recentBookings = await _context.Bookings
                    .Include(b => b.Service)
                    .Include(b => b.ServiceProvider)
                        .ThenInclude(sp => sp.User)
                    .Include(b => b.Dog)
                    .Where(b => b.CustomerId == userId && 
                              b.Status == "Completed" &&
                              b.UpdatedAt >= DateTime.UtcNow.AddDays(-14))
                    .OrderByDescending(b => b.UpdatedAt)
                    .Take(3)
                    .Select(b => new
                    {
                        id = $"booking_{b.Id}",
                        message = $"{b.Service.Name} completed for {b.Dog.Name}",
                        detail = $"with {b.ServiceProvider.User.DisplayName ?? b.ServiceProvider.User.FirstName}",
                        timestamp = GetRelativeTime(b.UpdatedAt),
                        icon = "fas fa-check-circle",
                        iconClass = "bg-green-100 text-green-600",
                        type = "service_completed",
                        priority = 1
                    })
                    .ToListAsync();

                activities.AddRange(recentBookings);

                // Recent medical records
                var recentMedicalRecords = await _context.MedicalRecords
                    .Include(mr => mr.Dog)
                    .Where(mr => mr.Dog.OwnerId == userId &&
                               mr.CreatedAt >= DateTime.UtcNow.AddDays(-14))
                    .OrderByDescending(mr => mr.CreatedAt)
                    .Take(3)
                    .Select(mr => new
                    {
                        id = $"medical_{mr.Id}",
                        message = $"Medical record added for {mr.Dog.Name}",
                        detail = $"{mr.RecordType}: {mr.Description}",
                        timestamp = GetRelativeTime(mr.CreatedAt),
                        icon = GetHealthIcon(mr.RecordType),
                        iconClass = "bg-blue-100 text-blue-600",
                        type = "medical_record",
                        priority = 2
                    })
                    .ToListAsync();

                activities.AddRange(recentMedicalRecords);

                // Recent AI health recommendations that were implemented
                var recentHealthActions = await _context.AIHealthRecommendations
                    .Include(ahr => ahr.Dog)
                    .Where(ahr => ahr.UserId == userId &&
                                ahr.ImplementationStatus == "Implemented" &&
                                ahr.ImplementedAt >= DateTime.UtcNow.AddDays(-14))
                    .OrderByDescending(ahr => ahr.ImplementedAt ?? ahr.CreatedAt)
                    .Take(2)
                    .Select(ahr => new
                    {
                        id = $"health_{ahr.Id}",
                        message = $"Health recommendation completed",
                        detail = $"{ahr.RecommendationType} for {ahr.Dog.Name}",
                        timestamp = GetRelativeTime(ahr.ImplementedAt ?? ahr.CreatedAt),
                        icon = "fas fa-heartbeat",
                        iconClass = "bg-red-100 text-red-600",
                        type = "health_action",
                        priority = 2
                    })
                    .ToListAsync();

                activities.AddRange(recentHealthActions);

                // Recent reviews given
                var recentReviews = await _context.ServiceProviderReviews
                    .Include(r => r.ServiceProvider)
                        .ThenInclude(sp => sp.User)
                    .Where(r => r.ReviewerId == userId &&
                              r.CreatedAt >= DateTime.UtcNow.AddDays(-14) &&
                              r.IsActive)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(2)
                    .Select(r => new
                    {
                        id = $"review_{r.Id}",
                        message = $"Review posted for {r.ServiceProvider.User.DisplayName ?? r.ServiceProvider.User.FirstName}",
                        detail = $"Rated {r.Rating} stars",
                        timestamp = GetRelativeTime(r.CreatedAt),
                        icon = "fas fa-star",
                        iconClass = "bg-yellow-100 text-yellow-600",
                        type = "review_posted",
                        priority = 3
                    })
                    .ToListAsync();

                activities.AddRange(recentReviews);

                // Sort all activities by priority and timestamp, then take top 5
                var sortedActivities = activities
                    .OrderBy(a => ((dynamic)a).priority)
                    .ThenByDescending(a => GetDateTimeFromRelativeTime(((dynamic)a).timestamp))
                    .Take(5)
                    .ToList();

                return Ok(sortedActivities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent activity for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get health reminders for user's pets
        /// </summary>
        [HttpGet("health-reminders")]
        public async Task<IActionResult> GetHealthReminders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var healthReminders = await _context.AIHealthRecommendations
                    .Where(ahr => ahr.UserId == userId && 
                                ahr.ImplementationStatus == "Pending")
                    .OrderBy(ahr => ahr.CreatedAt)
                    .Take(5)
                    .Select(ahr => new
                    {
                        id = ahr.Id,
                        pet = ahr.Dog != null ? ahr.Dog.Name : "General",
                        task = ahr.RecommendationType,
                        due = "Soon",
                        icon = GetHealthIcon(ahr.RecommendationType),
                        priority = "medium"
                    })
                    .ToListAsync();

                return Ok(healthReminders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching health reminders for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get weather data and pet care tips
        /// </summary>
        [HttpGet("weather")]
        public async Task<IActionResult> GetWeatherData()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Get user location
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Try to get real weather data
                var weather = await GetWeatherFromAPI(user);
                if (weather == null)
                {
                    // Fallback to default weather data
                    weather = new
                    {
                        temperature = 18,
                        condition = "Partly Cloudy",
                        feelsLike = 16,
                        location = !string.IsNullOrEmpty(user.City) ? $"{user.City}, {user.County}" : "London, UK",
                        icon = "fas fa-cloud-sun",
                        petTip = GetWeatherBasedTip(18, "Partly Cloudy")
                    };
                }

                return Ok(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        #region Helper Methods

        /// <summary>
        /// Get weather data from Open-Meteo API using user's real location data
        /// </summary>
        /// <param name="user">User with location data</param>
        /// <returns>Weather data object or null if unavailable</returns>
        private async Task<object?> GetWeatherFromAPI(ApplicationUser user)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                
                double latitude;
                double longitude;
                string locationName;

                // Try to use user's stored coordinates first
                if (user.Latitude.HasValue && user.Longitude.HasValue)
                {
                    latitude = (double)user.Latitude.Value;
                    longitude = (double)user.Longitude.Value;
                    locationName = !string.IsNullOrEmpty(user.City) 
                        ? $"{user.City}, {user.County ?? "UK"}" 
                        : "Your Location";
                    
                    _logger.LogInformation("Using stored coordinates for user {UserId}: {Lat}, {Lon}", 
                        user.Id, latitude, longitude);
                }
                // Try to get coordinates from PostCode
                else if (!string.IsNullOrEmpty(user.PostCode))
                {
                    var postcode = await _context.Postcodes
                        .FirstOrDefaultAsync(p => p.PostcodeCode == user.PostCode.Replace(" ", "").ToUpper());
                    
                    if (postcode != null)
                    {
                        latitude = (double)postcode.Latitude;
                        longitude = (double)postcode.Longitude;
                        locationName = $"{postcode.PostcodeFormatted}, UK";
                        
                        // Update user's coordinates for faster future lookups
                        user.Latitude = postcode.Latitude;
                        user.Longitude = postcode.Longitude;
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Resolved postcode {PostCode} to coordinates: {Lat}, {Lon}", 
                            user.PostCode, latitude, longitude);
                    }
                    else
                    {
                        // Postcode not found, use London as fallback
                        latitude = 51.5074;
                        longitude = -0.1278;
                        locationName = "London, UK (Default)";
                        
                        _logger.LogWarning("Postcode {PostCode} not found in database, using London fallback", 
                            user.PostCode);
                    }
                }
                // Use London as final fallback
                else
                {
                    latitude = 51.5074;
                    longitude = -0.1278;
                    locationName = "London, UK (Default)";
                    
                    _logger.LogInformation("No location data available for user {UserId}, using London fallback", 
                        user.Id);
                }

                // Open-Meteo API URL with dynamic timezone based on location
                var timezone = latitude > 50 && longitude > -10 && longitude < 5 ? "Europe/London" : "auto";
                var apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude:F4}&longitude={longitude:F4}&current=temperature_2m,apparent_temperature,weather_code,wind_speed_10m&timezone={timezone}";

                var response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch weather data from Open-Meteo: {StatusCode} for location {Location}", 
                        response.StatusCode, locationName);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

                var current = weatherData.GetProperty("current");
                var temperature = (int)Math.Round(current.GetProperty("temperature_2m").GetDouble());
                var feelsLike = (int)Math.Round(current.GetProperty("apparent_temperature").GetDouble());
                var weatherCode = current.GetProperty("weather_code").GetInt32();
                
                var (condition, description) = GetWeatherFromCode(weatherCode);

                _logger.LogInformation("Successfully fetched weather for {Location}: {Temp}°C, {Condition}", 
                    locationName, temperature, description);

                return new
                {
                    temperature = temperature,
                    condition = description,
                    feelsLike = feelsLike,
                    location = locationName,
                    icon = GetWeatherIconFromCode(weatherCode),
                    petTip = GetWeatherBasedTip(temperature, condition),
                    coordinates = new { latitude, longitude },
                    source = user.Latitude.HasValue ? "stored_coords" : !string.IsNullOrEmpty(user.PostCode) ? "postcode_lookup" : "default_location"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Open-Meteo API for user {UserId}", user.Id);
                return null;
            }
        }

        /// <summary>
        /// Convert Open-Meteo weather code to condition and description
        /// </summary>
        /// <param name="weatherCode">Open-Meteo weather code</param>
        /// <returns>Tuple of (condition, description)</returns>
        private static (string condition, string description) GetWeatherFromCode(int weatherCode)
        {
            return weatherCode switch
            {
                0 => ("Clear", "Clear Sky"),
                1 => ("Clear", "Mainly Clear"),
                2 => ("Clouds", "Partly Cloudy"),
                3 => ("Clouds", "Overcast"),
                45 or 48 => ("Mist", "Fog"),
                51 or 53 or 55 => ("Drizzle", "Light Drizzle"),
                56 or 57 => ("Drizzle", "Freezing Drizzle"),
                61 or 63 or 65 => ("Rain", "Rain"),
                66 or 67 => ("Rain", "Freezing Rain"),
                71 or 73 or 75 => ("Snow", "Snow Fall"),
                77 => ("Snow", "Snow Grains"),
                80 or 81 or 82 => ("Rain", "Rain Showers"),
                85 or 86 => ("Snow", "Snow Showers"),
                95 => ("Thunderstorm", "Thunderstorm"),
                96 or 99 => ("Thunderstorm", "Thunderstorm with Hail"),
                _ => ("Clear", "Unknown")
            };
        }

        /// <summary>
        /// Get appropriate weather icon class from Open-Meteo weather code
        /// </summary>
        /// <param name="weatherCode">Open-Meteo weather code</param>
        /// <returns>FontAwesome icon class</returns>
        private static string GetWeatherIconFromCode(int weatherCode)
        {
            return weatherCode switch
            {
                0 or 1 => "fas fa-sun",
                2 => "fas fa-cloud-sun",
                3 => "fas fa-cloud",
                45 or 48 => "fas fa-smog",
                51 or 53 or 55 or 56 or 57 => "fas fa-cloud-drizzle",
                61 or 63 or 65 or 66 or 67 or 80 or 81 or 82 => "fas fa-cloud-rain",
                71 or 73 or 75 or 77 or 85 or 86 => "fas fa-snowflake",
                95 or 96 or 99 => "fas fa-bolt",
                _ => "fas fa-cloud-sun"
            };
        }


        private static string GetRelativeTime(DateTimeOffset dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.DateTime;
            
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            
            return dateTime.ToString("MMM dd");
        }

        private static DateTime GetDateTimeFromRelativeTime(string relativeTime)
        {
            // Simple fallback for sorting - in real implementation this would be more sophisticated
            if (relativeTime.Contains("Just now"))
                return DateTime.UtcNow;
            if (relativeTime.Contains("minutes ago"))
            {
                var minutes = int.Parse(relativeTime.Split(' ')[0]);
                return DateTime.UtcNow.AddMinutes(-minutes);
            }
            if (relativeTime.Contains("hours ago"))
            {
                var hours = int.Parse(relativeTime.Split(' ')[0]);
                return DateTime.UtcNow.AddHours(-hours);
            }
            if (relativeTime.Contains("days ago"))
            {
                var days = int.Parse(relativeTime.Split(' ')[0]);
                return DateTime.UtcNow.AddDays(-days);
            }
            
            // For formatted dates, assume it's older
            return DateTime.UtcNow.AddDays(-30);
        }

        private static string GetHealthIcon(string recommendationType)
        {
            return recommendationType.ToLower() switch
            {
                "vaccination" => "fas fa-syringe",
                "dental" => "fas fa-teeth",
                "checkup" => "fas fa-stethoscope",
                "exercise" => "fas fa-running",
                "diet" => "fas fa-utensils",
                _ => "fas fa-heartbeat"
            };
        }

        private static string GetWeatherBasedTip(int temperature, string condition)
        {
            return temperature switch
            {
                < 5 => "Very cold weather! Keep walks short and consider a coat for your dog.",
                < 15 => "Cool weather perfect for active dogs. Great time for longer walks!",
                < 25 => "Perfect weather for a long walk! Remember to bring water for your dog.",
                _ => "Hot weather warning! Walk during cooler hours and always bring water."
            };
        }

        private static string CalculateHealthStatus(ICollection<MedicalRecord> medicalRecords, ICollection<AIHealthRecommendation> healthRecommendations, DateTime? dateOfBirth)
        {
            if (medicalRecords == null || !medicalRecords.Any())
                return "Unknown";

            var now = DateTime.UtcNow;
            var recentRecords = medicalRecords.Where(mr => mr.EventDate >= now.AddMonths(-6)).ToList();
            
            // Check for recent serious conditions
            var seriousConditions = recentRecords.Where(mr => 
                mr.Title.ToLower().Contains("high") || 
                mr.RecordType.ToLower().Contains("emergency") ||
                mr.RecordType.ToLower().Contains("surgery")).Any();
            
            if (seriousConditions)
                return "Needs Attention";

            // Check vaccination status
            var lastVaccination = medicalRecords
                .Where(mr => mr.RecordType.ToLower().Contains("vaccination"))
                .OrderByDescending(mr => mr.EventDate)
                .FirstOrDefault();

            var vaccinationOverdue = lastVaccination == null || 
                lastVaccination.EventDate < now.AddYears(-1);

            // Check for pending high-priority health recommendations
            var urgentRecommendations = healthRecommendations
                .Where(hr => hr.ImplementationStatus == "Pending" && 
                           hr.RecommendationType.ToLower().Contains("urgent")).Any();

            if (vaccinationOverdue || urgentRecommendations)
                return "Needs Attention";

            // Check last checkup
            var lastCheckup = medicalRecords
                .Where(mr => mr.RecordType == "Checkup")
                .OrderByDescending(mr => mr.EventDate)
                .FirstOrDefault();

            if (lastCheckup == null || lastCheckup.EventDate < now.AddMonths(-12))
                return "Checkup Due";

            // If recent checkup and no issues
            if (lastCheckup != null && lastCheckup.EventDate >= now.AddMonths(-3))
                return "Excellent";

            return "Good";
        }

        private static string CalculateVaccinationStatus(ICollection<MedicalRecord> medicalRecords)
        {
            if (medicalRecords == null || !medicalRecords.Any())
                return "Unknown";

            var lastVaccination = medicalRecords
                .Where(mr => mr.RecordType.ToLower().Contains("vaccination"))
                .OrderByDescending(mr => mr.EventDate)
                .FirstOrDefault();

            if (lastVaccination == null)
                return "No records";

            var monthsSinceVaccination = (DateTime.UtcNow - lastVaccination.EventDate).Days / 30;

            return monthsSinceVaccination switch
            {
                < 12 => "Up to date",
                < 18 => "Due soon",
                _ => "Overdue"
            };
        }

        private static int CalculateHealthScore(ICollection<MedicalRecord> medicalRecords, ICollection<AIHealthRecommendation> healthRecommendations, DateTime? dateOfBirth)
        {
            var score = 100;
            var now = DateTime.UtcNow;

            // Deduct for missing checkups
            var lastCheckup = medicalRecords?.Where(mr => mr.RecordType == "Checkup")
                .OrderByDescending(mr => mr.EventDate)
                .FirstOrDefault();

            if (lastCheckup == null)
                score -= 20;
            else if (lastCheckup.EventDate < now.AddMonths(-12))
                score -= 15;
            else if (lastCheckup.EventDate < now.AddMonths(-6))
                score -= 10;

            // Deduct for vaccination status
            var lastVaccination = medicalRecords?.Where(mr => mr.RecordType.ToLower().Contains("vaccination"))
                .OrderByDescending(mr => mr.EventDate)
                .FirstOrDefault();

            if (lastVaccination == null || lastVaccination.EventDate < now.AddYears(-1))
                score -= 15;

            // Deduct for pending high-priority health recommendations
            var urgentRecommendations = healthRecommendations?.Count(hr => 
                hr.ImplementationStatus == "Pending" && hr.RecommendationType.ToLower().Contains("urgent")) ?? 0;
            score -= urgentRecommendations * 10;

            // Deduct for recent serious medical issues
            var recentSeriousIssues = medicalRecords?.Count(mr => 
                mr.EventDate >= now.AddMonths(-6) && mr.Title.ToLower().Contains("high")) ?? 0;
            score -= recentSeriousIssues * 15;

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Save user dashboard preferences using UserSetting entity
        /// </summary>
        [HttpPost("preferences")]
        public async Task<IActionResult> SavePreferences([FromBody] DashboardPreferences preferences)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var now = DateTimeOffset.UtcNow;
                const string category = "Dashboard";

                // Prepare settings to save
                var settingsToSave = new List<(string key, string value, string dataType)>();

                if (preferences.Theme != null)
                    settingsToSave.Add(("theme", preferences.Theme, "string"));

                if (preferences.WidgetSizes != null)
                    settingsToSave.Add(("widgetSizes", JsonSerializer.Serialize(preferences.WidgetSizes), "json"));

                if (preferences.HiddenWidgets != null)
                    settingsToSave.Add(("hiddenWidgets", JsonSerializer.Serialize(preferences.HiddenWidgets), "json"));

                if (preferences.WidgetOrder != null)
                    settingsToSave.Add(("widgetOrder", JsonSerializer.Serialize(preferences.WidgetOrder), "json"));

                // Get existing settings for this category
                var existingSettings = await _context.UserSettings
                    .Where(s => s.UserId == userId && s.Category == category)
                    .ToListAsync();

                // Update or create settings
                foreach (var (key, value, dataType) in settingsToSave)
                {
                    var existingSetting = existingSettings.FirstOrDefault(s => s.Key == key);
                    
                    if (existingSetting != null)
                    {
                        // Update existing setting
                        existingSetting.Value = value;
                        existingSetting.DataType = dataType;
                        existingSetting.UpdatedAt = now;
                        _context.UserSettings.Update(existingSetting);
                    }
                    else
                    {
                        // Create new setting
                        var newSetting = new UserSetting
                        {
                            UserId = userId,
                            Key = key,
                            Value = value,
                            Category = category,
                            DataType = dataType,
                            CreatedAt = now,
                            UpdatedAt = now
                        };
                        await _context.UserSettings.AddAsync(newSetting);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Dashboard preferences saved for user {UserId}: {SettingCount} settings updated", 
                    userId, settingsToSave.Count);

                return Ok(new { 
                    message = "Preferences saved successfully",
                    savedSettings = settingsToSave.Count,
                    timestamp = now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving dashboard preferences for user {UserId}", 
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Error saving preferences");
            }
        }

        /// <summary>
        /// Load user dashboard preferences from UserSetting entity
        /// </summary>
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                const string category = "Dashboard";

                var settings = await _context.UserSettings
                    .Where(s => s.UserId == userId && s.Category == category)
                    .ToDictionaryAsync(s => s.Key, s => new { s.Value, s.DataType, s.UpdatedAt });

                var preferences = new DashboardPreferences();

                // Parse settings back into preferences object
                if (settings.TryGetValue("theme", out var themeValue))
                {
                    preferences.Theme = themeValue.Value;
                }

                if (settings.TryGetValue("widgetSizes", out var sizesValue) && sizesValue.DataType == "json")
                {
                    try
                    {
                        preferences.WidgetSizes = JsonSerializer.Deserialize<Dictionary<string, string>>(sizesValue.Value);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize widgetSizes for user {UserId}", userId);
                    }
                }

                if (settings.TryGetValue("hiddenWidgets", out var hiddenValue) && hiddenValue.DataType == "json")
                {
                    try
                    {
                        preferences.HiddenWidgets = JsonSerializer.Deserialize<string[]>(hiddenValue.Value);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize hiddenWidgets for user {UserId}", userId);
                    }
                }

                if (settings.TryGetValue("widgetOrder", out var orderValue) && orderValue.DataType == "json")
                {
                    try
                    {
                        preferences.WidgetOrder = JsonSerializer.Deserialize<string[]>(orderValue.Value);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize widgetOrder for user {UserId}", userId);
                    }
                }

                return Ok(new
                {
                    preferences,
                    settingsCount = settings.Count,
                    lastUpdated = settings.Values.MaxBy(v => v.UpdatedAt)?.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard preferences for user {UserId}", 
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Error loading preferences");
            }
        }

        #endregion
    }

    /// <summary>
    /// Dashboard preferences model for saving user customizations
    /// </summary>
    public class DashboardPreferences
    {
        public string[]? WidgetOrder { get; set; }
        public Dictionary<string, string>? WidgetSizes { get; set; }
        public string[]? HiddenWidgets { get; set; }
        public string? Theme { get; set; }
    }
}
