using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.PetCareReminders;

/// <summary>
/// Request model for completing reminders
/// </summary>
public class CompleteReminderRequest
{
    /// <summary>
    /// Notes about completion
    /// </summary>
    [StringLength(1000, ErrorMessage = "Completion notes cannot exceed 1000 characters")]
    public string? CompletionNotes { get; set; }
}