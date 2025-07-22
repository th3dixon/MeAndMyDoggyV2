namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Message access validation result
/// </summary>
public class MessageAccessValidationResult
{
    /// <summary>
    /// Whether access is granted
    /// </summary>
    public bool AccessGranted { get; set; }

    /// <summary>
    /// Reason if access was denied
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Risk score for this access attempt
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// Whether additional verification is required
    /// </summary>
    public bool RequiresVerification { get; set; }

    /// <summary>
    /// Verification method required
    /// </summary>
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// Verification challenge (if required)
    /// </summary>
    public string? VerificationChallenge { get; set; }

    /// <summary>
    /// Session token for continued access
    /// </summary>
    public string? SessionToken { get; set; }

    /// <summary>
    /// Security warnings for the user
    /// </summary>
    public List<string> SecurityWarnings { get; set; } = new();

    /// <summary>
    /// Access restrictions that apply
    /// </summary>
    public List<string> ActiveRestrictions { get; set; } = new();
}