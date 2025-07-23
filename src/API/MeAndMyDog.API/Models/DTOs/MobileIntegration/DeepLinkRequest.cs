namespace MeAndMyDog.API.Models.DTOs.MobileIntegration;

/// <summary>
/// Deep link processing request
/// </summary>
public class DeepLinkRequest
{
    public string DeepLink { get; set; } = string.Empty;
}