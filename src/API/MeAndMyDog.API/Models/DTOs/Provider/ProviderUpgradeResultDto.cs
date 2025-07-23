namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for upgrade result
/// </summary>
public class ProviderUpgradeResultDto
{
    /// <summary>
    /// Whether upgrade was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Result message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Provider ID if successful
    /// </summary>
    public string? ProviderId { get; set; }
    
    /// <summary>
    /// Next steps for user
    /// </summary>
    public List<string> NextSteps { get; set; } = new();
    
    /// <summary>
    /// Upgrade status
    /// </summary>
    public ProviderUpgradeStatus Status { get; set; }
}