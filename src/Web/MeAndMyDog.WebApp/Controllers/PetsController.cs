using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Controller for pet profile management views
/// </summary>
[Authorize]
public class PetsController : Controller
{
    private readonly ILogger<PetsController> _logger;

    /// <summary>
    /// Initializes a new instance of PetsController
    /// </summary>
    public PetsController(ILogger<PetsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the pet profile management page
    /// </summary>
    /// <returns>Pet profile view</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Display specific pet profile
    /// </summary>
    /// <param name="id">Pet ID</param>
    /// <returns>Pet profile view</returns>
    public IActionResult Profile(string id)
    {
        ViewData["PetId"] = id;
        return View("Index");
    }
}