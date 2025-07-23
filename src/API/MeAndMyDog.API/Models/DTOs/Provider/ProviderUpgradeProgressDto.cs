namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for upgrade progress
/// </summary>
public class ProviderUpgradeProgressDto
{
    /// <summary>
    /// Current upgrade status
    /// </summary>
    public ProviderUpgradeStatus Status { get; set; }
    
    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; }
    
    /// <summary>
    /// Current step description
    /// </summary>
    public string CurrentStep { get; set; } = string.Empty;
    
    /// <summary>
    /// Steps completed
    /// </summary>
    public List<string> CompletedSteps { get; set; } = new();
    
    /// <summary>
    /// Remaining steps
    /// </summary>
    public List<string> RemainingSteps { get; set; } = new();
    
    /// <summary>
    /// Estimated completion time
    /// </summary>
    public string? EstimatedCompletion { get; set; }
}