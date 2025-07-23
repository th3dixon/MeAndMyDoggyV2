using MeAndMyDog.WebApp.Models;

namespace MeAndMyDog.WebApp.Services;

/// <summary>
/// API-based role navigation service that doesn't require direct database access
/// </summary>
public class ApiRoleNavigationService : IRoleNavigationService
{
    private readonly IApiAuthService _apiAuthService;
    private readonly ILogger<ApiRoleNavigationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiRoleNavigationService(
        IApiAuthService apiAuthService,
        ILogger<ApiRoleNavigationService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _apiAuthService = apiAuthService;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<UserNavigationContext> GetUserNavigationContextAsync(string userId)
    {
        try
        {
            var userInfo = await _apiAuthService.GetCurrentUserAsync();
            if (userInfo == null)
            {
                return CreateGuestNavigationContext();
            }

            var availableRoles = new List<UserRole>();
            var currentRole = "PetOwner"; // Default role

            // Check for PetOwner role
            if (await _apiAuthService.HasRoleAsync("PetOwner"))
            {
                var petOwnerRole = new UserRole
                {
                    Name = "PetOwner",
                    DisplayName = "Pet Owner",
                    Description = "Manage your pets and book services",
                    Icon = "🐕",
                    DashboardUrl = "/dashboard",
                    IsActive = true,
                    Metadata = new Dictionary<string, object>()
                };

                availableRoles.Add(petOwnerRole);
            }

            // Check for ServiceProvider role
            if (await _apiAuthService.HasRoleAsync("ServiceProvider"))
            {
                var providerId = await _apiAuthService.GetProviderIdAsync();
                var isPremium = await GetProviderPremiumStatusAsync(providerId);
                
                var providerRole = new UserRole
                {
                    Name = "ServiceProvider",
                    DisplayName = isPremium ? "Premium Provider" : "Service Provider",
                    Description = "Manage your business and clients",
                    Icon = isPremium ? "👑" : "🏢",
                    DashboardUrl = "/provider/dashboard",
                    IsActive = true,
                    Metadata = new Dictionary<string, object>
                    {
                        { "ProviderId", providerId ?? "" },
                        { "IsPremium", isPremium }
                    }
                };

                availableRoles.Add(providerRole);
                currentRole = "ServiceProvider"; // Prefer provider role if available
            }

            return new UserNavigationContext
            {
                DisplayName = $"{userInfo.FirstName} {userInfo.LastName}".Trim(),
                CurrentRole = currentRole,
                AvailableRoles = availableRoles,
                CanSwitchRoles = availableRoles.Count > 1,
                DefaultDashboardUrl = availableRoles.FirstOrDefault()?.DashboardUrl ?? "/dashboard",
                Context = new Dictionary<string, object>
                {
                    { "UserId", userInfo.Id },
                    { "Email", userInfo.Email },
                    { "IsAuthenticated", true },
                    { "RequiresRoleSelection", availableRoles.Count == 0 }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user navigation context for user {UserId}", userId);
            return CreateGuestNavigationContext();
        }
    }

    public async Task<bool> HasRequiredRoleAsync(string userId, string requiredRole)
    {
        try
        {
            return await _apiAuthService.HasRoleAsync(requiredRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {Role} for user {UserId}", requiredRole, userId);
            return false;
        }
    }

    public async Task<string> GetPreferredDashboardAsync(string userId)
    {
        try
        {
            var isProvider = await _apiAuthService.IsServiceProviderAsync();
            var isPetOwner = await _apiAuthService.HasRoleAsync("PetOwner");

            // Prefer provider dashboard if user is a provider
            if (isProvider)
            {
                return "/provider/dashboard";
            }
            else if (isPetOwner)
            {
                return "/dashboard";
            }
            else
            {
                return "/auth/login";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferred dashboard for user {UserId}", userId);
            return "/dashboard";
        }
    }

    public async Task<bool> SwitchUserRoleAsync(string userId, string newRole)
    {
        try
        {
            // For API-based authentication, role switching is just a matter of 
            // redirecting to the appropriate dashboard since roles are determined by API
            var hasRole = await _apiAuthService.HasRoleAsync(newRole);
            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching to role {Role} for user {UserId}", newRole, userId);
            return false;
        }
    }

    public async Task<UserRole?> GetRoleDetailsAsync(string userId, string roleName)
    {
        try
        {
            var context = await GetUserNavigationContextAsync(userId);
            return context.AvailableRoles.FirstOrDefault(r => r.Name == roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role details for {Role} for user {UserId}", roleName, userId);
            return null;
        }
    }

    public async Task<List<NavigationMenuItem>> GetNavigationMenuAsync(string userId, string? currentRole = null)
    {
        try
        {
            var context = await GetUserNavigationContextAsync(userId);
            var menuItems = new List<NavigationMenuItem>();

            if (currentRole == "ServiceProvider" || (currentRole == null && await _apiAuthService.IsServiceProviderAsync()))
            {
                menuItems.AddRange(new[]
                {
                    new NavigationMenuItem { Text = "Dashboard", Url = "/provider/dashboard", Icon = "fas fa-tachometer-alt", IsActive = true },
                    new NavigationMenuItem { Text = "Bookings", Url = "/provider/bookings", Icon = "fas fa-calendar-check" },
                    new NavigationMenuItem { Text = "Clients", Url = "/provider/clients", Icon = "fas fa-users" },
                    new NavigationMenuItem { Text = "Invoicing", Url = "/provider/invoicing", Icon = "fas fa-file-invoice-dollar" },
                    new NavigationMenuItem { Text = "Analytics", Url = "/provider/analytics", Icon = "fas fa-chart-line" }
                });
            }
            else
            {
                menuItems.AddRange(new[]
                {
                    new NavigationMenuItem { Text = "Dashboard", Url = "/dashboard", Icon = "fas fa-home", IsActive = true },
                    new NavigationMenuItem { Text = "My Pets", Url = "/pets", Icon = "fas fa-paw" },
                    new NavigationMenuItem { Text = "Services", Url = "/search", Icon = "fas fa-search" },
                    new NavigationMenuItem { Text = "Health", Url = "/health", Icon = "fas fa-heart" }
                });
            }

            return menuItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting navigation menu for user {UserId}", userId);
            return new List<NavigationMenuItem>();
        }
    }

    public async Task<bool> HasRouteAccessAsync(string userId, string route, string requiredRole)
    {
        try
        {
            return await _apiAuthService.HasRoleAsync(requiredRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking route access for {Route} with role {Role} for user {UserId}", route, requiredRole, userId);
            return false;
        }
    }

    public async Task<string> GetDashboardUrlAsync(string userId, string? preferredRole = null)
    {
        return await GetPreferredDashboardAsync(userId);
    }

    private async Task<bool> GetProviderPremiumStatusAsync(string? providerId)
    {
        if (string.IsNullOrEmpty(providerId))
        {
            return false;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/v1/providers/{providerId}/premium-status");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return result?.isPremium ?? false;
            }
            else
            {
                _logger.LogWarning("Failed to get premium status for provider {ProviderId}. Status: {StatusCode}", 
                    providerId, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting premium status for provider {ProviderId}", providerId);
            return false;
        }
    }

    private static UserNavigationContext CreateGuestNavigationContext()
    {
        return new UserNavigationContext
        {
            DisplayName = "Guest",
            CurrentRole = string.Empty,
            AvailableRoles = new List<UserRole>(),
            CanSwitchRoles = false,
            DefaultDashboardUrl = "/auth/login",
            Context = new Dictionary<string, object>
            {
                { "UserId", string.Empty },
                { "Email", string.Empty },
                { "IsAuthenticated", false },
                { "RequiresRoleSelection", false }
            }
        };
    }
}