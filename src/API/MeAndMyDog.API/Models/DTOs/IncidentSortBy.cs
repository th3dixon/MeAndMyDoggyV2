namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Incident sort options
/// </summary>
public enum IncidentSortBy
{
    /// <summary>
    /// Sort by detection date
    /// </summary>
    DetectedAt = 0,
    
    /// <summary>
    /// Sort by occurrence date
    /// </summary>
    OccurredAt = 1,
    
    /// <summary>
    /// Sort by severity
    /// </summary>
    Severity = 2,
    
    /// <summary>
    /// Sort by status
    /// </summary>
    Status = 3,
    
    /// <summary>
    /// Sort by risk score
    /// </summary>
    RiskScore = 4,
    
    /// <summary>
    /// Sort by incident type
    /// </summary>
    IncidentType = 5,
    
    /// <summary>
    /// Sort by resolution time
    /// </summary>
    ResolutionTime = 6
}