namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile quick stats
/// </summary>
public class MobileQuickStatsDto
{
    public int PetCount { get; set; }
    public int UpcomingServices { get; set; }
    public int UnreadNotifications { get; set; }
    public string NextAppointment { get; set; } = string.Empty;
    public decimal MonthlySpending { get; set; }
}