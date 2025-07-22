namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Scheduled message statistics
/// </summary>
public class ScheduledMessageStatistics
{
    /// <summary>
    /// Total scheduled messages
    /// </summary>
    public int TotalScheduled { get; set; }

    /// <summary>
    /// Messages sent successfully
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Messages pending
    /// </summary>
    public int TotalPending { get; set; }

    /// <summary>
    /// Messages failed
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Messages cancelled
    /// </summary>
    public int TotalCancelled { get; set; }

    /// <summary>
    /// Recurring messages
    /// </summary>
    public int TotalRecurring { get; set; }

    /// <summary>
    /// Success rate percentage
    /// </summary>
    public decimal SuccessRate { get; set; }

    /// <summary>
    /// Most used templates
    /// </summary>
    public List<TemplateUsageDto> MostUsedTemplates { get; set; } = new();

    /// <summary>
    /// Daily statistics
    /// </summary>
    public List<DailyScheduledMessageStats> DailyStats { get; set; } = new();
}