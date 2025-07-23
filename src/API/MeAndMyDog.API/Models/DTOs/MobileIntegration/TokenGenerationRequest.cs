namespace MeAndMyDog.API.Models.DTOs.MobileIntegration;

/// <summary>
/// Token generation request
/// </summary>
public class TokenGenerationRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public int ExpiryDays { get; set; } = 30;
}