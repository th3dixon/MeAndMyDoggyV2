using System.Collections.Generic;

namespace MeAndMyDog.API.Models.DTOs.DashboardAnalytics;

/// <summary>
/// Widget tracking request
/// </summary>
public class WidgetTrackingRequest
{
    public string WidgetType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; set; }
}