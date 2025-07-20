using MeAndMyDog.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddressLookupController : ControllerBase
    {
        private readonly IAddressLookupService _addressLookupService;
        private readonly ILogger<AddressLookupController> _logger;

        public AddressLookupController(
            IAddressLookupService addressLookupService,
            ILogger<AddressLookupController> logger)
        {
            _addressLookupService = addressLookupService;
            _logger = logger;
        }

        /// <summary>
        /// Search for addresses with autocomplete functionality
        /// </summary>
        /// <param name="q">Search term (minimum 3 characters)</param>
        /// <param name="max">Maximum number of results (default 20)</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchAddresses([FromQuery] string q, [FromQuery] int max = 20)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return BadRequest(new { message = "Search term must be at least 3 characters" });
            }

            var result = await _addressLookupService.SearchAddressesAsync(q, max);
            
            if (!result.Success)
            {
                return BadRequest(new { message = string.Join(", ", result.Errors) });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Lookup detailed information for a specific postcode
        /// </summary>
        /// <param name="postcode">UK postcode to lookup</param>
        [HttpGet("postcode/{postcode}")]
        public async Task<IActionResult> LookupPostcode(string postcode)
        {
            var result = await _addressLookupService.LookupPostcodeAsync(postcode);
            
            if (!result.Success)
            {
                return NotFound(new { message = string.Join(", ", result.Errors) });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get full address details by address ID
        /// </summary>
        /// <param name="id">Address ID</param>
        [HttpGet("address/{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var result = await _addressLookupService.GetAddressByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(new { message = string.Join(", ", result.Errors) });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Search for addresses near a specific location
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lng">Longitude</param>
        /// <param name="radius">Search radius in miles (default 5)</param>
        /// <param name="q">Optional search term</param>
        /// <param name="max">Maximum number of results (default 50)</param>
        [HttpGet("nearby")]
        public async Task<IActionResult> SearchNearbyAddresses(
            [FromQuery] decimal lat, 
            [FromQuery] decimal lng, 
            [FromQuery] decimal radius = 5,
            [FromQuery] string? q = null,
            [FromQuery] int max = 50)
        {
            var result = await _addressLookupService.SearchAddressesNearLocationAsync(lat, lng, radius, q, max);
            
            if (!result.Success)
            {
                return BadRequest(new { message = string.Join(", ", result.Errors) });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Search for cities by name
        /// </summary>
        /// <param name="q">Search term</param>
        /// <param name="countyId">Optional county ID to filter by</param>
        /// <param name="max">Maximum number of results (default 20)</param>
        [HttpGet("cities")]
        public async Task<IActionResult> SearchCities(
            [FromQuery] string q, 
            [FromQuery] int? countyId = null,
            [FromQuery] int max = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var result = await _addressLookupService.SearchCitiesAsync(q, countyId, max);
            
            if (!result.Success)
            {
                return BadRequest(new { message = string.Join(", ", result.Errors) });
            }

            return Ok(result.Data);
        }
    }
}