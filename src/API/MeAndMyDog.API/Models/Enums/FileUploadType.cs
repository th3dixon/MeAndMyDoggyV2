namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// File upload type enumeration
/// </summary>
public enum FileUploadType
{
    /// <summary>
    /// Message attachment
    /// </summary>
    MessageAttachment,

    /// <summary>
    /// Profile picture
    /// </summary>
    ProfilePicture,

    /// <summary>
    /// Voice message audio file
    /// </summary>
    VoiceMessage,

    /// <summary>
    /// Document file
    /// </summary>
    Document,

    /// <summary>
    /// Image file
    /// </summary>
    Image,

    /// <summary>
    /// Video file
    /// </summary>
    Video,

    /// <summary>
    /// Audio file
    /// </summary>
    Audio,

    /// <summary>
    /// Temporary file
    /// </summary>
    Temporary,

    /// <summary>
    /// Service provider logo
    /// </summary>
    ServiceProviderLogo,

    /// <summary>
    /// Service provider document (license, insurance, etc.)
    /// </summary>
    ServiceProviderDocument,

    /// <summary>
    /// User verification document
    /// </summary>
    VerificationDocument,

    /// <summary>
    /// Pet photo
    /// </summary>
    PetPhoto,

    /// <summary>
    /// Service booking attachment
    /// </summary>
    BookingAttachment,

    /// <summary>
    /// Report or feedback attachment
    /// </summary>
    ReportAttachment,

    /// <summary>
    /// System generated file
    /// </summary>
    SystemGenerated
}