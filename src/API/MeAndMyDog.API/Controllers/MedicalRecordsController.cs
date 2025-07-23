using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs.MedicalRecords;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing pet medical records
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MedicalRecordsController> _logger;

    public MedicalRecordsController(ApplicationDbContext context, ILogger<MedicalRecordsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all medical records for a specific pet
    /// </summary>
    [HttpGet("pet/{petId}")]
    public async Task<IActionResult> GetMedicalRecordsByPet(string petId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Verify the pet belongs to the user
            var pet = await _context.DogProfiles
                .FirstOrDefaultAsync(d => d.Id == petId && d.OwnerId == userId);

            if (pet == null)
            {
                return NotFound("Pet not found or access denied");
            }

            var medicalRecords = await _context.MedicalRecords
                .Where(mr => mr.DogId == petId && mr.IsActive)
                .OrderByDescending(mr => mr.EventDate)
                .Select(mr => new
                {
                    mr.Id,
                    mr.RecordType,
                    mr.Title,
                    mr.Description,
                    mr.EventDate,
                    mr.VeterinarianName,
                    mr.ClinicName,
                    mr.Cost,
                    mr.Medications,
                    mr.FollowUpInstructions,
                    mr.NextAppointmentDate,
                    mr.CreatedAt,
                    mr.UpdatedAt
                })
                .ToListAsync();

            return Ok(medicalRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching medical records for pet {PetId}", petId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get a specific medical record
    /// </summary>
    [HttpGet("{recordId}")]
    public async Task<IActionResult> GetMedicalRecord(string recordId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Dog)
                .Where(mr => mr.Id == recordId && mr.Dog.OwnerId == userId)
                .Select(mr => new
                {
                    mr.Id,
                    mr.DogId,
                    mr.RecordType,
                    mr.Title,
                    mr.Description,
                    mr.EventDate,
                    mr.VeterinarianName,
                    mr.ClinicName,
                    mr.Cost,
                    mr.Medications,
                    mr.FollowUpInstructions,
                    mr.NextAppointmentDate,
                    mr.Attachments,
                    mr.CreatedAt,
                    mr.UpdatedAt,
                    PetName = mr.Dog.Name
                })
                .FirstOrDefaultAsync();

            if (medicalRecord == null)
            {
                return NotFound("Medical record not found");
            }

            return Ok(medicalRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching medical record {RecordId}", recordId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new medical record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Verify the pet belongs to the user
            var pet = await _context.DogProfiles
                .FirstOrDefaultAsync(d => d.Id == dto.DogId && d.OwnerId == userId);

            if (pet == null)
            {
                return BadRequest("Pet not found or access denied");
            }

            var medicalRecord = new MedicalRecord
            {
                DogId = dto.DogId,
                RecordType = dto.RecordType,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                VeterinarianName = dto.VeterinarianName,
                ClinicName = dto.ClinicName,
                Cost = dto.Cost,
                Medications = dto.Medications,
                FollowUpInstructions = dto.FollowUpInstructions,
                NextAppointmentDate = dto.NextAppointmentDate,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Medical record created successfully for pet {PetId} by user {UserId}", 
                dto.DogId, userId);

            return CreatedAtAction(nameof(GetMedicalRecord), new { recordId = medicalRecord.Id }, 
                new { 
                    id = medicalRecord.Id,
                    message = "Medical record created successfully" 
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medical record for pet {PetId}", dto.DogId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing medical record
    /// </summary>
    [HttpPut("{recordId}")]
    public async Task<IActionResult> UpdateMedicalRecord(string recordId, [FromBody] UpdateMedicalRecordDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Dog)
                .FirstOrDefaultAsync(mr => mr.Id == recordId && mr.Dog.OwnerId == userId);

            if (medicalRecord == null)
            {
                return NotFound("Medical record not found");
            }

            // Update fields
            medicalRecord.RecordType = dto.RecordType ?? medicalRecord.RecordType;
            medicalRecord.Title = dto.Title ?? medicalRecord.Title;
            medicalRecord.Description = dto.Description ?? medicalRecord.Description;
            medicalRecord.EventDate = dto.EventDate ?? medicalRecord.EventDate;
            medicalRecord.VeterinarianName = dto.VeterinarianName ?? medicalRecord.VeterinarianName;
            medicalRecord.ClinicName = dto.ClinicName ?? medicalRecord.ClinicName;
            medicalRecord.Cost = dto.Cost ?? medicalRecord.Cost;
            medicalRecord.Medications = dto.Medications ?? medicalRecord.Medications;
            medicalRecord.FollowUpInstructions = dto.FollowUpInstructions ?? medicalRecord.FollowUpInstructions;
            medicalRecord.NextAppointmentDate = dto.NextAppointmentDate ?? medicalRecord.NextAppointmentDate;
            medicalRecord.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Medical record {RecordId} updated successfully by user {UserId}", 
                recordId, userId);

            return Ok(new { message = "Medical record updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medical record {RecordId}", recordId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a medical record (soft delete)
    /// </summary>
    [HttpDelete("{recordId}")]
    public async Task<IActionResult> DeleteMedicalRecord(string recordId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Dog)
                .FirstOrDefaultAsync(mr => mr.Id == recordId && mr.Dog.OwnerId == userId);

            if (medicalRecord == null)
            {
                return NotFound("Medical record not found");
            }

            medicalRecord.IsActive = false;
            medicalRecord.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Medical record {RecordId} deleted successfully by user {UserId}", 
                recordId, userId);

            return Ok(new { message = "Medical record deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medical record {RecordId}", recordId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get health summary for all user's pets
    /// </summary>
    [HttpGet("health-summary")]
    public async Task<IActionResult> GetHealthSummary()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var healthSummary = await _context.DogProfiles
                .Where(d => d.OwnerId == userId && d.IsActive)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Breed,
                    d.DateOfBirth,
                    d.ProfileImageUrl,
                    Age = CalculateAge(d.DateOfBirth),
                    TotalRecords = d.MedicalRecords.Count(mr => mr.IsActive),
                    LastRecord = d.MedicalRecords
                        .Where(mr => mr.IsActive)
                        .OrderByDescending(mr => mr.EventDate)
                        .Select(mr => new { mr.Title, mr.EventDate, mr.RecordType })
                        .FirstOrDefault(),
                    VaccinationStatus = GetVaccinationStatus(d.MedicalRecords),
                    HealthStatus = CalculateHealthStatus(d.MedicalRecords, d.DateOfBirth)
                })
                .ToListAsync();

            return Ok(healthSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching health summary for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "Internal server error");
        }
    }

    #region Helper Methods

    private static string CalculateAge(DateTimeOffset? dateOfBirth)
    {
        if (!dateOfBirth.HasValue) return "Unknown";
        
        var age = DateTime.UtcNow - dateOfBirth.Value.DateTime;
        var years = (int)(age.TotalDays / 365.25);
        var months = (int)((age.TotalDays % 365.25) / 30.44);
        
        if (years > 0)
            return $"{years} year{(years != 1 ? "s" : "")}";
        else if (months > 0)
            return $"{months} month{(months != 1 ? "s" : "")}";
        else
            return "Puppy";
    }

    private static string GetVaccinationStatus(ICollection<MedicalRecord> medicalRecords)
    {
        var lastVaccination = medicalRecords
            .Where(mr => mr.IsActive && mr.RecordType.ToLower().Contains("vaccination"))
            .OrderByDescending(mr => mr.EventDate)
            .FirstOrDefault();

        if (lastVaccination == null)
            return "Unknown";

        var daysSinceLastVaccination = (DateTime.UtcNow - lastVaccination.EventDate.DateTime).TotalDays;
        
        return daysSinceLastVaccination switch
        {
            < 365 => "Up to date",
            < 400 => "Due soon",
            _ => "Overdue"
        };
    }

    private static string CalculateHealthStatus(ICollection<MedicalRecord> medicalRecords, DateTimeOffset? dateOfBirth)
    {
        if (medicalRecords == null || !medicalRecords.Any())
            return "Unknown";

        var now = DateTime.UtcNow;
        var recentRecords = medicalRecords.Where(mr => mr.IsActive && mr.EventDate >= now.AddMonths(-6)).ToList();
        
        // Check for recent serious conditions
        var seriousConditions = recentRecords.Where(mr => 
            mr.Title.ToLower().Contains("emergency") || 
            mr.RecordType.ToLower().Contains("emergency") ||
            mr.RecordType.ToLower().Contains("surgery")).Any();
        
        if (seriousConditions)
            return "Needs Attention";

        // Check vaccination status
        var lastVaccination = medicalRecords
            .Where(mr => mr.IsActive && mr.RecordType.ToLower().Contains("vaccination"))
            .OrderByDescending(mr => mr.EventDate)
            .FirstOrDefault();

        var lastCheckup = medicalRecords
            .Where(mr => mr.IsActive && mr.RecordType.ToLower().Contains("checkup"))
            .OrderByDescending(mr => mr.EventDate)
            .FirstOrDefault();

        var hasRecentVaccination = lastVaccination != null && 
            (now - lastVaccination.EventDate.DateTime).TotalDays < 365;
        
        var hasRecentCheckup = lastCheckup != null && 
            (now - lastCheckup.EventDate.DateTime).TotalDays < 365;

        if (hasRecentVaccination && hasRecentCheckup)
            return "Excellent";
        else if (hasRecentVaccination || hasRecentCheckup)
            return "Good";
        else
            return "Fair";
    }

    #endregion
}