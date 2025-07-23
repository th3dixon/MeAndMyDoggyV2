namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for checking provider upgrade eligibility
/// </summary>
public class ProviderUpgradeEligibilityDto
{
    /// <summary>
    /// Whether user is eligible for upgrade
    /// </summary>
    public bool IsEligible { get; set; }
    
    /// <summary>
    /// Reason if not eligible
    /// </summary>
    public string? IneligibilityReason { get; set; }
    
    /// <summary>
    /// Whether user already has provider role
    /// </summary>
    public bool IsAlreadyProvider { get; set; }
    
    /// <summary>
    /// Account age in days
    /// </summary>
    public int AccountAgeDays { get; set; }
    
    /// <summary>
    /// Minimum requirements met
    /// </summary>
    public UpgradeRequirementsDto Requirements { get; set; } = new();
}