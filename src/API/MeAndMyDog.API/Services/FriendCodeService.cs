using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using System.Security.Cryptography;
using System.Text;

namespace MeAndMyDog.API.Services;

/// <summary>
/// Service for generating and managing unique friend codes
/// </summary>
public class FriendCodeService
{
    private readonly ApplicationDbContext _context;
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CODE_LENGTH = 8;

    /// <summary>
    /// Initializes a new instance of the FriendCodeService
    /// </summary>
    /// <param name="context">Database context</param>
    public FriendCodeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Generates a unique 8-character alphanumeric friend code
    /// </summary>
    /// <returns>Unique friend code</returns>
    public async Task<string> GenerateUniqueFriendCodeAsync()
    {
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            var code = GenerateRandomCode();
            var exists = await _context.Users
                .AnyAsync(u => u.FriendCode == code);

            if (!exists)
            {
                return code;
            }

            attempts++;
        }

        throw new InvalidOperationException("Unable to generate unique friend code after maximum attempts");
    }

    /// <summary>
    /// Generates a random 8-character alphanumeric code
    /// </summary>
    /// <returns>Random friend code</returns>
    private string GenerateRandomCode()
    {
        var result = new StringBuilder(CODE_LENGTH);
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[CODE_LENGTH];
        rng.GetBytes(buffer);

        for (int i = 0; i < CODE_LENGTH; i++)
        {
            result.Append(ALLOWED_CHARS[buffer[i] % ALLOWED_CHARS.Length]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Validates friend code format
    /// </summary>
    /// <param name="friendCode">Friend code to validate</param>
    /// <returns>True if valid format</returns>
    public bool IsValidFriendCodeFormat(string friendCode)
    {
        if (string.IsNullOrWhiteSpace(friendCode) || friendCode.Length != CODE_LENGTH)
        {
            return false;
        }

        return friendCode.All(c => ALLOWED_CHARS.Contains(char.ToUpper(c)));
    }

    /// <summary>
    /// Normalizes friend code to uppercase
    /// </summary>
    /// <param name="friendCode">Friend code to normalize</param>
    /// <returns>Normalized friend code</returns>
    public string NormalizeFriendCode(string friendCode)
    {
        return friendCode?.ToUpper().Trim() ?? string.Empty;
    }

    /// <summary>
    /// Regenerates friend code for a user (in case they want a new one)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>New friend code</returns>
    public async Task<string> RegenerateFriendCodeAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var newCode = await GenerateUniqueFriendCodeAsync();
        user.FriendCode = newCode;
        await _context.SaveChangesAsync();

        return newCode;
    }
}