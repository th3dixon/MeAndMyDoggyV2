namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Severity levels for security incidents
/// </summary>
public enum IncidentSeverity
{
    /// <summary>
    /// Low severity - minimal impact
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Medium severity - moderate impact
    /// </summary>
    Medium = 1,
    
    /// <summary>
    /// High severity - significant impact
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Critical severity - severe impact
    /// </summary>
    Critical = 3,
    
    /// <summary>
    /// Emergency - immediate action required
    /// </summary>
    Emergency = 4
}