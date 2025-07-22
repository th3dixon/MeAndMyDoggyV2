namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Verification methods for secure message access
/// </summary>
public enum VerificationMethod
{
    /// <summary>
    /// No additional verification required
    /// </summary>
    None = 0,
    
    /// <summary>
    /// SMS verification code
    /// </summary>
    SMS = 1,
    
    /// <summary>
    /// Email verification code
    /// </summary>
    Email = 2,
    
    /// <summary>
    /// Two-factor authentication app
    /// </summary>
    TwoFactorApp = 3,
    
    /// <summary>
    /// Biometric verification (fingerprint, face)
    /// </summary>
    Biometric = 4,
    
    /// <summary>
    /// Hardware security key
    /// </summary>
    HardwareKey = 5,
    
    /// <summary>
    /// Push notification approval
    /// </summary>
    PushApproval = 6,
    
    /// <summary>
    /// Voice verification call
    /// </summary>
    VoiceCall = 7,
    
    /// <summary>
    /// Security question
    /// </summary>
    SecurityQuestion = 8,
    
    /// <summary>
    /// Multi-factor combination
    /// </summary>
    MultiFactor = 9,
    
    /// <summary>
    /// Certificate-based authentication
    /// </summary>
    Certificate = 10,
    
    /// <summary>
    /// Custom verification method
    /// </summary>
    Custom = 11
}