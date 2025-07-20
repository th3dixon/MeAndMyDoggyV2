using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing service catalog operations including categories and sub-services
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;
    private readonly ILogger<ServiceCatalogController> _logger;

    /// <summary>
    /// Initializes a new instance of the ServiceCatalogController
    /// </summary>
    /// <param name="serviceCatalogService">Service for managing service catalog operations</param>
    /// <param name="logger">Logger instance for this controller</param>
    public ServiceCatalogController(IServiceCatalogService serviceCatalogService, ILogger<ServiceCatalogController> logger)
    {
        _serviceCatalogService = serviceCatalogService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active service categories with their sub-services
    /// </summary>
    /// <returns>List of active service categories with their sub-services</returns>
    /// <response code="200">Returns the list of service categories</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetServiceCategories()
    {
        var result = await _serviceCatalogService.GetServiceCategoriesAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Gets all active service categories with their sub-services (public endpoint for registration)
    /// </summary>
    /// <returns>List of active service categories with their sub-services</returns>
    /// <response code="200">Returns the list of service categories</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("public/categories")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPublicServiceCategories()
    {
        var result = await _serviceCatalogService.GetServiceCategoriesAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Gets a specific service category by ID with its sub-services
    /// </summary>
    /// <param name="categoryId">The unique identifier of the service category</param>
    /// <returns>The service category with its sub-services</returns>
    /// <response code="200">Returns the service category</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the service category is not found</response>
    [HttpGet("categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceCategory(Guid categoryId)
    {
        var result = await _serviceCatalogService.GetServiceCategoryByIdAsync(categoryId);
        
        if (!result.Success)
        {
            return BadRequest(new { errors = result.Errors });
        }

        if (result.Data == null)
        {
            return NotFound(new { message = "Service category not found" });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Gets all sub-services for a specific category
    /// </summary>
    /// <param name="categoryId">The unique identifier of the service category</param>
    /// <returns>List of sub-services for the specified category</returns>
    /// <response code="200">Returns the list of sub-services</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("categories/{categoryId:guid}/subservices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSubServicesByCategory(Guid categoryId)
    {
        var result = await _serviceCatalogService.GetSubServicesByCategoryAsync(categoryId);
        
        if (!result.Success)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}