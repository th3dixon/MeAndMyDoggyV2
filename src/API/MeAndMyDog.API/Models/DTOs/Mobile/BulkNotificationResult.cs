namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Bulk notification result
/// </summary>
public class BulkNotificationResult
{
    public bool Success { get; set; }
    public int TotalTargetUsers { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}