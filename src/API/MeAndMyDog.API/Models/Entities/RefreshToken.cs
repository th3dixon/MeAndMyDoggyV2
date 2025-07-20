using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a refresh token for JWT authentication
/// </summary>
[Table("RefreshTokens")]
public class RefreshToken
{
    /// <summary>
    /// Unique identifier for the refresh token
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The actual refresh token value
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User ID this token belongs to
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Whether the token is still active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the token was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the token was revoked (if applicable)
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// IP address where the token was created
    /// </summary>
    [MaxLength(45)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// IP address where the token was revoked
    /// </summary>
    [MaxLength(45)]
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Reason for revoking the token
    /// </summary>
    [MaxLength(100)]
    public string? ReasonRevoked { get; set; }

    /// <summary>
    /// Check if the refresh token is expired
    /// </summary>
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Check if the refresh token is valid (active and not expired)
    /// </summary>
    [NotMapped]
    public bool IsValid => IsActive && !IsExpired;
}