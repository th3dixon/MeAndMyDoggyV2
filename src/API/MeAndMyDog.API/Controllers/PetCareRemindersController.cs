using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs.Dogs;
using MeAndMyDog.API.Models.DTOs.PetCareReminders;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing pet care reminders
/// </summary>
[ApiController]
[Route("api/v1/pets/{petId}/reminders")]
[Authorize]
public class PetCareRemindersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PetCareRemindersController> _logger;

    /// <summary>
    /// Initializes a new instance of PetCareRemindersController
    /// </summary>
    public PetCareRemindersController(ApplicationDbContext context, ILogger<PetCareRemindersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all care reminders for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="includeCompleted">Include completed reminders</param>
    /// <param name="careType">Filter by care type</param>
    /// <returns>List of care reminders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PetCareReminderDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCareReminders(
        string petId, 
        [FromQuery] bool includeCompleted = false,
        [FromQuery] string? careType = null)
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

            var query = _context.PetCareReminders
                .Where(r => r.PetId == petId && r.IsActive);

            if (!includeCompleted)
            {
                query = query.Where(r => !r.IsCompleted);
            }

            if (!string.IsNullOrEmpty(careType))
            {
                query = query.Where(r => r.CareType.ToLower() == careType.ToLower());
            }

            var reminders = await query
                .OrderBy(r => r.DueDate)
                .ToListAsync();

            var reminderDtos = reminders.Select(MapToPetCareReminderDto).ToList();

            return Ok(reminderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving care reminders for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while retrieving care reminders");
        }
    }

    /// <summary>
    /// Get upcoming reminders (due within next 7 days)
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <returns>List of upcoming reminders</returns>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<PetCareReminderDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUpcomingReminders(string petId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var petExists = await _context.DogProfiles
                .AnyAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (!petExists)
            {
                return NotFound("Pet not found");
            }

            var upcomingDate = DateTimeOffset.UtcNow.AddDays(7);

            var reminders = await _context.PetCareReminders
                .Where(r => r.PetId == petId && r.IsActive && !r.IsCompleted &&
                           r.DueDate <= upcomingDate)
                .OrderBy(r => r.DueDate)
                .ToListAsync();

            var reminderDtos = reminders.Select(MapToPetCareReminderDto).ToList();

            return Ok(reminderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming reminders for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while retrieving upcoming reminders");
        }
    }

    /// <summary>
    /// Get a specific care reminder by ID
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <returns>Reminder details</returns>
    [HttpGet("{reminderId}")]
    [ProducesResponseType(typeof(PetCareReminderDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCareReminder(string petId, string reminderId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var reminder = await _context.PetCareReminders
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.PetId == petId && 
                                        r.Pet.OwnerId == userId && r.IsActive);

            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }

            var reminderDto = MapToPetCareReminderDto(reminder);

            return Ok(reminderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reminder {ReminderId}", reminderId);
            return StatusCode(500, "An error occurred while retrieving the reminder");
        }
    }

    /// <summary>
    /// Create a new care reminder
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="request">Reminder details</param>
    /// <returns>Created reminder</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PetCareReminderDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateCareReminder(string petId, [FromBody] CreateReminderRequest request)
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

            var petExists = await _context.DogProfiles
                .AnyAsync(p => p.Id == petId && p.OwnerId == userId && p.IsActive);

            if (!petExists)
            {
                return NotFound("Pet not found");
            }

            var reminder = new PetCareReminder
            {
                Id = Guid.NewGuid().ToString(),
                PetId = petId,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                CareType = request.CareType.Trim(),
                Priority = request.Priority,
                DueDate = request.DueDate,
                Frequency = request.Frequency,
                Interval = request.Interval,
                NotificationMinutes = request.NotificationMinutes,
                NotificationsEnabled = request.NotificationsEnabled,
                NextDueDate = CalculateNextDueDate(request.DueDate, request.Frequency, request.Interval),
                IsCompleted = false,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.PetCareReminders.Add(reminder);
            await _context.SaveChangesAsync();

            var reminderDto = MapToPetCareReminderDto(reminder);

            _logger.LogInformation("Care reminder created for pet {PetId}: {ReminderId}", petId, reminder.Id);

            return CreatedAtAction(nameof(GetCareReminder), 
                new { petId, reminderId = reminder.Id }, reminderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating care reminder for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while creating the care reminder");
        }
    }

    /// <summary>
    /// Update an existing care reminder
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <param name="request">Updated reminder details</param>
    /// <returns>Updated reminder</returns>
    [HttpPut("{reminderId}")]
    [ProducesResponseType(typeof(PetCareReminderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateCareReminder(
        string petId, 
        string reminderId, 
        [FromBody] CreateReminderRequest request)
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

            var reminder = await _context.PetCareReminders
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.PetId == petId && 
                                        r.Pet.OwnerId == userId && r.IsActive);

            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }

            // Update reminder properties
            reminder.Title = request.Title.Trim();
            reminder.Description = request.Description?.Trim();
            reminder.CareType = request.CareType.Trim();
            reminder.Priority = request.Priority;
            reminder.DueDate = request.DueDate;
            reminder.Frequency = request.Frequency;
            reminder.Interval = request.Interval;
            reminder.NotificationMinutes = request.NotificationMinutes;
            reminder.NotificationsEnabled = request.NotificationsEnabled;
            reminder.NextDueDate = CalculateNextDueDate(request.DueDate, request.Frequency, request.Interval);
            reminder.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var reminderDto = MapToPetCareReminderDto(reminder);

            _logger.LogInformation("Care reminder updated: {ReminderId}", reminderId);

            return Ok(reminderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating care reminder {ReminderId}", reminderId);
            return StatusCode(500, "An error occurred while updating the care reminder");
        }
    }

    /// <summary>
    /// Mark a care reminder as completed
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <param name="request">Completion details</param>
    /// <returns>Updated reminder</returns>
    [HttpPost("{reminderId}/complete")]
    [ProducesResponseType(typeof(PetCareReminderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CompleteReminder(
        string petId, 
        string reminderId, 
        [FromBody] CompleteReminderRequest request)
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

            var reminder = await _context.PetCareReminders
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.PetId == petId && 
                                        r.Pet.OwnerId == userId && r.IsActive);

            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }

            // Mark as completed
            reminder.IsCompleted = true;
            reminder.CompletedAt = DateTimeOffset.UtcNow;
            reminder.CompletionNotes = request.CompletionNotes?.Trim();
            reminder.UpdatedAt = DateTimeOffset.UtcNow;

            // Create next recurring reminder if applicable
            if (reminder.Frequency != "Once" && reminder.NextDueDate.HasValue)
            {
                var nextReminder = new PetCareReminder
                {
                    Id = Guid.NewGuid().ToString(),
                    PetId = petId,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    CareType = reminder.CareType,
                    Priority = reminder.Priority,
                    DueDate = reminder.NextDueDate.Value,
                    Frequency = reminder.Frequency,
                    Interval = reminder.Interval,
                    NotificationMinutes = reminder.NotificationMinutes,
                    NotificationsEnabled = reminder.NotificationsEnabled,
                    NextDueDate = CalculateNextDueDate(reminder.NextDueDate.Value, reminder.Frequency, reminder.Interval),
                    IsCompleted = false,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                _context.PetCareReminders.Add(nextReminder);
            }

            await _context.SaveChangesAsync();

            var reminderDto = MapToPetCareReminderDto(reminder);

            _logger.LogInformation("Care reminder completed: {ReminderId}", reminderId);

            return Ok(reminderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing care reminder {ReminderId}", reminderId);
            return StatusCode(500, "An error occurred while completing the care reminder");
        }
    }

    /// <summary>
    /// Delete a care reminder (soft delete)
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{reminderId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteCareReminder(string petId, string reminderId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var reminder = await _context.PetCareReminders
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.PetId == petId && 
                                        r.Pet.OwnerId == userId && r.IsActive);

            if (reminder == null)
            {
                return NotFound("Reminder not found");
            }

            // Soft delete
            reminder.IsActive = false;
            reminder.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Care reminder deleted: {ReminderId}", reminderId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting care reminder {ReminderId}", reminderId);
            return StatusCode(500, "An error occurred while deleting the care reminder");
        }
    }

    #region Private Helper Methods

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
    /// Calculates the next due date for recurring reminders
    /// </summary>
    private static DateTimeOffset? CalculateNextDueDate(DateTimeOffset currentDueDate, string frequency, int interval)
    {
        return frequency.ToLower() switch
        {
            "daily" => currentDueDate.AddDays(interval),
            "weekly" => currentDueDate.AddDays(interval * 7),
            "monthly" => currentDueDate.AddMonths(interval),
            "yearly" => currentDueDate.AddYears(interval),
            _ => null // "Once" or unknown frequency
        };
    }

    #endregion
}