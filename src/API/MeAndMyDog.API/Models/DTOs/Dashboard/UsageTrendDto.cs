namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Usage trend data point
/// </summary>
public class UsageTrendDto
{
    public DateTime Date { get; set; }
    public int Sessions { get; set; }
    public int Actions { get; set; }
    public TimeSpan Duration { get; set; }
}