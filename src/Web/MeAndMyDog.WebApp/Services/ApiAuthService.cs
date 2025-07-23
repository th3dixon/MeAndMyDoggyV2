using System.Security.Claims;
using System.Text.Json;

namespace MeAndMyDog.WebApp.Services;

/// <summary>
/// Service for handling API-based authentication and user information
/// </summary>
public interface IApiAuthService
{
    /// <summary>
    /// Get current user information from API
    /// </summary>
    Task<UserInfo?> GetCurrentUserAsync();
    
    /// <summary>
    /// Check if user has specific role
    /// </summary>
    Task<bool> HasRoleAsync(string role);
    
    /// <summary>
    /// Get user roles from API
    /// </summary>
    Task<List<string>> GetUserRolesAsync();
    
    /// <summary>
    /// Check if user is a service provider
    /// </summary>
    Task<bool> IsServiceProviderAsync();
    
    /// <summary>
    /// Get service provider ID for current user
    /// </summary>
    Task<string?> GetProviderIdAsync();
}

/// <summary>
/// Implementation of API-based authentication service
/// </summary>
public class ApiAuthService : IApiAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiAuthService> _logger;

    public ApiAuthService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiAuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            
            // Add auth token if available
            if (_httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey("authToken") == true)
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["authToken"];
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/v1/auth/me");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserInfo>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user from API");
            return null;
        }
    }

    public async Task<bool> HasRoleAsync(string role)
    {
        var roles = await GetUserRolesAsync();
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetUserRolesAsync()
    {
        var userInfo = await GetCurrentUserAsync();
        return userInfo?.Roles ?? new List<string>();
    }

    public async Task<bool> IsServiceProviderAsync()
    {
        return await HasRoleAsync("ServiceProvider");
    }

    public async Task<string?> GetProviderIdAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            
            // Add auth token if available
            if (_httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey("authToken") == true)
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["authToken"];
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/v1/providerupgrade/eligibility");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProviderEligibilityInfo>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data?.ProviderId;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider ID from API");
            return null;
        }
    }
}

/// <summary>
/// User information from API
/// </summary>
public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// Provider eligibility information from API
/// </summary>
public class ProviderEligibilityInfo
{
    public bool IsEligible { get; set; }
    public bool IsAlreadyProvider { get; set; }
    public string? ProviderId { get; set; }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
}