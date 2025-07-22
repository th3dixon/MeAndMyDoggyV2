namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of security incidents
/// </summary>
public enum IncidentStatus
{
    /// <summary>
    /// Incident reported but not yet triaged
    /// </summary>
    New = 0,
    
    /// <summary>
    /// Incident acknowledged and assigned
    /// </summary>
    Assigned = 1,
    
    /// <summary>
    /// Investigation in progress
    /// </summary>
    InProgress = 2,
    
    /// <summary>
    /// Incident on hold pending additional information
    /// </summary>
    OnHold = 3,
    
    /// <summary>
    /// Incident resolved
    /// </summary>
    Resolved = 4,
    
    /// <summary>
    /// Incident closed
    /// </summary>
    Closed = 5,
    
    /// <summary>
    /// False positive - no actual incident
    /// </summary>
    FalsePositive = 6,
    
    /// <summary>
    /// Incident escalated to higher authority
    /// </summary>
    Escalated = 7,
    
    /// <summary>
    /// Incident requires external assistance
    /// </summary>
    External = 8
}