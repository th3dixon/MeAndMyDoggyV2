namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of message recurrence patterns
/// </summary>
public enum RecurrenceType
{
    /// <summary>
    /// No recurrence - one-time message
    /// </summary>
    None,

    /// <summary>
    /// Daily recurrence
    /// </summary>
    Daily,

    /// <summary>
    /// Weekly recurrence
    /// </summary>
    Weekly,

    /// <summary>
    /// Monthly recurrence
    /// </summary>
    Monthly,

    /// <summary>
    /// Yearly recurrence
    /// </summary>
    Yearly,

    /// <summary>
    /// Custom interval recurrence
    /// </summary>
    Custom
}