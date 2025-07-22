using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Helpers;
using MeAndMyDog.API.Services.Interfaces;
using System.Diagnostics;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for calendar and appointment functionality
/// </summary>
public class CalendarService : ICalendarService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(
        ApplicationDbContext context,
        ILogger<CalendarService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    public async Task<AppointmentResponse> CreateAppointmentAsync(string userId, CreateAppointmentRequest request)
    {
        try
        {
            // Validate request
            var validationErrors = ValidateCreateAppointmentRequest(request);
            if (validationErrors.Any())
            {
                return new AppointmentResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationErrors
                };
            }

            // Create appointment entity
            var appointment = new CalendarAppointment
            {
                Id = Guid.NewGuid().ToString(),
                CreatedByUserId = userId,
                ConversationId = request.ConversationId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TimeZone = request.TimeZone,
                Location = request.Location,
                MeetingUrl = request.MeetingUrl,
                AppointmentType = EnumConverter.ToString(request.AppointmentType),
                Status = EnumConverter.ToString(AppointmentStatus.Scheduled),
                Priority = EnumConverter.ToString(request.Priority),
                IsAllDay = request.IsAllDay,
                IsRecurring = request.IsRecurring,
                RecurrencePattern = request.RecurrencePattern?.ToString(),
                RecurrenceInterval = request.RecurrenceInterval,
                RecurrenceEndDate = request.RecurrenceEndDate,
                MaxOccurrences = request.MaxOccurrences,
                RemindersEnabled = request.RemindersEnabled,
                DefaultReminderMinutes = request.DefaultReminderMinutes,
                ColorCode = request.ColorCode,
                Notes = request.Notes,
                AttachmentFiles = request.AttachmentFiles.Any() ? string.Join(",", request.AttachmentFiles) : null,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.CalendarAppointments.Add(appointment);

            // Add participants
            var participantsAdded = 0;
            foreach (var participantRequest in request.Participants)
            {
                var participant = new AppointmentParticipant
                {
                    Id = Guid.NewGuid().ToString(),
                    AppointmentId = appointment.Id,
                    UserId = participantRequest.UserId ?? string.Empty,
                    Email = participantRequest.Email,
                    DisplayName = participantRequest.DisplayName,
                    Role = EnumConverter.ToString(participantRequest.Role),
                    ResponseStatus = EnumConverter.ToString(ResponseStatus.Pending),
                    IsRequired = participantRequest.IsRequired,
                    IsOrganizer = participantRequest.Role == ParticipantRole.Organizer,
                    InvitedAt = DateTimeOffset.UtcNow,
                    TimeZone = participantRequest.TimeZone,
                    Notes = participantRequest.Notes
                };

                _context.AppointmentParticipants.Add(participant);
                participantsAdded++;
            }

            // Add organizer as participant if not already added
            if (!request.Participants.Any(p => p.UserId == userId))
            {
                var organizerParticipant = new AppointmentParticipant
                {
                    Id = Guid.NewGuid().ToString(),
                    AppointmentId = appointment.Id,
                    UserId = userId,
                    Email = string.Empty, // Could be populated from user service
                    DisplayName = "Organizer",
                    Role = EnumConverter.ToString(ParticipantRole.Organizer),
                    ResponseStatus = EnumConverter.ToString(ResponseStatus.Accepted),
                    IsRequired = true,
                    IsOrganizer = true,
                    InvitedAt = DateTimeOffset.UtcNow
                };

                _context.AppointmentParticipants.Add(organizerParticipant);
                participantsAdded++;
            }

            // Add reminders
            var remindersCreated = 0;
            if (request.RemindersEnabled)
            {
                // Add default reminder
                var defaultReminder = new AppointmentReminder
                {
                    Id = Guid.NewGuid().ToString(),
                    AppointmentId = appointment.Id,
                    UserId = userId,
                    ReminderType = EnumConverter.ToString(ReminderType.PushNotification),
                    MinutesBefore = request.DefaultReminderMinutes,
                    ReminderTime = appointment.StartTime.AddMinutes(-request.DefaultReminderMinutes),
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.AppointmentReminders.Add(defaultReminder);
                remindersCreated++;

                // Add custom reminders
                foreach (var reminderRequest in request.CustomReminders)
                {
                    var reminder = new AppointmentReminder
                    {
                        Id = Guid.NewGuid().ToString(),
                        AppointmentId = appointment.Id,
                        UserId = reminderRequest.UserId ?? userId,
                        ReminderType = EnumConverter.ToString(reminderRequest.ReminderType),
                        MinutesBefore = reminderRequest.MinutesBefore,
                        ReminderTime = appointment.StartTime.AddMinutes(-reminderRequest.MinutesBefore),
                        CustomMessage = reminderRequest.CustomMessage,
                        MaxRetries = reminderRequest.MaxRetries,
                        IsActive = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    _context.AppointmentReminders.Add(reminder);
                    remindersCreated++;
                }
            }

            await _context.SaveChangesAsync();

            // Generate recurring instances if needed
            if (appointment.IsRecurring && request.RecurrencePattern.HasValue)
            {
                var instancesGenerated = await GenerateRecurringInstancesInternalAsync(
                    appointment.Id, 
                    appointment.StartTime.Date,
                    appointment.RecurrenceEndDate ?? appointment.StartTime.AddYears(1));
                
                _logger.LogInformation("Generated {Count} recurring instances for appointment {AppointmentId}", 
                    instancesGenerated, appointment.Id);
            }

            var appointmentDto = await MapToAppointmentDtoAsync(appointment);

            return new AppointmentResponse
            {
                Appointment = appointmentDto,
                Success = true,
                Message = "Appointment created successfully",
                ParticipantsInvited = participantsAdded,
                RemindersCreated = remindersCreated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment for user {UserId}", userId);
            return new AppointmentResponse
            {
                Success = false,
                Message = "An error occurred while creating the appointment",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Update an existing appointment
    /// </summary>
    public async Task<AppointmentResponse> UpdateAppointmentAsync(string userId, string appointmentId, UpdateAppointmentRequest request)
    {
        try
        {
            var appointment = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return new AppointmentResponse
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            // Check permissions
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
            {
                return new AppointmentResponse
                {
                    Success = false,
                    Message = "You don't have permission to update this appointment"
                };
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Title))
                appointment.Title = request.Title;
            
            if (request.Description != null)
                appointment.Description = request.Description;
            
            if (request.StartTime.HasValue)
                appointment.StartTime = request.StartTime.Value;
            
            if (request.EndTime.HasValue)
                appointment.EndTime = request.EndTime.Value;
            
            if (!string.IsNullOrEmpty(request.TimeZone))
                appointment.TimeZone = request.TimeZone;
            
            if (request.Location != null)
                appointment.Location = request.Location;
            
            if (request.MeetingUrl != null)
                appointment.MeetingUrl = request.MeetingUrl;
            
            if (request.AppointmentType.HasValue)
                appointment.AppointmentType = EnumConverter.ToString(request.AppointmentType.Value);
            
            if (request.Status.HasValue)
                appointment.Status = EnumConverter.ToString(request.Status.Value);
            
            if (request.Priority.HasValue)
                appointment.Priority = EnumConverter.ToString(request.Priority.Value);
            
            if (request.ColorCode != null)
                appointment.ColorCode = request.ColorCode;
            
            if (request.Notes != null)
                appointment.Notes = request.Notes;
            
            if (request.RemindersEnabled.HasValue)
                appointment.RemindersEnabled = request.RemindersEnabled.Value;
            
            if (request.DefaultReminderMinutes.HasValue)
                appointment.DefaultReminderMinutes = request.DefaultReminderMinutes.Value;

            appointment.UpdatedAt = DateTimeOffset.UtcNow;
            appointment.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            var appointmentDto = await MapToAppointmentDtoAsync(appointment);

            return new AppointmentResponse
            {
                Appointment = appointmentDto,
                Success = true,
                Message = "Appointment updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {AppointmentId} for user {UserId}", appointmentId, userId);
            return new AppointmentResponse
            {
                Success = false,
                Message = "An error occurred while updating the appointment",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    public async Task<CalendarAppointmentDto?> GetAppointmentAsync(string userId, string appointmentId)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
                return null;

            var appointment = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            return appointment != null ? await MapToAppointmentDtoAsync(appointment) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment {AppointmentId} for user {UserId}", appointmentId, userId);
            return null;
        }
    }

    /// <summary>
    /// Delete an appointment
    /// </summary>
    public async Task<bool> DeleteAppointmentAsync(string userId, string appointmentId, string? cancellationReason = null)
    {
        try
        {
            var appointment = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .Include(a => a.RecurringInstances)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return false;

            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
                return false;

            // Remove related entities
            _context.AppointmentParticipants.RemoveRange(appointment.Participants);
            _context.AppointmentReminders.RemoveRange(appointment.Reminders);
            _context.AppointmentInstances.RemoveRange(appointment.RecurringInstances);
            _context.CalendarAppointments.Remove(appointment);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted appointment {AppointmentId} by user {UserId}", appointmentId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {AppointmentId} for user {UserId}", appointmentId, userId);
            return false;
        }
    }

    /// <summary>
    /// Cancel an appointment
    /// </summary>
    public async Task<bool> CancelAppointmentAsync(string userId, string appointmentId, string cancellationReason)
    {
        try
        {
            var appointment = await _context.CalendarAppointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return false;

            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
                return false;

            appointment.Status = EnumConverter.ToString(AppointmentStatus.Cancelled);
            appointment.CancelledAt = DateTimeOffset.UtcNow;
            appointment.CancellationReason = cancellationReason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;
            appointment.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cancelled appointment {AppointmentId} by user {UserId}: {Reason}", 
                appointmentId, userId, cancellationReason);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling appointment {AppointmentId} for user {UserId}", appointmentId, userId);
            return false;
        }
    }

    /// <summary>
    /// Search appointments with filters and pagination
    /// </summary>
    public async Task<SearchAppointmentsResponse> SearchAppointmentsAsync(string userId, SearchAppointmentsRequest request)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            var query = BuildAppointmentSearchQuery(userId, request);
            
            var totalCount = await query.CountAsync();
            
            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);
            
            // Apply pagination
            var appointments = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .ToListAsync();

            var appointmentDtos = new List<CalendarAppointmentDto>();
            foreach (var appointment in appointments)
            {
                appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
            }

            stopwatch.Stop();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var appliedFilters = BuildAppliedFiltersList(request);

            return new SearchAppointmentsResponse
            {
                Appointments = appointmentDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                AppliedFilters = appliedFilters
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching appointments for user {UserId}", userId);
            return new SearchAppointmentsResponse
            {
                Appointments = new List<CalendarAppointmentDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                ExecutionTimeMs = 0
            };
        }
    }

    /// <summary>
    /// Get appointments for a date range
    /// </summary>
    public async Task<List<CalendarAppointmentDto>> GetAppointmentsByDateRangeAsync(string userId, DateTimeOffset startDate, DateTimeOffset endDate, string timeZone = "UTC")
    {
        try
        {
            var appointments = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .Where(a => a.Participants.Any(p => p.UserId == userId) &&
                           a.StartTime >= startDate &&
                           a.EndTime <= endDate &&
                           a.Status != EnumConverter.ToString(AppointmentStatus.Cancelled))
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            var appointmentDtos = new List<CalendarAppointmentDto>();
            foreach (var appointment in appointments)
            {
                appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
            }

            return appointmentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments by date range for user {UserId}", userId);
            return new List<CalendarAppointmentDto>();
        }
    }

    /// <summary>
    /// Get upcoming appointments for a user
    /// </summary>
    public async Task<List<CalendarAppointmentDto>> GetUpcomingAppointmentsAsync(string userId, int limit = 10)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            
            var appointments = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Include(a => a.Reminders)
                .Where(a => a.Participants.Any(p => p.UserId == userId) &&
                           a.StartTime > now &&
                           a.Status != EnumConverter.ToString(AppointmentStatus.Cancelled))
                .OrderBy(a => a.StartTime)
                .Take(limit)
                .ToListAsync();

            var appointmentDtos = new List<CalendarAppointmentDto>();
            foreach (var appointment in appointments)
            {
                appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
            }

            return appointmentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming appointments for user {UserId}", userId);
            return new List<CalendarAppointmentDto>();
        }
    }

    /// <summary>
    /// Check availability for users in a time range
    /// </summary>
    public async Task<CheckAvailabilityResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request)
    {
        try
        {
            var response = new CheckAvailabilityResponse();
            
            // Get existing appointments for all users in the time range
            var existingAppointments = await _context.CalendarAppointments
                .Include(a => a.Participants)
                .Where(a => a.StartTime < request.EndTime &&
                           a.EndTime > request.StartTime &&
                           a.Status != EnumConverter.ToString(AppointmentStatus.Cancelled) &&
                           a.Participants.Any(p => request.UserIds.Contains(p.UserId)))
                .ToListAsync();

            // Process each user's availability
            foreach (var userId in request.UserIds)
            {
                var userAvailability = await ProcessUserAvailabilityAsync(
                    userId, request, existingAppointments);
                response.UserAvailability.Add(userAvailability);
            }

            // Find common available slots
            response.CommonAvailableSlots = FindCommonAvailableSlots(
                response.UserAvailability, request);

            // Generate suggested alternatives if no common slots found
            if (!response.CommonAvailableSlots.Any())
            {
                response.SuggestedAlternatives = await GenerateAlternativeTimeSlotsAsync(
                    request.UserIds, request.StartTime, request.EndTime, request.MinimumDurationMinutes);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for users {UserIds}", string.Join(",", request.UserIds));
            return new CheckAvailabilityResponse();
        }
    }

    /// <summary>
    /// Add participant to appointment
    /// </summary>
    public async Task<ParticipantResponse> AddParticipantAsync(string userId, string appointmentId, CreateParticipantRequest request)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
            {
                return new ParticipantResponse
                {
                    Success = false,
                    Message = "You don't have permission to add participants to this appointment"
                };
            }

            var participant = new AppointmentParticipant
            {
                Id = Guid.NewGuid().ToString(),
                AppointmentId = appointmentId,
                UserId = request.UserId ?? string.Empty,
                Email = request.Email,
                DisplayName = request.DisplayName,
                Role = EnumConverter.ToString(request.Role),
                ResponseStatus = EnumConverter.ToString(ResponseStatus.Pending),
                IsRequired = request.IsRequired,
                IsOrganizer = request.Role == ParticipantRole.Organizer,
                InvitedAt = DateTimeOffset.UtcNow,
                TimeZone = request.TimeZone,
                Notes = request.Notes
            };

            _context.AppointmentParticipants.Add(participant);
            await _context.SaveChangesAsync();

            var participantDto = MapToParticipantDto(participant);

            return new ParticipantResponse
            {
                Participant = participantDto,
                Success = true,
                Message = "Participant added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant to appointment {AppointmentId}", appointmentId);
            return new ParticipantResponse
            {
                Success = false,
                Message = "An error occurred while adding the participant",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Update participant details
    /// </summary>
    public async Task<ParticipantResponse> UpdateParticipantAsync(string userId, string participantId, UpdateParticipantRequest request)
    {
        try
        {
            var participant = await _context.AppointmentParticipants
                .FirstOrDefaultAsync(p => p.Id == participantId);

            if (participant == null)
            {
                return new ParticipantResponse
                {
                    Success = false,
                    Message = "Participant not found"
                };
            }

            if (!await HasAppointmentPermissionAsync(userId, participant.AppointmentId))
            {
                return new ParticipantResponse
                {
                    Success = false,
                    Message = "You don't have permission to update this participant"
                };
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.DisplayName))
                participant.DisplayName = request.DisplayName;
            
            if (request.Role.HasValue)
                participant.Role = EnumConverter.ToString(request.Role.Value);
            
            if (request.IsRequired.HasValue)
                participant.IsRequired = request.IsRequired.Value;
            
            if (request.TimeZone != null)
                participant.TimeZone = request.TimeZone;
            
            if (request.Notes != null)
                participant.Notes = request.Notes;

            await _context.SaveChangesAsync();

            var participantDto = MapToParticipantDto(participant);

            return new ParticipantResponse
            {
                Participant = participantDto,
                Success = true,
                Message = "Participant updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant {ParticipantId}", participantId);
            return new ParticipantResponse
            {
                Success = false,
                Message = "An error occurred while updating the participant",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Remove participant from appointment
    /// </summary>
    public async Task<bool> RemoveParticipantAsync(string userId, string participantId)
    {
        try
        {
            var participant = await _context.AppointmentParticipants
                .FirstOrDefaultAsync(p => p.Id == participantId);

            if (participant == null)
                return false;

            if (!await HasAppointmentPermissionAsync(userId, participant.AppointmentId))
                return false;

            _context.AppointmentParticipants.Remove(participant);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant {ParticipantId}", participantId);
            return false;
        }
    }

    /// <summary>
    /// Respond to appointment invitation
    /// </summary>
    public async Task<ParticipantResponse> RespondToInvitationAsync(string userId, string participantId, RespondToInvitationRequest request)
    {
        try
        {
            var participant = await _context.AppointmentParticipants
                .FirstOrDefaultAsync(p => p.Id == participantId && p.UserId == userId);

            if (participant == null)
            {
                return new ParticipantResponse
                {
                    Success = false,
                    Message = "Participant not found or you don't have permission to respond"
                };
            }

            participant.ResponseStatus = EnumConverter.ToString(request.ResponseStatus);
            participant.ResponseMessage = request.ResponseMessage;
            participant.RespondedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var participantDto = MapToParticipantDto(participant);

            return new ParticipantResponse
            {
                Participant = participantDto,
                Success = true,
                Message = "Response recorded successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to invitation for participant {ParticipantId}", participantId);
            return new ParticipantResponse
            {
                Success = false,
                Message = "An error occurred while recording your response",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Get participant details
    /// </summary>
    public async Task<AppointmentParticipantDto?> GetParticipantAsync(string userId, string participantId)
    {
        try
        {
            var participant = await _context.AppointmentParticipants
                .FirstOrDefaultAsync(p => p.Id == participantId);

            if (participant == null)
                return null;

            if (!await HasAppointmentPermissionAsync(userId, participant.AppointmentId))
                return null;

            return MapToParticipantDto(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participant {ParticipantId}", participantId);
            return null;
        }
    }

    /// <summary>
    /// Add reminder to appointment
    /// </summary>
    public async Task<ReminderResponse> AddReminderAsync(string userId, string appointmentId, CreateReminderRequest request)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
            {
                return new ReminderResponse
                {
                    Success = false,
                    Message = "You don't have permission to add reminders to this appointment"
                };
            }

            var appointment = await _context.CalendarAppointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return new ReminderResponse
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            var reminder = new AppointmentReminder
            {
                Id = Guid.NewGuid().ToString(),
                AppointmentId = appointmentId,
                UserId = request.UserId ?? userId,
                ReminderType = EnumConverter.ToString(request.ReminderType),
                MinutesBefore = request.MinutesBefore,
                ReminderTime = appointment.StartTime.AddMinutes(-request.MinutesBefore),
                CustomMessage = request.CustomMessage,
                MaxRetries = request.MaxRetries,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.AppointmentReminders.Add(reminder);
            await _context.SaveChangesAsync();

            var reminderDto = MapToReminderDto(reminder);

            return new ReminderResponse
            {
                Reminder = reminderDto,
                Success = true,
                Message = "Reminder added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reminder to appointment {AppointmentId}", appointmentId);
            return new ReminderResponse
            {
                Success = false,
                Message = "An error occurred while adding the reminder",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Update reminder details
    /// </summary>
    public async Task<ReminderResponse> UpdateReminderAsync(string userId, string reminderId, UpdateReminderRequest request)
    {
        try
        {
            var reminder = await _context.AppointmentReminders
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
            {
                return new ReminderResponse
                {
                    Success = false,
                    Message = "Reminder not found"
                };
            }

            if (!await HasAppointmentPermissionAsync(userId, reminder.AppointmentId))
            {
                return new ReminderResponse
                {
                    Success = false,
                    Message = "You don't have permission to update this reminder"
                };
            }

            // Update fields if provided
            if (request.ReminderType.HasValue)
                reminder.ReminderType = EnumConverter.ToString(request.ReminderType.Value);
            
            if (request.MinutesBefore.HasValue)
            {
                reminder.MinutesBefore = request.MinutesBefore.Value;
                reminder.ReminderTime = reminder.Appointment.StartTime.AddMinutes(-request.MinutesBefore.Value);
            }
            
            if (request.CustomMessage != null)
                reminder.CustomMessage = request.CustomMessage;
            
            if (request.IsActive.HasValue)
                reminder.IsActive = request.IsActive.Value;

            await _context.SaveChangesAsync();

            var reminderDto = MapToReminderDto(reminder);

            return new ReminderResponse
            {
                Reminder = reminderDto,
                Success = true,
                Message = "Reminder updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reminder {ReminderId}", reminderId);
            return new ReminderResponse
            {
                Success = false,
                Message = "An error occurred while updating the reminder",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Delete reminder
    /// </summary>
    public async Task<bool> DeleteReminderAsync(string userId, string reminderId)
    {
        try
        {
            var reminder = await _context.AppointmentReminders
                .FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
                return false;

            if (!await HasAppointmentPermissionAsync(userId, reminder.AppointmentId))
                return false;

            _context.AppointmentReminders.Remove(reminder);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting reminder {ReminderId}", reminderId);
            return false;
        }
    }

    /// <summary>
    /// Get reminder details
    /// </summary>
    public async Task<AppointmentReminderDto?> GetReminderAsync(string userId, string reminderId)
    {
        try
        {
            var reminder = await _context.AppointmentReminders
                .FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
                return null;

            if (!await HasAppointmentPermissionAsync(userId, reminder.AppointmentId))
                return null;

            return MapToReminderDto(reminder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reminder {ReminderId}", reminderId);
            return null;
        }
    }

    /// <summary>
    /// Get reminders for an appointment
    /// </summary>
    public async Task<List<AppointmentReminderDto>> GetAppointmentRemindersAsync(string userId, string appointmentId)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
                return new List<AppointmentReminderDto>();

            var reminders = await _context.AppointmentReminders
                .Where(r => r.AppointmentId == appointmentId)
                .OrderBy(r => r.MinutesBefore)
                .ToListAsync();

            return reminders.Select(MapToReminderDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reminders for appointment {AppointmentId}", appointmentId);
            return new List<AppointmentReminderDto>();
        }
    }

    /// <summary>
    /// Get pending reminders that need to be sent
    /// </summary>
    public async Task<List<AppointmentReminderDto>> GetPendingRemindersAsync(DateTimeOffset beforeTime)
    {
        try
        {
            var reminders = await _context.AppointmentReminders
                .Include(r => r.Appointment)
                .Where(r => r.IsActive &&
                           !r.IsSent &&
                           r.ReminderTime <= beforeTime &&
                           r.Appointment.Status != EnumConverter.ToString(AppointmentStatus.Cancelled))
                .OrderBy(r => r.ReminderTime)
                .ToListAsync();

            return reminders.Select(MapToReminderDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending reminders");
            return new List<AppointmentReminderDto>();
        }
    }

    /// <summary>
    /// Mark reminder as sent
    /// </summary>
    public async Task<bool> MarkReminderSentAsync(string reminderId, DateTimeOffset sentAt, string deliveryMethod, bool isDelivered, string? error = null)
    {
        try
        {
            var reminder = await _context.AppointmentReminders
                .FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
                return false;

            reminder.IsSent = true;
            reminder.SentAt = sentAt;
            reminder.IsDelivered = isDelivered;
            reminder.DeliveryMethod = deliveryMethod;
            reminder.DeliveryError = error;

            if (!isDelivered)
            {
                reminder.RetryCount++;
                if (reminder.RetryCount < reminder.MaxRetries)
                {
                    // Schedule next retry (exponential backoff)
                    var retryDelayMinutes = Math.Pow(2, reminder.RetryCount) * 5; // 5, 10, 20, 40 minutes
                    reminder.NextRetryAt = DateTimeOffset.UtcNow.AddMinutes(retryDelayMinutes);
                    reminder.IsSent = false; // Allow retry
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reminder {ReminderId} as sent", reminderId);
            return false;
        }
    }

    /// <summary>
    /// Get reminder statistics for a user
    /// </summary>
    public async Task<ReminderStatsDto> GetReminderStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            fromDate ??= DateTimeOffset.UtcNow.AddMonths(-1);
            toDate ??= DateTimeOffset.UtcNow;

            var reminders = await _context.AppointmentReminders
                .Include(r => r.Appointment)
                .Where(r => r.UserId == userId &&
                           r.CreatedAt >= fromDate &&
                           r.CreatedAt <= toDate)
                .ToListAsync();

            var stats = new ReminderStatsDto
            {
                UserId = userId,
                TotalReminders = reminders.Count,
                RemindersDelivered = reminders.Count(r => r.IsDelivered),
                ReminderFailures = reminders.Count(r => r.IsSent && !r.IsDelivered),
                PendingReminders = reminders.Count(r => !r.IsSent && r.IsActive),
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };

            stats.SuccessRate = stats.TotalReminders > 0 ? (double)stats.RemindersDelivered / stats.TotalReminders : 0;

            // Calculate most used reminder type
            var reminderTypes = reminders.GroupBy(r => r.ReminderType)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (reminderTypes != null)
            {
                stats.MostUsedReminderType = EnumConverter.ToReminderType(reminderTypes.Key);
            }

            // Calculate average reminder time
            if (reminders.Any())
            {
                stats.AverageReminderTime = reminders.Average(r => r.MinutesBefore);
            }

            // Build reminder type statistics
            var typeStats = reminders.GroupBy(r => r.ReminderType)
                .Select(g => new ReminderTypeStatsDto
                {
                    ReminderType = EnumConverter.ToReminderType(g.Key),
                    TypeName = g.Key,
                    Count = g.Count(),
                    Delivered = g.Count(r => r.IsDelivered),
                    Failed = g.Count(r => r.IsSent && !r.IsDelivered),
                    SuccessRate = g.Count() > 0 ? (double)g.Count(r => r.IsDelivered) / g.Count() : 0
                })
                .ToList();

            stats.ReminderTypeStats = typeStats;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reminder statistics for user {UserId}", userId);
            return new ReminderStatsDto { UserId = userId };
        }
    }

    /// <summary>
    /// Generate recurring appointment instances
    /// </summary>
    public async Task<int> GenerateRecurringInstancesAsync(string userId, string appointmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, appointmentId))
                return 0;

            return await GenerateRecurringInstancesInternalAsync(appointmentId, fromDate, toDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recurring instances for appointment {AppointmentId}", appointmentId);
            return 0;
        }
    }

    /// <summary>
    /// Get recurring appointment instances
    /// </summary>
    public async Task<List<AppointmentInstanceDto>> GetRecurringInstancesAsync(string userId, string parentAppointmentId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            if (!await HasAppointmentPermissionAsync(userId, parentAppointmentId))
                return new List<AppointmentInstanceDto>();

            var query = _context.AppointmentInstances
                .Include(i => i.ParentAppointment)
                .Where(i => i.ParentAppointmentId == parentAppointmentId);

            if (fromDate.HasValue)
                query = query.Where(i => i.ActualStartTime >= fromDate);

            if (toDate.HasValue)
                query = query.Where(i => i.ActualEndTime <= toDate);

            var instances = await query
                .OrderBy(i => i.InstanceNumber)
                .ToListAsync();

            return instances.Select(MapToInstanceDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recurring instances for appointment {AppointmentId}", parentAppointmentId);
            return new List<AppointmentInstanceDto>();
        }
    }

    /// <summary>
    /// Update recurring appointment instance
    /// </summary>
    public async Task<AppointmentInstanceDto> UpdateRecurringInstanceAsync(string userId, string instanceId, UpdateRecurringInstanceRequest request)
    {
        try
        {
            var instance = await _context.AppointmentInstances
                .Include(i => i.ParentAppointment)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new ArgumentException("Instance not found");

            if (!await HasAppointmentPermissionAsync(userId, instance.ParentAppointmentId))
                throw new UnauthorizedAccessException("You don't have permission to update this instance");

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.CustomTitle))
                instance.CustomTitle = request.CustomTitle;

            if (request.CustomDescription != null)
                instance.CustomDescription = request.CustomDescription;

            if (request.CustomLocation != null)
                instance.CustomLocation = request.CustomLocation;

            if (request.ActualStartTime.HasValue)
                instance.ActualStartTime = request.ActualStartTime.Value;

            if (request.ActualEndTime.HasValue)
                instance.ActualEndTime = request.ActualEndTime.Value;

            if (request.Status.HasValue)
                instance.Status = EnumConverter.ToString(request.Status.Value);

            if (request.Notes != null)
                instance.Notes = request.Notes;

            // Mark as modified if any changes were made
            instance.IsModified = true;
            instance.UpdatedAt = DateTimeOffset.UtcNow;
            instance.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return MapToInstanceDto(instance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recurring instance {InstanceId}", instanceId);
            throw;
        }
    }

    /// <summary>
    /// Cancel recurring appointment instance
    /// </summary>
    public async Task<bool> CancelRecurringInstanceAsync(string userId, string instanceId, string cancellationReason)
    {
        try
        {
            var instance = await _context.AppointmentInstances
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                return false;

            if (!await HasAppointmentPermissionAsync(userId, instance.ParentAppointmentId))
                return false;

            instance.IsCancelled = true;
            instance.Status = EnumConverter.ToString(AppointmentStatus.Cancelled);
            instance.CancellationReason = cancellationReason;
            instance.CancelledAt = DateTimeOffset.UtcNow;
            instance.UpdatedAt = DateTimeOffset.UtcNow;
            instance.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cancelled recurring instance {InstanceId} by user {UserId}: {Reason}", 
                instanceId, userId, cancellationReason);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling recurring instance {InstanceId}", instanceId);
            return false;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Check if user has permission to access appointment
    /// </summary>
    private async Task<bool> HasAppointmentPermissionAsync(string userId, string appointmentId)
    {
        return await _context.CalendarAppointments
            .AnyAsync(a => a.Id == appointmentId && 
                          (a.CreatedByUserId == userId || 
                           a.Participants.Any(p => p.UserId == userId)));
    }

    /// <summary>
    /// Validate appointment creation request
    /// </summary>
    private static List<string> ValidateCreateAppointmentRequest(CreateAppointmentRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Title is required");

        if (request.StartTime >= request.EndTime)
            errors.Add("Start time must be before end time");

        if (request.StartTime < DateTimeOffset.UtcNow.AddMinutes(-5)) // Allow 5 minutes grace period
            errors.Add("Start time cannot be in the past");

        if (request.IsRecurring && !request.RecurrencePattern.HasValue)
            errors.Add("Recurrence pattern is required for recurring appointments");

        if (request.DefaultReminderMinutes < 0)
            errors.Add("Reminder time cannot be negative");

        return errors;
    }

    /// <summary>
    /// Build appointment search query
    /// </summary>
    private IQueryable<CalendarAppointment> BuildAppointmentSearchQuery(string userId, SearchAppointmentsRequest request)
    {
        var query = _context.CalendarAppointments
            .Where(a => a.Participants.Any(p => p.UserId == userId));

        // Apply text search
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerm = request.Query.ToLower();
            query = query.Where(a => 
                a.Title.ToLower().Contains(searchTerm) ||
                a.Description!.ToLower().Contains(searchTerm) ||
                a.Location!.ToLower().Contains(searchTerm));
        }

        // Apply date filters
        if (request.StartDate.HasValue)
            query = query.Where(a => a.StartTime >= request.StartDate);

        if (request.EndDate.HasValue)
            query = query.Where(a => a.EndTime <= request.EndDate);

        // Apply type filter
        if (request.AppointmentType.HasValue)
            query = query.Where(a => a.AppointmentType == EnumConverter.ToString(request.AppointmentType.Value));

        // Apply status filter
        if (request.Status.HasValue)
            query = query.Where(a => a.Status == EnumConverter.ToString(request.Status.Value));

        // Apply priority filter
        if (request.Priority.HasValue)
            query = query.Where(a => a.Priority == EnumConverter.ToString(request.Priority.Value));

        // Apply location filter
        if (!string.IsNullOrWhiteSpace(request.Location))
            query = query.Where(a => a.Location!.ToLower().Contains(request.Location.ToLower()));

        // Apply participant filter
        if (!string.IsNullOrWhiteSpace(request.ParticipantEmail))
            query = query.Where(a => a.Participants.Any(p => p.Email.ToLower().Contains(request.ParticipantEmail.ToLower())));

        // Apply boolean filters
        if (request.IsRecurring.HasValue)
            query = query.Where(a => a.IsRecurring == request.IsRecurring);

        if (request.IsAllDay.HasValue)
            query = query.Where(a => a.IsAllDay == request.IsAllDay);

        if (request.HasReminders.HasValue)
            query = query.Where(a => a.RemindersEnabled == request.HasReminders);

        // Apply color filter
        if (!string.IsNullOrWhiteSpace(request.ColorCode))
            query = query.Where(a => a.ColorCode == request.ColorCode);

        // Apply cancelled filter
        if (!request.IncludeCancelled)
            query = query.Where(a => a.Status != EnumConverter.ToString(AppointmentStatus.Cancelled));

        // Apply past appointments filter
        if (!request.IncludePast)
            query = query.Where(a => a.EndTime >= DateTimeOffset.UtcNow);

        return query;
    }

    /// <summary>
    /// Apply sorting to appointment query
    /// </summary>
    private static IQueryable<CalendarAppointment> ApplySorting(IQueryable<CalendarAppointment> query, AppointmentSortBy sortBy, SortDirection direction)
    {
        return sortBy switch
        {
            AppointmentSortBy.StartTime => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.StartTime) : query.OrderByDescending(a => a.StartTime),
            AppointmentSortBy.EndTime => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.EndTime) : query.OrderByDescending(a => a.EndTime),
            AppointmentSortBy.CreatedAt => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.CreatedAt) : query.OrderByDescending(a => a.CreatedAt),
            AppointmentSortBy.UpdatedAt => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.UpdatedAt) : query.OrderByDescending(a => a.UpdatedAt),
            AppointmentSortBy.Title => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title),
            AppointmentSortBy.AppointmentType => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.AppointmentType) : query.OrderByDescending(a => a.AppointmentType),
            AppointmentSortBy.Status => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status),
            AppointmentSortBy.Priority => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.Priority) : query.OrderByDescending(a => a.Priority),
            AppointmentSortBy.Duration => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.EndTime.Subtract(a.StartTime)) : query.OrderByDescending(a => a.EndTime.Subtract(a.StartTime)),
            AppointmentSortBy.ParticipantCount => direction == SortDirection.Ascending ? 
                query.OrderBy(a => a.Participants.Count) : query.OrderByDescending(a => a.Participants.Count),
            _ => query.OrderBy(a => a.StartTime)
        };
    }

    /// <summary>
    /// Build list of applied filters for search response
    /// </summary>
    private static List<string> BuildAppliedFiltersList(SearchAppointmentsRequest request)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Query))
            filters.Add($"Text: {request.Query}");

        if (request.StartDate.HasValue)
            filters.Add($"From: {request.StartDate:yyyy-MM-dd}");

        if (request.EndDate.HasValue)
            filters.Add($"To: {request.EndDate:yyyy-MM-dd}");

        if (request.AppointmentType.HasValue)
            filters.Add($"Type: {request.AppointmentType}");

        if (request.Status.HasValue)
            filters.Add($"Status: {request.Status}");

        if (request.Priority.HasValue)
            filters.Add($"Priority: {request.Priority}");

        if (!string.IsNullOrWhiteSpace(request.Location))
            filters.Add($"Location: {request.Location}");

        if (!string.IsNullOrWhiteSpace(request.ParticipantEmail))
            filters.Add($"Participant: {request.ParticipantEmail}");

        if (request.IsRecurring.HasValue)
            filters.Add($"Recurring: {request.IsRecurring}");

        if (request.IsAllDay.HasValue)
            filters.Add($"All Day: {request.IsAllDay}");

        if (request.HasReminders.HasValue)
            filters.Add($"Has Reminders: {request.HasReminders}");

        if (!string.IsNullOrWhiteSpace(request.ColorCode))
            filters.Add($"Color: {request.ColorCode}");

        return filters;
    }

    /// <summary>
    /// Generate recurring appointment instances
    /// </summary>
    private async Task<int> GenerateRecurringInstancesInternalAsync(string appointmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        var appointment = await _context.CalendarAppointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.IsRecurring);

        if (appointment == null || string.IsNullOrEmpty(appointment.RecurrencePattern))
            return 0;

        var pattern = EnumConverter.ToRecurrencePattern(appointment.RecurrencePattern);
        var interval = appointment.RecurrenceInterval ?? 1;
        var endDate = appointment.RecurrenceEndDate ?? toDate;
        var maxOccurrences = appointment.MaxOccurrences;

        var instances = new List<AppointmentInstance>();
        var currentDate = appointment.StartTime;
        var instanceNumber = 1;

        while (currentDate <= endDate && currentDate <= toDate)
        {
            if (currentDate >= fromDate)
            {
                var instanceEndTime = currentDate.Add(appointment.EndTime - appointment.StartTime);

                var instance = new AppointmentInstance
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentAppointmentId = appointmentId,
                    InstanceNumber = instanceNumber,
                    OriginalStartTime = currentDate,
                    OriginalEndTime = instanceEndTime,
                    ActualStartTime = currentDate,
                    ActualEndTime = instanceEndTime,
                    Status = appointment.Status,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                instances.Add(instance);
            }

            // Calculate next occurrence
            currentDate = CalculateNextOccurrence(currentDate, pattern, interval);
            instanceNumber++;

            if (maxOccurrences.HasValue && instanceNumber > maxOccurrences)
                break;
        }

        if (instances.Any())
        {
            _context.AppointmentInstances.AddRange(instances);
            await _context.SaveChangesAsync();
        }

        return instances.Count;
    }

    /// <summary>
    /// Calculate next occurrence based on recurrence pattern
    /// </summary>
    private static DateTimeOffset CalculateNextOccurrence(DateTimeOffset currentDate, RecurrencePattern pattern, int interval)
    {
        return pattern switch
        {
            RecurrencePattern.Daily => currentDate.AddDays(interval),
            RecurrencePattern.Weekly => currentDate.AddDays(7 * interval),
            RecurrencePattern.BiWeekly => currentDate.AddDays(14 * interval),
            RecurrencePattern.Monthly => currentDate.AddMonths(interval),
            RecurrencePattern.Quarterly => currentDate.AddMonths(3 * interval),
            RecurrencePattern.Yearly => currentDate.AddYears(interval),
            RecurrencePattern.Weekdays => currentDate.DayOfWeek switch
            {
                DayOfWeek.Friday => currentDate.AddDays(3), // Friday to Monday
                _ => currentDate.AddDays(1) // Any weekday to next day
            },
            _ => currentDate.AddDays(interval)
        };
    }

    /// <summary>
    /// Process user availability for availability check
    /// </summary>
    private async Task<UserAvailabilityDto> ProcessUserAvailabilityAsync(
        string userId, 
        CheckAvailabilityRequest request, 
        List<CalendarAppointment> existingAppointments)
    {
        var userAppointments = existingAppointments
            .Where(a => a.Participants.Any(p => p.UserId == userId))
            .ToList();

        var conflicts = userAppointments.Select(a => new AppointmentConflictDto
        {
            AppointmentId = a.Id,
            Title = a.Title,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = EnumConverter.ToAppointmentStatus(a.Status),
            UserId = userId
        }).ToList();

        var availableSlots = CalculateAvailableSlots(
            request.StartTime, 
            request.EndTime, 
            userAppointments, 
            request.MinimumDurationMinutes);

        var isAvailable = !conflicts.Any(c => !c.IsTentative || request.IncludeTentative);

        return new UserAvailabilityDto
        {
            UserId = userId,
            DisplayName = $"User {userId}", // Could be populated from user service
            IsAvailable = isAvailable,
            AvailableSlots = availableSlots,
            Conflicts = conflicts,
            TimeZone = request.TimeZone
        };
    }

    /// <summary>
    /// Find common available slots for all users
    /// </summary>
    private static List<AvailableTimeSlotDto> FindCommonAvailableSlots(
        List<UserAvailabilityDto> userAvailability, 
        CheckAvailabilityRequest request)
    {
        if (!userAvailability.Any())
            return new List<AvailableTimeSlotDto>();

        // Start with the first user's available slots
        var commonSlots = userAvailability.First().AvailableSlots.ToList();

        // Intersect with each subsequent user's available slots
        foreach (var user in userAvailability.Skip(1))
        {
            var userSlots = user.AvailableSlots;
            var intersectedSlots = new List<AvailableTimeSlotDto>();

            foreach (var commonSlot in commonSlots)
            {
                foreach (var userSlot in userSlots)
                {
                    var intersectionStart = commonSlot.StartTime > userSlot.StartTime ? 
                        commonSlot.StartTime : userSlot.StartTime;
                    var intersectionEnd = commonSlot.EndTime < userSlot.EndTime ? 
                        commonSlot.EndTime : userSlot.EndTime;

                    if (intersectionStart < intersectionEnd && 
                        (intersectionEnd - intersectionStart).TotalMinutes >= request.MinimumDurationMinutes)
                    {
                        intersectedSlots.Add(new AvailableTimeSlotDto
                        {
                            StartTime = intersectionStart,
                            EndTime = intersectionEnd,
                            TimeZone = request.TimeZone
                        });
                    }
                }
            }

            commonSlots = intersectedSlots;
        }

        return commonSlots.OrderBy(s => s.StartTime).ToList();
    }

    /// <summary>
    /// Calculate available time slots for a user
    /// </summary>
    private static List<AvailableTimeSlotDto> CalculateAvailableSlots(
        DateTimeOffset startTime, 
        DateTimeOffset endTime, 
        List<CalendarAppointment> appointments, 
        int minimumDurationMinutes)
    {
        var slots = new List<AvailableTimeSlotDto>();
        var sortedAppointments = appointments
            .Where(a => a.Status != EnumConverter.ToString(AppointmentStatus.Cancelled))
            .OrderBy(a => a.StartTime)
            .ToList();

        var currentTime = startTime;

        foreach (var appointment in sortedAppointments)
        {
            if (appointment.StartTime > currentTime)
            {
                var slotDuration = (appointment.StartTime - currentTime).TotalMinutes;
                if (slotDuration >= minimumDurationMinutes)
                {
                    slots.Add(new AvailableTimeSlotDto
                    {
                        StartTime = currentTime,
                        EndTime = appointment.StartTime,
                        TimeZone = "UTC"
                    });
                }
            }

            currentTime = appointment.EndTime > currentTime ? appointment.EndTime : currentTime;
        }

        // Add final slot if there's time remaining
        if (currentTime < endTime)
        {
            var finalSlotDuration = (endTime - currentTime).TotalMinutes;
            if (finalSlotDuration >= minimumDurationMinutes)
            {
                slots.Add(new AvailableTimeSlotDto
                {
                    StartTime = currentTime,
                    EndTime = endTime,
                    TimeZone = "UTC"
                });
            }
        }

        return slots;
    }

    /// <summary>
    /// Generate alternative time slots when no common availability found
    /// </summary>
    private async Task<List<AvailableTimeSlotDto>> GenerateAlternativeTimeSlotsAsync(
        List<string> userIds, 
        DateTimeOffset originalStart, 
        DateTimeOffset originalEnd, 
        int minimumDurationMinutes)
    {
        var alternatives = new List<AvailableTimeSlotDto>();
        var duration = originalEnd - originalStart;

        // Suggest slots in the next 7 days
        for (int days = 1; days <= 7; days++)
        {
            var newStart = originalStart.AddDays(days);
            var newEnd = newStart.Add(duration);

            var checkRequest = new CheckAvailabilityRequest
            {
                UserIds = userIds,
                StartTime = newStart,
                EndTime = newEnd,
                MinimumDurationMinutes = minimumDurationMinutes
            };

            var availability = await CheckAvailabilityAsync(checkRequest);
            alternatives.AddRange(availability.CommonAvailableSlots.Take(2)); // Add up to 2 slots per day

            if (alternatives.Count >= 10) // Limit to 10 alternatives
                break;
        }

        return alternatives.Take(10).ToList();
    }

    /// <summary>
    /// Map calendar appointment entity to DTO
    /// </summary>
    private async Task<CalendarAppointmentDto> MapToAppointmentDtoAsync(CalendarAppointment appointment)
    {
        var participants = appointment.Participants?.Select(MapToParticipantDto).ToList() ?? new List<AppointmentParticipantDto>();
        var reminders = appointment.Reminders?.Select(MapToReminderDto).ToList() ?? new List<AppointmentReminderDto>();

        return new CalendarAppointmentDto
        {
            Id = appointment.Id,
            CreatedByUserId = appointment.CreatedByUserId,
            ConversationId = appointment.ConversationId,
            Title = appointment.Title,
            Description = appointment.Description,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            TimeZone = appointment.TimeZone,
            Location = appointment.Location,
            MeetingUrl = appointment.MeetingUrl,
            AppointmentType = EnumConverter.ToAppointmentType(appointment.AppointmentType),
            Status = EnumConverter.ToAppointmentStatus(appointment.Status),
            Priority = EnumConverter.ToAppointmentPriority(appointment.Priority),
            IsAllDay = appointment.IsAllDay,
            IsRecurring = appointment.IsRecurring,
            RecurrencePattern = !string.IsNullOrEmpty(appointment.RecurrencePattern) ? 
                EnumConverter.ToRecurrencePattern(appointment.RecurrencePattern) : null,
            RecurrenceInterval = appointment.RecurrenceInterval,
            RecurrenceEndDate = appointment.RecurrenceEndDate,
            MaxOccurrences = appointment.MaxOccurrences,
            ExternalEventId = appointment.ExternalEventId,
            ExternalProvider = !string.IsNullOrEmpty(appointment.ExternalProvider) ? 
                EnumConverter.ToCalendarProvider(appointment.ExternalProvider) : null,
            RemindersEnabled = appointment.RemindersEnabled,
            DefaultReminderMinutes = appointment.DefaultReminderMinutes,
            ColorCode = appointment.ColorCode,
            Notes = appointment.Notes,
            AttachmentFiles = !string.IsNullOrEmpty(appointment.AttachmentFiles) ? 
                appointment.AttachmentFiles.Split(',').ToList() : new List<string>(),
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt,
            UpdatedByUserId = appointment.UpdatedByUserId,
            CancelledAt = appointment.CancelledAt,
            CancellationReason = appointment.CancellationReason,
            Participants = participants,
            Reminders = reminders
        };
    }

    /// <summary>
    /// Map appointment participant entity to DTO
    /// </summary>
    private static AppointmentParticipantDto MapToParticipantDto(AppointmentParticipant participant)
    {
        return new AppointmentParticipantDto
        {
            Id = participant.Id,
            AppointmentId = participant.AppointmentId,
            UserId = participant.UserId,
            Email = participant.Email,
            DisplayName = participant.DisplayName,
            Role = EnumConverter.ToParticipantRole(participant.Role),
            ResponseStatus = EnumConverter.ToResponseStatus(participant.ResponseStatus),
            IsRequired = participant.IsRequired,
            IsOrganizer = participant.IsOrganizer,
            InvitedAt = participant.InvitedAt,
            RespondedAt = participant.RespondedAt,
            ResponseMessage = participant.ResponseMessage,
            ExternalParticipantId = participant.ExternalParticipantId,
            TimeZone = participant.TimeZone,
            Notes = participant.Notes
        };
    }

    /// <summary>
    /// Map appointment reminder entity to DTO
    /// </summary>
    private static AppointmentReminderDto MapToReminderDto(AppointmentReminder reminder)
    {
        return new AppointmentReminderDto
        {
            Id = reminder.Id,
            AppointmentId = reminder.AppointmentId,
            UserId = reminder.UserId,
            ReminderType = EnumConverter.ToReminderType(reminder.ReminderType),
            MinutesBefore = reminder.MinutesBefore,
            ReminderTime = reminder.ReminderTime,
            IsSent = reminder.IsSent,
            SentAt = reminder.SentAt,
            IsDelivered = reminder.IsDelivered,
            DeliveryMethod = reminder.DeliveryMethod,
            DeliveryError = reminder.DeliveryError,
            RetryCount = reminder.RetryCount,
            MaxRetries = reminder.MaxRetries,
            NextRetryAt = reminder.NextRetryAt,
            CustomMessage = reminder.CustomMessage,
            IsActive = reminder.IsActive,
            CreatedAt = reminder.CreatedAt
        };
    }

    /// <summary>
    /// Map appointment instance entity to DTO
    /// </summary>
    private static AppointmentInstanceDto MapToInstanceDto(AppointmentInstance instance)
    {
        return new AppointmentInstanceDto
        {
            Id = instance.Id,
            ParentAppointmentId = instance.ParentAppointmentId,
            InstanceNumber = instance.InstanceNumber,
            OriginalStartTime = instance.OriginalStartTime,
            OriginalEndTime = instance.OriginalEndTime,
            ActualStartTime = instance.ActualStartTime,
            ActualEndTime = instance.ActualEndTime,
            Status = EnumConverter.ToAppointmentStatus(instance.Status),
            IsModified = instance.IsModified,
            IsCancelled = instance.IsCancelled,
            CustomTitle = instance.CustomTitle,
            CustomDescription = instance.CustomDescription,
            CustomLocation = instance.CustomLocation,
            ExternalEventId = instance.ExternalEventId,
            CreatedAt = instance.CreatedAt,
            UpdatedAt = instance.UpdatedAt,
            UpdatedByUserId = instance.UpdatedByUserId,
            CancellationReason = instance.CancellationReason,
            CancelledAt = instance.CancelledAt,
            Notes = instance.Notes
        };
    }

    #endregion
}