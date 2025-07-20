using Microsoft.AspNetCore.Identity;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a user in the MeAndMyDog application, extending ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser<string>
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to the user's profile photo
    /// </summary>
    public string? ProfilePhotoUrl { get; set; }
    
    /// <summary>
    /// Type of user (PetOwner, ServiceProvider, or Both)
    /// </summary>
    public UserType UserType { get; set; }
    
    /// <summary>
    /// First line of user's address
    /// </summary>
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// Second line of user's address
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// User's city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// User's county
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// User's postal code
    /// </summary>
    public string? PostCode { get; set; }
    
    /// <summary>
    /// Latitude coordinate for user's location
    /// </summary>
    public decimal? Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate for user's location
    /// </summary>
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the user account was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Additional properties for comprehensive user management
    /// <summary>
    /// URL to the user's profile image
    /// </summary>
    public string? ProfileImageUrl { get; set; }
    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTimeOffset? DateOfBirth { get; set; }
    /// <summary>
    /// User's gender
    /// </summary>
    public string? Gender { get; set; }
    /// <summary>
    /// User's preferred timezone
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
    /// <summary>
    /// User's preferred language code
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";
    /// <summary>
    /// Last time the user was active on the platform
    /// </summary>
    public DateTimeOffset? LastSeenAt { get; set; }
    /// <summary>
    /// Whether the user has completed KYC verification
    /// </summary>
    public bool IsKYCVerified { get; set; } = false;
    /// <summary>
    /// Current account status (Active, Suspended, Closed)
    /// </summary>
    public string AccountStatus { get; set; } = "Active";
    /// <summary>
    /// Current subscription type (Free, Premium, Pro)
    /// </summary>
    public string SubscriptionType { get; set; } = "Free";
    
    // Navigation properties
    /// <summary>
    /// Collection of user authentication sessions
    /// </summary>
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    /// <summary>
    /// Collection of dog profiles owned by this user
    /// </summary>
    public virtual ICollection<DogProfile> DogProfiles { get; set; } = new List<DogProfile>();
    /// <summary>
    /// Collection of KYC verification attempts for this user
    /// </summary>
    public virtual ICollection<KYCVerification> KYCVerifications { get; set; } = new List<KYCVerification>();
    /// <summary>
    /// Collection of user subscriptions and billing history
    /// </summary>
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    /// <summary>
    /// Collection of conversations created by this user
    /// </summary>
    public virtual ICollection<Conversation> CreatedConversations { get; set; } = new List<Conversation>();
    /// <summary>
    /// Collection of messages sent by this user
    /// </summary>
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    /// <summary>
    /// Collection of appointments where this user is the pet owner
    /// </summary>
    public virtual ICollection<Appointment> PetOwnerAppointments { get; set; } = new List<Appointment>();
    /// <summary>
    /// Collection of reviews written by this user
    /// </summary>
    public virtual ICollection<ServiceProviderReview> WrittenReviews { get; set; } = new List<ServiceProviderReview>();
    /// <summary>
    /// Collection of AI health recommendations for this user
    /// </summary>
    public virtual ICollection<AIHealthRecommendation> AIHealthRecommendations { get; set; } = new List<AIHealthRecommendation>();
}