using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Simple test controller to debug routing issues
/// </summary>
public class TestController : Controller
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Test action without authorization
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogWarning("TestController.Index was hit!");
        return Content("TestController.Index works!");
    }

    /// <summary>
    /// Test action with authorization
    /// </summary>
    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Secure()
    {
        _logger.LogWarning("TestController.Secure was hit!");
        return Content($"TestController.Secure works! User: {User.Identity?.Name}");
    }
}