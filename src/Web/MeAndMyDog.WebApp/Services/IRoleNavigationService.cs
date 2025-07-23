using MeAndMyDog.WebApp.Models;

namespace MeAndMyDog.WebApp.Services;

/// <summary>
/// Service interface for managing role-based navigation and user context
/// </summary>
public interface IRoleNavigationService
{
    /// <summary>
    /// Get the current user's roles and navigation context
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User navigation context</returns>
    Task<UserNavigationContext> GetUserNavigationContextAsync(string userId);
    
    /// <summary>
    /// Get navigation menu items based on user roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currentRole">Currently active role</param>
    /// <returns>Navigation menu items</returns>
    Task<List<NavigationMenuItem>> GetNavigationMenuAsync(string userId, string? currentRole = null);
    
    /// <summary>
    /// Check if user has access to a specific route
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="route">Route to check</param>
    /// <param name="requiredRole">Required role for the route</param>
    /// <returns>True if user has access</returns>
    Task<bool> HasRouteAccessAsync(string userId, string route, string requiredRole);
    
    /// <summary>
    /// Get the appropriate dashboard URL for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="preferredRole">Preferred role if user has multiple</param>
    /// <returns>Dashboard URL</returns>
    Task<string> GetDashboardUrlAsync(string userId, string? preferredRole = null);
}