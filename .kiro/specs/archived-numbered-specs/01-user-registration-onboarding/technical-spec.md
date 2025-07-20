# User Registration & Onboarding - Technical Specification

## Component Overview
The User Registration & Onboarding system provides a dual-path registration flow for pet owners (free only) and service providers (free + premium tiers), with comprehensive onboarding to ensure profile completion and platform engagement.

## Database Schema

### Primary Tables
- **AspNetUsers** (extended) - Core user identity
- **Addresses** - User location data
- **UserOnboarding** - Tracks onboarding progress
- **UserPreferences** - Platform preferences
- **EmailVerifications** - Email verification tokens
- **PasswordResets** - Password reset tokens

### New Tables for This Component

```sql
-- UserOnboarding
CREATE TABLE [dbo].[UserOnboarding] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [OnboardingStatus] INT NOT NULL DEFAULT 0, -- 0: NotStarted, 1: InProgress, 2: Completed
    [CurrentStep] INT NOT NULL DEFAULT 0,
    [TotalSteps] INT NOT NULL,
    [CompletedSteps] NVARCHAR(MAX) NULL, -- JSON array
    [SkippedSteps] NVARCHAR(MAX) NULL, -- JSON array
    [StartedAt] DATETIME2 NULL,
    [CompletedAt] DATETIME2 NULL,
    [LastActivityAt] DATETIME2 NULL,
    [DeviceInfo] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserOnboarding_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- UserPreferences
CREATE TABLE [dbo].[UserPreferences] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [CommunicationPreferences] NVARCHAR(MAX) NOT NULL, -- JSON
    [NotificationSettings] NVARCHAR(MAX) NOT NULL, -- JSON
    [PrivacySettings] NVARCHAR(MAX) NOT NULL, -- JSON
    [DisplayPreferences] NVARCHAR(MAX) NULL, -- JSON
    [MarketingOptIn] BIT NOT NULL DEFAULT 0,
    [DataSharingOptIn] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_UserPreferences_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- EmailVerifications
CREATE TABLE [dbo].[EmailVerifications] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [Token] NVARCHAR(500) NOT NULL UNIQUE,
    [Purpose] INT NOT NULL, -- 0: Registration, 1: EmailChange
    [IsUsed] BIT NOT NULL DEFAULT 0,
    [UsedAt] DATETIME2 NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_EmailVerifications_Token] ([Token]),
    CONSTRAINT [FK_EmailVerifications_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- PasswordResets
CREATE TABLE [dbo].[PasswordResets] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [Token] NVARCHAR(500) NOT NULL UNIQUE,
    [IsUsed] BIT NOT NULL DEFAULT 0,
    [UsedAt] DATETIME2 NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_PasswordResets_Token] ([Token]),
    CONSTRAINT [FK_PasswordResets_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- SocialLogins
CREATE TABLE [dbo].[SocialLogins] (
    [UserId] NVARCHAR(450) NOT NULL,
    [Provider] NVARCHAR(50) NOT NULL, -- Google, Facebook, Apple
    [ProviderUserId] NVARCHAR(200) NOT NULL,
    [AccessToken] NVARCHAR(MAX) NULL,
    [RefreshToken] NVARCHAR(MAX) NULL,
    [TokenExpiresAt] DATETIME2 NULL,
    [ProfileData] NVARCHAR(MAX) NULL, -- JSON
    [ConnectedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastUsedAt] DATETIME2 NULL,
    PRIMARY KEY ([UserId], [Provider]),
    CONSTRAINT [FK_SocialLogins_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);
```

## API Endpoints

### Registration Flow

```yaml
/api/v1/auth/register:
  /check-email:
    POST:
      description: Check if email is available
      body:
        email: string
      responses:
        200: 
          available: boolean
          suggestions: array[string] # If not available
  
  /pet-owner:
    POST:
      description: Register as pet owner
      body:
        email: string
        password: string
        firstName: string
        lastName: string
        phoneNumber: string (optional)
        marketingOptIn: boolean
      responses:
        201:
          userId: string
          emailVerificationRequired: boolean
          onboardingToken: string
  
  /service-provider:
    POST:
      description: Register as service provider
      body:
        # Personal details
        email: string
        password: string
        firstName: string
        lastName: string
        phoneNumber: string
        
        # Business details
        businessName: string
        businessType: enum [Individual, Company]
        companyNumber: string (optional)
        
        # Service details
        serviceCategories: array[number]
        serviceAreas: array[string] # Postcodes
        
        # Subscription
        subscriptionTier: enum [Free, Premium]
        paymentMethodId: string (if Premium)
      responses:
        201:
          userId: string
          providerId: number
          emailVerificationRequired: boolean
          onboardingToken: string
          subscriptionStatus: object

/api/v1/auth/verify:
  /email:
    POST:
      description: Verify email address
      body:
        token: string
      responses:
        200: Email verified successfully
        400: Invalid or expired token
  
  /resend:
    POST:
      description: Resend verification email
      auth: required
      responses:
        200: Email sent
        429: Too many requests

/api/v1/auth/social:
  /google:
    POST:
      description: Register/login with Google
      body:
        idToken: string
      responses:
        200: User authenticated
  
  /facebook:
    POST:
      description: Register/login with Facebook
      body:
        accessToken: string
      responses:
        200: User authenticated
  
  /apple:
    POST:
      description: Register/login with Apple
      body:
        identityToken: string
        authorizationCode: string
      responses:
        200: User authenticated
```

### Onboarding Flow

```yaml
/api/v1/onboarding:
  /status:
    GET:
      description: Get onboarding status
      auth: required
      responses:
        200:
          status: enum [NotStarted, InProgress, Completed]
          currentStep: number
          totalSteps: number
          completedSteps: array[number]
          nextStep: object
  
  /steps:
    GET:
      description: Get all onboarding steps
      auth: required
      responses:
        200:
          steps: array[OnboardingStep]
  
  /steps/{stepId}/complete:
    POST:
      description: Mark step as completed
      auth: required
      body:
        data: object # Step-specific data
      responses:
        200:
          nextStep: object
          progress: number # Percentage
  
  /steps/{stepId}/skip:
    POST:
      description: Skip an optional step
      auth: required
      responses:
        200:
          nextStep: object
  
  /complete:
    POST:
      description: Complete onboarding
      auth: required
      responses:
        200:
          redirectUrl: string
          rewards: array[object] # Any welcome rewards
```

### Profile Setup

```yaml
/api/v1/profile:
  /setup:
    POST:
      description: Initial profile setup
      auth: required
      body:
        # Location
        address:
          addressLine1: string
          addressLine2: string
          city: string
          postCode: string
        
        # Preferences
        preferences:
          language: string
          timezone: string
          currency: string
        
        # Profile
        dateOfBirth: date
        profileImageUrl: string
      responses:
        200: Profile updated
  
  /preferences:
    GET:
      description: Get user preferences
      auth: required
      responses:
        200: UserPreferences object
    
    PUT:
      description: Update preferences
      auth: required
      body:
        communicationPreferences: object
        notificationSettings: object
        privacySettings: object
      responses:
        200: Updated preferences
```

## Frontend Components

### Registration Components (Vue.js)

```typescript
// RegistrationWizard.vue
interface RegistrationWizardProps {
  userType: 'pet-owner' | 'service-provider'
}

interface RegistrationStep {
  id: string
  title: string
  component: Component
  validation: ValidationRules
  optional: boolean
}

// Steps:
// 1. AccountTypeSelection.vue
// 2. PersonalDetails.vue
// 3. BusinessDetails.vue (providers only)
// 4. ServiceSelection.vue (providers only)
// 5. LocationSetup.vue
// 6. SubscriptionPlan.vue (providers only)
// 7. EmailVerification.vue
// 8. WelcomeScreen.vue
```

### Onboarding Components

```typescript
// OnboardingFlow.vue
interface OnboardingFlowProps {
  token: string
  userType: UserType
}

// Pet Owner Steps:
// 1. ProfilePhotoUpload.vue
// 2. FirstDogProfile.vue
// 3. PreferencesSetup.vue
// 4. FindProviders.vue
// 5. OnboardingComplete.vue

// Provider Steps:
// 1. BusinessProfileSetup.vue
// 2. ServiceConfiguration.vue
// 3. AvailabilitySetup.vue
// 4. PricingSetup.vue
// 5. VerificationDocuments.vue
// 6. OnboardingComplete.vue
```

## Technical Implementation Details

### Password Security

```csharp
public class PasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    
    public string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256);
        
        var salt = algorithm.Salt;
        var key = algorithm.GetBytes(KeySize);
        
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }
}
```

### Email Verification

```csharp
public class EmailVerificationService
{
    private readonly TimeSpan TokenExpiry = TimeSpan.FromHours(24);
    
    public async Task<string> GenerateVerificationToken(string userId, string email)
    {
        var token = GenerateSecureToken();
        var verification = new EmailVerification
        {
            UserId = userId,
            Email = email,
            Token = token,
            Purpose = VerificationPurpose.Registration,
            ExpiresAt = DateTime.UtcNow.Add(TokenExpiry)
        };
        
        await _context.EmailVerifications.AddAsync(verification);
        await _context.SaveChangesAsync();
        
        return token;
    }
    
    private string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
```

### Social Authentication

```csharp
public class SocialAuthService
{
    public async Task<AuthResult> AuthenticateWithGoogle(string idToken)
    {
        var payload = await ValidateGoogleToken(idToken);
        
        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            user = await CreateUserFromSocialLogin(payload, "Google");
        }
        
        await LinkSocialAccount(user.Id, "Google", payload.Subject);
        
        return await GenerateAuthTokens(user);
    }
    
    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _googleClientId }
        };
        
        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}
```

### Onboarding State Machine

```csharp
public class OnboardingStateMachine
{
    private readonly Dictionary<UserType, List<OnboardingStep>> _steps;
    
    public OnboardingStateMachine()
    {
        _steps = new Dictionary<UserType, List<OnboardingStep>>
        {
            [UserType.PetOwner] = new List<OnboardingStep>
            {
                new OnboardingStep { Id = "profile-photo", Required = false },
                new OnboardingStep { Id = "first-dog", Required = true },
                new OnboardingStep { Id = "preferences", Required = true },
                new OnboardingStep { Id = "find-providers", Required = false }
            },
            [UserType.ServiceProvider] = new List<OnboardingStep>
            {
                new OnboardingStep { Id = "business-profile", Required = true },
                new OnboardingStep { Id = "services", Required = true },
                new OnboardingStep { Id = "availability", Required = true },
                new OnboardingStep { Id = "pricing", Required = true },
                new OnboardingStep { Id = "verification", Required = false }
            }
        };
    }
    
    public OnboardingStep GetNextStep(string userId, UserType userType)
    {
        var completedSteps = GetCompletedSteps(userId);
        var allSteps = _steps[userType];
        
        return allSteps.FirstOrDefault(s => 
            !completedSteps.Contains(s.Id) && 
            (s.Required || !HasSkippedStep(userId, s.Id)));
    }
}
```

## Security Considerations

### Registration Security
1. **Email Verification**: Required for all accounts
2. **Password Requirements**:
   - Minimum 8 characters
   - At least one uppercase, lowercase, number, and special character
   - Password strength meter
   - Common password blacklist
3. **Rate Limiting**:
   - Registration: 5 attempts per IP per hour
   - Email verification: 3 resends per day
   - Password reset: 3 attempts per day
4. **CAPTCHA**: For suspicious registration patterns

### Data Protection
1. **Personal Data Encryption**: All PII encrypted at rest
2. **Secure Token Generation**: Cryptographically secure random tokens
3. **Session Management**: 
   - Secure, httpOnly cookies
   - Session timeout after 30 minutes of inactivity
4. **Audit Trail**: All registration attempts logged

## Performance Optimization

### Caching Strategy
```csharp
// Redis caching for onboarding state
public async Task<OnboardingStatus> GetOnboardingStatus(string userId)
{
    var cacheKey = $"onboarding:status:{userId}";
    var cached = await _cache.GetAsync<OnboardingStatus>(cacheKey);
    
    if (cached != null) return cached;
    
    var status = await _repository.GetOnboardingStatus(userId);
    await _cache.SetAsync(cacheKey, status, TimeSpan.FromMinutes(10));
    
    return status;
}
```

### Database Optimization
- Indexes on email fields for quick lookups
- Partitioned tables for audit logs
- Async operations throughout

## Monitoring & Analytics

### Key Metrics
1. **Registration Funnel**:
   - Start rate
   - Step completion rates
   - Drop-off points
   - Time to complete
2. **Verification Metrics**:
   - Email verification rate
   - Time to verify
   - Resend rate
3. **Social Login Usage**:
   - Provider breakdown
   - Conversion rates
4. **Error Tracking**:
   - Failed registrations
   - Validation errors
   - System errors

### Event Tracking
```csharp
public async Task TrackRegistrationEvent(string eventName, Dictionary<string, object> properties)
{
    var analyticsEvent = new UserAnalytics
    {
        UserId = properties.GetValueOrDefault("userId")?.ToString(),
        EventType = eventName,
        EventCategory = "Registration",
        EventData = JsonSerializer.Serialize(properties),
        Timestamp = DateTime.UtcNow
    };
    
    await _context.UserAnalytics.AddAsync(analyticsEvent);
}
```

## Integration Points

### Email Service (SendGrid)
```csharp
public async Task SendVerificationEmail(string email, string token)
{
    var template = await _templateService.GetTemplate("email-verification");
    var verificationUrl = $"{_baseUrl}/verify-email?token={token}";
    
    var msg = new SendGridMessage
    {
        From = new EmailAddress("noreply@meandmydoggy.co.uk", "MeAndMyDoggy"),
        Subject = "Verify your email address",
        HtmlContent = template.Replace("{{verification_url}}", verificationUrl)
    };
    
    msg.AddTo(email);
    await _sendGridClient.SendEmailAsync(msg);
}
```

### Payment Processing (Stripe) - For Premium Providers
```csharp
public async Task<Subscription> CreateProviderSubscription(string providerId, string paymentMethodId)
{
    var customer = await CreateOrGetStripeCustomer(providerId);
    
    await _stripe.PaymentMethods.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
    {
        Customer = customer.Id
    });
    
    var subscription = await _stripe.Subscriptions.CreateAsync(new SubscriptionCreateOptions
    {
        Customer = customer.Id,
        Items = new List<SubscriptionItemOptions>
        {
            new SubscriptionItemOptions
            {
                Price = _premiumPriceId // £15-19.99/month
            }
        },
        DefaultPaymentMethod = paymentMethodId,
        TrialPeriodDays = 14 // Free trial
    });
    
    return subscription;
}
```

## Error Handling

### Registration Errors
```csharp
public enum RegistrationError
{
    EmailAlreadyExists = 1001,
    InvalidPassword = 1002,
    InvalidEmail = 1003,
    PhoneNumberInvalid = 1004,
    BusinessNameTaken = 1005,
    PaymentMethodFailed = 1006,
    VerificationTokenInvalid = 1007,
    VerificationTokenExpired = 1008,
    SocialLoginFailed = 1009,
    RateLimitExceeded = 1010
}

public class RegistrationException : Exception
{
    public RegistrationError ErrorCode { get; }
    public Dictionary<string, string> ValidationErrors { get; }
}
```

## Testing Strategy

### Unit Tests
```csharp
[TestClass]
public class RegistrationServiceTests
{
    [TestMethod]
    public async Task RegisterPetOwner_ValidData_CreatesUser()
    {
        // Arrange
        var request = new PetOwnerRegistrationRequest
        {
            Email = "test@example.com",
            Password = "Test123!@#",
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Act
        var result = await _service.RegisterPetOwner(request);
        
        // Assert
        Assert.IsNotNull(result.UserId);
        Assert.IsTrue(result.EmailVerificationRequired);
    }
}
```

### Integration Tests
- Full registration flow testing
- Email verification testing
- Social login flow testing
- Onboarding completion testing

### Load Testing
- Target: 1000 concurrent registrations
- Response time: < 500ms p95
- Error rate: < 0.1%