using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for calendar and appointment functionality
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Create a new appointment
    /// </summary>
    /// <param name="userId">User creating the appointment</param>
    /// <param name="request">Appointment creation request</param>
    /// <returns>Created appointment response</returns>
    Task<AppointmentResponse> CreateAppointmentAsync(string userId, CreateAppointmentRequest request);

    /// <summary>
    /// Update an existing appointment
    /// </summary>
    /// <param name="userId">User updating the appointment</param>
    /// <param name="appointmentId">Appointment ID to update</param>
    /// <param name="request">Update request details</param>
    /// <returns>Updated appointment response</returns>
    Task<AppointmentResponse> UpdateAppointmentAsync(string userId, string appointmentId, UpdateAppointmentRequest request);

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    /// <param name="userId">User requesting the appointment</param>
    /// <param name="appointmentId">Appointment ID</param>
    /// <returns>Appointment details or null if not found</returns>
    Task<CalendarAppointmentDto?> GetAppointmentAsync(string userId, string appointmentId);

    /// <summary>
    /// Delete an appointment
    /// </summary>
    /// <param name="userId">User deleting the appointment</param>
    /// <param name="appointmentId">Appointment ID to delete</param>
    /// <param name="cancellationReason">Reason for cancellation</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAppointmentAsync(string userId, string appointmentId, string? cancellationReason = null);

    /// <summary>
    /// Cancel an appointment
    /// </summary>
    /// <param name="userId">User cancelling the appointment</param>
    /// <param name="appointmentId">Appointment ID to cancel</param>
    /// <param name="cancellationReason">Reason for cancellation</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelAppointmentAsync(string userId, string appointmentId, string cancellationReason);

    /// <summary>
    /// Search appointments with filters and pagination
    /// </summary>
    /// <param name="userId">User performing the search</param>
    /// <param name="request">Search request with filters</param>
    /// <returns>Search results</returns>
    Task<SearchAppointmentsResponse> SearchAppointmentsAsync(string userId, SearchAppointmentsRequest request);

    /// <summary>
    /// Get appointments for a date range
    /// </summary>
    /// <param name="userId">User requesting appointments</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="timeZone">Time zone for date filtering</param>
    /// <returns>List of appointments in the date range</returns>
    Task<List<CalendarAppointmentDto>> GetAppointmentsByDateRangeAsync(string userId, DateTimeOffset startDate, DateTimeOffset endDate, string timeZone = "UTC");

    /// <summary>
    /// Get upcoming appointments for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="limit">Maximum number of appointments to return</param>
    /// <returns>List of upcoming appointments</returns>
    Task<List<CalendarAppointmentDto>> GetUpcomingAppointmentsAsync(string userId, int limit = 10);

    /// <summary>
    /// Check availability for users in a time range
    /// </summary>
    /// <param name="request">Availability check request</param>
    /// <returns>Availability information</returns>
    Task<CheckAvailabilityResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request);

    /// <summary>
    /// Add participant to appointment
    /// </summary>
    /// <param name="userId">User adding the participant</param>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="request">Participant details</param>
    /// <returns>Participant response</returns>
    Task<ParticipantResponse> AddParticipantAsync(string userId, string appointmentId, CreateParticipantRequest request);

    /// <summary>
    /// Update participant details
    /// </summary>
    /// <param name="userId">User updating the participant</param>
    /// <param name="participantId">Participant ID</param>
    /// <param name="request">Updated participant details</param>
    /// <returns>Participant response</returns>
    Task<ParticipantResponse> UpdateParticipantAsync(string userId, string participantId, UpdateParticipantRequest request);

    /// <summary>
    /// Remove participant from appointment
    /// </summary>
    /// <param name="userId">User removing the participant</param>
    /// <param name="participantId">Participant ID to remove</param>
    /// <returns>True if removed successfully</returns>
    Task<bool> RemoveParticipantAsync(string userId, string participantId);

    /// <summary>
    /// Respond to appointment invitation
    /// </summary>
    /// <param name="userId">User responding to invitation</param>
    /// <param name="participantId">Participant ID</param>
    /// <param name="request">Response details</param>
    /// <returns>Participant response</returns>
    Task<ParticipantResponse> RespondToInvitationAsync(string userId, string participantId, RespondToInvitationRequest request);

    /// <summary>
    /// Get participant details
    /// </summary>
    /// <param name="userId">User requesting participant details</param>
    /// <param name="participantId">Participant ID</param>
    /// <returns>Participant details or null if not found</returns>
    Task<AppointmentParticipantDto?> GetParticipantAsync(string userId, string participantId);

    /// <summary>
    /// Add reminder to appointment
    /// </summary>
    /// <param name="userId">User adding the reminder</param>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="request">Reminder details</param>
    /// <returns>Reminder response</returns>
    Task<ReminderResponse> AddReminderAsync(string userId, string appointmentId, CreateReminderRequest request);

    /// <summary>
    /// Update reminder details
    /// </summary>
    /// <param name="userId">User updating the reminder</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <param name="request">Updated reminder details</param>
    /// <returns>Reminder response</returns>
    Task<ReminderResponse> UpdateReminderAsync(string userId, string reminderId, UpdateReminderRequest request);

    /// <summary>
    /// Delete reminder
    /// </summary>
    /// <param name="userId">User deleting the reminder</param>
    /// <param name="reminderId">Reminder ID to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteReminderAsync(string userId, string reminderId);

    /// <summary>
    /// Get reminder details
    /// </summary>
    /// <param name="userId">User requesting reminder details</param>
    /// <param name="reminderId">Reminder ID</param>
    /// <returns>Reminder details or null if not found</returns>
    Task<AppointmentReminderDto?> GetReminderAsync(string userId, string reminderId);

    /// <summary>
    /// Get reminders for an appointment
    /// </summary>
    /// <param name="userId">User requesting reminders</param>
    /// <param name="appointmentId">Appointment ID</param>
    /// <returns>List of reminders for the appointment</returns>
    Task<List<AppointmentReminderDto>> GetAppointmentRemindersAsync(string userId, string appointmentId);

    /// <summary>
    /// Get pending reminders that need to be sent
    /// </summary>
    /// <param name="beforeTime">Get reminders that should be sent before this time</param>
    /// <returns>List of pending reminders</returns>
    Task<List<AppointmentReminderDto>> GetPendingRemindersAsync(DateTimeOffset beforeTime);

    /// <summary>
    /// Mark reminder as sent
    /// </summary>
    /// <param name="reminderId">Reminder ID</param>
    /// <param name="sentAt">When the reminder was sent</param>
    /// <param name="deliveryMethod">Method used to deliver reminder</param>
    /// <param name="isDelivered">Whether delivery was successful</param>
    /// <param name="error">Error message if delivery failed</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> MarkReminderSentAsync(string reminderId, DateTimeOffset sentAt, string deliveryMethod, bool isDelivered, string? error = null);

    /// <summary>
    /// Get reminder statistics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Statistics from date</param>
    /// <param name="toDate">Statistics to date</param>
    /// <returns>Reminder statistics</returns>
    Task<ReminderStatsDto> GetReminderStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

    /// <summary>
    /// Generate recurring appointment instances
    /// </summary>
    /// <param name="userId">User generating instances</param>
    /// <param name="appointmentId">Recurring appointment ID</param>
    /// <param name="fromDate">Generate instances from this date</param>
    /// <param name="toDate">Generate instances until this date</param>
    /// <returns>Number of instances generated</returns>
    Task<int> GenerateRecurringInstancesAsync(string userId, string appointmentId, DateTimeOffset fromDate, DateTimeOffset toDate);

    /// <summary>
    /// Get recurring appointment instances
    /// </summary>
    /// <param name="userId">User requesting instances</param>
    /// <param name="parentAppointmentId">Parent appointment ID</param>
    /// <param name="fromDate">From date filter</param>
    /// <param name="toDate">To date filter</param>
    /// <returns>List of appointment instances</returns>
    Task<List<AppointmentInstanceDto>> GetRecurringInstancesAsync(string userId, string parentAppointmentId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

    /// <summary>
    /// Update recurring appointment instance
    /// </summary>
    /// <param name="userId">User updating the instance</param>
    /// <param name="instanceId">Instance ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated instance</returns>
    Task<AppointmentInstanceDto> UpdateRecurringInstanceAsync(string userId, string instanceId, UpdateRecurringInstanceRequest request);

    /// <summary>
    /// Cancel recurring appointment instance
    /// </summary>
    /// <param name="userId">User cancelling the instance</param>
    /// <param name="instanceId">Instance ID to cancel</param>
    /// <param name="cancellationReason">Reason for cancellation</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelRecurringInstanceAsync(string userId, string instanceId, string cancellationReason);
}