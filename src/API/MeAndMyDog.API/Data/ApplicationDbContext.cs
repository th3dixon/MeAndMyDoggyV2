using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Models.Entities;
using ServiceProviderEntity = MeAndMyDog.API.Models.Entities.ServiceProvider;

namespace MeAndMyDog.API.Data;

/// <summary>
/// Main application database context for MeAndMyDoggyV2
/// Implements comprehensive entity model supporting all platform features
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext
    /// </summary>
    /// <param name="options">The database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    #region Core Security & User Management
    /// <summary>
    /// DbSet for managing system permissions and access control rights
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }
    /// <summary>
    /// DbSet for managing role-permission mappings and RBAC relationships
    /// </summary>
    public DbSet<RolePermission> RolePermissions { get; set; }
    /// <summary>
    /// DbSet for managing user authentication sessions and login tracking
    /// </summary>
    public DbSet<UserSession> UserSessions { get; set; }
    /// <summary>
    /// DbSet for managing global system configuration settings
    /// </summary>
    public DbSet<SystemSetting> SystemSettings { get; set; }
    /// <summary>
    /// DbSet for managing individual user preferences and configuration settings
    /// </summary>
    public DbSet<UserSetting> UserSettings { get; set; }
    /// <summary>
    /// DbSet for managing security audit logs and system activity tracking
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; }
    #endregion
    
    #region Pet Management
    /// <summary>
    /// DbSet for managing dog profiles and pet information
    /// </summary>
    public DbSet<DogProfile> DogProfiles { get; set; }
    /// <summary>
    /// DbSet for managing veterinary medical records and health history
    /// </summary>
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    #endregion
    
    #region Service Providers & Services
    /// <summary>
    /// DbSet for managing service provider profiles and business information
    /// </summary>
    public DbSet<ServiceProviderEntity> ServiceProviders { get; set; }
    /// <summary>
    /// DbSet for managing service offerings and provider capabilities
    /// </summary>
    public DbSet<Service> Services { get; set; }
    /// <summary>
    /// DbSet for managing customer reviews and ratings for service providers
    /// </summary>
    public DbSet<ServiceProviderReview> ServiceProviderReviews { get; set; }
    /// <summary>
    /// DbSet for managing geographical locations and service areas for providers
    /// </summary>
    public DbSet<ProviderLocation> ProviderLocations { get; set; }
    /// <summary>
    /// DbSet for managing service bookings and appointments
    /// </summary>
    public DbSet<Booking> Bookings { get; set; }
    #endregion
    
    #region Communication & Messaging
    /// <summary>
    /// DbSet for managing chat conversations between users and service providers
    /// </summary>
    public DbSet<Conversation> Conversations { get; set; }
    /// <summary>
    /// DbSet for managing conversation participants and their roles
    /// </summary>
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    /// <summary>
    /// DbSet for managing chat messages and communication content
    /// </summary>
    public DbSet<Message> Messages { get; set; }
    /// <summary>
    /// DbSet for managing message attachments and file uploads
    /// </summary>
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    /// <summary>
    /// DbSet for managing message reactions and emoji responses
    /// </summary>
    public DbSet<MessageReaction> MessageReactions { get; set; }
    /// <summary>
    /// DbSet for managing message read receipts and delivery status
    /// </summary>
    public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }
    /// <summary>
    /// DbSet for managing video call sessions and WebRTC connections
    /// </summary>
    public DbSet<VideoCallSession> VideoCallSessions { get; set; }
    #endregion
    
    #region Scheduling & Appointments
    /// <summary>
    /// DbSet for managing provider availability schedules and time slots
    /// </summary>
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
    /// <summary>
    /// DbSet for managing scheduled appointments and calendar events
    /// </summary>
    public DbSet<Appointment> Appointments { get; set; }
    #endregion
    
    #region KYC & Verification
    /// <summary>
    /// DbSet for managing Know Your Customer verification processes
    /// </summary>
    public DbSet<KYCVerification> KYCVerifications { get; set; }
    /// <summary>
    /// DbSet for managing KYC verification documents and attachments
    /// </summary>
    public DbSet<KYCDocument> KYCDocuments { get; set; }
    #endregion
    
    #region AI Features
    /// <summary>
    /// DbSet for managing AI-generated health recommendations for pets
    /// </summary>
    public DbSet<AIHealthRecommendation> AIHealthRecommendations { get; set; }
    /// <summary>
    /// DbSet for managing AI content moderation results and actions
    /// </summary>
    public DbSet<AIContentModeration> AIContentModerations { get; set; }
    /// <summary>
    /// DbSet for tracking AI service usage and billing metrics
    /// </summary>
    public DbSet<AIUsageTracking> AIUsageTrackings { get; set; }
    #endregion
    
    #region Subscriptions & Billing
    /// <summary>
    /// DbSet for managing subscription plans and pricing tiers
    /// </summary>
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    /// <summary>
    /// DbSet for managing user subscription statuses and billing history
    /// </summary>
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    #endregion
    
    #region Legacy Service Catalog (to be migrated)
    /// <summary>
    /// DbSet for managing service categories in the platform catalog
    /// </summary>
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    /// <summary>
    /// DbSet for managing sub-services within service categories
    /// </summary>
    public DbSet<SubService> SubServices { get; set; }
    /// <summary>
    /// DbSet for managing provider services
    /// </summary>
    public DbSet<ProviderService> ProviderService { get; set; }
    /// <summary>
    /// DbSet for managing provider service pricing
    /// </summary>
    public DbSet<ProviderServicePricing> ProviderServicePricing { get; set; }
    #endregion
    
    #region Authentication & Security
    /// <summary>
    /// DbSet for managing JWT refresh tokens
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    #endregion
    
    /// <summary>
    /// Configures the model and entity relationships using Entity Framework model builder
    /// </summary>
    /// <param name="builder">The model builder instance used to configure the database model</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply all entity configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        #region Identity Table Naming
        // Configure Identity table names to match our existing schema
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        #endregion
        
        #region Core Entity Configurations
        
        // ApplicationUser enhanced configuration
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.TimeZone).HasMaxLength(50).HasDefaultValue("UTC");
            entity.Property(e => e.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
            entity.Property(e => e.AccountStatus).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.SubscriptionType).HasMaxLength(20).HasDefaultValue("Free");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            // Indexes for performance
            entity.HasIndex(e => new { e.FirstName, e.LastName }).HasDatabaseName("IX_Users_Name");
            entity.HasIndex(e => e.LastSeenAt).HasDatabaseName("IX_Users_LastSeen");
            entity.HasIndex(e => e.AccountStatus).HasDatabaseName("IX_Users_AccountStatus");
            entity.HasIndex(e => e.IsKYCVerified).HasDatabaseName("IX_Users_KYCVerified");
            entity.HasIndex(e => e.SubscriptionType).HasDatabaseName("IX_Users_SubscriptionType");
        });
        
        // Permission configuration
        builder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Permissions_Name");
            entity.HasIndex(e => e.Category).HasDatabaseName("IX_Permissions_Category");
        });
        
        // RolePermission many-to-many configuration
        builder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.Property(e => e.GrantedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(e => e.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.GrantedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.GrantedBy)
                  .OnDelete(DeleteBehavior.NoAction);
        });
        
        // UserSession configuration
        builder.Entity<UserSession>(entity =>
        {
            entity.Property(e => e.SessionToken).HasMaxLength(500).IsRequired();
            entity.Property(e => e.RefreshToken).HasMaxLength(500).IsRequired();
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.DeviceInfo).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.EndReason).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.LastActivityAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.SessionToken).IsUnique().HasDatabaseName("IX_UserSessions_Token");
            entity.HasIndex(e => e.RefreshToken).IsUnique().HasDatabaseName("IX_UserSessions_RefreshToken");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_UserSessions_User");
            entity.HasIndex(e => e.ExpiresAt).HasDatabaseName("IX_UserSessions_ExpiresAt");
        });
        
        // DogProfile configuration
        builder.Entity<DogProfile>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Breed).HasMaxLength(100);
            entity.Property(e => e.SecondaryBreed).HasMaxLength(100);
            entity.Property(e => e.Weight).HasPrecision(5, 2);
            entity.Property(e => e.Height).HasPrecision(5, 2);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.CoatColor).HasMaxLength(100);
            entity.Property(e => e.CoatType).HasMaxLength(50);
            entity.Property(e => e.EyeColor).HasMaxLength(50);
            entity.Property(e => e.MicrochipNumber).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(100);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_DogProfiles_Owner");
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_DogProfiles_Name");
            entity.HasIndex(e => e.Breed).HasDatabaseName("IX_DogProfiles_Breed");
            entity.HasIndex(e => new { e.OwnerId, e.IsActive }).HasDatabaseName("IX_DogProfiles_Owner_Active");
        });
        
        // ServiceProvider configuration
        builder.Entity<ServiceProviderEntity>(entity =>
        {
            entity.Property(e => e.BusinessName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.BusinessAddress).HasMaxLength(500);
            entity.Property(e => e.BusinessPhone).HasMaxLength(50);
            entity.Property(e => e.BusinessEmail).HasMaxLength(256);
            entity.Property(e => e.BusinessWebsite).HasMaxLength(500);
            entity.Property(e => e.BusinessLicense).HasMaxLength(100);
            entity.Property(e => e.InsurancePolicy).HasMaxLength(200);
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            entity.Property(e => e.Rating).HasPrecision(3, 2).HasDefaultValue(0.0m);
            entity.Property(e => e.ResponseTimeHours).HasPrecision(5, 2).HasDefaultValue(0.0m);
            entity.Property(e => e.ReliabilityScore).HasPrecision(3, 2).HasDefaultValue(0.0m);
            entity.Property(e => e.TimeZone).HasMaxLength(50).HasDefaultValue("UTC");
            entity.Property(e => e.GoogleCalendarId).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_ServiceProviders_User");
            entity.HasIndex(e => e.Rating).HasDatabaseName("IX_ServiceProviders_Rating");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_ServiceProviders_Active");
        });
        
        // Message configuration
        builder.Entity<Message>(entity =>
        {
            entity.Property(e => e.MessageType).HasMaxLength(20).HasDefaultValue("Text");
            entity.Property(e => e.DeliveryStatus).HasMaxLength(20).HasDefaultValue("Sent");
            entity.Property(e => e.ModerationResult).HasMaxLength(20);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.ConversationId, e.Timestamp })
                  .HasDatabaseName("IX_Messages_Conversation_Timestamp");
            entity.HasIndex(e => e.SenderId).HasDatabaseName("IX_Messages_Sender");
            entity.HasIndex(e => e.MessageType).HasDatabaseName("IX_Messages_Type");
        });
        
        // MessageReaction configuration
        builder.Entity<MessageReaction>(entity =>
        {
            entity.Property(e => e.Reaction).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.MessageId, e.UserId, e.Reaction })
                  .IsUnique()
                  .HasDatabaseName("IX_MessageReactions_Message_User_Reaction");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_MessageReactions_User");
        });
        
        // MessageReadReceipt configuration
        builder.Entity<MessageReadReceipt>(entity =>
        {
            entity.Property(e => e.ReadAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.MessageId, e.UserId })
                  .IsUnique()
                  .HasDatabaseName("IX_MessageReadReceipts_Message_User");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_MessageReadReceipts_User");
        });
        
        // ConversationParticipant configuration
        builder.Entity<ConversationParticipant>(entity =>
        {
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.ConversationId, e.UserId })
                  .IsUnique()
                  .HasDatabaseName("IX_ConversationParticipants_Conversation_User");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_ConversationParticipants_User");
        });
        
        // VideoCallSession configuration
        builder.Entity<VideoCallSession>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.RoomId).HasMaxLength(200);
            entity.Property(e => e.StartTime).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.ConversationId, e.StartTime })
                  .HasDatabaseName("IX_VideoCallSessions_Conversation_StartTime");
            entity.HasIndex(e => e.InitiatorId).HasDatabaseName("IX_VideoCallSessions_Initiator");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_VideoCallSessions_Status");
        });
        
        // AvailabilitySlot configuration
        builder.Entity<AvailabilitySlot>(entity =>
        {
            entity.Property(e => e.RecurrenceRule).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.ServiceProviderId, e.StartTime, e.EndTime })
                  .HasDatabaseName("IX_AvailabilitySlots_Provider_TimeRange");
            entity.HasIndex(e => new { e.StartTime, e.EndTime, e.IsAvailable })
                  .HasDatabaseName("IX_AvailabilitySlots_TimeRange_Available");
        });
        
        // Appointment configuration
        builder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.ServiceType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.GoogleCalendarEventId).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.ServiceProviderId, e.StartTime })
                  .HasDatabaseName("IX_Appointments_Provider_Date");
            entity.HasIndex(e => new { e.PetOwnerId, e.StartTime })
                  .HasDatabaseName("IX_Appointments_PetOwner_Date");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Appointments_Status");
        });
        
        // AIHealthRecommendation configuration
        builder.Entity<AIHealthRecommendation>(entity =>
        {
            entity.Property(e => e.RecommendationType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Confidence).HasPrecision(5, 4);
            entity.Property(e => e.ModelUsed).HasMaxLength(50).HasDefaultValue("gemini-1.5-flash");
            entity.Property(e => e.Cost).HasPrecision(10, 6).HasDefaultValue(0.0m);
            entity.Property(e => e.ImplementationStatus).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                  .HasDatabaseName("IX_AIHealthRecommendations_User_Date");
            entity.HasIndex(e => e.RecommendationType)
                  .HasDatabaseName("IX_AIHealthRecommendations_Type");
        });
        
        // KYCVerification configuration
        builder.Entity<KYCVerification>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.DocumentNumber).HasMaxLength(100);
            entity.Property(e => e.DocumentImageUrl).HasMaxLength(500);
            entity.Property(e => e.VerificationResult).HasMaxLength(200);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.VerifiedBy).HasMaxLength(450);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => new { e.UserId, e.Status })
                  .HasDatabaseName("IX_KYCVerifications_User_Status");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_KYCVerifications_Status");
            entity.HasIndex(e => e.SubmittedAt).HasDatabaseName("IX_KYCVerifications_SubmittedAt");
        });
        
        // KYCDocument configuration
        builder.Entity<KYCDocument>(entity =>
        {
            entity.Property(e => e.DocumentType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DocumentUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.KYCVerificationId)
                  .HasDatabaseName("IX_KYCDocuments_KYCVerification");
            entity.HasIndex(e => e.DocumentType).HasDatabaseName("IX_KYCDocuments_DocumentType");
        });
        
        // AIContentModeration configuration
        builder.Entity<AIContentModeration>(entity =>
        {
            entity.Property(e => e.ContentType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ContentId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.ModerationResult).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ToxicityScore).HasPrecision(5, 4);
            entity.Property(e => e.Flags).HasMaxLength(500);
            entity.Property(e => e.ModelUsed).HasMaxLength(50).HasDefaultValue("gemini-1.5-flash");
            entity.Property(e => e.Cost).HasPrecision(10, 6).HasDefaultValue(0.0m);
            entity.Property(e => e.ProcessedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UserId).HasMaxLength(450);
            
            entity.HasIndex(e => new { e.ContentType, e.ContentId })
                  .HasDatabaseName("IX_AIContentModerations_Content");
            entity.HasIndex(e => e.ModerationResult)
                  .HasDatabaseName("IX_AIContentModerations_Result");
            entity.HasIndex(e => e.ProcessedAt)
                  .HasDatabaseName("IX_AIContentModerations_ProcessedAt");
            entity.HasIndex(e => e.UserId)
                  .HasDatabaseName("IX_AIContentModerations_User");
        });
        
        // AIUsageTracking configuration
        builder.Entity<AIUsageTracking>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.FeatureType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ModelUsed).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Cost).HasPrecision(10, 6).HasDefaultValue(0.0m);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.Metadata).HasMaxLength(1000);
            
            entity.HasIndex(e => new { e.UserId, e.Timestamp })
                  .HasDatabaseName("IX_AIUsageTracking_User_Timestamp");
            entity.HasIndex(e => e.FeatureType)
                  .HasDatabaseName("IX_AIUsageTracking_FeatureType");
            entity.HasIndex(e => e.Timestamp)
                  .HasDatabaseName("IX_AIUsageTracking_Timestamp");
        });
        
        // SubscriptionPlan configuration
        builder.Entity<SubscriptionPlan>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.BillingCycle).HasMaxLength(20).HasDefaultValue("Monthly");
            entity.Property(e => e.Features).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_SubscriptionPlans_Name");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_SubscriptionPlans_IsActive");
            entity.HasIndex(e => e.Price).HasDatabaseName("IX_SubscriptionPlans_Price");
        });
        
        #endregion
        
        #region Relationship Configurations
        
        // Configure all foreign key relationships
        ConfigureUserRelationships(builder);
        ConfigurePetRelationships(builder);
        ConfigureServiceRelationships(builder);
        ConfigureProviderSearchRelationships(builder);
        ConfigureMessagingRelationships(builder);
        ConfigureAppointmentRelationships(builder);
        ConfigureAIRelationships(builder);
        
        #endregion
        
        #region Legacy Service Catalog Configuration (to be removed in future migration)
        ConfigureBasicServiceCatalog(builder); // Basic configuration without cascade conflicts
        #endregion
    }
    
    private void ConfigureProviderSearchRelationships(ModelBuilder builder)
        {
            // ProviderLocation configuration
            builder.Entity<ProviderLocation>(entity =>
            {
                entity.Property(e => e.Postcode).HasMaxLength(10).IsRequired();
                entity.Property(e => e.FullAddress).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.County).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(50).HasDefaultValue("United Kingdom");
                entity.Property(e => e.LocationType).HasMaxLength(50).HasDefaultValue("Business");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Indexes for performance
                entity.HasIndex(e => e.ServiceProviderId).HasDatabaseName("IX_ProviderLocations_ServiceProvider");
                entity.HasIndex(e => e.Postcode).HasDatabaseName("IX_ProviderLocations_Postcode");
                entity.HasIndex(e => new { e.Latitude, e.Longitude }).HasDatabaseName("IX_ProviderLocations_Coordinates");
                entity.HasIndex(e => new { e.IsPrimary, e.IsActive }).HasDatabaseName("IX_ProviderLocations_Primary_Active");
            });
            
            // Booking configuration
            builder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.BookingReference).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.PaymentStatus).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.ServiceLocation).HasMaxLength(500);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(1000);
                entity.Property(e => e.ProviderNotes).HasMaxLength(1000);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                entity.Property(e => e.CancelledBy).HasMaxLength(50);
                entity.Property(e => e.RecurrencePattern).HasMaxLength(200);
                entity.Property(e => e.ExternalCalendarEventId).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Decimal precision for prices
                entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
                entity.Property(e => e.BasePrice).HasPrecision(10, 2);
                entity.Property(e => e.WeekendSurcharge).HasPrecision(10, 2);
                entity.Property(e => e.EveningSurcharge).HasPrecision(10, 2);
                entity.Property(e => e.EmergencySurcharge).HasPrecision(10, 2);
                
                // Indexes for performance
                entity.HasIndex(e => e.BookingReference).IsUnique().HasDatabaseName("IX_Bookings_Reference");
                entity.HasIndex(e => new { e.ServiceProviderId, e.StartDateTime }).HasDatabaseName("IX_Bookings_Provider_StartDate");
                entity.HasIndex(e => new { e.CustomerId, e.StartDateTime }).HasDatabaseName("IX_Bookings_Customer_StartDate");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Bookings_Status");
                entity.HasIndex(e => new { e.StartDateTime, e.EndDateTime }).HasDatabaseName("IX_Bookings_DateRange");
                entity.HasIndex(e => e.ParentBookingId).HasDatabaseName("IX_Bookings_ParentBooking");
            });
            
            // ProviderLocation -> ServiceProvider
            builder.Entity<ProviderLocation>()
                .HasOne(pl => pl.ServiceProvider)
                .WithMany()
                .HasForeignKey(pl => pl.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Booking -> ServiceProvider
            builder.Entity<Booking>()
                .HasOne(b => b.ServiceProvider)
                .WithMany()
                .HasForeignKey(b => b.ServiceProviderId)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Booking -> Customer
            builder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Booking -> DogProfile
            builder.Entity<Booking>()
                .HasOne(b => b.Dog)
                .WithMany()
                .HasForeignKey(b => b.DogId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Booking -> Service
            builder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Booking -> ServiceCategory
            builder.Entity<Booking>()
                .HasOne(b => b.ServiceCategory)
                .WithMany()
                .HasForeignKey(b => b.ServiceCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Booking -> SubService
            builder.Entity<Booking>()
                .HasOne(b => b.SubService)
                .WithMany()
                .HasForeignKey(b => b.SubServiceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    
    private void ConfigureUserRelationships(ModelBuilder builder)
    {
        // UserSession -> User
        builder.Entity<UserSession>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // KYCVerification -> User
        builder.Entity<KYCVerification>()
            .HasOne(kyc => kyc.User)
            .WithMany(u => u.KYCVerifications)
            .HasForeignKey(kyc => kyc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // UserSubscription -> User
        builder.Entity<UserSubscription>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSubscriptions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // UserSubscription -> SubscriptionPlan
        builder.Entity<UserSubscription>()
            .HasOne(us => us.SubscriptionPlan)
            .WithMany(sp => sp.UserSubscriptions)
            .HasForeignKey(us => us.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // KYCDocument -> KYCVerification
        builder.Entity<KYCDocument>()
            .HasOne(kd => kd.KYCVerification)
            .WithMany(kyc => kyc.Documents)
            .HasForeignKey(kd => kd.KYCVerificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private void ConfigurePetRelationships(ModelBuilder builder)
    {
        // DogProfile -> User (Owner)
        builder.Entity<DogProfile>()
            .HasOne(dp => dp.Owner)
            .WithMany(u => u.DogProfiles)
            .HasForeignKey(dp => dp.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // MedicalRecord -> DogProfile
        builder.Entity<MedicalRecord>()
            .HasOne(mr => mr.Dog)
            .WithMany(dp => dp.MedicalRecords)
            .HasForeignKey(mr => mr.DogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private void ConfigureServiceRelationships(ModelBuilder builder)
    {
        // ServiceProvider -> User
        builder.Entity<ServiceProviderEntity>()
            .HasOne(sp => sp.User)
            .WithOne()
            .HasForeignKey<ServiceProviderEntity>(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Service -> ServiceProvider
        builder.Entity<Service>()
            .HasOne(s => s.ServiceProvider)
            .WithMany(sp => sp.Services)
            .HasForeignKey(s => s.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // ServiceProviderReview relationships
        builder.Entity<ServiceProviderReview>()
            .HasOne(spr => spr.ServiceProvider)
            .WithMany(sp => sp.Reviews)
            .HasForeignKey(spr => spr.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<ServiceProviderReview>()
            .HasOne(spr => spr.Reviewer)
            .WithMany(u => u.WrittenReviews)
            .HasForeignKey(spr => spr.ReviewerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    
    private void ConfigureMessagingRelationships(ModelBuilder builder)
    {
        // Conversation -> User (Creator)
        builder.Entity<Conversation>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.CreatedConversations)
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
            
        // Message -> Conversation
        builder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Message -> User (Sender)  
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // MessageAttachment -> Message
        builder.Entity<MessageAttachment>()
            .HasOne(ma => ma.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(ma => ma.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // ConversationParticipant -> Conversation
        builder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // ConversationParticipant -> User
        builder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // MessageReaction -> Message
        builder.Entity<MessageReaction>()
            .HasOne(mr => mr.Message)
            .WithMany(m => m.Reactions)
            .HasForeignKey(mr => mr.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // MessageReaction -> User
        builder.Entity<MessageReaction>()
            .HasOne(mr => mr.User)
            .WithMany()
            .HasForeignKey(mr => mr.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // MessageReadReceipt -> Message
        builder.Entity<MessageReadReceipt>()
            .HasOne(mrr => mrr.Message)
            .WithMany(m => m.ReadReceipts)
            .HasForeignKey(mrr => mrr.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // MessageReadReceipt -> User
        builder.Entity<MessageReadReceipt>()
            .HasOne(mrr => mrr.User)
            .WithMany()
            .HasForeignKey(mrr => mrr.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // VideoCallSession -> Conversation
        builder.Entity<VideoCallSession>()
            .HasOne(vcs => vcs.Conversation)
            .WithMany()
            .HasForeignKey(vcs => vcs.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // VideoCallSession -> User (Initiator)
        builder.Entity<VideoCallSession>()
            .HasOne(vcs => vcs.Initiator)
            .WithMany()
            .HasForeignKey(vcs => vcs.InitiatorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    
    private void ConfigureAppointmentRelationships(ModelBuilder builder)
    {
        // Appointment -> ServiceProvider
        builder.Entity<Appointment>()
            .HasOne(a => a.ServiceProvider)
            .WithMany(sp => sp.Appointments)
            .HasForeignKey(a => a.ServiceProviderId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // Appointment -> User (PetOwner)
        builder.Entity<Appointment>()
            .HasOne(a => a.PetOwner)
            .WithMany(u => u.PetOwnerAppointments)
            .HasForeignKey(a => a.PetOwnerId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // Appointment -> DogProfile
        builder.Entity<Appointment>()
            .HasOne(a => a.Dog)
            .WithMany(dp => dp.Appointments)
            .HasForeignKey(a => a.DogId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // AvailabilitySlot -> ServiceProvider
        builder.Entity<AvailabilitySlot>()
            .HasOne(avs => avs.ServiceProvider)
            .WithMany(sp => sp.AvailabilitySlots)
            .HasForeignKey(avs => avs.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private void ConfigureAIRelationships(ModelBuilder builder)
    {
        // AIHealthRecommendation -> User
        builder.Entity<AIHealthRecommendation>()
            .HasOne(ai => ai.User)
            .WithMany(u => u.AIHealthRecommendations)
            .HasForeignKey(ai => ai.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // AIHealthRecommendation -> DogProfile
        builder.Entity<AIHealthRecommendation>()
            .HasOne(ai => ai.Dog)
            .WithMany(dp => dp.AIHealthRecommendations)
            .HasForeignKey(ai => ai.DogId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // AIContentModeration -> User (optional)
        builder.Entity<AIContentModeration>()
            .HasOne(acm => acm.User)
            .WithMany()
            .HasForeignKey(acm => acm.UserId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // AIUsageTracking -> User (optional)
        builder.Entity<AIUsageTracking>()
            .HasOne(aut => aut.User)
            .WithMany()
            .HasForeignKey(aut => aut.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
    
    private void ConfigureBasicServiceCatalog(ModelBuilder builder)
    {
        // Basic service catalog configurations without cascade conflicts
        builder.Entity<ServiceCategory>(entity =>
        {
            entity.Property(sc => sc.Name).HasMaxLength(100);
            entity.Property(sc => sc.Description).HasMaxLength(500);
            entity.HasIndex(sc => new { sc.DisplayOrder, sc.IsActive });
        });
            
        builder.Entity<SubService>(entity =>
        {
            entity.HasOne(ss => ss.ServiceCategory)
                  .WithMany(sc => sc.SubServices)
                  .HasForeignKey(ss => ss.ServiceCategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(ss => ss.SuggestedMinPrice).HasPrecision(10, 2);
            entity.Property(ss => ss.SuggestedMaxPrice).HasPrecision(10, 2);
            entity.HasIndex(ss => new { ss.ServiceCategoryId, ss.DisplayOrder, ss.IsActive });
        });
    }
    
    private void ConfigureLegacyServiceCatalog(ModelBuilder builder)
    {
        // Legacy service catalog configurations (to be migrated)
        builder.Entity<ServiceCategory>(entity =>
        {
            entity.Property(sc => sc.Name).HasMaxLength(100);
            entity.Property(sc => sc.Description).HasMaxLength(500);
            entity.HasIndex(sc => new { sc.DisplayOrder, sc.IsActive });
        });
            
        builder.Entity<SubService>(entity =>
        {
            entity.HasOne(ss => ss.ServiceCategory)
                  .WithMany(sc => sc.SubServices)
                  .HasForeignKey(ss => ss.ServiceCategoryId);
            entity.Property(ss => ss.SuggestedMinPrice).HasPrecision(10, 2);
            entity.Property(ss => ss.SuggestedMaxPrice).HasPrecision(10, 2);
            entity.HasIndex(ss => new { ss.ServiceCategoryId, ss.DisplayOrder, ss.IsActive });
        });
            
        builder.Entity<ProviderService>(entity =>
        {
            entity.HasOne(ps => ps.ServiceCategory)
                  .WithMany(sc => sc.ProviderServices)
                  .HasForeignKey(ps => ps.ServiceCategoryId);
        });
            
        builder.Entity<ProviderServicePricing>(entity =>
        {
            entity.HasOne(psp => psp.ProviderService)
                  .WithMany(ps => ps.Pricing)
                  .HasForeignKey(psp => psp.ProviderServiceId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(psp => psp.SubService)
                  .WithMany()
                  .HasForeignKey(psp => psp.SubServiceId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.Property(psp => psp.Price).HasPrecision(10, 2);
            entity.Property(psp => psp.WeekendSurchargePercentage).HasPrecision(5, 2);
            entity.Property(psp => psp.EveningSurchargePercentage).HasPrecision(5, 2);
        });
    }
    
    /// <summary>
    /// Override SaveChangesAsync to automatically update timestamps and perform audit logging
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for entities that implement timestamp tracking
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            
        foreach (var entry in entries)
        {
            // Update timestamps for entities with CreatedAt/UpdatedAt properties
            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
            
            if (entry.State == EntityState.Added && entry.Entity.GetType().GetProperty("CreatedAt") != null)
            {
                entry.Property("CreatedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}