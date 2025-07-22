using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// Address Lookup Proxy Controller - forwards address lookup requests to the backend API server
    /// </summary>
    [ApiController]
    [Route("api/v1/addresslookup")]
    public class AddressLookupProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AddressLookupProxyController> _logger;

        public AddressLookupProxyController(IHttpClientFactory httpClientFactory, ILogger<AddressLookupProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Proxy address search requests to the API server
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int max = 10)
        {
            return await ProxyGetRequest($"AddressLookup/search?q={Uri.EscapeDataString(q)}&max={max}");
        }

        private async Task<IActionResult> ProxyGetRequest(string endpoint)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";

                _logger.LogInformation("Proxying GET request to: {TargetUrl}", targetUrl);

                // Copy authentication headers if present
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
                }

                // Send the request
                var response = await httpClient.GetAsync(targetUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying request to API server");
                return StatusCode(503, new { message = "Unable to connect to address lookup service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying address lookup request to API");
                return StatusCode(500, new { message = "Internal server error while processing address lookup request" });
            }
        }
    }
}