namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a KYC (Know Your Customer) verification request for a user
/// </summary>
public class KYCVerification
{
    /// <summary>
    /// Unique identifier for the KYC verification
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status of the verification (Pending, Approved, Rejected, Expired)
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Primary document type used for verification
    /// </summary>
    public string? DocumentType { get; set; }
    
    /// <summary>
    /// Document number from the verification document
    /// </summary>
    public string? DocumentNumber { get; set; }
    
    /// <summary>
    /// Expiry date of the verification document
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; }
    
    /// <summary>
    /// Primary document image URL (legacy field, use Documents collection instead)
    /// </summary>
    public string? DocumentImageUrl { get; set; }
    
    /// <summary>
    /// Result of the verification process
    /// </summary>
    public string? VerificationResult { get; set; }
    
    /// <summary>
    /// Reason for rejection if status is Rejected
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// When the verification was submitted
    /// </summary>
    public DateTimeOffset SubmittedAt { get; set; }
    
    /// <summary>
    /// When the verification was completed
    /// </summary>
    public DateTimeOffset? VerifiedAt { get; set; }
    
    /// <summary>
    /// User ID who performed the verification
    /// </summary>
    public string? VerifiedBy { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to uploaded documents
    /// </summary>
    public virtual ICollection<KYCDocument> Documents { get; set; } = new List<KYCDocument>();
}