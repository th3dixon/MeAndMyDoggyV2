using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeAndMyDog.WebApp.Attributes;

/// <summary>
/// Attribute to require specific role for controller actions
/// </summary>
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _requiredRole;
    private readonly string? _redirectAction;
    private readonly string? _redirectController;

    public RequireRoleAttribute(string requiredRole, string? redirectAction = null, string? redirectController = null)
    {
        _requiredRole = requiredRole;
        _redirectAction = redirectAction;
        _redirectController = redirectController;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!user.IsInRole(_requiredRole))
        {
            // If redirect specified, go there, otherwise forbidden
            if (!string.IsNullOrEmpty(_redirectAction))
            {
                context.Result = new RedirectToActionResult(
                    _redirectAction, 
                    _redirectController ?? "Home", 
                    null);
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}