namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Appointment sort options
/// </summary>
public enum AppointmentSortBy
{
    /// <summary>
    /// Sort by start time
    /// </summary>
    StartTime = 0,
    
    /// <summary>
    /// Sort by end time
    /// </summary>
    EndTime = 1,
    
    /// <summary>
    /// Sort by creation date
    /// </summary>
    CreatedAt = 2,
    
    /// <summary>
    /// Sort by last update
    /// </summary>
    UpdatedAt = 3,
    
    /// <summary>
    /// Sort by title
    /// </summary>
    Title = 4,
    
    /// <summary>
    /// Sort by appointment type
    /// </summary>
    AppointmentType = 5,
    
    /// <summary>
    /// Sort by status
    /// </summary>
    Status = 6,
    
    /// <summary>
    /// Sort by priority
    /// </summary>
    Priority = 7,
    
    /// <summary>
    /// Sort by duration
    /// </summary>
    Duration = 8,
    
    /// <summary>
    /// Sort by participant count
    /// </summary>
    ParticipantCount = 9
}