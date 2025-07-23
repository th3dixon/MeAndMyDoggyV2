using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs.Dogs;
using MeAndMyDog.API.Models;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing pet profiles and related operations
/// </summary>
[ApiController]
[Route("api/v1/pets")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PetsController> _logger;

    /// <summary>
    /// Initializes a new instance of PetsController
    /// </summary>
    public PetsController(ApplicationDbContext context, ILogger<PetsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all pets for the current user
    /// </summary>
    /// <returns>List of user's pets</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PetDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetPets()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var pets = await _context.DogProfiles
                .Include(p => p.Photos.Where(photo => photo.IsActive))
                .Include(p => p.MedicalRecords.Where(mr => mr.IsActive).OrderByDescending(mr => mr.EventDate).Take(5))
                .Include(p => p.CareReminders.Where(cr => cr.IsActive && !cr.IsCompleted).OrderBy(cr => cr.DueDate).Take(5))
                .Include(p => p.Medications.Where(m => m.IsActive && m.IsCurrentlyTaking))
                .Include(p => p.Vaccinations.Where(v => v.IsActive).OrderByDescending(v => v.DateAdministered).Take(5))
                .Where(p => p.OwnerId == userId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var petDtos = pets.Select(MapToPetDto).ToList();

            return Ok(petDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pets for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while retrieving pets");
        }
    }

    /// <summary>
    /// Get a specific pet by ID
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <returns>Pet details</returns>
    [HttpGet("{petId}")]
    [ProducesResponseType(typeof(PetDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPet(string petId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var pet = await _context.DogProfiles
                .Include(p => p.Photos.Where(photo => photo.IsActive).OrderBy(photo => photo.DisplayOrder))
                .Include(p => p.MedicalRecords.Where(mr => mr.IsActive).OrderByDescending(mr => mr.EventDate))
                .Include(p => p.CareReminders.Where(cr => cr.IsActive).OrderBy(cr => cr.DueDate))
                .Include(p => p.Medications.Where(m => m.IsActive).OrderByDescending(m => m.StartDate))
                .Include(p => p.Vaccinations.Where(v => v.IsActive).OrderByDescending(v => v.DateAdministered))
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (pet == null)
            {
                return NotFound("Pet not found");
            }

            var petDto = MapToPetDto(pet);

            return Ok(petDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pet {PetId}", petId);
            return StatusCode(500, "An error occurred while retrieving the pet");
        }
    }

    /// <summary>
    /// Create a new pet profile
    /// </summary>
    /// <param name="request">Pet creation details</param>
    /// <returns>Created pet</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PetDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var pet = new DogProfile
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = userId,
                Name = request.Name.Trim(),
                Breed = request.Breed?.Trim(),
                SecondaryBreed = request.SecondaryBreed?.Trim(),
                DateOfBirth = request.DateOfBirth,
                Weight = request.Weight,
                Height = request.Height,
                Gender = request.Gender?.Trim(),
                IsNeutered = request.IsNeutered,
                CoatColor = request.CoatColor?.Trim(),
                CoatType = request.CoatType?.Trim(),
                EyeColor = request.EyeColor?.Trim(),
                MicrochipNumber = request.MicrochipNumber?.Trim(),
                RegistrationNumber = request.RegistrationNumber?.Trim(),
                DietaryRequirements = request.DietaryRequirements?.Trim(),
                Allergies = request.Allergies?.Trim(),
                Temperament = request.Temperament?.Trim(),
                EnergyLevel = request.EnergyLevel,
                SocializationLevel = request.SocializationLevel,
                TrainingLevel = request.TrainingLevel,
                EmergencyContact = request.EmergencyContact?.Trim(),
                EmergencyContactPhone = request.EmergencyContactPhone?.Trim(),
                PreferredVet = request.PreferredVet?.Trim(),
                PreferredVetPhone = request.PreferredVetPhone?.Trim(),
                InsuranceProvider = request.InsuranceProvider?.Trim(),
                InsurancePolicyNumber = request.InsurancePolicyNumber?.Trim(),
                Notes = request.Notes?.Trim(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.DogProfiles.Add(pet);
            await _context.SaveChangesAsync();

            var petDto = MapToPetDto(pet);

            _logger.LogInformation("Pet profile created successfully: {PetId} for user {UserId}", pet.Id, userId);

            return CreatedAtAction(nameof(GetPet), new { petId = pet.Id }, petDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pet profile for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while creating the pet profile");
        }
    }

    /// <summary>
    /// Update an existing pet profile
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="request">Updated pet details</param>
    /// <returns>Updated pet</returns>
    [HttpPut("{petId}")]
    [ProducesResponseType(typeof(PetDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePet(string petId, [FromBody] UpdatePetRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var pet = await _context.DogProfiles
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (pet == null)
            {
                return NotFound("Pet not found");
            }

            // Update pet properties
            pet.Name = request.Name.Trim();
            pet.Breed = request.Breed?.Trim();
            pet.SecondaryBreed = request.SecondaryBreed?.Trim();
            pet.DateOfBirth = request.DateOfBirth;
            pet.Weight = request.Weight;
            pet.Height = request.Height;
            pet.Gender = request.Gender?.Trim();
            pet.IsNeutered = request.IsNeutered;
            pet.CoatColor = request.CoatColor?.Trim();
            pet.CoatType = request.CoatType?.Trim();
            pet.EyeColor = request.EyeColor?.Trim();
            pet.MicrochipNumber = request.MicrochipNumber?.Trim();
            pet.RegistrationNumber = request.RegistrationNumber?.Trim();
            pet.DietaryRequirements = request.DietaryRequirements?.Trim();
            pet.Allergies = request.Allergies?.Trim();
            pet.Temperament = request.Temperament?.Trim();
            pet.EnergyLevel = request.EnergyLevel;
            pet.SocializationLevel = request.SocializationLevel;
            pet.TrainingLevel = request.TrainingLevel;
            pet.EmergencyContact = request.EmergencyContact?.Trim();
            pet.EmergencyContactPhone = request.EmergencyContactPhone?.Trim();
            pet.PreferredVet = request.PreferredVet?.Trim();
            pet.PreferredVetPhone = request.PreferredVetPhone?.Trim();
            pet.InsuranceProvider = request.InsuranceProvider?.Trim();
            pet.InsurancePolicyNumber = request.InsurancePolicyNumber?.Trim();
            pet.Notes = request.Notes?.Trim();
            pet.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var petDto = MapToPetDto(pet);

            _logger.LogInformation("Pet profile updated successfully: {PetId}", petId);

            return Ok(petDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pet {PetId}", petId);
            return StatusCode(500, "An error occurred while updating the pet profile");
        }
    }

    /// <summary>
    /// Delete a pet profile (soft delete)
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{petId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeletePet(string petId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var pet = await _context.DogProfiles
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (pet == null)
            {
                return NotFound("Pet not found");
            }

            // Soft delete
            pet.IsActive = false;
            pet.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pet profile deleted successfully: {PetId}", petId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pet {PetId}", petId);
            return StatusCode(500, "An error occurred while deleting the pet profile");
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Maps a DogProfile entity to PetDto
    /// </summary>
    private static PetDto MapToPetDto(DogProfile pet)
    {
        var age = pet.DateOfBirth.HasValue 
            ? CalculateAge(pet.DateOfBirth.Value) 
            : (int?)null;

        return new PetDto
        {
            Id = pet.Id,
            OwnerId = pet.OwnerId,
            Name = pet.Name,
            Breed = pet.Breed,
            SecondaryBreed = pet.SecondaryBreed,
            DateOfBirth = pet.DateOfBirth,
            Age = age,
            Weight = pet.Weight,
            Height = pet.Height,
            Gender = pet.Gender,
            IsNeutered = pet.IsNeutered,
            CoatColor = pet.CoatColor,
            CoatType = pet.CoatType,
            EyeColor = pet.EyeColor,
            MicrochipNumber = pet.MicrochipNumber,
            RegistrationNumber = pet.RegistrationNumber,
            ProfileImageUrl = pet.ProfileImageUrl,
            DietaryRequirements = pet.DietaryRequirements,
            Allergies = pet.Allergies,
            Temperament = pet.Temperament,
            EnergyLevel = pet.EnergyLevel,
            SocializationLevel = pet.SocializationLevel,
            TrainingLevel = pet.TrainingLevel,
            EmergencyContact = pet.EmergencyContact,
            EmergencyContactPhone = pet.EmergencyContactPhone,
            PreferredVet = pet.PreferredVet,
            PreferredVetPhone = pet.PreferredVetPhone,
            InsuranceProvider = pet.InsuranceProvider,
            InsurancePolicyNumber = pet.InsurancePolicyNumber,
            Notes = pet.Notes,
            IsActive = pet.IsActive,
            CreatedAt = pet.CreatedAt,
            UpdatedAt = pet.UpdatedAt,
            Photos = pet.Photos?.Select(MapToPetPhotoDto).ToList() ?? new List<PetPhotoDto>(),
            RecentMedicalRecords = pet.MedicalRecords?.Select(MapToMedicalRecordSummaryDto).ToList() ?? new List<MedicalRecordSummaryDto>(),
            UpcomingReminders = pet.CareReminders?.Select(MapToPetCareReminderDto).ToList() ?? new List<PetCareReminderDto>(),
            CurrentMedications = pet.Medications?.Select(MapToPetMedicationDto).ToList() ?? new List<PetMedicationDto>(),
            RecentVaccinations = pet.Vaccinations?.Select(MapToPetVaccinationDto).ToList() ?? new List<PetVaccinationDto>()
        };
    }

    /// <summary>
    /// Maps a PetPhoto entity to PetPhotoDto
    /// </summary>
    private static PetPhotoDto MapToPetPhotoDto(PetPhoto photo)
    {
        return new PetPhotoDto
        {
            Id = photo.Id,
            PetId = photo.PetId,
            FileName = photo.FileName,
            PhotoUrl = photo.PhotoUrl,
            ThumbnailUrl = photo.ThumbnailUrl,
            Caption = photo.Caption,
            FileSize = photo.FileSize,
            MimeType = photo.MimeType,
            Width = photo.Width,
            Height = photo.Height,
            IsPrimary = photo.IsPrimary,
            DisplayOrder = photo.DisplayOrder,
            Tags = string.IsNullOrEmpty(photo.Tags) ? new List<string>() : 
                   System.Text.Json.JsonSerializer.Deserialize<List<string>>(photo.Tags) ?? new List<string>(),
            Category = photo.Category,
            DateTaken = photo.DateTaken,
            Location = photo.Location,
            CreatedAt = photo.CreatedAt
        };
    }

    /// <summary>
    /// Maps a MedicalRecord entity to MedicalRecordSummaryDto
    /// </summary>
    private static MedicalRecordSummaryDto MapToMedicalRecordSummaryDto(MedicalRecord record)
    {
        return new MedicalRecordSummaryDto
        {
            Id = record.Id,
            PetId = record.DogId,
            RecordType = record.RecordType,
            Title = record.Title,
            RecordDate = record.EventDate,
            VetOrClinic = record.VeterinarianName ?? record.ClinicName,
            Summary = record.Description,
            Diagnosis = record.Description,
            Treatment = record.Medications,
            FollowUpRequired = record.NextAppointmentDate.HasValue,
            FollowUpDate = record.NextAppointmentDate,
            Cost = record.Cost,
            CreatedAt = record.CreatedAt
        };
    }

    /// <summary>
    /// Maps a PetCareReminder entity to PetCareReminderDto
    /// </summary>
    private static PetCareReminderDto MapToPetCareReminderDto(PetCareReminder reminder)
    {
        var daysUntilDue = (int)(reminder.DueDate - DateTimeOffset.UtcNow).TotalDays;
        var isOverdue = reminder.DueDate < DateTimeOffset.UtcNow;

        return new PetCareReminderDto
        {
            Id = reminder.Id,
            PetId = reminder.PetId,
            Title = reminder.Title,
            Description = reminder.Description,
            CareType = reminder.CareType,
            Priority = reminder.Priority,
            DueDate = reminder.DueDate,
            Frequency = reminder.Frequency,
            Interval = reminder.Interval,
            NotificationMinutes = reminder.NotificationMinutes,
            IsCompleted = reminder.IsCompleted,
            CompletedAt = reminder.CompletedAt,
            CompletionNotes = reminder.CompletionNotes,
            NotificationsEnabled = reminder.NotificationsEnabled,
            NextDueDate = reminder.NextDueDate,
            DaysUntilDue = daysUntilDue,
            IsOverdue = isOverdue,
            CreatedAt = reminder.CreatedAt
        };
    }

    /// <summary>
    /// Maps a PetMedication entity to PetMedicationDto
    /// </summary>
    private static PetMedicationDto MapToPetMedicationDto(PetMedication medication)
    {
        var daysRemaining = medication.EndDate.HasValue 
            ? (int?)(medication.EndDate.Value - DateTimeOffset.UtcNow).TotalDays 
            : null;
        
        var needsRefill = medication.RefillsRemaining.HasValue && medication.RefillsRemaining <= 1;

        return new PetMedicationDto
        {
            Id = medication.Id,
            PetId = medication.PetId,
            MedicationName = medication.MedicationName,
            GenericName = medication.GenericName,
            MedicationType = medication.MedicationType,
            Dosage = medication.Dosage,
            DosageUnit = medication.DosageUnit,
            Frequency = medication.Frequency,
            AdministrationRoute = medication.AdministrationRoute,
            StartDate = medication.StartDate,
            EndDate = medication.EndDate,
            Reason = medication.Reason,
            PrescribingVet = medication.PrescribingVet,
            PrescribingClinic = medication.PrescribingClinic,
            PrescriptionNumber = medication.PrescriptionNumber,
            Instructions = medication.Instructions,
            SideEffects = medication.SideEffects,
            FoodInteractions = medication.FoodInteractions,
            DrugInteractions = medication.DrugInteractions,
            Cost = medication.Cost,
            RefillsRemaining = medication.RefillsRemaining,
            IsCurrentlyTaking = medication.IsCurrentlyTaking,
            ReminderEnabled = medication.ReminderEnabled,
            ReminderTimes = string.IsNullOrEmpty(medication.ReminderTimes) ? new List<string>() :
                           System.Text.Json.JsonSerializer.Deserialize<List<string>>(medication.ReminderTimes) ?? new List<string>(),
            Notes = medication.Notes,
            DaysRemaining = daysRemaining,
            NeedsRefill = needsRefill,
            CreatedAt = medication.CreatedAt
        };
    }

    /// <summary>
    /// Maps a PetVaccination entity to PetVaccinationDto
    /// </summary>
    private static PetVaccinationDto MapToPetVaccinationDto(PetVaccination vaccination)
    {
        var daysUntilNextDue = vaccination.NextDueDate.HasValue 
            ? (int?)(vaccination.NextDueDate.Value - DateTimeOffset.UtcNow).TotalDays 
            : null;
        
        var isOverdue = vaccination.NextDueDate.HasValue && vaccination.NextDueDate < DateTimeOffset.UtcNow;
        var isCurrent = !isOverdue && (vaccination.ExpirationDate == null || vaccination.ExpirationDate > DateTimeOffset.UtcNow);

        return new PetVaccinationDto
        {
            Id = vaccination.Id,
            PetId = vaccination.PetId,
            VaccineName = vaccination.VaccineName,
            VaccineType = vaccination.VaccineType,
            Manufacturer = vaccination.Manufacturer,
            BatchNumber = vaccination.BatchNumber,
            DateAdministered = vaccination.DateAdministered,
            NextDueDate = vaccination.NextDueDate,
            ExpirationDate = vaccination.ExpirationDate,
            VeterinarianName = vaccination.VeterinarianName,
            ClinicName = vaccination.ClinicName,
            ClinicAddress = vaccination.ClinicAddress,
            ClinicContact = vaccination.ClinicContact,
            Cost = vaccination.Cost,
            Notes = vaccination.Notes,
            AdverseReactions = vaccination.AdverseReactions,
            CertificateUrl = vaccination.CertificateUrl,
            ReminderSet = vaccination.ReminderSet,
            ReminderDaysBefore = vaccination.ReminderDaysBefore,
            DaysUntilNextDue = daysUntilNextDue,
            IsOverdue = isOverdue,
            IsCurrent = isCurrent,
            CreatedAt = vaccination.CreatedAt
        };
    }

    /// <summary>
    /// Calculates age from date of birth
    /// </summary>
    private static int CalculateAge(DateTimeOffset dateOfBirth)
    {
        var today = DateTimeOffset.UtcNow;
        var age = today.Year - dateOfBirth.Year;
        
        if (dateOfBirth.Date > today.AddYears(-age).Date)
        {
            age--;
        }
        
        return Math.Max(0, age);
    }

    #endregion
}