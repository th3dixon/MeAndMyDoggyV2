namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Recent client message information
/// </summary>
public class RecentMessageDto
{
    public string Id { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientAvatar { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public bool Unread { get; set; }
}