---
inclusion: always
---

# MeAndMyDog API Patterns & Common Scenarios

## API Design Patterns

### Controller Structure
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DogProfilesController : ControllerBase
{
    private readonly IDogProfileService _dogProfileService;
    private readonly ILogger<DogProfilesController> _logger;

    public DogProfilesController(
        IDogProfileService dogProfileService,
        ILogger<DogProfilesController> logger)
    {
        _dogProfileService = dogProfileService;
        _logger = logger;
    }
}
```

### Standard CRUD Operations
```csharp
// GET api/v1/dogprofiles
[HttpGet]
public async Task<ActionResult<PagedResult<DogProfileDto>>> GetDogProfiles(
    [FromQuery] DogProfileSearchRequest request)
{
    var result = await _dogProfileService.GetDogProfilesAsync(request);
    return Ok(result);
}

// GET api/v1/dogprofiles/{id}
[HttpGet("{id:guid}")]
public async Task<ActionResult<DogProfileDto>> GetDogProfile(Guid id)
{
    var dogProfile = await _dogProfileService.GetDogProfileAsync(id);
    if (dogProfile == null)
        return NotFound();
    
    return Ok(dogProfile);
}

// POST api/v1/dogprofiles
[HttpPost]
public async Task<ActionResult<DogProfileDto>> CreateDogProfile(
    CreateDogProfileRequest request)
{
    var dogProfile = await _dogProfileService.CreateDogProfileAsync(request);
    return CreatedAtAction(nameof(GetDogProfile), 
        new { id = dogProfile.Id }, dogProfile);
}

// PUT api/v1/dogprofiles/{id}
[HttpPut("{id:guid}")]
public async Task<IActionResult> UpdateDogProfile(
    Guid id, UpdateDogProfileRequest request)
{
    if (id != request.Id)
        return BadRequest();

    await _dogProfileService.UpdateDogProfileAsync(request);
    return NoContent();
}

// DELETE api/v1/dogprofiles/{id}
[HttpDelete("{id:guid}")]
public async Task<IActionResult> DeleteDogProfile(Guid id)
{
    await _dogProfileService.DeleteDogProfileAsync(id);
    return NoContent();
}
```

### Response Patterns

#### Standard API Response
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public string CorrelationId { get; set; }
}
```

#### Paged Results
```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
```

### Error Handling Patterns

#### Global Exception Handler
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = new ApiResponse<object>
        {
            Success = false,
            CorrelationId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse.Message = "Validation failed";
                apiResponse.Errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                break;
            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse.Message = "Resource not found";
                break;
            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiResponse.Message = "Unauthorized access";
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = "An error occurred while processing your request";
                break;
        }

        await response.WriteAsync(JsonSerializer.Serialize(apiResponse));
    }
}
```

## Service Layer Patterns

### Service Interface
```csharp
public interface IDogProfileService
{
    Task<PagedResult<DogProfileDto>> GetDogProfilesAsync(DogProfileSearchRequest request);
    Task<DogProfileDto?> GetDogProfileAsync(Guid id);
    Task<DogProfileDto> CreateDogProfileAsync(CreateDogProfileRequest request);
    Task UpdateDogProfileAsync(UpdateDogProfileRequest request);
    Task DeleteDogProfileAsync(Guid id);
}
```

### Service Implementation
```csharp
public class DogProfileService : IDogProfileService
{
    private readonly IRepository<DogProfile> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DogProfileService> _logger;

    public DogProfileService(
        IRepository<DogProfile> repository,
        IMapper mapper,
        ILogger<DogProfileService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DogProfileDto> CreateDogProfileAsync(CreateDogProfileRequest request)
    {
        _logger.LogInformation("Creating dog profile for user {UserId}", request.UserId);

        var dogProfile = _mapper.Map<DogProfile>(request);
        dogProfile.Id = Guid.NewGuid();
        dogProfile.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(dogProfile);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Dog profile created with ID {DogProfileId}", dogProfile.Id);

        return _mapper.Map<DogProfileDto>(dogProfile);
    }
}
```


## Authentication & Authorization Patterns

### JWT Token Generation
```csharp
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

### Authorization Policies
```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PetOwnerOnly", policy =>
        policy.RequireRole("PetOwner"));
    
    options.AddPolicy("ServiceProviderOnly", policy =>
        policy.RequireRole("ServiceProvider"));
    
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("PremiumUser", policy =>
        policy.RequireClaim("subscription", "Premium"));
});
```

### Custom Authorization Attributes
```csharp
public class ResourceOwnerAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Custom authorization logic
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var resourceUserId = context.RouteData.Values["userId"]?.ToString();

        if (userId != resourceUserId && !user.IsInRole("Admin"))
        {
            context.Result = new ForbidResult();
        }
    }
}
```

## Validation Patterns

### FluentValidation
```csharp
public class CreateDogProfileRequestValidator : AbstractValidator<CreateDogProfileRequest>
{
    public CreateDogProfileRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Dog name is required")
            .MaximumLength(100).WithMessage("Dog name cannot exceed 100 characters");

        RuleFor(x => x.Breed)
            .NotEmpty().WithMessage("Breed is required");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-30)).WithMessage("Date of birth is too far in the past");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0")
            .LessThan(200).WithMessage("Weight seems unrealistic");
    }
}
```

## Caching Patterns

### Distributed Caching
```csharp
public class CachedDogProfileService : IDogProfileService
{
    private readonly IDogProfileService _dogProfileService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedDogProfileService> _logger;

    public async Task<DogProfileDto?> GetDogProfileAsync(Guid id)
    {
        var cacheKey = $"dogprofile:{id}";
        var cachedProfile = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProfile))
        {
            _logger.LogInformation("Retrieved dog profile {Id} from cache", id);
            return JsonSerializer.Deserialize<DogProfileDto>(cachedProfile);
        }

        var profile = await _dogProfileService.GetDogProfileAsync(id);
        if (profile != null)
        {
            var serializedProfile = JsonSerializer.Serialize(profile);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, serializedProfile, options);
            _logger.LogInformation("Cached dog profile {Id}", id);
        }

        return profile;
    }
}
```

## Background Job Patterns

### Hosted Service
```csharp
public class EmailNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailNotificationService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await ProcessPendingEmails(emailService);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email notifications");
            }
        }
    }
}
```

## SignalR Patterns

### Hub Implementation
```csharp
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", Context.User.Identity.Name);
    }

    public async Task SendMessage(string groupName, string message)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var chatMessage = await _chatService.SaveMessageAsync(userId, groupName, message);
        
        await Clients.Group(groupName).SendAsync("ReceiveMessage", new
        {
            User = Context.User.Identity.Name,
            Message = message,
            Timestamp = chatMessage.CreatedAt
        });
    }
}
```

## File Upload Patterns

### File Upload Controller
```csharp
[HttpPost("upload")]
public async Task<ActionResult<FileUploadResult>> UploadFile(
    IFormFile file, [FromQuery] string category = "general")
{
    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded");

    // Validate file type and size
    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
    if (!allowedTypes.Contains(file.ContentType))
        return BadRequest("Invalid file type");

    if (file.Length > 5 * 1024 * 1024) // 5MB
        return BadRequest("File too large");

    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var result = await _blobStorageService.UploadFileAsync(file, category, userId);

    return Ok(result);
}
```

## Health Check Patterns

### Custom Health Check
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not accessible", ex);
        }
    }
}
```