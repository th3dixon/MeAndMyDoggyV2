namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Provider business information
/// </summary>
public class ProviderInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsPremium { get; set; }
}