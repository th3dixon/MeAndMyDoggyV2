using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs.Dogs;
using MeAndMyDog.API.Models.DTOs.PetMedications;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing pet medications
/// </summary>
[ApiController]
[Route("api/v1/pets/{petId}/medications")]
[Authorize]
public class PetMedicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PetMedicationsController> _logger;

    /// <summary>
    /// Initializes a new instance of PetMedicationsController
    /// </summary>
    public PetMedicationsController(ApplicationDbContext context, ILogger<PetMedicationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all medications for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="includeInactive">Include inactive medications</param>
    /// <returns>List of pet medications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PetMedicationDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPetMedications(string petId, [FromQuery] bool includeInactive = false)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Verify pet ownership
            var petExists = await _context.DogProfiles
                .AnyAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (!petExists)
            {
                return NotFound("Pet not found");
            }

            var query = _context.PetMedications
                .Where(m => m.PetId == petId);

            if (!includeInactive)
            {
                query = query.Where(m => m.IsActive);
            }

            var medications = await query
                .OrderByDescending(m => m.StartDate)
                .ToListAsync();

            var medicationDtos = medications.Select(MapToPetMedicationDto).ToList();

            return Ok(medicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medications for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while retrieving medications");
        }
    }

    /// <summary>
    /// Get a specific medication by ID
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="medicationId">Medication ID</param>
    /// <returns>Medication details</returns>
    [HttpGet("{medicationId}")]
    [ProducesResponseType(typeof(PetMedicationDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMedication(string petId, string medicationId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var medication = await _context.PetMedications
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == medicationId && m.PetId == petId && 
                                        m.Pet.OwnerId == userId && m.IsActive);

            if (medication == null)
            {
                return NotFound("Medication not found");
            }

            var medicationDto = MapToPetMedicationDto(medication);

            return Ok(medicationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medication {MedicationId}", medicationId);
            return StatusCode(500, "An error occurred while retrieving the medication");
        }
    }

    /// <summary>
    /// Add a new medication record
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="request">Medication details</param>
    /// <returns>Created medication</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PetMedicationDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddMedication(string petId, [FromBody] AddMedicationRequest request)
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

            // Verify pet ownership
            var petExists = await _context.DogProfiles
                .AnyAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (!petExists)
            {
                return NotFound("Pet not found");
            }

            var medication = new PetMedication
            {
                Id = Guid.NewGuid().ToString(),
                PetId = petId,
                MedicationName = request.MedicationName.Trim(),
                GenericName = request.GenericName?.Trim(),
                MedicationType = request.MedicationType.Trim(),
                Dosage = request.Dosage.Trim(),
                DosageUnit = request.DosageUnit.Trim(),
                Frequency = request.Frequency.Trim(),
                AdministrationRoute = request.AdministrationRoute.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason?.Trim(),
                PrescribingVet = request.PrescribingVet?.Trim(),
                PrescribingClinic = request.PrescribingClinic?.Trim(),
                PrescriptionNumber = request.PrescriptionNumber?.Trim(),
                Instructions = request.Instructions?.Trim(),
                SideEffects = request.SideEffects?.Trim(),
                FoodInteractions = request.FoodInteractions?.Trim(),
                DrugInteractions = request.DrugInteractions?.Trim(),
                Cost = request.Cost,
                RefillsRemaining = request.RefillsRemaining,
                IsCurrentlyTaking = request.IsCurrentlyTaking,
                ReminderEnabled = request.ReminderEnabled,
                ReminderTimes = request.ReminderTimes?.Any() == true 
                    ? System.Text.Json.JsonSerializer.Serialize(request.ReminderTimes) 
                    : null,
                Notes = request.Notes?.Trim(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.PetMedications.Add(medication);
            await _context.SaveChangesAsync();

            var medicationDto = MapToPetMedicationDto(medication);

            _logger.LogInformation("Medication added for pet {PetId}: {MedicationId}", petId, medication.Id);

            return CreatedAtAction(nameof(GetMedication), 
                new { petId, medicationId = medication.Id }, medicationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding medication for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while adding the medication");
        }
    }

    /// <summary>
    /// Update an existing medication record
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="medicationId">Medication ID</param>
    /// <param name="request">Updated medication details</param>
    /// <returns>Updated medication</returns>
    [HttpPut("{medicationId}")]
    [ProducesResponseType(typeof(PetMedicationDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMedication(
        string petId, 
        string medicationId, 
        [FromBody] AddMedicationRequest request)
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

            var medication = await _context.PetMedications
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == medicationId && m.PetId == petId && 
                                        m.Pet.OwnerId == userId && m.IsActive);

            if (medication == null)
            {
                return NotFound("Medication not found");
            }

            // Update medication properties
            medication.MedicationName = request.MedicationName.Trim();
            medication.GenericName = request.GenericName?.Trim();
            medication.MedicationType = request.MedicationType.Trim();
            medication.Dosage = request.Dosage.Trim();
            medication.DosageUnit = request.DosageUnit.Trim();
            medication.Frequency = request.Frequency.Trim();
            medication.AdministrationRoute = request.AdministrationRoute.Trim();
            medication.StartDate = request.StartDate;
            medication.EndDate = request.EndDate;
            medication.Reason = request.Reason?.Trim();
            medication.PrescribingVet = request.PrescribingVet?.Trim();
            medication.PrescribingClinic = request.PrescribingClinic?.Trim();
            medication.PrescriptionNumber = request.PrescriptionNumber?.Trim();
            medication.Instructions = request.Instructions?.Trim();
            medication.SideEffects = request.SideEffects?.Trim();
            medication.FoodInteractions = request.FoodInteractions?.Trim();
            medication.DrugInteractions = request.DrugInteractions?.Trim();
            medication.Cost = request.Cost;
            medication.RefillsRemaining = request.RefillsRemaining;
            medication.IsCurrentlyTaking = request.IsCurrentlyTaking;
            medication.ReminderEnabled = request.ReminderEnabled;
            medication.ReminderTimes = request.ReminderTimes?.Any() == true 
                ? System.Text.Json.JsonSerializer.Serialize(request.ReminderTimes) 
                : null;
            medication.Notes = request.Notes?.Trim();
            medication.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var medicationDto = MapToPetMedicationDto(medication);

            _logger.LogInformation("Medication updated: {MedicationId}", medicationId);

            return Ok(medicationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medication {MedicationId}", medicationId);
            return StatusCode(500, "An error occurred while updating the medication");
        }
    }

    /// <summary>
    /// Delete a medication record (soft delete)
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="medicationId">Medication ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{medicationId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMedication(string petId, string medicationId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var medication = await _context.PetMedications
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == medicationId && m.PetId == petId && 
                                        m.Pet.OwnerId == userId && m.IsActive);

            if (medication == null)
            {
                return NotFound("Medication not found");
            }

            // Soft delete
            medication.IsActive = false;
            medication.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Medication deleted: {MedicationId}", medicationId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medication {MedicationId}", medicationId);
            return StatusCode(500, "An error occurred while deleting the medication");
        }
    }

    #region Private Helper Methods

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

    #endregion
}