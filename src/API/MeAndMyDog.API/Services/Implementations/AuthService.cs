using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.DTOs.Auth;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Implementation of authentication service with role-based access control
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILocationService _locationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context,
        ILocationService locationService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
        _locationService = locationService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto model)
    {
        try
        {
            _logger.LogInformation("Starting registration for user: {Email}", model.Email);

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return ServiceResult<AuthResponseDto>.FailureResult("User with this email already exists");
            }

            // Get coordinates for the postcode
            var coordinates = await _locationService.GetCoordinatesFromPostCodeAsync(model.PostCode);
            
            // Create the user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true, // For demo purposes - in production, implement email confirmation
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                PostCode = model.PostCode,
                City = model.City,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                County = model.County,
                UserType = model.UserType,
                Latitude = coordinates?.Latitude,
                Longitude = coordinates?.Longitude,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Create user account
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<AuthResponseDto>.FailureResult(errors.ToArray());
            }

            _logger.LogInformation("User account created successfully for: {Email}", model.Email);

            // Assign roles based on user type
            await AssignUserRolesAsync(user, model.UserType);

            // Create ServiceProvider record if user is a service provider
            if (model.UserType == UserType.ServiceProvider)
            {
                await CreateServiceProviderRecordAsync(user, model);
            }

            // Generate tokens
            var authResponse = await GenerateAuthResponseAsync(user);

            _logger.LogInformation("Registration completed successfully for: {Email}", model.Email);
            return ServiceResult<AuthResponseDto>.SuccessResult(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for: {Email}", model.Email);
            return ServiceResult<AuthResponseDto>.FailureResult("An error occurred during registration");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login failed - user not found or inactive: {Email}", model.Email);
                return ServiceResult<AuthResponseDto>.FailureResult("Invalid credentials");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", model.Email);
                return ServiceResult<AuthResponseDto>.FailureResult("Invalid credentials");
            }

            var authResponse = await GenerateAuthResponseAsync(user);

            _logger.LogInformation("Login successful for: {Email}", model.Email);
            return ServiceResult<AuthResponseDto>.SuccessResult(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for: {Email}", model.Email);
            return ServiceResult<AuthResponseDto>.FailureResult("An error occurred during login");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Find the refresh token in the database
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IsActive);

            if (storedToken == null || storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return ServiceResult<AuthResponseDto>.FailureResult("Invalid or expired refresh token");
            }

            // Invalidate the old refresh token
            storedToken.IsActive = false;
            storedToken.RevokedAt = DateTimeOffset.UtcNow;

            // Generate new tokens
            var authResponse = await GenerateAuthResponseAsync(storedToken.User);

            await _context.SaveChangesAsync();

            return ServiceResult<AuthResponseDto>.SuccessResult(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return ServiceResult<AuthResponseDto>.FailureResult("An error occurred during token refresh");
        }
    }

    /// <inheritdoc />
    public async Task LogoutAsync(string refreshToken)
    {
        try
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.IsActive = false;
                storedToken.RevokedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal whether the user exists or not
                return ServiceResult<object>.SuccessResult(new { message = "If an account with that email exists, a password reset link has been sent." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // TODO: Send email with reset link containing the token
            // For now, just log it (in production, implement proper email service)
            _logger.LogInformation("Password reset token for {Email}: {Token}", email, token);

            return ServiceResult<object>.SuccessResult(new { message = "Password reset link sent to your email address" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for: {Email}", email);
            return ServiceResult<object>.FailureResult("An error occurred while processing your request");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordDto model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return ServiceResult<object>.FailureResult("Invalid reset token");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<object>.FailureResult("Failed to reset password");
            }

            return ServiceResult<object>.SuccessResult(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for: {Email}", model.Email);
            return ServiceResult<object>.FailureResult("An error occurred while resetting your password");
        }
    }

    #region Private Methods

    /// <summary>
    /// Assigns appropriate roles to user based on their type
    /// </summary>
    private async Task AssignUserRolesAsync(ApplicationUser user, UserType userType)
    {
        try
        {
            // Always assign the base User role
            await EnsureRoleExistsAndAssignAsync(user, "User");

            // Assign specific role based on user type
            switch (userType)
            {
                case UserType.PetOwner:
                    await EnsureRoleExistsAndAssignAsync(user, "PetOwner");
                    break;
                case UserType.ServiceProvider:
                    await EnsureRoleExistsAndAssignAsync(user, "ServiceProvider");
                    break;
                default:
                    _logger.LogWarning("Unknown user type {UserType} for user {Email}", userType, user.Email);
                    break;
            }

            _logger.LogInformation("Roles assigned successfully for user: {Email}, Type: {UserType}", user.Email, userType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles for user: {Email}", user.Email);
            throw;
        }
    }

    /// <summary>
    /// Ensures role exists and assigns it to the user
    /// </summary>
    private async Task EnsureRoleExistsAndAssignAsync(ApplicationUser user, string roleName)
    {
        // Check if role exists, create if it doesn't
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var role = new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                Description = $"{roleName} role",
                IsSystemRole = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var roleResult = await _roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create role {roleName}");
            }

            _logger.LogInformation("Created missing role: {RoleName}", roleName);
        }

        // Assign role to user
        var assignResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!assignResult.Succeeded)
        {
            throw new InvalidOperationException($"Failed to assign role {roleName} to user {user.Email}");
        }
    }

    /// <summary>
    /// Creates a ServiceProvider record for service provider users
    /// </summary>
    private async Task CreateServiceProviderRecordAsync(ApplicationUser user, RegisterDto model)
    {
        try
        {
            var serviceProvider = new Models.Entities.ServiceProvider
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                BusinessName = model.BusinessName ?? $"{user.FirstName} {user.LastName}",
                BusinessDescription = "Professional pet care services",
                // CompanyNumber and VatNumber properties not available in ServiceProvider entity
                // BusinessLicense = model.CompanyNumber, // Could map company number here if needed
                IsActive = true,
                IsVerified = false, // Requires manual verification
                Rating = 0.0m,
                ReviewCount = 0,
                YearsOfExperience = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.ServiceProviders.Add(serviceProvider);

            // Create provider services if specified
            if (model.Services?.Any() == true)
            {
                var providerServices = model.Services
                    .Where(serviceDto => Guid.TryParse(serviceDto.ServiceCategoryId, out _))
                    .Select(serviceDto => new ProviderService
                    {
                        ProviderServiceId = Guid.NewGuid(),
                        ProviderId = Guid.Parse(serviceProvider.Id),
                        ServiceCategoryId = Guid.Parse(serviceDto.ServiceCategoryId),
                        IsOffered = true,
                        OffersEmergencyService = serviceDto.OffersEmergencyService,
                        OffersWeekendService = serviceDto.OffersWeekendService,
                        OffersEveningService = serviceDto.OffersEveningService,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    })
                    .ToList();

                _context.ProviderService.AddRange(providerServices);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("ServiceProvider record created for user: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ServiceProvider record for user: {Email}", user.Email);
            throw;
        }
    }

    /// <summary>
    /// Generates authentication response with JWT tokens
    /// </summary>
    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        // Generate JWT access token
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["JWT--SecretKey"] ?? _configuration["Jwt:SecretKey"] ?? "your-super-secret-key-here-must-be-at-least-256-bits-long-for-security";
        var key = Encoding.UTF8.GetBytes(secretKey);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("user_type", user.UserType.ToString())
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // 1 hour expiry
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Generate refresh token
        var refreshToken = GenerateRefreshToken();
        
        // Store refresh token in database
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30), // 30 days expiry
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType.ToString(),
                IsServiceProvider = roles.Contains("ServiceProvider"),
                ServiceProviderId = null // TODO: Set this if user is a service provider
            }
        };
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}