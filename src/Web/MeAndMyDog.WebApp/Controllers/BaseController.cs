using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MeAndMyDog.WebApp.Services;
using System.Security.Claims;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Base controller with role-based access control and navigation context
/// </summary>
[Authorize]
public abstract class BaseController : Controller
{
    protected readonly IRoleNavigationService _roleNavigationService;
    protected readonly ILogger _logger;

    protected BaseController(IRoleNavigationService roleNavigationService, ILogger logger)
    {
        _roleNavigationService = roleNavigationService;
        _logger = logger;
    }

    /// <summary>
    /// Get the current user's ID
    /// </summary>
    protected string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Get the current user's navigation context
    /// </summary>
    protected async Task<Models.UserNavigationContext> GetNavigationContextAsync()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return new Models.UserNavigationContext();
        }

        return await _roleNavigationService.GetUserNavigationContextAsync(CurrentUserId);
    }

    /// <summary>
    /// Set navigation context in ViewBag for use in views
    /// </summary>
    protected async Task SetNavigationContextAsync(string? activeRole = null)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return;
        }

        try
        {
            var context = await GetNavigationContextAsync();
            var menuItems = await _roleNavigationService.GetNavigationMenuAsync(CurrentUserId, activeRole);

            ViewBag.NavigationContext = context;
            ViewBag.NavigationMenu = menuItems;
            ViewBag.CurrentRole = activeRole ?? context.CurrentRole;
            ViewBag.CanSwitchRoles = context.CanSwitchRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting navigation context for user {UserId}", CurrentUserId);
        }
    }

    /// <summary>
    /// Check if current user has a specific role
    /// </summary>
    protected async Task<bool> HasRoleAsync(string role)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return false;

        return await _roleNavigationService.HasRouteAccessAsync(CurrentUserId, Request.Path, role);
    }

    /// <summary>
    /// Redirect to appropriate dashboard based on user roles
    /// </summary>
    protected async Task<IActionResult> RedirectToDashboardAsync(string? preferredRole = null)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var dashboardUrl = await _roleNavigationService.GetDashboardUrlAsync(CurrentUserId, preferredRole);
        return Redirect(dashboardUrl);
    }

    /// <summary>
    /// Called before action execution to set up navigation context
    /// </summary>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Set navigation context for all views
        await SetNavigationContextAsync();
        
        await base.OnActionExecutionAsync(context, next);
    }
}