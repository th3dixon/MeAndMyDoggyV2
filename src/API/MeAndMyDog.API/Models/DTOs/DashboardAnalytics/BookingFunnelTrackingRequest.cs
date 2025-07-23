using System.Collections.Generic;

namespace MeAndMyDog.API.Models.DTOs.DashboardAnalytics;

/// <summary>
/// Booking funnel tracking request
/// </summary>
public class BookingFunnelTrackingRequest
{
    public string ProviderId { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}