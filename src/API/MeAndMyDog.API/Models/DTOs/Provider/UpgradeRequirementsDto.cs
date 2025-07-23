namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for upgrade requirements status
/// </summary>
public class UpgradeRequirementsDto
{
    /// <summary>
    /// Email is verified
    /// </summary>
    public bool EmailVerified { get; set; }
    
    /// <summary>
    /// Profile is complete
    /// </summary>
    public bool ProfileComplete { get; set; }
    
    /// <summary>
    /// Account age meets minimum (7 days)
    /// </summary>
    public bool MinimumAccountAge { get; set; }
    
    /// <summary>
    /// No recent violations
    /// </summary>
    public bool NoViolations { get; set; }
}