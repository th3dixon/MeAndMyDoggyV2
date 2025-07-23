namespace MeAndMyDog.API.Models.DTOs.MedicalRecords;

/// <summary>
/// DTO for updating an existing medical record
/// </summary>
public class UpdateMedicalRecordDto
{
    public string? RecordType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? EventDate { get; set; }
    public string? VeterinarianName { get; set; }
    public string? ClinicName { get; set; }
    public decimal? Cost { get; set; }
    public string? Medications { get; set; }
    public string? FollowUpInstructions { get; set; }
    public DateTimeOffset? NextAppointmentDate { get; set; }
}