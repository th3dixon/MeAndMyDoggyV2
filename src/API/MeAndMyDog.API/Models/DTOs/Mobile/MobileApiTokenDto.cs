namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile API token
/// </summary>
public class MobileApiTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public List<string> Scopes { get; set; } = new();
}