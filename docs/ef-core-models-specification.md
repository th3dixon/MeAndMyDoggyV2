# Entity Framework Core Models Specification

## Overview

This document provides the complete Entity Framework Core model definitions for MeAndMyDoggyV2, including all entity classes, configurations, and relationships required for the unified architecture.

## Entity Definitions

### Phase 1: Foundation & Security Entities

#### Enhanced ApplicationUser
```csharp
// src/API/MeAndMyDog.Domain/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Enhanced profile information
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string PreferredLanguage { get; set; } = "en";
    
    // Notification preferences
    public bool IsEmailNotificationsEnabled { get; set; } = true;
    public bool IsSmsNotificationsEnabled { get; set; } = false;
    
    // Account status and activity
    public DateTimeOffset? LastSeenAt { get; set; }
    public string AccountStatus { get; set; } = "Active"; // Active, Suspended, Pending, Deactivated
    
    // KYC and subscription status
    public bool IsKYCVerified { get; set; } = false;
    public DateTimeOffset? KYCCompletedAt { get; set; }
    public string SubscriptionType { get; set; } = "Free"; // Free, Premium
    
    // Activity tracking
    public DateTimeOffset? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Timestamps
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<DogProfile> DogProfiles { get; set; } = new List<DogProfile>();
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<KYCVerification> KYCVerifications { get; set; } = new List<KYCVerification>();
    public virtual ICollection<Conversation> CreatedConversations { get; set; } = new List<Conversation>();
    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<AIHealthRecommendation> AIHealthRecommendations { get; set; } = new List<AIHealthRecommendation>();
    public virtual ICollection<Appointment> PetOwnerAppointments { get; set; } = new List<Appointment>();
    public virtual ICollection<ServiceProviderReview> ReceivedReviews { get; set; } = new List<ServiceProviderReview>();
    public virtual ICollection<ServiceProviderReview> WrittenReviews { get; set; } = new List<ServiceProviderReview>();
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public virtual ICollection<UserSetting> UserSettings { get; set; } = new List<UserSetting>();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    public int Age => DateOfBirth.HasValue ? 
        DateTime.UtcNow.Year - DateOfBirth.Value.Year - 
        (DateTime.UtcNow.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0) : 0;
}
```

#### Permission Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/Permission.cs
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

#### RolePermission Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/RolePermission.cs
public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public DateTimeOffset GrantedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? GrantedBy { get; set; }
    
    // Navigation properties
    public virtual ApplicationRole Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
    public virtual ApplicationUser? GrantedByUser { get; set; }
}
```

#### Enhanced ApplicationRole
```csharp
// src/API/MeAndMyDog.Domain/Entities/ApplicationRole.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole<string>
{
    public int Level { get; set; } = 1;
    public bool IsSystemRole { get; set; } = false;
    public int? MaxUsers { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

#### UserSession Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/UserSession.cs
public class UserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string SessionToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset LastActivityAt { get; set; } = DateTimeOffset.UtcNow;
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? EndedAt { get; set; }
    public string? EndReason { get; set; } // Logout, Expired, Revoked, Replaced
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
```

#### SystemSetting Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/SystemSetting.cs
public class SystemSetting
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string DataType { get; set; } = "String"; // String, Integer, Boolean, JSON, Decimal
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsUserEditable { get; set; } = false;
    public bool IsEncrypted { get; set; } = false;
    public string? ValidationRule { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? UpdatedByUser { get; set; }
}
```

#### UserSetting Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/UserSetting.cs
public class UserSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
```

#### AuditLog Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/AuditLog.cs
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public Guid? SessionId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual UserSession? Session { get; set; }
}
```

### Phase 2: Core Business Entities

#### DogProfile Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/DogProfile.cs
public class DogProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? SecondaryBreed { get; set; }
    public DateTime? Birthdate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string? Gender { get; set; }
    public bool IsSpayedNeutered { get; set; } = false;
    public string? CoatColor { get; set; }
    public string? CoatType { get; set; }
    public string? EyeColor { get; set; }
    public string? MicrochipNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    
    // Emergency & Veterinary Contacts
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? VeterinarianName { get; set; }
    public string? VeterinarianPhone { get; set; }
    public string? VeterinarianAddress { get; set; }
    
    // Health & Behavior
    public string? EnergyLevel { get; set; } // Low, Medium, High
    public string? SocializationLevel { get; set; } // Shy, Friendly, Very Social
    public string? TrainingLevel { get; set; } // None, Basic, Advanced
    public string? SpecialNeeds { get; set; }
    public string? Medications { get; set; }
    public string? Allergies { get; set; }
    public string? BehaviorNotes { get; set; }
    public string? DietaryRestrictions { get; set; }
    
    // Media & Documentation
    public string? ProfileImageUrl { get; set; }
    public string? AdditionalPhotos { get; set; } // JSON array of URLs
    
    // Metadata
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<AIHealthRecommendation> AIHealthRecommendations { get; set; } = new List<AIHealthRecommendation>();
    
    // Computed properties
    public int AgeInMonths => Birthdate.HasValue ? 
        ((DateTime.UtcNow.Year - Birthdate.Value.Year) * 12) + DateTime.UtcNow.Month - Birthdate.Value.Month : 0;
    public string DisplayBreed => !string.IsNullOrEmpty(SecondaryBreed) ? $"{Breed} / {SecondaryBreed}" : Breed ?? "Unknown";
}
```

#### MedicalRecord Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/MedicalRecord.cs
public class MedicalRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DogId { get; set; }
    public string RecordType { get; set; } = string.Empty; // Vaccination, Checkup, Treatment, Surgery, Emergency
    public DateTimeOffset Date { get; set; }
    public string? VeterinarianName { get; set; }
    public string? ClinicName { get; set; }
    public string? Description { get; set; }
    public string? Medications { get; set; }
    public string? Notes { get; set; }
    public string? DocumentPath { get; set; } // Azure Blob Storage path
    public decimal? Cost { get; set; }
    public string? Insurance { get; set; }
    public DateTimeOffset? NextAppointmentDate { get; set; }
    public bool IsEmergency { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual DogProfile Dog { get; set; } = null!;
}
```

#### ServiceProvider Entity (Enhanced)
```csharp
// src/API/MeAndMyDog.Domain/Entities/ServiceProvider.cs
public class ServiceProvider
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessWebsite { get; set; }
    public string? BusinessLicense { get; set; }
    public string? InsurancePolicy { get; set; }
    public DateTimeOffset? InsuranceExpiryDate { get; set; }
    
    // Service area and pricing
    public int ServiceRadius { get; set; } = 25; // miles
    public decimal? HourlyRate { get; set; }
    public string? ServiceAreas { get; set; } // JSON array of areas served
    
    // Ratings and performance
    public decimal Rating { get; set; } = 0.0m;
    public int TotalReviews { get; set; } = 0;
    public int CompletedServices { get; set; } = 0;
    public decimal ResponseTimeHours { get; set; } = 0;
    public decimal ReliabilityScore { get; set; } = 0;
    
    // Availability and calendar
    public string? GoogleCalendarId { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public bool AutoAcceptBookings { get; set; } = false;
    public int AdvanceBookingDays { get; set; } = 30;
    public int MinBookingNoticeHours { get; set; } = 24;
    
    // Status and verification
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    public DateTimeOffset? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    public bool IsBackgroundChecked { get; set; } = false;
    public DateTimeOffset? BackgroundCheckDate { get; set; }
    
    // Metadata
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    public virtual ICollection<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<ServiceProviderReview> Reviews { get; set; } = new List<ServiceProviderReview>();
}
```

#### Service Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/Service.cs
public class Service
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty; // Walking, Grooming, Training, Boarding, Sitting, etc.
    public string? SubCategory { get; set; }
    public decimal BasePrice { get; set; }
    public string PriceType { get; set; } = "Hourly"; // Hourly, Fixed, Daily, Weekly
    public int? Duration { get; set; } // in minutes
    public int? MaxDogs { get; set; }
    public string? Requirements { get; set; } // Dog size, temperament, etc.
    public string? Inclusions { get; set; } // What's included in the service
    public string? Equipment { get; set; } // Required equipment
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
```

#### ServiceProviderReview Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/ServiceProviderReview.cs
public class ServiceProviderReview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceProviderId { get; set; }
    public string ReviewerId { get; set; } = string.Empty;
    public Guid? AppointmentId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Title { get; set; }
    public string? Comment { get; set; }
    
    // Detailed ratings
    public int? CommunicationRating { get; set; }
    public int? PunctualityRating { get; set; }
    public int? QualityRating { get; set; }
    public int? CareRating { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsVerified { get; set; } = false; // Verified purchase
    public bool IsVisible { get; set; } = true;
    public string? ProviderResponse { get; set; }
    public DateTimeOffset? ProviderResponseDate { get; set; }
    
    // Navigation properties
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    public virtual ApplicationUser Reviewer { get; set; } = null!;
    public virtual Appointment? Appointment { get; set; }
}
```

### Phase 3: Communication & Interaction Entities

#### Enhanced Conversation Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/Conversation.cs
public class Conversation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = "Direct"; // Direct, Group, Service, Support
    public string? Title { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastActivity { get; set; } = DateTimeOffset.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsArchived { get; set; } = false;
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
    public Guid? RelatedAppointmentId { get; set; }
    public string? Metadata { get; set; } // JSON for additional context
    
    // Navigation properties
    public virtual ApplicationUser Creator { get; set; } = null!;
    public virtual Appointment? RelatedAppointment { get; set; }
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<VideoCallSession> VideoCallSessions { get; set; } = new List<VideoCallSession>();
}
```

#### Enhanced Message Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/Message.cs
public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string MessageType { get; set; } = "Text"; // Text, Image, Video, File, System, AI, Location
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid? ReplyToMessageId { get; set; }
    public bool IsAIModerated { get; set; } = false;
    public string? ModerationResult { get; set; } // Approved, Flagged, Rejected
    public string DeliveryStatus { get; set; } = "Sent"; // Sent, Delivered, Read
    public string? Metadata { get; set; } // JSON for type-specific data
    
    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser Sender { get; set; } = null!;
    public virtual Message? ReplyToMessage { get; set; }
    public virtual ICollection<Message> Replies { get; set; } = new List<Message>();
    public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
    public virtual ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
    public virtual ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
}
```

#### MessageAttachment Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/MessageAttachment.cs
public class MessageAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty; // Azure Blob Storage path
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; } // For images/videos
    public int? Width { get; set; } // For images
    public int? Height { get; set; } // For images
    public int? Duration { get; set; } // For videos/audio in seconds
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual Message Message { get; set; } = null!;
}
```

#### MessageReaction Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/MessageReaction.cs
public class MessageReaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ReactionType { get; set; } = string.Empty; // Like, Love, Laugh, Sad, Angry, Care
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual Message Message { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
```

#### MessageReadReceipt Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/MessageReadReceipt.cs
public class MessageReadReceipt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual Message Message { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
```

#### VideoCallSession Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/VideoCallSession.cs
public class VideoCallSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConversationId { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndedAt { get; set; }
    public int? Duration { get; set; } // seconds
    public string Status { get; set; } = "Started"; // Started, Ended, Failed, Cancelled
    public int ParticipantCount { get; set; } = 1;
    public string? CallQuality { get; set; } // Poor, Fair, Good, Excellent
    public string? EndReason { get; set; } // Normal, NetworkIssue, UserHangup, etc.
    public string? Metadata { get; set; } // JSON for call-specific data
    
    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser InitiatedByUser { get; set; } = null!;
}
```

### Phase 4: Scheduling & Appointments

#### AvailabilitySlot Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/AvailabilitySlot.cs
public class AvailabilitySlot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceProviderId { get; set; }
    public int DayOfWeek { get; set; } // 0=Sunday, 1=Monday, etc.
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? RecurrencePattern { get; set; } // Weekly, BiWeekly, Monthly, etc.
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}
```

#### Enhanced Appointment Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/Appointment.cs
public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceProviderId { get; set; }
    public string PetOwnerId { get; set; } = string.Empty;
    public Guid? DogId { get; set; }
    public Guid? ServiceId { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    
    // Scheduling
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, InProgress, Completed, Cancelled, NoShow
    
    // Location and logistics
    public string? Location { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
    public string? EquipmentNeeded { get; set; }
    
    // Pricing and payment
    public decimal? Price { get; set; }
    public string? PaymentStatus { get; set; } // Pending, Paid, Refunded
    public string? PaymentMethod { get; set; }
    
    // External integration
    public string? GoogleCalendarEventId { get; set; }
    public Guid? ConversationId { get; set; }
    
    // Status tracking
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? CompletionNotes { get; set; }
    
    // Navigation properties
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    public virtual ApplicationUser PetOwner { get; set; } = null!;
    public virtual DogProfile? Dog { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Conversation? Conversation { get; set; }
    public virtual ICollection<ServiceProviderReview> Reviews { get; set; } = new List<ServiceProviderReview>();
}
```

### Phase 5: AI & Analytics

#### Enhanced KYCVerification Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/KYCVerification.cs
public class KYCVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string VerificationProvider { get; set; } = "Didit";
    public string? ExternalVerificationId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Approved, Rejected, ManualReview
    public string DocumentType { get; set; } = string.Empty;
    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? ReviewedBy { get; set; }
    public string? AdminNotes { get; set; }
    public bool AutoApproved { get; set; } = false;
    public decimal? ComplianceScore { get; set; } // 0-100
    public string? DocumentPath { get; set; } // Azure Blob Storage path
    public string? VerificationData { get; set; } // JSON from provider
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser? ReviewedByUser { get; set; }
    public virtual ICollection<KYCDocument> Documents { get; set; } = new List<KYCDocument>();
}
```

#### KYCDocument Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/KYCDocument.cs
public class KYCDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KYCVerificationId { get; set; }
    public string DocumentType { get; set; } = string.Empty; // DriversLicense, Passport, BusinessLicense
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty; // Azure Blob Storage path
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsVerified { get; set; } = false;
    public string? VerificationNotes { get; set; }
    public string? ExtractionData { get; set; } // JSON of extracted data
    
    // Navigation properties
    public virtual KYCVerification KYCVerification { get; set; } = null!;
}
```

#### Enhanced AIHealthRecommendation Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/AIHealthRecommendation.cs
public class AIHealthRecommendation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public Guid? DogId { get; set; }
    public string RecommendationType { get; set; } = string.Empty; // Health, Nutrition, Exercise, Veterinary, Behavioral
    public string Prompt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public string ModelUsed { get; set; } = "gemini-1.5-flash";
    public int TokensUsed { get; set; } = 0;
    public decimal Cost { get; set; } = 0.0m;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool? IsAccepted { get; set; }
    public string? UserFeedback { get; set; }
    public string ImplementationStatus { get; set; } = "Pending"; // Pending, InProgress, Completed, Dismissed
    public DateTimeOffset? ImplementedAt { get; set; }
    public string? ImplementationNotes { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual DogProfile? Dog { get; set; }
}
```

#### AIContentModeration Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/AIContentModeration.cs
public class AIContentModeration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ContentType { get; set; } = string.Empty; // Message, Image, Profile, Comment
    public Guid ContentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ModerationResult { get; set; } = string.Empty; // Approved, Flagged, Rejected
    public decimal Confidence { get; set; }
    public string? Reasons { get; set; }
    public string ModelUsed { get; set; } = "gemini-1.5-flash";
    public DateTimeOffset ProcessedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
    public string? AutoAction { get; set; } // AutoApprove, AutoReject, RequireReview
    public DateTimeOffset? ReviewedAt { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser? ReviewedByUser { get; set; }
}
```

#### AIUsageTracking Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/AIUsageTracking.cs
public class AIUsageTracking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty; // HealthRecommendation, ContentModeration, ImageAnalysis
    public string RequestType { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public decimal Cost { get; set; }
    public int ResponseTime { get; set; } // milliseconds
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string? Metadata { get; set; } // JSON for additional tracking data
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
```

### Phase 6: Advanced Features

#### SubscriptionPlan Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/SubscriptionPlan.cs
public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal? YearlyPrice { get; set; }
    public int? MaxDogs { get; set; }
    public int? MaxServiceProviders { get; set; }
    public int? MaxAIRequests { get; set; }
    public bool HasPrioritySupport { get; set; } = false;
    public bool HasAdvancedFeatures { get; set; } = false;
    public string? Features { get; set; } // JSON array of features
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
```

#### UserSubscription Entity
```csharp
// src/API/MeAndMyDog.Domain/Entities/UserSubscription.cs
public class UserSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public int SubscriptionPlanId { get; set; }
    public string Status { get; set; } = "Active"; // Active, Cancelled, Expired, Suspended
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndDate { get; set; }
    public DateTimeOffset? NextBillingDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? PaymentMethod { get; set; } // Card, PayPal, etc.
    public string? ExternalSubscriptionId { get; set; } // Stripe, PayPal subscription ID
    public decimal? DiscountPercent { get; set; }
    public string? CouponCode { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
```

## Entity Configurations

### ApplicationDbContext Configuration
```csharp
// src/API/MeAndMyDog.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    // Core entities
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    
    // Pet management
    public DbSet<DogProfile> DogProfiles { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    
    // Service providers and services
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceProviderReview> ServiceProviderReviews { get; set; }
    
    // Communication
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<MessageReaction> MessageReactions { get; set; }
    public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }
    public DbSet<VideoCallSession> VideoCallSessions { get; set; }
    
    // Scheduling
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    
    // KYC and verification
    public DbSet<KYCVerification> KYCVerifications { get; set; }
    public DbSet<KYCDocument> KYCDocuments { get; set; }
    
    // AI features
    public DbSet<AIHealthRecommendation> AIHealthRecommendations { get; set; }
    public DbSet<AIContentModeration> AIContentModerations { get; set; }
    public DbSet<AIUsageTracking> AIUsageTrackings { get; set; }
    
    // Subscriptions
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply all entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Configure Identity table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            
        foreach (var entry in entries)
        {
            if (entry.Entity is IHasTimestamps timestamps)
            {
                if (entry.State == EntityState.Added)
                {
                    timestamps.CreatedAt = DateTimeOffset.UtcNow;
                }
                timestamps.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}

// Interface for timestamp management
public interface IHasTimestamps
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
```

This completes the comprehensive Entity Framework Core models specification. Each entity includes proper navigation properties, computed properties where applicable, and follows EF Core best practices.

Next, I'll create the SQL migration scripts for all these entities.