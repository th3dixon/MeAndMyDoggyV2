using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Helpers;

/// <summary>
/// Helper class for converting between enums and strings for database compatibility
/// </summary>
public static class EnumConverter
{
    /// <summary>
    /// Convert MessageType enum to string
    /// </summary>
    public static string ToString(MessageType messageType)
    {
        return messageType.ToString();
    }
    
    /// <summary>
    /// Convert string to MessageType enum
    /// </summary>
    public static MessageType ToMessageType(string messageType)
    {
        return Enum.TryParse<MessageType>(messageType, true, out var result) ? result : MessageType.Text;
    }
    
    /// <summary>
    /// Convert MessageStatus enum to string
    /// </summary>
    public static string ToString(MessageStatus messageStatus)
    {
        return messageStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to MessageStatus enum
    /// </summary>
    public static MessageStatus ToMessageStatus(string messageStatus)
    {
        return Enum.TryParse<MessageStatus>(messageStatus, true, out var result) ? result : MessageStatus.Sent;
    }
    
    /// <summary>
    /// Convert ConversationType enum to string
    /// </summary>
    public static string ToString(ConversationType conversationType)
    {
        return conversationType.ToString();
    }
    
    /// <summary>
    /// Convert SearchSortBy enum to string
    /// </summary>
    public static string ToString(SearchSortBy searchSortBy)
    {
        return searchSortBy.ToString();
    }
    
    /// <summary>
    /// Convert string to SearchSortBy enum
    /// </summary>
    public static SearchSortBy ToSearchSortBy(string searchSortBy)
    {
        return Enum.TryParse<SearchSortBy>(searchSortBy, true, out var result) ? result : SearchSortBy.Relevance;
    }
    
    /// <summary>
    /// Convert SearchDateRange enum to string
    /// </summary>
    public static string ToString(SearchDateRange searchDateRange)
    {
        return searchDateRange.ToString();
    }
    
    /// <summary>
    /// Convert string to SearchDateRange enum
    /// </summary>
    public static SearchDateRange ToSearchDateRange(string searchDateRange)
    {
        return Enum.TryParse<SearchDateRange>(searchDateRange, true, out var result) ? result : SearchDateRange.AllTime;
    }
    
    /// <summary>
    /// Convert LocationShareType enum to string
    /// </summary>
    public static string ToString(LocationShareType locationShareType)
    {
        return locationShareType.ToString();
    }
    
    /// <summary>
    /// Convert string to LocationShareType enum
    /// </summary>
    public static LocationShareType ToLocationShareType(string locationShareType)
    {
        return Enum.TryParse<LocationShareType>(locationShareType, true, out var result) ? result : LocationShareType.Static;
    }
    
    /// <summary>
    /// Convert LocationVisibility enum to string
    /// </summary>
    public static string ToString(LocationVisibility locationVisibility)
    {
        return locationVisibility.ToString();
    }
    
    /// <summary>
    /// Convert string to LocationVisibility enum
    /// </summary>
    public static LocationVisibility ToLocationVisibility(string locationVisibility)
    {
        return Enum.TryParse<LocationVisibility>(locationVisibility, true, out var result) ? result : LocationVisibility.Conversation;
    }
    
    /// <summary>
    /// Convert LocationCategory enum to string
    /// </summary>
    public static string ToString(LocationCategory locationCategory)
    {
        return locationCategory.ToString();
    }
    
    /// <summary>
    /// Convert string to LocationCategory enum
    /// </summary>
    public static LocationCategory ToLocationCategory(string locationCategory)
    {
        return Enum.TryParse<LocationCategory>(locationCategory, true, out var result) ? result : LocationCategory.General;
    }
    
    /// <summary>
    /// Convert string to ConversationType enum
    /// </summary>
    public static ConversationType ToConversationType(string conversationType)
    {
        return Enum.TryParse<ConversationType>(conversationType, true, out var result) ? result : ConversationType.Direct;
    }
    
    /// <summary>
    /// Convert ConversationRole enum to string
    /// </summary>
    public static string ToString(ConversationRole conversationRole)
    {
        return conversationRole.ToString();
    }
    
    /// <summary>
    /// Convert string to ConversationRole enum
    /// </summary>
    public static ConversationRole ToConversationRole(string conversationRole)
    {
        return Enum.TryParse<ConversationRole>(conversationRole, true, out var result) ? result : ConversationRole.Member;
    }
    
    /// <summary>
    /// Convert AttachmentType enum to string
    /// </summary>
    public static string ToString(AttachmentType attachmentType)
    {
        return attachmentType.ToString();
    }
    
    /// <summary>
    /// Convert string to AttachmentType enum
    /// </summary>
    public static AttachmentType ToAttachmentType(string attachmentType)
    {
        return Enum.TryParse<AttachmentType>(attachmentType, true, out var result) ? result : AttachmentType.File;
    }
    
    /// <summary>
    /// Convert CallStatus enum to string
    /// </summary>
    public static string ToString(CallStatus callStatus)
    {
        return callStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to CallStatus enum
    /// </summary>
    public static CallStatus ToCallStatus(string callStatus)
    {
        return Enum.TryParse<CallStatus>(callStatus, true, out var result) ? result : CallStatus.Pending;
    }
    
    /// <summary>
    /// Convert ConnectionStatus enum to string
    /// </summary>
    public static string ToString(ConnectionStatus connectionStatus)
    {
        return connectionStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to ConnectionStatus enum
    /// </summary>
    public static ConnectionStatus ToConnectionStatus(string connectionStatus)
    {
        return Enum.TryParse<ConnectionStatus>(connectionStatus, true, out var result) ? result : ConnectionStatus.Connecting;
    }
    
    /// <summary>
    /// Convert NetworkQuality enum to string
    /// </summary>
    public static string ToString(NetworkQuality networkQuality)
    {
        return networkQuality.ToString();
    }
    
    /// <summary>
    /// Convert string to NetworkQuality enum
    /// </summary>
    public static NetworkQuality ToNetworkQuality(string networkQuality)
    {
        return Enum.TryParse<NetworkQuality>(networkQuality, true, out var result) ? result : NetworkQuality.Unknown;
    }
    
    /// <summary>
    /// Convert NotificationType enum to string
    /// </summary>
    public static string ToString(NotificationType notificationType)
    {
        return notificationType.ToString();
    }
    
    /// <summary>
    /// Convert string to NotificationType enum
    /// </summary>
    public static NotificationType ToNotificationType(string notificationType)
    {
        return Enum.TryParse<NotificationType>(notificationType, true, out var result) ? result : NotificationType.General;
    }
    
    /// <summary>
    /// Convert NotificationStatus enum to string
    /// </summary>
    public static string ToString(NotificationStatus notificationStatus)
    {
        return notificationStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to NotificationStatus enum
    /// </summary>
    public static NotificationStatus ToNotificationStatus(string notificationStatus)
    {
        return Enum.TryParse<NotificationStatus>(notificationStatus, true, out var result) ? result : NotificationStatus.Pending;
    }
    
    /// <summary>
    /// Convert NotificationPlatform enum to string
    /// </summary>
    public static string ToString(NotificationPlatform notificationPlatform)
    {
        return notificationPlatform.ToString();
    }
    
    /// <summary>
    /// Convert string to NotificationPlatform enum
    /// </summary>
    public static NotificationPlatform ToNotificationPlatform(string notificationPlatform)
    {
        return Enum.TryParse<NotificationPlatform>(notificationPlatform, true, out var result) ? result : NotificationPlatform.Android;
    }
    
    /// <summary>
    /// Convert NotificationPriority enum to string
    /// </summary>
    public static string ToString(NotificationPriority notificationPriority)
    {
        return notificationPriority.ToString();
    }
    
    /// <summary>
    /// Convert string to NotificationPriority enum
    /// </summary>
    public static NotificationPriority ToNotificationPriority(string notificationPriority)
    {
        return Enum.TryParse<NotificationPriority>(notificationPriority, true, out var result) ? result : NotificationPriority.Normal;
    }
    
    /// <summary>
    /// Convert DeliveryStatus enum to string
    /// </summary>
    public static string ToString(DeliveryStatus deliveryStatus)
    {
        return deliveryStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to DeliveryStatus enum
    /// </summary>
    public static DeliveryStatus ToDeliveryStatus(string deliveryStatus)
    {
        return Enum.TryParse<DeliveryStatus>(deliveryStatus, true, out var result) ? result : DeliveryStatus.Pending;
    }
    
    /// <summary>
    /// Convert ScheduledMessageStatus enum to string
    /// </summary>
    public static string ToString(ScheduledMessageStatus scheduledMessageStatus)
    {
        return scheduledMessageStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to ScheduledMessageStatus enum
    /// </summary>
    public static ScheduledMessageStatus ToScheduledMessageStatus(string scheduledMessageStatus)
    {
        return Enum.TryParse<ScheduledMessageStatus>(scheduledMessageStatus, true, out var result) ? result : ScheduledMessageStatus.Pending;
    }
    
    /// <summary>
    /// Convert RecurrenceType enum to string
    /// </summary>
    public static string ToString(RecurrenceType recurrenceType)
    {
        return recurrenceType.ToString();
    }
    
    /// <summary>
    /// Convert string to RecurrenceType enum
    /// </summary>
    public static RecurrenceType ToRecurrenceType(string recurrenceType)
    {
        return Enum.TryParse<RecurrenceType>(recurrenceType, true, out var result) ? result : RecurrenceType.None;
    }
    
    /// <summary>
    /// Convert AppointmentType enum to string
    /// </summary>
    public static string ToString(AppointmentType appointmentType)
    {
        return appointmentType.ToString();
    }
    
    /// <summary>
    /// Convert string to AppointmentType enum
    /// </summary>
    public static AppointmentType ToAppointmentType(string appointmentType)
    {
        return Enum.TryParse<AppointmentType>(appointmentType, true, out var result) ? result : AppointmentType.Meeting;
    }
    
    /// <summary>
    /// Convert AppointmentStatus enum to string
    /// </summary>
    public static string ToString(AppointmentStatus appointmentStatus)
    {
        return appointmentStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to AppointmentStatus enum
    /// </summary>
    public static AppointmentStatus ToAppointmentStatus(string appointmentStatus)
    {
        return Enum.TryParse<AppointmentStatus>(appointmentStatus, true, out var result) ? result : AppointmentStatus.Scheduled;
    }
    
    /// <summary>
    /// Convert AppointmentPriority enum to string
    /// </summary>
    public static string ToString(AppointmentPriority appointmentPriority)
    {
        return appointmentPriority.ToString();
    }
    
    /// <summary>
    /// Convert string to AppointmentPriority enum
    /// </summary>
    public static AppointmentPriority ToAppointmentPriority(string appointmentPriority)
    {
        return Enum.TryParse<AppointmentPriority>(appointmentPriority, true, out var result) ? result : AppointmentPriority.Normal;
    }
    
    /// <summary>
    /// Convert ParticipantRole enum to string
    /// </summary>
    public static string ToString(ParticipantRole participantRole)
    {
        return participantRole.ToString();
    }
    
    /// <summary>
    /// Convert string to ParticipantRole enum
    /// </summary>
    public static ParticipantRole ToParticipantRole(string participantRole)
    {
        return Enum.TryParse<ParticipantRole>(participantRole, true, out var result) ? result : ParticipantRole.RequiredAttendee;
    }
    
    /// <summary>
    /// Convert ResponseStatus enum to string
    /// </summary>
    public static string ToString(ResponseStatus responseStatus)
    {
        return responseStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to ResponseStatus enum
    /// </summary>
    public static ResponseStatus ToResponseStatus(string responseStatus)
    {
        return Enum.TryParse<ResponseStatus>(responseStatus, true, out var result) ? result : ResponseStatus.Pending;
    }
    
    /// <summary>
    /// Convert ReminderType enum to string
    /// </summary>
    public static string ToString(ReminderType reminderType)
    {
        return reminderType.ToString();
    }
    
    /// <summary>
    /// Convert string to ReminderType enum
    /// </summary>
    public static ReminderType ToReminderType(string reminderType)
    {
        return Enum.TryParse<ReminderType>(reminderType, true, out var result) ? result : ReminderType.Email;
    }
    
    /// <summary>
    /// Convert RecurrencePattern enum to string
    /// </summary>
    public static string ToString(RecurrencePattern recurrencePattern)
    {
        return recurrencePattern.ToString();
    }
    
    /// <summary>
    /// Convert string to RecurrencePattern enum
    /// </summary>
    public static RecurrencePattern ToRecurrencePattern(string recurrencePattern)
    {
        return Enum.TryParse<RecurrencePattern>(recurrencePattern, true, out var result) ? result : RecurrencePattern.None;
    }
    
    /// <summary>
    /// Convert CalendarProvider enum to string
    /// </summary>
    public static string ToString(CalendarProvider calendarProvider)
    {
        return calendarProvider.ToString();
    }
    
    /// <summary>
    /// Convert string to CalendarProvider enum
    /// </summary>
    public static CalendarProvider ToCalendarProvider(string calendarProvider)
    {
        return Enum.TryParse<CalendarProvider>(calendarProvider, true, out var result) ? result : CalendarProvider.Internal;
    }
    
    /// <summary>
    /// Convert SyncDirection enum to string
    /// </summary>
    public static string ToString(SyncDirection syncDirection)
    {
        return syncDirection.ToString();
    }
    
    /// <summary>
    /// Convert string to SyncDirection enum
    /// </summary>
    public static SyncDirection ToSyncDirection(string syncDirection)
    {
        return Enum.TryParse<SyncDirection>(syncDirection, true, out var result) ? result : SyncDirection.Bidirectional;
    }
    
    /// <summary>
    /// Convert DestructMode enum to string
    /// </summary>
    public static string ToString(DestructMode destructMode)
    {
        return destructMode.ToString();
    }
    
    /// <summary>
    /// Convert string to DestructMode enum
    /// </summary>
    public static DestructMode ToDestructMode(string destructMode)
    {
        return Enum.TryParse<DestructMode>(destructMode, true, out var result) ? result : DestructMode.None;
    }
    
    /// <summary>
    /// Convert TriggerEvent enum to string
    /// </summary>
    public static string ToString(TriggerEvent triggerEvent)
    {
        return triggerEvent.ToString();
    }
    
    /// <summary>
    /// Convert string to TriggerEvent enum
    /// </summary>
    public static TriggerEvent ToTriggerEvent(string triggerEvent)
    {
        return Enum.TryParse<TriggerEvent>(triggerEvent, true, out var result) ? result : TriggerEvent.MessageSent;
    }
    
    /// <summary>
    /// Convert SecurityLevel enum to string
    /// </summary>
    public static string ToString(SecurityLevel securityLevel)
    {
        return securityLevel.ToString();
    }
    
    /// <summary>
    /// Convert string to SecurityLevel enum
    /// </summary>
    public static SecurityLevel ToSecurityLevel(string securityLevel)
    {
        return Enum.TryParse<SecurityLevel>(securityLevel, true, out var result) ? result : SecurityLevel.Standard;
    }
    
    /// <summary>
    /// Convert VerificationMethod enum to string
    /// </summary>
    public static string ToString(VerificationMethod verificationMethod)
    {
        return verificationMethod.ToString();
    }
    
    /// <summary>
    /// Convert string to VerificationMethod enum
    /// </summary>
    public static VerificationMethod ToVerificationMethod(string verificationMethod)
    {
        return Enum.TryParse<VerificationMethod>(verificationMethod, true, out var result) ? result : VerificationMethod.None;
    }
    
    /// <summary>
    /// Convert IncidentType enum to string
    /// </summary>
    public static string ToString(IncidentType incidentType)
    {
        return incidentType.ToString();
    }
    
    /// <summary>
    /// Convert string to IncidentType enum
    /// </summary>
    public static IncidentType ToIncidentType(string incidentType)
    {
        return Enum.TryParse<IncidentType>(incidentType, true, out var result) ? result : IncidentType.Other;
    }
    
    /// <summary>
    /// Convert IncidentSeverity enum to string
    /// </summary>
    public static string ToString(IncidentSeverity incidentSeverity)
    {
        return incidentSeverity.ToString();
    }
    
    /// <summary>
    /// Convert string to IncidentSeverity enum
    /// </summary>
    public static IncidentSeverity ToIncidentSeverity(string incidentSeverity)
    {
        return Enum.TryParse<IncidentSeverity>(incidentSeverity, true, out var result) ? result : IncidentSeverity.Medium;
    }
    
    /// <summary>
    /// Convert IncidentStatus enum to string
    /// </summary>
    public static string ToString(IncidentStatus incidentStatus)
    {
        return incidentStatus.ToString();
    }
    
    /// <summary>
    /// Convert string to IncidentStatus enum
    /// </summary>
    public static IncidentStatus ToIncidentStatus(string incidentStatus)
    {
        return Enum.TryParse<IncidentStatus>(incidentStatus, true, out var result) ? result : IncidentStatus.New;
    }
    
    /// <summary>
    /// Convert TemplateCategory enum to string
    /// </summary>
    public static string ToString(TemplateCategory templateCategory)
    {
        return templateCategory.ToString();
    }
    
    /// <summary>
    /// Convert string to TemplateCategory enum
    /// </summary>
    public static TemplateCategory ToTemplateCategory(string templateCategory)
    {
        return Enum.TryParse<TemplateCategory>(templateCategory, true, out var result) ? result : TemplateCategory.General;
    }
    
    /// <summary>
    /// Convert TranslationProvider enum to string
    /// </summary>
    public static string ToString(TranslationProvider translationProvider)
    {
        return translationProvider.ToString();
    }
    
    /// <summary>
    /// Convert string to TranslationProvider enum
    /// </summary>
    public static TranslationProvider ToTranslationProvider(string translationProvider)
    {
        return Enum.TryParse<TranslationProvider>(translationProvider, true, out var result) ? result : TranslationProvider.Azure;
    }
    
    /// <summary>
    /// Convert TranslationMethod enum to string
    /// </summary>
    public static string ToString(TranslationMethod translationMethod)
    {
        return translationMethod.ToString();
    }
    
    /// <summary>
    /// Convert string to TranslationMethod enum
    /// </summary>
    public static TranslationMethod ToTranslationMethod(string translationMethod)
    {
        return Enum.TryParse<TranslationMethod>(translationMethod, true, out var result) ? result : TranslationMethod.Automatic;
    }
}