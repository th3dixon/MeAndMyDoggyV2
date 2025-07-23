namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Provider upgrade status enumeration
/// </summary>
public enum ProviderUpgradeStatus
{
    /// <summary>
    /// Not started
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// Initial request submitted
    /// </summary>
    Initiated,
    
    /// <summary>
    /// Business details being collected
    /// </summary>
    CollectingDetails,
    
    /// <summary>
    /// Under review
    /// </summary>
    UnderReview,
    
    /// <summary>
    /// Approved and active
    /// </summary>
    Approved,
    
    /// <summary>
    /// Rejected
    /// </summary>
    Rejected,
    
    /// <summary>
    /// Suspended
    /// </summary>
    Suspended
}