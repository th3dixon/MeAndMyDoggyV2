using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs.Provider;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Implementations;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for upgrading pet owners to service providers
/// </summary>
public class ProviderUpgradeService : IProviderUpgradeService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProviderUpgradeService> _logger;

    public ProviderUpgradeService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<ProviderUpgradeService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ProviderUpgradePromotionDto> GetUpgradePromotionAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ProviderUpgradePromotionDto { ShowPromotion = false };
            }

            // Check if user is already a provider
            var isProvider = await _userManager.IsInRoleAsync(user, "ServiceProvider");
            if (isProvider)
            {
                return new ProviderUpgradePromotionDto { ShowPromotion = false };
            }

            // Check if user is a pet owner with pets (good candidates for becoming providers)
            var hasPets = await _context.DogProfiles
                .AnyAsync(d => d.OwnerId == userId && d.IsActive);

            if (!hasPets)
            {
                return new ProviderUpgradePromotionDto { ShowPromotion = false };
            }

            return new ProviderUpgradePromotionDto
            {
                ShowPromotion = true,
                Message = "Love dogs? Turn your passion into income!",
                Benefits = new List<string>
                {
                    "Earn money doing what you love",
                    "Set your own rates and schedule",
                    "Help other pet parents in your area",
                    "Built-in client base from our platform"
                },
                CallToAction = "Become a Service Provider"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade promotion for user {UserId}", userId);
            return new ProviderUpgradePromotionDto { ShowPromotion = false };
        }
    }

    public async Task<ProviderUpgradeEligibilityDto> CheckUpgradeEligibilityAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ProviderUpgradeEligibilityDto
                {
                    IsEligible = false,
                    IneligibilityReason = "User not found"
                };
            }

            // Check if already a provider
            var isProvider = await _userManager.IsInRoleAsync(user, "ServiceProvider");
            if (isProvider)
            {
                return new ProviderUpgradeEligibilityDto
                {
                    IsEligible = false,
                    IsAlreadyProvider = true,
                    IneligibilityReason = "User is already a service provider"
                };
            }

            var accountAge = (DateTime.UtcNow - user.CreatedAt.DateTime).Days;
            var hasViolations = await CheckUserViolationsAsync(userId);
            
            var requirements = new UpgradeRequirementsDto
            {
                EmailVerified = user.EmailConfirmed,
                ProfileComplete = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName),
                MinimumAccountAge = accountAge >= 7,
                NoViolations = !hasViolations
            };

            var isEligible = requirements.EmailVerified && 
                           requirements.ProfileComplete && 
                           requirements.MinimumAccountAge && 
                           requirements.NoViolations;

            return new ProviderUpgradeEligibilityDto
            {
                IsEligible = isEligible,
                IsAlreadyProvider = false,
                AccountAgeDays = accountAge,
                Requirements = requirements,
                IneligibilityReason = isEligible ? null : "Not all requirements met"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking upgrade eligibility for user {UserId}", userId);
            return new ProviderUpgradeEligibilityDto
            {
                IsEligible = false,
                IneligibilityReason = "Error checking eligibility"
            };
        }
    }

    public async Task<ProviderUpgradeResultDto> ProcessProviderUpgradeAsync(string userId, ProviderUpgradeRequestDto upgradeRequest)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ProviderUpgradeResultDto
                {
                    Success = false,
                    Message = "User not found",
                    Status = ProviderUpgradeStatus.Rejected
                };
            }

            // Check eligibility first
            var eligibility = await CheckUpgradeEligibilityAsync(userId);
            if (!eligibility.IsEligible)
            {
                return new ProviderUpgradeResultDto
                {
                    Success = false,
                    Message = eligibility.IneligibilityReason ?? "Not eligible for upgrade",
                    Status = ProviderUpgradeStatus.Rejected
                };
            }

            // Create ServiceProvider entity
            var serviceProvider = new Models.Entities.ServiceProvider
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                BusinessName = upgradeRequest.BusinessName,
                BusinessEmail = user.Email,
                BusinessPhone = user.PhoneNumber,
                BusinessAddress = $"{upgradeRequest.AddressLine1}, {upgradeRequest.AddressLine2}, {upgradeRequest.City}, {upgradeRequest.County}, {upgradeRequest.PostCode}".Trim(',', ' '),
                IsActive = true,
                IsPremium = false,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.ServiceProviders.Add(serviceProvider);

            // Add ServiceProvider role to user
            var roleResult = await _userManager.AddToRoleAsync(user, "ServiceProvider");
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to add ServiceProvider role to user {UserId}: {Errors}", 
                    userId, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                
                return new ProviderUpgradeResultDto
                {
                    Success = false,
                    Message = "Failed to assign provider role",
                    Status = ProviderUpgradeStatus.Rejected
                };
            }

            // Process services - reuse registration logic
            foreach (var serviceDto in upgradeRequest.Services)
            {
                var providerService = new ProviderService
                {
                    ProviderServiceId = Guid.NewGuid(),
                    ProviderId = Guid.Parse(serviceProvider.Id),
                    ServiceCategoryId = serviceDto.ServiceCategoryId,
                    OffersEmergencyService = serviceDto.OffersEmergencyService,
                    OffersWeekendService = serviceDto.OffersWeekendService,
                    OffersEveningService = serviceDto.OffersEveningService,
                    IsOffered = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ProviderService.Add(providerService);

                // Add pricing for sub-services
                foreach (var subServiceDto in serviceDto.SubServices)
                {
                    var pricing = new ProviderServicePricing
                    {
                        ProviderServicePricingId = Guid.NewGuid(),
                        ProviderServiceId = providerService.ProviderServiceId,
                        SubServiceId = subServiceDto.SubServiceId,
                        Price = subServiceDto.Price,
                        PricingType = (PricingType)subServiceDto.PricingType,
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.ProviderServicePricing.Add(pricing);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Successfully upgraded user {UserId} to service provider {ProviderId}", 
                userId, serviceProvider.Id);

            return new ProviderUpgradeResultDto
            {
                Success = true,
                Message = "Successfully upgraded to service provider!",
                ProviderId = serviceProvider.Id,
                Status = ProviderUpgradeStatus.Approved,
                NextSteps = new List<string>
                {
                    "Complete your provider profile",
                    "Upload a profile photo",
                    "Set your availability schedule",
                    "Start receiving bookings!"
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error processing provider upgrade for user {UserId}", userId);
            
            return new ProviderUpgradeResultDto
            {
                Success = false,
                Message = "An error occurred during upgrade process",
                Status = ProviderUpgradeStatus.Rejected
            };
        }
    }

    public async Task<ProviderUpgradeProgressDto> GetUpgradeProgressAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ProviderUpgradeProgressDto
                {
                    Status = ProviderUpgradeStatus.NotStarted,
                    ProgressPercentage = 0,
                    CurrentStep = "Account not found"
                };
            }

            var isProvider = await _userManager.IsInRoleAsync(user, "ServiceProvider");
            if (isProvider)
            {
                return new ProviderUpgradeProgressDto
                {
                    Status = ProviderUpgradeStatus.Approved,
                    ProgressPercentage = 100,
                    CurrentStep = "Provider account active",
                    CompletedSteps = new List<string>
                    {
                        "Eligibility check passed",
                        "Business details submitted",
                        "Services configured",
                        "Account upgraded"
                    }
                };
            }

            var eligibility = await CheckUpgradeEligibilityAsync(userId);
            if (!eligibility.IsEligible)
            {
                var completedReqs = new List<string>();
                var remainingReqs = new List<string>();

                if (eligibility.Requirements.EmailVerified) completedReqs.Add("Email verified");
                else remainingReqs.Add("Verify your email address");

                if (eligibility.Requirements.ProfileComplete) completedReqs.Add("Profile complete");
                else remainingReqs.Add("Complete your profile");

                if (eligibility.Requirements.MinimumAccountAge) completedReqs.Add("Account age requirement met");
                else remainingReqs.Add($"Wait {7 - eligibility.AccountAgeDays} more days");

                return new ProviderUpgradeProgressDto
                {
                    Status = ProviderUpgradeStatus.NotStarted,
                    ProgressPercentage = (int)((completedReqs.Count / 4.0) * 100),
                    CurrentStep = "Complete requirements to upgrade",
                    CompletedSteps = completedReqs,
                    RemainingSteps = remainingReqs
                };
            }

            return new ProviderUpgradeProgressDto
            {
                Status = ProviderUpgradeStatus.NotStarted,
                ProgressPercentage = 25,
                CurrentStep = "Ready to upgrade",
                CompletedSteps = new List<string> { "All requirements met" },
                RemainingSteps = new List<string> { "Complete upgrade wizard" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade progress for user {UserId}", userId);
            return new ProviderUpgradeProgressDto
            {
                Status = ProviderUpgradeStatus.NotStarted,
                ProgressPercentage = 0,
                CurrentStep = "Error checking progress"
            };
        }
    }

    private async Task<bool> CheckUserViolationsAsync(string userId)
    {
        try
        {
            // Check for various violation types that would prevent provider upgrade
            var violations = new List<bool>();

            // Check for suspended user account
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                violations.Add(true); // Account is currently locked out
            }

            // Check for recent security incidents (if SecurityIncident table exists)
            var hasRecentSecurityIncidents = await _context.Database
                .SqlQuery<int>($@"
                    SELECT CAST(CASE WHEN EXISTS(
                        SELECT 1 FROM SecurityIncident 
                        WHERE UserId = {userId} 
                        AND Severity IN ('High', 'Critical')
                        AND CreatedAt > DATEADD(month, -3, GETUTCDATE())
                        AND Status != 'Resolved'
                    ) THEN 1 ELSE 0 END AS int)")
                .FirstOrDefaultAsync() > 0;
            
            violations.Add(hasRecentSecurityIncidents);

            // Check for policy violations (if we have a violations tracking system)
            // This could include spam reports, content violations, etc.
            var hasPolicyViolations = await _context.Database
                .SqlQuery<int>($@"
                    SELECT CAST(CASE WHEN EXISTS(
                        SELECT 1 FROM UserViolation 
                        WHERE UserId = {userId} 
                        AND ViolationType IN ('Spam', 'ContentViolation', 'Fraud')
                        AND CreatedAt > DATEADD(month, -6, GETUTCDATE())
                        AND Status = 'Confirmed'
                    ) THEN 1 ELSE 0 END AS int)")
                .FirstOrDefaultAsync() > 0;
            
            violations.Add(hasPolicyViolations);

            // Return true if any violations exist
            return violations.Any(v => v);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking violations for user {UserId}", userId);
            
            // In case of error, be conservative and assume there might be violations
            // This prevents upgrading users when we can't properly check their status
            return true;
        }
    }
}