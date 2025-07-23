namespace MeAndMyDog.API.Models.DTOs.MedicalRecords;

/// <summary>
/// DTO for creating a new medical record
/// </summary>
public class CreateMedicalRecordDto
{
    public string DogId { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public string? VeterinarianName { get; set; }
    public string? ClinicName { get; set; }
    public decimal? Cost { get; set; }
    public string? Medications { get; set; }
    public string? FollowUpInstructions { get; set; }
    public DateTimeOffset? NextAppointmentDate { get; set; }
}