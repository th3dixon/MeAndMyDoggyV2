namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Deep link processing result
/// </summary>
public class DeepLinkResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string TargetScreen { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool RequiresAuthentication { get; set; }
    public string? RedirectUrl { get; set; }
}