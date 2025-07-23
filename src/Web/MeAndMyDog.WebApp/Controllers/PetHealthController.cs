using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.WebApp.Services;
using System.Text.Json;

namespace MeAndMyDog.WebApp.Controllers
{
    [Authorize]
    // Allow both "PetOwner" and "User" roles to access pet health dashboard
    public class PetHealthController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PetHealthController(
            IHttpClientFactory httpClientFactory, 
            IRoleNavigationService roleNavigationService,
            ILogger<PetHealthController> logger)
            : base(roleNavigationService, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Pet health dashboard main page
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Pet Health Dashboard";
            ViewData["Description"] = "Manage your pets' medical records, vaccinations, and health reminders";
            return View();
        }

        /// <summary>
        /// Get all pets with their health summary
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPetsHealthSummary()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync("api/v1/medicalrecords/health-summary");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(new object[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pets health summary");
                return Json(new object[0]);
            }
        }

        /// <summary>
        /// Get medical records for a specific pet
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMedicalRecords(string petId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var response = await client.GetAsync($"api/v1/medicalrecords/pet/{petId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(content));
                }
                
                return Json(new object[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching medical records for pet {PetId}", petId);
                return Json(new object[0]);
            }
        }

        /// <summary>
        /// Get health reminders for all pets
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
                
                return Json(new object[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching health reminders");
                return Json(new object[0]);
            }
        }

        /// <summary>
        /// Add a new medical record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddMedicalRecord([FromBody] object medicalRecord)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                
                if (Request.Cookies.ContainsKey("authToken"))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["authToken"]);
                }

                var json = JsonSerializer.Serialize(medicalRecord);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("api/v1/medicalrecords", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Json(JsonSerializer.Deserialize<object>(responseContent));
                }
                
                return Json(new { success = false, message = "Failed to add medical record" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding medical record");
                return Json(new { success = false, message = "Error adding medical record" });
            }
        }
    }
}