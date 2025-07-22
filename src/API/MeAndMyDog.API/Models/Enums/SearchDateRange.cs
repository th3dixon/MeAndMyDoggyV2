namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Predefined date ranges for message search
/// </summary>
public enum SearchDateRange
{
    /// <summary>
    /// All time (no date filter)
    /// </summary>
    AllTime = 0,
    
    /// <summary>
    /// Today only
    /// </summary>
    Today = 1,
    
    /// <summary>
    /// Yesterday only
    /// </summary>
    Yesterday = 2,
    
    /// <summary>
    /// Last 7 days
    /// </summary>
    LastWeek = 3,
    
    /// <summary>
    /// Last 30 days
    /// </summary>
    LastMonth = 4,
    
    /// <summary>
    /// Last 90 days
    /// </summary>
    Last3Months = 5,
    
    /// <summary>
    /// Last 365 days
    /// </summary>
    LastYear = 6,
    
    /// <summary>
    /// Custom date range (use DateFrom/DateTo)
    /// </summary>
    Custom = 7
}