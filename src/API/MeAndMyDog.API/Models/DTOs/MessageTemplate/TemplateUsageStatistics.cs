namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Template usage statistics
/// </summary>
public class TemplateUsageStatistics
{
    /// <summary>
    /// Template ID
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Total usage count
    /// </summary>
    public int TotalUsage { get; set; }

    /// <summary>
    /// Usage this month
    /// </summary>
    public int UsageThisMonth { get; set; }

    /// <summary>
    /// Usage this week
    /// </summary>
    public int UsageThisWeek { get; set; }

    /// <summary>
    /// Last used date
    /// </summary>
    public DateTimeOffset? LastUsed { get; set; }

    /// <summary>
    /// Most recent conversations where used
    /// </summary>
    public List<string> RecentConversations { get; set; } = new();
}