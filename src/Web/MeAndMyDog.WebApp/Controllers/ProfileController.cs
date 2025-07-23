using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Controller for user profile management
/// </summary>
[Authorize]
public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Configuration instance</param>
    public ProfileController(ILogger<ProfileController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display user profile page
    /// </summary>
    /// <returns>Profile view</returns>
    public IActionResult Index()
    {
        _logger.LogInformation("=== ProfileController.Index START ===");
        _logger.LogInformation("User.Identity.IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
        _logger.LogInformation("User.Identity.Name: {Name}", User.Identity?.Name ?? "null");
        _logger.LogInformation("User.Identity.AuthenticationType: {AuthType}", User.Identity?.AuthenticationType ?? "null");
        _logger.LogInformation("User Claims Count: {Count}", User.Claims.Count());
        
        foreach (var claim in User.Claims)
        {
            _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        
        _logger.LogInformation("ProfileController.Index called for user: {User}", User.Identity?.Name ?? "Anonymous");
        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"] ?? "";
        
        _logger.LogInformation("=== ProfileController.Index END - Returning View ===");
        return View();
    }
    
    /// <summary>
    /// Test action to verify controller is reachable
    /// </summary>
    /// <returns>JSON result</returns>
    [HttpGet]
    [Route("Profile/Test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        _logger.LogInformation("=== ProfileController.Test called ===");
        return Json(new 
        { 
            message = "ProfileController is reachable!", 
            timestamp = DateTime.UtcNow,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            userName = User.Identity?.Name ?? "Anonymous"
        });
    }
}