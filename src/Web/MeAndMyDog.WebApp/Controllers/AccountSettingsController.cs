using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// Controller for managing user account settings
    /// </summary>
    [Authorize]
    public class AccountSettingsController : Controller
    {
        private readonly ILogger<AccountSettingsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountSettingsController(ILogger<AccountSettingsController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Display the main account settings page
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Account Settings";
            ViewData["Description"] = "Manage your account settings, privacy, notifications, and billing preferences";
            
            // Get user information for display
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? "";
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            
            ViewData["UserFirstName"] = firstName;
            ViewData["UserLastName"] = lastName;
            ViewData["UserEmail"] = email;
            ViewData["UserId"] = userId;
            ViewData["IsPetOwner"] = User.IsInRole("PetOwner") || User.IsInRole("User");
            ViewData["IsServiceProvider"] = User.IsInRole("ServiceProvider");
            ViewData["IsAdmin"] = User.IsInRole("Admin");
            
            return View();
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PutAsJsonAsync("api/v1/user/profile", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Profile updated successfully" });
                }

                return Json(new { success = false, message = "Failed to update profile" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return Json(new { success = false, message = "An error occurred while updating your profile" });
            }
        }

        /// <summary>
        /// Update password
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PostAsJsonAsync("api/v1/user/change-password", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Password changed successfully" });
                }

                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Failed to change password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return Json(new { success = false, message = "An error occurred while changing your password" });
            }
        }

        /// <summary>
        /// Update notification preferences
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateNotifications([FromBody] NotificationPreferencesRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PutAsJsonAsync("api/v1/user/notifications", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Notification preferences updated" });
                }

                return Json(new { success = false, message = "Failed to update notification preferences" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences");
                return Json(new { success = false, message = "An error occurred while updating notification preferences" });
            }
        }

        /// <summary>
        /// Update privacy settings
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdatePrivacy([FromBody] PrivacySettingsRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PutAsJsonAsync("api/v1/user/privacy", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Privacy settings updated" });
                }

                return Json(new { success = false, message = "Failed to update privacy settings" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating privacy settings");
                return Json(new { success = false, message = "An error occurred while updating privacy settings" });
            }
        }

        /// <summary>
        /// Request data export
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RequestDataExport()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PostAsync("api/v1/user/data-export", null);
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data export requested. You will receive an email when ready." });
                }

                return Json(new { success = false, message = "Failed to request data export" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting data export");
                return Json(new { success = false, message = "An error occurred while requesting data export" });
            }
        }

        /// <summary>
        /// Request account deletion
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                // Add JWT token for authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                var response = await httpClient.PostAsJsonAsync("api/v1/user/delete-account", request);
                
                if (response.IsSuccessStatusCode)
                {
                    // Sign out the user
                    await HttpContext.SignOutAsync();
                    return Json(new { success = true, message = "Account deletion requested", redirect = "/" });
                }

                return Json(new { success = false, message = "Failed to delete account" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                return Json(new { success = false, message = "An error occurred while deleting your account" });
            }
        }
    }
}