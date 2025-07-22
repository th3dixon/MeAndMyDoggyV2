using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a friendship relationship between two users
/// Supports friend requests, acceptance, and blocking functionality
/// </summary>
public class Friendship
{
    /// <summary>
    /// Unique identifier for the friendship relationship
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the user who initiated the friend request
    /// </summary>
    [Required]
    public string RequesterId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for the user who sent the friend request
    /// </summary>
    [ForeignKey(nameof(RequesterId))]
    public virtual ApplicationUser Requester { get; set; } = null!;

    /// <summary>
    /// ID of the user who received the friend request
    /// </summary>
    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for the user who received the friend request
    /// </summary>
    [ForeignKey(nameof(ReceiverId))]
    public virtual ApplicationUser Receiver { get; set; } = null!;

    /// <summary>
    /// Current status of the friendship
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// When the friend request was initially sent
    /// </summary>
    [Required]
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the friend request was accepted (null if still pending or blocked)
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    /// <summary>
    /// When the friendship was blocked (null if not blocked)
    /// </summary>
    public DateTime? BlockedAt { get; set; }

    /// <summary>
    /// Optional note from the requester when sending the friend request
    /// </summary>
    [MaxLength(500)]
    public string? RequestNote { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last updated
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Check if the friendship is in Accepted status
    /// </summary>
    public bool IsAccepted => Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Check if the friendship is in Pending status
    /// </summary>
    public bool IsPending => Status.Equals("Pending", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Check if the friendship is in Blocked status
    /// </summary>
    public bool IsBlocked => Status.Equals("Blocked", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Get the other user in this friendship relationship
    /// </summary>
    /// <param name="currentUserId">Current user's ID</param>
    /// <returns>The other user's ID in the friendship</returns>
    public string GetOtherUserId(string currentUserId)
    {
        return RequesterId == currentUserId ? ReceiverId : RequesterId;
    }

    /// <summary>
    /// Check if the current user is the one who initiated the friend request
    /// </summary>
    /// <param name="currentUserId">Current user's ID</param>
    /// <returns>True if current user is the requester</returns>
    public bool IsRequester(string currentUserId)
    {
        return RequesterId == currentUserId;
    }
}

/// <summary>
/// Enum for friendship status values
/// </summary>
public static class FriendshipStatus
{
    public const string Pending = "Pending";
    public const string Accepted = "Accepted";
    public const string Blocked = "Blocked";
}