using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs.Provider;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for handling provider upgrade operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProviderUpgradeController : ControllerBase
{
    private readonly IProviderUpgradeService _providerUpgradeService;
    private readonly ILogger<ProviderUpgradeController> _logger;

    public ProviderUpgradeController(
        IProviderUpgradeService providerUpgradeService,
        ILogger<ProviderUpgradeController> logger)
    {
        _providerUpgradeService = providerUpgradeService;
        _logger = logger;
    }

    /// <summary>
    /// Get upgrade promotion data for dashboard display
    /// </summary>
    /// <returns>Promotion details</returns>
    [HttpGet("promotion")]
    [ProducesResponseType(typeof(ProviderUpgradePromotionDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUpgradePromotion()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var promotion = await _providerUpgradeService.GetUpgradePromotionAsync(userId);
            return Ok(promotion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade promotion");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if user is eligible for provider upgrade
    /// </summary>
    /// <returns>Eligibility status</returns>
    [HttpGet("eligibility")]
    [ProducesResponseType(typeof(ProviderUpgradeEligibilityDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CheckEligibility()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var eligibility = await _providerUpgradeService.CheckUpgradeEligibilityAsync(userId);
            return Ok(eligibility);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking upgrade eligibility");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Process provider upgrade request
    /// </summary>
    /// <param name="upgradeRequest">Upgrade request details</param>
    /// <returns>Upgrade result</returns>
    [HttpPost("upgrade")]
    [ProducesResponseType(typeof(ProviderUpgradeResultDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ProcessUpgrade([FromBody] ProviderUpgradeRequestDto upgradeRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _providerUpgradeService.ProcessProviderUpgradeAsync(userId, upgradeRequest);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing provider upgrade");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get current upgrade progress
    /// </summary>
    /// <returns>Upgrade progress</returns>
    [HttpGet("progress")]
    [ProducesResponseType(typeof(ProviderUpgradeProgressDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetProgress()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var progress = await _providerUpgradeService.GetUpgradeProgressAsync(userId);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade progress");
            return StatusCode(500, "Internal server error");
        }
    }
}