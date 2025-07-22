namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Client information captured during incident
/// </summary>
public class ClientInformationDto
{
    /// <summary>
    /// IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Device fingerprint
    /// </summary>
    public string? DeviceFingerprint { get; set; }

    /// <summary>
    /// Geographic location
    /// </summary>
    public string? GeographicLocation { get; set; }

    /// <summary>
    /// Browser information
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// Operating system
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// Screen resolution
    /// </summary>
    public string? ScreenResolution { get; set; }

    /// <summary>
    /// Time zone offset
    /// </summary>
    public int? TimezoneOffset { get; set; }
}