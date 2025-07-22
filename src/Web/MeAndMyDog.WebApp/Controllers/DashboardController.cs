using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MeAndMyDog.WebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Main dashboard page for pet owners
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["Description"] = "Your personalized pet care dashboard";
            return View();
        }

        /// <summary>
        /// Get dashboard configuration and widget preferences
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetConfiguration()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                // Add user token to request if available
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/configuration");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                // Return default configuration if API fails
                return Json(GetDefaultConfiguration());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard configuration");
                return Json(GetDefaultConfiguration());
            }
        }

        /// <summary>
        /// Get quick stats data for dashboard widgets
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetQuickStats()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/quick-stats");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultQuickStats());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching quick stats");
                return Json(GetDefaultQuickStats());
            }
        }

        /// <summary>
        /// Get pet profiles with health status
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/pets");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultPets());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pet data");
                return Json(GetDefaultPets());
            }
        }

        /// <summary>
        /// Get upcoming services and bookings
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUpcomingServices()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/upcoming-services");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultUpcomingServices());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching upcoming services");
                return Json(GetDefaultUpcomingServices());
            }
        }

        /// <summary>
        /// Get recent activity timeline
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/recent-activity");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultRecentActivity());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent activity");
                return Json(GetDefaultRecentActivity());
            }
        }

        /// <summary>
        /// Get health reminders for pets
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHealthReminders()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/health-reminders");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultHealthReminders());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching health reminders");
                return Json(GetDefaultHealthReminders());
            }
        }

        /// <summary>
        /// Get weather data and pet care tips
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWeatherData()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/dashboard/weather");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(GetDefaultWeatherData());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data");
                return Json(GetDefaultWeatherData());
            }
        }

        /// <summary>
        /// Upload a pet photo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadPetPhoto(IFormFile photo, string petId)
        {
            try
            {
                if (photo == null || photo.Length == 0)
                {
                    return Json(new { success = false, message = "No photo provided" });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                {
                    return Json(new { success = false, message = "Invalid file type. Only JPEG, PNG, and WebP are allowed." });
                }

                // Validate file size (max 5MB)
                if (photo.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size too large. Maximum size is 5MB." });
                }

                // Create uploads directory if it doesn't exist
                var uploadsDir = Path.Combine("wwwroot", "uploads", "pets");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(photo.FileName);
                var fileName = $"{petId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var photoUrl = $"/uploads/pets/{fileName}";

                // TODO: Update pet profile in database via API call
                // For now, just return the URL
                
                return Json(new { 
                    success = true, 
                    photoUrl = photoUrl,
                    message = "Photo uploaded successfully!" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading pet photo");
                return Json(new { success = false, message = "Error uploading photo. Please try again." });
            }
        }

        #region Default Data Methods (Fallback for offline development)

        private object GetDefaultConfiguration()
        {
            return new
            {
                widgetLayout = new[] { "quickStats", "pets", "upcomingServices", "recentActivity", "weather", "healthReminders" },
                preferences = new { theme = "light", notifications = true }
            };
        }

        private object GetDefaultQuickStats()
        {
            return new
            {
                petCount = 2,
                upcomingServices = 3,
                averageRating = 4.9,
                monthlyBookings = 8
            };
        }

        private object GetDefaultPets()
        {
            return new[]
            {
                new {
                    id = 1,
                    name = "Buddy",
                    breed = "Golden Retriever",
                    age = "3 years",
                    image = "/images/placeholder-dog.jpg",
                    healthStatus = "Excellent",
                    lastCheckup = "2024-01-15",
                    nextAppointment = "2024-02-20"
                },
                new {
                    id = 2,
                    name = "Luna",
                    breed = "Border Collie",
                    age = "2 years",
                    image = "/images/placeholder-dog.jpg",
                    healthStatus = "Good",
                    lastCheckup = "2024-01-10",
                    nextAppointment = "2024-02-25"
                }
            };
        }

        private object GetDefaultUpcomingServices()
        {
            return new[]
            {
                new {
                    id = 1,
                    serviceName = "Dog Walking",
                    providerName = "Emma Thompson",
                    providerImage = "/images/placeholder-provider.jpg",
                    date = "Today",
                    time = "2:00 PM",
                    pet = "Buddy",
                    status = "Confirmed"
                },
                new {
                    id = 2,
                    serviceName = "Grooming Session",
                    providerName = "Paws & Claws Spa",
                    providerImage = "/images/placeholder-provider.jpg",
                    date = "Tomorrow",
                    time = "10:00 AM",
                    pet = "Luna",
                    status = "Confirmed"
                }
            };
        }

        private object GetDefaultRecentActivity()
        {
            return new[]
            {
                new {
                    id = 1,
                    message = "Buddy completed his walk with Emma Thompson",
                    timestamp = "2 hours ago",
                    icon = "fas fa-check",
                    iconClass = "bg-green-100 text-green-600",
                    type = "service_completed"
                },
                new {
                    id = 2,
                    message = "New message from your pet sitter",
                    timestamp = "4 hours ago",
                    icon = "fas fa-comment",
                    iconClass = "bg-blue-100 text-blue-600",
                    type = "message"
                },
                new {
                    id = 3,
                    message = "Luna's vaccination reminder: Due in 5 days",
                    timestamp = "1 day ago",
                    icon = "fas fa-syringe",
                    iconClass = "bg-yellow-100 text-yellow-600",
                    type = "health_reminder"
                }
            };
        }

        private object GetDefaultHealthReminders()
        {
            return new[]
            {
                new {
                    id = 1,
                    pet = "Buddy",
                    task = "Vaccination due",
                    due = "In 5 days",
                    icon = "fas fa-syringe",
                    priority = "high"
                },
                new {
                    id = 2,
                    pet = "Luna",
                    task = "Dental checkup",
                    due = "Next week",
                    icon = "fas fa-teeth",
                    priority = "medium"
                }
            };
        }

        private object GetDefaultWeatherData()
        {
            return new
            {
                temperature = 18,
                condition = "Partly Cloudy",
                feelsLike = 16,
                location = "London, UK",
                icon = "fas fa-cloud-sun",
                petTip = "Perfect weather for a long walk! Remember to bring water for your dog."
            };
        }

        #endregion
    }
}