using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Appointment participant entity
/// </summary>
[Table("AppointmentParticipants")]
public class AppointmentParticipant
{
    /// <summary>
    /// Participant unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Appointment ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(Appointment))]
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Participant user ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Participant email address
    /// </summary>
    [Required]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Participant display name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Participant role in the appointment
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Participant response status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ResponseStatus { get; set; } = string.Empty;

    /// <summary>
    /// Whether this participant is required
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Whether this participant is the organizer
    /// </summary>
    public bool IsOrganizer { get; set; } = false;

    /// <summary>
    /// When the participant was invited
    /// </summary>
    public DateTimeOffset InvitedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the participant responded
    /// </summary>
    public DateTimeOffset? RespondedAt { get; set; }

    /// <summary>
    /// Participant's response message
    /// </summary>
    [MaxLength(500)]
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// External participant ID (for calendar sync)
    /// </summary>
    [MaxLength(500)]
    public string? ExternalParticipantId { get; set; }

    /// <summary>
    /// Participant's time zone
    /// </summary>
    [MaxLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Additional participant notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to appointment
    /// </summary>
    public virtual CalendarAppointment Appointment { get; set; } = null!;
}