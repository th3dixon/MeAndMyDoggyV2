using MeAndMyDog.API.Models.DTOs.Provider;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for upgrading pet owners to service providers
/// </summary>
public interface IProviderUpgradeService
{
    /// <summary>
    /// Get upgrade promotion data for dashboard display
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Promotion details</returns>
    Task<ProviderUpgradePromotionDto> GetUpgradePromotionAsync(string userId);
    
    /// <summary>
    /// Check if user can upgrade to service provider
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Eligibility status</returns>
    Task<ProviderUpgradeEligibilityDto> CheckUpgradeEligibilityAsync(string userId);
    
    /// <summary>
    /// Process provider upgrade - same as registration wizard
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="upgradeRequest">Upgrade request with business and service details</param>
    /// <returns>Upgrade result</returns>
    Task<ProviderUpgradeResultDto> ProcessProviderUpgradeAsync(string userId, ProviderUpgradeRequestDto upgradeRequest);
    
    /// <summary>
    /// Get upgrade progress for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Upgrade progress</returns>
    Task<ProviderUpgradeProgressDto> GetUpgradeProgressAsync(string userId);
}