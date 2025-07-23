using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs.DogBreeds;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// API controller for managing dog breed information and autocomplete functionality
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DogBreedsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DogBreedsController> _logger;

    public DogBreedsController(ApplicationDbContext context, ILogger<DogBreedsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Search dog breeds for autocomplete functionality
    /// </summary>
    /// <param name="query">Search query for breed names</param>
    /// <param name="limit">Maximum number of results to return (default: 10)</param>
    /// <returns>List of matching dog breeds</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DogBreedDto>>> SearchBreeds([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                // Return most common breeds if no query or query too short
                var commonBreeds = await _context.DogBreeds
                    .Where(b => b.IsCommon == true)
                    .OrderBy(b => b.Name)
                    .Take(limit)
                    .Select(b => new DogBreedDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        SizeCategory = b.SizeCategory
                    })
                    .ToListAsync();

                return Ok(commonBreeds);
            }

            var searchQuery = query.ToLower().Trim();

            // Search in breed names and alternative names
            var breeds = await _context.DogBreeds
                .Where(b => 
                    b.Name.ToLower().Contains(searchQuery) ||
                    (b.AlternativeNames != null && b.AlternativeNames.ToLower().Contains(searchQuery)))
                .OrderBy(b => b.IsCommon ? 0 : 1) // Common breeds first
                .ThenBy(b => b.Name.ToLower().StartsWith(searchQuery) ? 0 : 1) // Exact matches first
                .ThenBy(b => b.Name)
                .Take(limit)
                .Select(b => new DogBreedDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    SizeCategory = b.SizeCategory
                })
                .ToListAsync();

            return Ok(breeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching dog breeds with query: {Query}", query);
            return StatusCode(500, new { error = "Failed to search dog breeds" });
        }
    }

    /// <summary>
    /// Get all dog breeds grouped by size category
    /// </summary>
    /// <returns>Dog breeds grouped by size</returns>
    [HttpGet("by-size")]
    public async Task<ActionResult<IEnumerable<DogBreedBySizeDto>>> GetBreedsBySize()
    {
        try
        {
            var breedsBySize = await _context.DogBreeds
                .Where(b => b.IsCommon == true)
                .GroupBy(b => b.SizeCategory ?? "Unknown")
                .Select(g => new DogBreedBySizeDto
                {
                    SizeCategory = g.Key,
                    Breeds = g.Select(b => new DogBreedDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        SizeCategory = b.SizeCategory
                    }).OrderBy(b => b.Name).ToList()
                })
                .OrderBy(g => GetSizeOrder(g.SizeCategory))
                .ToListAsync();

            return Ok(breedsBySize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dog breeds by size");
            return StatusCode(500, new { error = "Failed to get dog breeds by size" });
        }
    }

    /// <summary>
    /// Get most popular/common dog breeds
    /// </summary>
    /// <param name="limit">Maximum number of results to return (default: 20)</param>
    /// <returns>List of most common dog breeds</returns>
    [HttpGet("popular")]
    public async Task<ActionResult<IEnumerable<DogBreedDto>>> GetPopularBreeds([FromQuery] int limit = 20)
    {
        try
        {
            var popularBreeds = await _context.DogBreeds
                .Where(b => b.IsCommon == true)
                .OrderBy(b => b.Name)
                .Take(limit)
                .Select(b => new DogBreedDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    SizeCategory = b.SizeCategory
                })
                .ToListAsync();

            return Ok(popularBreeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular dog breeds");
            return StatusCode(500, new { error = "Failed to get popular dog breeds" });
        }
    }

    /// <summary>
    /// Helper method to determine size category ordering
    /// </summary>
    /// <param name="sizeCategory">Size category name</param>
    /// <returns>Numeric order for sorting</returns>
    private static int GetSizeOrder(string sizeCategory)
    {
        return sizeCategory switch
        {
            "Small" => 1,
            "Medium" => 2,
            "Large" => 3,
            "Giant" => 4,
            _ => 5
        };
    }
}