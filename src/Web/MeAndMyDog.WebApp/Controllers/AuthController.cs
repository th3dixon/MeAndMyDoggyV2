using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// Controller for handling authentication views and redirects
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthController> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Display login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handle login form submission via AJAX
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <param name="rememberMe">Remember me option</param>
        /// <returns>JSON response with login result</returns>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                
                var loginData = new
                {
                    email = email,
                    password = password,
                    rememberMe = rememberMe
                };

                var json = JsonSerializer.Serialize(loginData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/v1/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent, _jsonOptions);
                    _logger.LogInformation("User logged in successfully: {Email}", email);
                    
                    // Extract user data and token from API response
                    if (result.TryGetProperty("data", out var dataElement))
                    {
                        var jwtToken = "";
                        var refreshToken = "";
                        
                        // Extract JWT token and refresh token
                        if (dataElement.TryGetProperty("token", out var tokenElement))
                        {
                            jwtToken = tokenElement.GetString() ?? "";
                        }
                        if (dataElement.TryGetProperty("refreshToken", out var refreshTokenElement))
                        {
                            refreshToken = refreshTokenElement.GetString() ?? "";
                        }
                        
                        if (dataElement.TryGetProperty("user", out var userElement))
                        {
                            // Create claims for the authenticated user
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, email),
                                new Claim(ClaimTypes.Email, email)
                            };
                            
                            // Store JWT token as a claim for proxy controllers
                            if (!string.IsNullOrEmpty(jwtToken))
                            {
                                claims.Add(new Claim("jwt_token", jwtToken));
                            }
                            if (!string.IsNullOrEmpty(refreshToken))
                            {
                                claims.Add(new Claim("refresh_token", refreshToken));
                            }
                        
                            // Add user ID if available
                            if (userElement.TryGetProperty("id", out var idElement))
                            {
                                claims.Add(new Claim(ClaimTypes.NameIdentifier, idElement.GetString() ?? ""));
                            }
                        
                        // Add first name if available
                        if (userElement.TryGetProperty("firstName", out var firstNameElement))
                        {
                            claims.Add(new Claim(ClaimTypes.GivenName, firstNameElement.GetString() ?? ""));
                        }
                        
                        // Add last name if available
                        if (userElement.TryGetProperty("lastName", out var lastNameElement))
                        {
                            claims.Add(new Claim(ClaimTypes.Surname, lastNameElement.GetString() ?? ""));
                        }
                        
                        // Add user type if available
                        if (userElement.TryGetProperty("userType", out var userTypeElement))
                        {
                            var userType = userTypeElement.GetString();
                            claims.Add(new Claim("UserType", userType ?? ""));
                            
                            // Add role claims based on user type
                            switch (userType)
                            {
                                case "PetOwner":
                                    claims.Add(new Claim(ClaimTypes.Role, "PetOwner"));
                                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                                    break;
                                case "ServiceProvider":
                                    claims.Add(new Claim(ClaimTypes.Role, "ServiceProvider"));
                                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                                    break;
                                case "Both":
                                    claims.Add(new Claim(ClaimTypes.Role, "PetOwner"));
                                    claims.Add(new Claim(ClaimTypes.Role, "ServiceProvider"));
                                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                                    break;
                            }
                        }
                        
                            // Create claims identity and principal
                            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        
                            // Sign in the user with cookies
                            await HttpContext.SignInAsync("Cookies", claimsPrincipal, new AuthenticationProperties
                            {
                                IsPersistent = rememberMe,
                                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
                            });
                            
                            _logger.LogInformation("User signed in with cookie authentication: {Email}", email);
                        }
                    }
                    
                    return Json(new { success = true, data = result });
                }
                else
                {
                    _logger.LogWarning("Login failed for user: {Email}, Status: {StatusCode}", email, response.StatusCode);
                    var errorResult = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    return Json(new { success = false, message = "Login failed. Please check your credentials.", details = errorResult });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Email}", email);
                return Json(new { success = false, message = "An error occurred during login. Please try again." });
            }
        }

        /// <summary>
        /// Display registration page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handle logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out of cookie authentication
                await HttpContext.SignOutAsync("Cookies");
                _logger.LogInformation("User signed out successfully");
                
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}