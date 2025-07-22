namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to generate recurring instances
/// </summary>
public class GenerateRecurringInstancesRequest
{
    /// <summary>
    /// Recurring appointment ID
    /// </summary>
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Generate instances from this date
    /// </summary>
    public DateTimeOffset FromDate { get; set; }

    /// <summary>
    /// Generate instances until this date
    /// </summary>
    public DateTimeOffset ToDate { get; set; }

    /// <summary>
    /// Maximum number of instances to generate
    /// </summary>
    public int? MaxInstances { get; set; }

    /// <summary>
    /// Time zone for instance generation
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}