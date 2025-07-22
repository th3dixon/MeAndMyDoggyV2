using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// Auth Proxy Controller - forwards authentication requests to the backend API server
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthProxyController> _logger;

        public AuthProxyController(IHttpClientFactory httpClientFactory, ILogger<AuthProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Proxy registration requests to the API server
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] object registrationData)
        {
            return await ProxyRequest("auth/register", registrationData);
        }

        /// <summary>
        /// Proxy login requests to the API server
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] object loginData)
        {
            return await ProxyRequest("auth/login", loginData);
        }

        /// <summary>
        /// Proxy refresh token requests to the API server
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] object refreshData)
        {
            return await ProxyRequest("auth/refresh", refreshData);
        }

        /// <summary>
        /// Proxy logout requests to the API server
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] object logoutData)
        {
            return await ProxyRequest("auth/logout", logoutData);
        }

        /// <summary>
        /// Proxy forgot password requests to the API server
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] object forgotPasswordData)
        {
            return await ProxyRequest("auth/forgot-password", forgotPasswordData);
        }

        /// <summary>
        /// Proxy reset password requests to the API server
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] object resetPasswordData)
        {
            return await ProxyRequest("auth/reset-password", resetPasswordData);
        }

        /// <summary>
        /// Proxy get current user requests to the API server
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await ProxyGetRequest("auth/me");
        }

        /// <summary>
        /// Get JWT token for SignalR authentication
        /// </summary>
        [HttpGet("token")]
        [Authorize]
        public IActionResult GetJWTToken()
        {
            try
            {
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized(new { message = "JWT token not found in user claims" });
                }

                return Ok(new { token = jwtToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving JWT token for user");
                return StatusCode(500, new { message = "Internal server error while retrieving token" });
            }
        }

        private async Task<IActionResult> ProxyGetRequest(string endpoint)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";

                _logger.LogInformation("Proxying GET request to: {TargetUrl}", targetUrl);

                // Add JWT token from user claims for API authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                // Send the GET request
                var response = await httpClient.GetAsync(targetUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying GET request to API server");
                return StatusCode(503, new { message = "Unable to connect to authentication service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying GET authentication request to API");
                return StatusCode(500, new { message = "Internal server error while processing authentication request" });
            }
        }

        private async Task<IActionResult> ProxyRequest(string endpoint, object data)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";

                _logger.LogInformation("Proxying POST request to: {TargetUrl}", targetUrl);

                // Serialize the data
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Add JWT token from user claims for API authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                // Send the request
                var response = await httpClient.PostAsync(targetUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying request to API server");
                return StatusCode(503, new { message = "Unable to connect to authentication service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying authentication request to API");
                return StatusCode(500, new { message = "Internal server error while processing authentication request" });
            }
        }
    }
}