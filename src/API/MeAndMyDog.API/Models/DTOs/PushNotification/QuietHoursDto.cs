using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for quiet hours configuration
/// </summary>
public class QuietHoursDto
{
    /// <summary>
    /// Quiet hours start time (24-hour format)
    /// </summary>
    public string? Start { get; set; }

    /// <summary>
    /// Quiet hours end time (24-hour format)
    /// </summary>
    public string? End { get; set; }

    /// <summary>
    /// Time zone for quiet hours
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Days of week when quiet hours apply
    /// </summary>
    public List<string>? Days { get; set; }
}