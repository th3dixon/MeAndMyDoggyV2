namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of security incidents
/// </summary>
public enum IncidentType
{
    /// <summary>
    /// Unauthorized access attempt
    /// </summary>
    UnauthorizedAccess = 0,
    
    /// <summary>
    /// Failed authentication attempts
    /// </summary>
    AuthenticationFailure = 1,
    
    /// <summary>
    /// Suspicious access pattern
    /// </summary>
    SuspiciousActivity = 2,
    
    /// <summary>
    /// Data breach or leak
    /// </summary>
    DataBreach = 3,
    
    /// <summary>
    /// Malware or virus detected
    /// </summary>
    MalwareDetection = 4,
    
    /// <summary>
    /// Phishing attempt
    /// </summary>
    PhishingAttempt = 5,
    
    /// <summary>
    /// Social engineering attack
    /// </summary>
    SocialEngineering = 6,
    
    /// <summary>
    /// Account compromise
    /// </summary>
    AccountCompromise = 7,
    
    /// <summary>
    /// Privilege escalation attempt
    /// </summary>
    PrivilegeEscalation = 8,
    
    /// <summary>
    /// Policy violation
    /// </summary>
    PolicyViolation = 9,
    
    /// <summary>
    /// System intrusion
    /// </summary>
    SystemIntrusion = 10,
    
    /// <summary>
    /// Denial of service attack
    /// </summary>
    DenialOfService = 11,
    
    /// <summary>
    /// Data corruption or tampering
    /// </summary>
    DataTampering = 12,
    
    /// <summary>
    /// Other security incident
    /// </summary>
    Other = 13
}