using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Proxy controller for friends functionality, forwarding requests to the API backend
/// </summary>
[Authorize]
[Route("api/v1/[controller]")]
public class FriendsProxyController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FriendsProxyController> _logger;

    /// <summary>
    /// Initializes a new instance of the FriendsProxyController
    /// </summary>
    public FriendsProxyController(IHttpClientFactory httpClientFactory, ILogger<FriendsProxyController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("API");
        _logger = logger;
    }

    /// <summary>
    /// Get current user's friend code
    /// </summary>
    /// <returns>User's friend code</returns>
    [HttpGet("my-friend-code")]
    public async Task<IActionResult> GetMyFriendCode()
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var response = await _httpClient.GetAsync("api/v1/friends/my-friend-code");
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friend code");
            return StatusCode(500, new { error = "Failed to get friend code" });
        }
    }

    /// <summary>
    /// Regenerate current user's friend code
    /// </summary>
    /// <returns>New friend code</returns>
    [HttpPost("regenerate-friend-code")]
    public async Task<IActionResult> RegenerateFriendCode()
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var response = await _httpClient.PostAsync("api/v1/friends/regenerate-friend-code", null);
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating friend code");
            return StatusCode(500, new { error = "Failed to regenerate friend code" });
        }
    }

    /// <summary>
    /// Look up a user by friend code
    /// </summary>
    /// <param name="friendCode">Friend code to lookup</param>
    /// <returns>Friend code lookup result</returns>
    [HttpGet("lookup/{friendCode}")]
    public async Task<IActionResult> LookupByFriendCode(string friendCode)
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var response = await _httpClient.GetAsync($"api/v1/friends/lookup/{Uri.EscapeDataString(friendCode)}");
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up friend code {FriendCode}", friendCode);
            return StatusCode(500, new { error = "Failed to lookup friend code" });
        }
    }

    /// <summary>
    /// Send a friend request using friend code
    /// </summary>
    /// <param name="request">Friend request details</param>
    /// <returns>Result of friend request</returns>
    [HttpPost("send-request")]
    public async Task<IActionResult> SendFriendRequest([FromBody] object request)
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/friends/send-request", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending friend request");
            return StatusCode(500, new { error = "Failed to send friend request" });
        }
    }

    /// <summary>
    /// Get list of friends and pending requests
    /// </summary>
    /// <returns>Friends list</returns>
    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var response = await _httpClient.GetAsync("api/v1/friends");
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends list");
            return StatusCode(500, new { error = "Failed to get friends list" });
        }
    }

    /// <summary>
    /// Respond to a friend request (accept or reject)
    /// </summary>
    /// <param name="response">Friend request response</param>
    /// <returns>Result of response</returns>
    [HttpPost("respond")]
    public async Task<IActionResult> RespondToFriendRequest([FromBody] object response)
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var json = JsonSerializer.Serialize(response);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiResponse = await _httpClient.PostAsync("api/v1/friends/respond", content);
            var responseContent = await apiResponse.Content.ReadAsStringAsync();

            return StatusCode((int)apiResponse.StatusCode, responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to friend request");
            return StatusCode(500, new { error = "Failed to respond to friend request" });
        }
    }

    /// <summary>
    /// Remove/unfriend a friend
    /// </summary>
    /// <param name="friendshipId">ID of friendship to remove</param>
    /// <returns>Result of unfriend action</returns>
    [HttpDelete("{friendshipId}")]
    public async Task<IActionResult> Unfriend(string friendshipId)
    {
        try
        {
            var jwtToken = User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            }

            var response = await _httpClient.DeleteAsync($"api/v1/friends/{Uri.EscapeDataString(friendshipId)}");
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfriending user {FriendshipId}", friendshipId);
            return StatusCode(500, new { error = "Failed to unfriend user" });
        }
    }
}