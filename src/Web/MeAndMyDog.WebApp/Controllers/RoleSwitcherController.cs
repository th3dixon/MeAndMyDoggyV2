using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.WebApp.Services;
using MeAndMyDog.WebApp.Models.DTOs.RoleSwitcher;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Controller for handling role switching functionality
/// </summary>
public class RoleSwitcherController : BaseController
{
    public RoleSwitcherController(
        IRoleNavigationService roleNavigationService,
        ILogger<RoleSwitcherController> logger)
        : base(roleNavigationService, logger)
    {
    }

    /// <summary>
    /// API endpoint to get current user's available roles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAvailableRoles()
    {
        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized();
            }

            var context = await GetNavigationContextAsync();
            return Json(new
            {
                availableRoles = context.AvailableRoles,
                currentRole = context.CurrentRole,
                canSwitchRoles = context.CanSwitchRoles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available roles for user {UserId}", CurrentUserId);
            return BadRequest(new { error = "Failed to load available roles" });
        }
    }

    /// <summary>
    /// Switch to a specific role and redirect to appropriate dashboard
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SwitchRole([FromBody] SwitchRoleRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { error = "Role is required" });
            }

            // Verify user has access to the requested role
            var hasAccess = await _roleNavigationService.HasRouteAccessAsync(CurrentUserId, "/", request.Role);
            if (!hasAccess)
            {
                return Forbid();
            }

            // Get the dashboard URL for the requested role
            var dashboardUrl = await _roleNavigationService.GetDashboardUrlAsync(CurrentUserId, request.Role);

            // Store the selected role in session or cookie for persistence
            HttpContext.Session.SetString("SelectedRole", request.Role);

            return Json(new
            {
                success = true,
                redirectUrl = dashboardUrl,
                message = $"Switched to {request.Role} view"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching role for user {UserId} to {Role}", CurrentUserId, request.Role);
            return BadRequest(new { error = "Failed to switch role" });
        }
    }

    /// <summary>
    /// Get the role switcher component view
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRoleSwitcherComponent()
    {
        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return PartialView("_RoleSwitcher", new Models.UserNavigationContext());
            }

            var context = await GetNavigationContextAsync();
            return PartialView("_RoleSwitcher", context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role switcher component for user {UserId}", CurrentUserId);
            return PartialView("_RoleSwitcher", new Models.UserNavigationContext());
        }
    }

    /// <summary>
    /// Redirect to appropriate dashboard based on current role preference
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Check for preferred role in session
            var preferredRole = HttpContext.Session.GetString("SelectedRole");
            
            return await RedirectToDashboardAsync(preferredRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting to dashboard for user {UserId}", CurrentUserId);
            return RedirectToAction("Index", "Home");
        }
    }
}