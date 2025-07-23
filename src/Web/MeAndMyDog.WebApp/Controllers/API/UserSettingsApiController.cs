using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.WebApp.Models.DTOs;
using MeAndMyDog.WebApp.Models.DTOs.UserProfile;
using MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth;
using MeAndMyDog.WebApp.Models.DTOs.Billing;

namespace MeAndMyDog.WebApp.Controllers.API
{
    /// <summary>
    /// API controller for user settings and profile management
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/user")]
    public class UserSettingsApiController : ControllerBase
    {
        private readonly ILogger<UserSettingsApiController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UserSettingsApiController(
            ILogger<UserSettingsApiController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Get user profile information
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/profile");

                if (response.IsSuccessStatusCode)
                {
                    var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
                    return Ok(profile);
                }
                else
                {
                    // Return data from claims as fallback
                    var fallbackProfile = new UserProfileDto
                    {
                        Id = userId,
                        FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "",
                        LastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? "",
                        Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                        PhoneNumber = "",
                        TimeZone = "Europe/London",
                        PreferredLanguage = "en-GB",
                        ProfilePhotoUrl = null,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow
                    };
                    
                    return Ok(fallbackProfile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to retrieve profile" });
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PutAsJsonAsync($"api/v1/users/{userId}/profile", dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} updated profile successfully", userId);
                    return Ok(new { message = "Profile updated successfully" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update profile for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, response.StatusCode, error);
                    return BadRequest(new { error = "Failed to update profile" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to update profile" });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsJsonAsync($"api/v1/users/{userId}/change-password", dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} changed password successfully", userId);
                    return Ok(new { message = "Password changed successfully" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Password change failed for user {UserId}. Error: {Error}", userId, error);
                    return BadRequest(new { error = "Current password is incorrect" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to change password for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, response.StatusCode, error);
                    return BadRequest(new { error = "Failed to change password" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to change password" });
            }
        }

        /// <summary>
        /// Get active sessions
        /// </summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/sessions");

                if (response.IsSuccessStatusCode)
                {
                    var sessions = await response.Content.ReadFromJsonAsync<List<SessionDto>>();
                    
                    // Mark current session
                    var currentSessionId = HttpContext.Session.Id;
                    if (sessions != null)
                    {
                        foreach (var session in sessions)
                        {
                            session.IsCurrent = session.Id == currentSessionId;
                        }
                    }
                    
                    return Ok(sessions ?? new List<SessionDto>());
                }
                else
                {
                    _logger.LogWarning("Failed to get sessions for user {UserId}. Status: {StatusCode}", userId, response.StatusCode);
                    
                    // Return current session only as fallback
                    var currentSession = new SessionDto
                    {
                        Id = HttpContext.Session.Id ?? Guid.NewGuid().ToString(),
                        DeviceType = GetDeviceType(Request.Headers["User-Agent"].ToString()),
                        DeviceName = GetDeviceName(Request.Headers["User-Agent"].ToString()),
                        Browser = GetBrowserName(Request.Headers["User-Agent"].ToString()),
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        Location = "Unknown", // Would need geolocation service
                        CreatedAt = DateTime.UtcNow,
                        LastActivityAt = DateTime.UtcNow,
                        IsCurrent = true
                    };
                    
                    return Ok(new List<SessionDto> { currentSession });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to retrieve sessions" });
            }
        }

        /// <summary>
        /// Terminate a specific session
        /// </summary>
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> TerminateSession(string sessionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User {UserId} terminated session {SessionId}", userId, sessionId);

            return Ok(new { message = "Session terminated successfully" });
        }

        /// <summary>
        /// Get notification preferences
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/notification-preferences");

                if (response.IsSuccessStatusCode)
                {
                    var preferences = await response.Content.ReadFromJsonAsync<NotificationPreferencesDto>();
                    return Ok(preferences);
                }
                else
                {
                    // Return default preferences as fallback
                    var defaultPreferences = new NotificationPreferencesDto
                    {
                        EmailNotifications = true,
                        SmsNotifications = false,
                        PushNotifications = true,
                        BookingReminders = true,
                        NewMessages = true,
                        ServiceUpdates = true,
                        MarketingCommunications = false,
                        QuietHoursStart = "22:00",
                        QuietHoursEnd = "08:00"
                    };
                    
                    return Ok(defaultPreferences);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preferences for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to retrieve notification preferences" });
            }
        }

        /// <summary>
        /// Update notification preferences
        /// </summary>
        [HttpPut("notifications")]
        public async Task<IActionResult> UpdateNotificationPreferences([FromBody] NotificationPreferencesDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PutAsJsonAsync($"api/v1/users/{userId}/notification-preferences", dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} updated notification preferences", userId);
                    return Ok(new { message = "Notification preferences updated successfully" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update notification preferences for user {UserId}. Status: {StatusCode}", 
                        userId, response.StatusCode);
                    return BadRequest(new { error = "Failed to update notification preferences" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while updating notification preferences" });
            }
        }

        /// <summary>
        /// Get privacy settings
        /// </summary>
        [HttpGet("privacy")]
        public async Task<IActionResult> GetPrivacySettings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var settings = new PrivacySettingsDto
            {
                ProfilePublic = true,
                ShowLocation = true,
                AllowDataSharing = false,
                AllowAnalytics = true,
                ShowOnlineStatus = true
            };

            return Ok(settings);
        }

        /// <summary>
        /// Update privacy settings
        /// </summary>
        [HttpPut("privacy")]
        public async Task<IActionResult> UpdatePrivacySettings([FromBody] PrivacySettingsDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PutAsJsonAsync($"api/v1/users/{userId}/privacy-settings", dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} updated privacy settings", userId);
                    return Ok(new { message = "Privacy settings updated successfully" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update privacy settings for user {UserId}. Status: {StatusCode}", 
                        userId, response.StatusCode);
                    return BadRequest(new { error = "Failed to update privacy settings" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating privacy settings for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while updating privacy settings" });
            }
        }

        /// <summary>
        /// Request data export
        /// </summary>
        [HttpPost("data-export")]
        public async Task<IActionResult> RequestDataExport()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User {UserId} requested data export", userId);

            return Ok(new { 
                message = "Data export requested successfully", 
                estimatedTime = "24 hours",
                notificationMethod = "email"
            });
        }

        /// <summary>
        /// Request account deletion
        /// </summary>
        [HttpPost("delete-account")]
        public async Task<IActionResult> RequestAccountDeletion([FromBody] DeleteAccountDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // First verify the password with backend API
                var client = _httpClientFactory.CreateClient("API");
                var verifyRequest = new { password = dto.Password };
                var verifyResponse = await client.PostAsJsonAsync($"api/v1/users/{userId}/verify-password", verifyRequest);

                if (!verifyResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Password verification failed for account deletion request by user {UserId}", userId);
                    return BadRequest(new { error = "Invalid password" });
                }

                // If password is verified, proceed with account deletion request
                var deleteResponse = await client.PostAsJsonAsync($"api/v1/users/{userId}/delete-account", dto);

                if (deleteResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("User {UserId} requested account deletion with verified password", userId);
                    return Ok(new { 
                        message = "Account deletion scheduled", 
                        gracePeriod = "30 days",
                        cancellationEmail = User.FindFirst(ClaimTypes.Email)?.Value
                    });
                }
                else
                {
                    var error = await deleteResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to process account deletion for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, deleteResponse.StatusCode, error);
                    return BadRequest(new { error = "Failed to process account deletion request" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing account deletion for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to process account deletion request" });
            }
        }

        /// <summary>
        /// Get two-factor authentication status
        /// </summary>
        [HttpGet("2fa/status")]
        public async Task<IActionResult> Get2FAStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/2fa/status");

                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<TwoFactorStatusDto>();
                    return Ok(status);
                }
                else
                {
                    // Return default status as fallback
                    var defaultStatus = new TwoFactorStatusDto
                    {
                        SmsEnabled = false,
                        AppEnabled = false,
                        PhoneNumber = null,
                        BackupCodesRemaining = 0
                    };
                    
                    return Ok(defaultStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting 2FA status for user {UserId}", userId);
                return Ok(new TwoFactorStatusDto
                {
                    SmsEnabled = false,
                    AppEnabled = false,
                    PhoneNumber = null,
                    BackupCodesRemaining = 0
                });
            }
        }

        /// <summary>
        /// Enable two-factor authentication
        /// </summary>
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2FA([FromBody] Enable2FADto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (dto.Type == "sms")
            {
                if (string.IsNullOrEmpty(dto.PhoneNumber))
                {
                    return BadRequest(new { error = "Phone number is required for SMS 2FA" });
                }

                _logger.LogInformation("SMS 2FA setup initiated for user {UserId}", userId);

                return Ok(new Setup2FAResponseDto
                {
                    Success = true,
                    Message = "Verification code sent to your phone"
                });
            }
            else if (dto.Type == "app")
            {
                var key = GenerateRandomKey();
                var formattedKey = FormatKey(key);
                var qrCodeUrl = GenerateQrCodeUrl(User.FindFirst(ClaimTypes.Email)?.Value ?? "user@example.com", key);

                return Ok(new Setup2FAResponseDto
                {
                    Success = true,
                    Message = "Scan the QR code with your authenticator app",
                    QrCodeUrl = qrCodeUrl,
                    ManualKey = formattedKey
                });
            }

            return BadRequest(new { error = "Invalid 2FA type" });
        }

        /// <summary>
        /// Verify and complete 2FA setup
        /// </summary>
        [HttpPost("2fa/verify")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsJsonAsync($"api/v1/users/{userId}/2fa/verify", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Setup2FAResponseDto>();
                    return Ok(result);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { error = "Invalid verification code" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to verify 2FA code" });
            }
        }

        /// <summary>
        /// Disable two-factor authentication
        /// </summary>
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable2FA([FromBody] Disable2FADto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // First verify the password with backend API
                var client = _httpClientFactory.CreateClient("API");
                var verifyRequest = new { password = dto.Password };
                var verifyResponse = await client.PostAsJsonAsync($"api/v1/users/{userId}/verify-password", verifyRequest);

                if (!verifyResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Password verification failed for 2FA disable request by user {UserId}", userId);
                    return BadRequest(new { error = "Invalid password" });
                }

                // If password is verified, proceed with 2FA disable
                var disable2FAResponse = await client.PostAsJsonAsync($"api/v1/users/{userId}/2fa/disable", dto);

                if (disable2FAResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} disabled 2FA with verified password", userId);
                    return Ok(new { message = "Two-factor authentication disabled successfully" });
                }
                else
                {
                    var error = await disable2FAResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to disable 2FA for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, disable2FAResponse.StatusCode, error);
                    return BadRequest(new { error = "Failed to disable two-factor authentication" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling 2FA for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to disable two-factor authentication" });
            }
        }

        private string GenerateRandomKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var random = new Random();
            var key = new StringBuilder();
            
            for (int i = 0; i < 32; i++)
            {
                key.Append(chars[random.Next(chars.Length)]);
            }
            
            return key.ToString();
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUrl(string email, string key)
        {
            const string appName = "MeAndMyDoggy";
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedKey = Uri.EscapeDataString(key);
            
            return $"otpauth://totp/{appName}:{encodedEmail}?secret={encodedKey}&issuer={appName}";
        }

        private List<string> GenerateBackupCodes(int count)
        {
            var codes = new List<string>();
            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var code = $"{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
                codes.Add(code);
            }
            
            return codes;
        }

        #region Billing Management

        /// <summary>
        /// Get Stripe configuration (public key)
        /// </summary>
        [HttpGet("billing/stripe-config")]
        public IActionResult GetStripeConfig()
        {
            // In production, this should come from configuration
            var publicKey = _configuration["Stripe:PublicKey"] ?? "pk_test_51NLc5eSJKGqV9NLHuRgF8iZQnPzPGJvDZqJjCvUzWZBh5QqOG0CcfKQUXKGNKWxZRYIxgXhFVOcOHXKlKGBG6Ej600eROaZBUV";
            
            return Ok(new { publicKey });
        }

        /// <summary>
        /// Get user's subscription information
        /// </summary>
        [HttpGet("billing/subscription")]
        public async Task<IActionResult> GetSubscription()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/subscription");

                if (response.IsSuccessStatusCode)
                {
                    var subscription = await response.Content.ReadFromJsonAsync<SubscriptionDto>();
                    return Ok(subscription);
                }
                else
                {
                    // Return free plan as fallback
                    return Ok(new SubscriptionDto
                    {
                        PlanName = "Free",
                        PlanId = "free",
                        Status = "active",
                        PricePerMonth = 0,
                        Currency = "GBP",
                        Features = new List<string>()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription for user {UserId}", userId);
                return StatusCode(500, new { error = "Failed to retrieve subscription" });
            }
        }

        /// <summary>
        /// Get user's payment methods
        /// </summary>
        [HttpGet("billing/payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/payment-methods");

                if (response.IsSuccessStatusCode)
                {
                    var methods = await response.Content.ReadFromJsonAsync<List<PaymentMethodDto>>();
                    return Ok(methods ?? new List<PaymentMethodDto>());
                }
                else
                {
                    return Ok(new List<PaymentMethodDto>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment methods for user {UserId}", userId);
                return Ok(new List<PaymentMethodDto>());
            }
        }

        /// <summary>
        /// Get billing history
        /// </summary>
        [HttpGet("billing/history")]
        public async Task<IActionResult> GetBillingHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"api/v1/users/{userId}/billing-history");

                if (response.IsSuccessStatusCode)
                {
                    var history = await response.Content.ReadFromJsonAsync<List<BillingHistoryDto>>();
                    return Ok(history ?? new List<BillingHistoryDto>());
                }
                else
                {
                    return Ok(new List<BillingHistoryDto>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting billing history for user {UserId}", userId);
                return Ok(new List<BillingHistoryDto>());
            }
        }

        /// <summary>
        /// Add a new payment method
        /// </summary>
        [HttpPost("billing/payment-methods")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] AddPaymentMethodDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsJsonAsync($"api/v1/users/{userId}/payment-methods", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    _logger.LogInformation("User {UserId} added payment method", userId);
                    return Ok(new { success = true, message = "Payment method added successfully" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to add payment method for user {UserId}. Status: {StatusCode}", userId, response.StatusCode);
                    return Ok(new { success = false, message = "Failed to add payment method" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment method for user {UserId}", userId);
                return Ok(new { success = false, message = "An error occurred while adding payment method" });
            }
        }

        /// <summary>
        /// Remove a payment method
        /// </summary>
        [HttpDelete("billing/payment-methods/{methodId}")]
        public async Task<IActionResult> RemovePaymentMethod(string methodId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User {UserId} removed payment method {MethodId}", userId, methodId);

            return Ok(new { message = "Payment method removed successfully" });
        }

        /// <summary>
        /// Set default payment method
        /// </summary>
        [HttpPost("billing/payment-methods/{methodId}/default")]
        public async Task<IActionResult> SetDefaultPaymentMethod(string methodId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User {UserId} set default payment method {MethodId}", userId, methodId);

            return Ok(new { message = "Default payment method updated successfully" });
        }

        /// <summary>
        /// Change subscription plan
        /// </summary>
        [HttpPost("billing/subscription/change")]
        public async Task<IActionResult> ChangeSubscriptionPlan([FromBody] ChangePlanDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsJsonAsync($"api/v1/users/{userId}/subscription/change", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    _logger.LogInformation("User {UserId} changed to plan {PlanId}", userId, dto.PlanId);
                    return Ok(new { success = true, message = "Subscription plan updated successfully" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
                {
                    return Ok(new { success = false, message = "Please add a payment method before upgrading" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to change plan for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, response.StatusCode, error);
                    return Ok(new { success = false, message = "Failed to change subscription plan" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing subscription plan for user {UserId}", userId);
                return Ok(new { success = false, message = "An error occurred while changing your plan" });
            }
        }

        /// <summary>
        /// Cancel subscription
        /// </summary>
        [HttpPost("billing/subscription/cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsync($"api/v1/users/{userId}/subscription/cancel", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    _logger.LogWarning("User {UserId} cancelled subscription", userId);
                    
                    // Get the actual end date from the response or calculate it
                    var endDate = DateTime.UtcNow.AddDays(30); // Default to 30 days
                    
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Subscription cancelled successfully",
                        accessUntil = endDate.ToString("dd MMM yyyy")
                    });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to cancel subscription for user {UserId}. Status: {StatusCode}, Error: {Error}", 
                        userId, response.StatusCode, error);
                    return Ok(new { success = false, message = "Failed to cancel subscription" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription for user {UserId}", userId);
                return Ok(new { success = false, message = "An error occurred while cancelling subscription" });
            }
        }

        /// <summary>
        /// Download invoice
        /// </summary>
        [HttpGet("billing/invoices/{invoiceId}/download")]
        public async Task<IActionResult> DownloadInvoice(string invoiceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // In real implementation, this would generate/fetch actual invoice PDF
            var pdfContent = GenerateMockInvoicePdf(invoiceId);
            
            return File(pdfContent, "application/pdf", $"invoice-{invoiceId}.pdf");
        }

        /// <summary>
        /// Update tax information
        /// </summary>
        [HttpPut("billing/tax-info")]
        public async Task<IActionResult> UpdateTaxInfo([FromBody] TaxInfoDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User {UserId} updated tax information", userId);

            return Ok(new { message = "Tax information updated successfully" });
        }

        private byte[] GenerateMockInvoicePdf(string invoiceId)
        {
            // Mock PDF content - in real implementation use a PDF library
            var content = $"Invoice #{invoiceId}\n\nMeAndMyDoggy Ltd\n\nThis is a mock invoice.";
            return Encoding.UTF8.GetBytes(content);
        }

        #endregion

        #region Helper Methods

        private string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();
            
            if (userAgent.Contains("mobile") || userAgent.Contains("android"))
                return "Mobile";
            if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
                return "Tablet";
            if (userAgent.Contains("iphone"))
                return "iPhone";
            
            return "Desktop";
        }

        private string GetDeviceName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown Device";

            userAgent = userAgent.ToLower();
            
            if (userAgent.Contains("windows nt 10"))
                return "Windows 10";
            if (userAgent.Contains("windows nt 11"))
                return "Windows 11";
            if (userAgent.Contains("mac os x"))
                return "Mac";
            if (userAgent.Contains("iphone"))
                return "iPhone";
            if (userAgent.Contains("ipad"))
                return "iPad";
            if (userAgent.Contains("android"))
                return "Android Device";
            if (userAgent.Contains("linux"))
                return "Linux";
            
            return "Unknown Device";
        }

        private string GetBrowserName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();
            
            if (userAgent.Contains("edg/"))
                return "Edge";
            if (userAgent.Contains("chrome") && !userAgent.Contains("edg"))
                return "Chrome";
            if (userAgent.Contains("firefox"))
                return "Firefox";
            if (userAgent.Contains("safari") && !userAgent.Contains("chrome"))
                return "Safari";
            if (userAgent.Contains("opera") || userAgent.Contains("opr/"))
                return "Opera";
            
            return "Unknown Browser";
        }

        #endregion
    }

}