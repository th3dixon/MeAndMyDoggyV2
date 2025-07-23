using System;
using System.Collections.Generic;

namespace MeAndMyDog.API.Models.DTOs.DashboardAnalytics;

/// <summary>
/// Session tracking request
/// </summary>
public class SessionTrackingRequest
{
    public TimeSpan Duration { get; set; }
    public Dictionary<string, int> Actions { get; set; } = new();
}