namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a document uploaded for KYC verification
/// </summary>
public class KYCDocument
{
    /// <summary>
    /// Unique identifier for the document
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the KYC verification
    /// </summary>
    public string KYCVerificationId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of document (passport, driver_license, national_id, etc.)
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;
    
    /// <summary>
    /// URL where the document is stored
    /// </summary>
    public string DocumentUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Original filename of the uploaded document
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// When the document was uploaded
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the KYC verification
    /// </summary>
    public virtual KYCVerification KYCVerification { get; set; } = null!;
}