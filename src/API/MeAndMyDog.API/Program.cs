using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services;
using MeAndMyDog.API.Services.Implementations;
using MeAndMyDog.API.Services.Interfaces;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Serilog;
using System.Text;

// Configure Serilog with structured logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/meandmydog-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MeAndMyDoggyV2.API")
    .CreateLogger();

try
{
    Log.Information("Starting MeAndMyDoggyV2 API application...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    #region Azure Key Vault Configuration
    
    // Add Azure Key Vault configuration if not in development
    if (!builder.Environment.IsDevelopment())
    {
        var keyVaultEndpoint = builder.Configuration["KeyVault:VaultUri"];
        if (!string.IsNullOrEmpty(keyVaultEndpoint))
        {
            Log.Information("Configuring Azure Key Vault integration: {KeyVaultUri}", keyVaultEndpoint);
            
            // Use Managed Identity in production
            var credential = new DefaultAzureCredential();
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), credential);
            
            Log.Information("Azure Key Vault configuration completed successfully");
        }
        else
        {
            Log.Warning("KeyVault:VaultUri not found in configuration. Falling back to local configuration.");
        }
    }
    else
    {
        Log.Information("Development environment detected. Using local configuration.");
    }
    
    #endregion

    #region Service Registration
    
    // Add framework services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    // Configure Swagger with comprehensive documentation
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() 
        { 
            Title = "MeAndMyDoggyV2 API", 
            Version = "v1",
            Description = "Comprehensive pet services platform API supporting real-time messaging, AI health recommendations, and service provider management",
            Contact = new() { Name = "Architecture Team", Email = "architecture@meandmydoggyv2.com" }
        });
        
        c.AddSecurityDefinition("Bearer", new()
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        
        c.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    #endregion

    #region Database Configuration
    
    // Database configuration with enhanced connection resilience
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Log.Fatal("Database connection string 'DefaultConnection' is not configured");
        throw new InvalidOperationException("Database connection string not found");
    }
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        });
        
        // Enable sensitive data logging in development only
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    #endregion

    #region Identity & Authentication Configuration
    
    // Enhanced Identity configuration
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Password requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        
        // User requirements
        options.User.RequireUniqueEmail = true;
        
        // Sign-in requirements
        options.SignIn.RequireConfirmedEmail = true;
        
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // JWT Authentication with Azure Key Vault integration
    // Try Azure Key Vault format first, then fall back to standard format
    var jwtSecretKey = builder.Configuration["JWT--SecretKey"] ?? builder.Configuration["Jwt:SecretKey"];
    var jwtRefreshKey = builder.Configuration["JWT--RefreshKey"] ?? builder.Configuration["Jwt:RefreshKey"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://api.meandmydoggy.com";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "https://meandmydoggy.com";
    
    if (string.IsNullOrEmpty(jwtSecretKey))
    {
        Log.Fatal("JWT SecretKey is not configured. Check Azure Key Vault or local configuration.");
        throw new InvalidOperationException("JWT Secret Key not configured");
    }
    
    if (string.IsNullOrEmpty(jwtRefreshKey))
    {
        Log.Fatal("JWT RefreshKey is not configured. Check Azure Key Vault or local configuration.");
        throw new InvalidOperationException("JWT Refresh Key not configured");
    }
    
    Log.Information("JWT authentication configured with issuer: {Issuer} and audience: {Audience}", jwtIssuer, jwtAudience);
    
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew
            
            // Enhanced token validation
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
        
        // Configure events for better logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("JWT authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("JWT token validated successfully for user: {UserId}", 
                    context.Principal?.FindFirst("sub")?.Value);
                return Task.CompletedTask;
            }
        };
    });

    // Authorization policies for RBAC
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireServiceProviderRole", policy =>
            policy.RequireRole("ServiceProvider"));
        options.AddPolicy("RequireAdminRole", policy =>
            policy.RequireRole("Admin"));
        options.AddPolicy("RequireKYCReviewPermission", policy =>
            policy.RequireClaim("permission", "kyc.review"));
        options.AddPolicy("RequireAIAccess", policy =>
            policy.RequireClaim("permission", "ai.access"));
    });

    #endregion

    #region Business Services Registration
    
    // Register all business services
    builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
    builder.Services.AddScoped<IAddressLookupService, AddressLookupService>();
    builder.Services.AddScoped<IProviderSearchService, ProviderSearchService>();
    builder.Services.AddScoped<ILocationService, LocationService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<FriendCodeService>();
    builder.Services.AddScoped<FriendshipValidationService>();
    
    // HTTP Client for external API calls
    builder.Services.AddHttpClient();
    
    // Messaging services (Phase 1 - Core Messaging Foundation)
    builder.Services.AddScoped<IMessagingService, MessagingService>();
    builder.Services.AddScoped<IConversationService, ConversationService>();
    builder.Services.AddScoped<IFileUploadService, FileUploadService>();
    builder.Services.AddScoped<IVideoCallService, VideoCallService>();
    builder.Services.AddScoped<IVoiceMessageService, VoiceMessageService>();
    builder.Services.AddScoped<IEncryptionService, EncryptionService>();
    builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
    builder.Services.AddScoped<IMessagingNotificationService, MessagingNotificationService>();
    builder.Services.AddScoped<IMessageTemplateService, MessageTemplateService>();
    builder.Services.AddScoped<IScheduledMessageService, ScheduledMessageService>();
    builder.Services.AddScoped<IMessageSearchService, MessageSearchService>();
    builder.Services.AddScoped<ILocationSharingService, LocationSharingService>();
    builder.Services.AddScoped<ITranslationService, TranslationService>();
    builder.Services.AddScoped<ICalendarService, CalendarService>();
    
    // Dashboard performance and analytics services
    builder.Services.AddScoped<IDashboardCacheService, DashboardCacheService>();
    builder.Services.AddScoped<IDashboardAnalyticsService, DashboardAnalyticsService>();
    builder.Services.AddScoped<IMobileIntegrationService, MobileIntegrationService>();

    #endregion

    #region CORS Configuration
    
    // CORS configuration for multiple environments
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowWebApp", policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? new[] 
                {
                    "https://localhost:63346",
                    "http://localhost:63346", 
                    "https://localhost:56682",
                    "http://localhost:56682", 
                    "https://meandmydoggy.co.uk",
                    "https://www.meandmydoggy.co.uk"
                };
                
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    #endregion

    #region SignalR Configuration
    
    // SignalR for real-time communication
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    });
    
    // Add Redis backplane if connection string is available
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "MeAndMyDoggyV2";
        });
        
        Log.Information("Redis caching configured successfully");
    }
    else
    {
        Log.Information("Redis connection string not found. Using in-memory caching.");
        builder.Services.AddMemoryCache();              // For IMemoryCache
        builder.Services.AddDistributedMemoryCache();   // For IDistributedCache
    }

    #endregion

    #region Health Checks
    
    builder.Services.AddHealthChecks()
        .AddSqlServer(connectionString, name: "database")
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

    #endregion

    var app = builder.Build();

    #region Middleware Pipeline Configuration
    
    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeAndMyDoggyV2 API v1");
            c.RoutePrefix = string.Empty; // Serve Swagger UI at the root
        });
    }
    else
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        };
    });
    
    app.UseCors("AllowWebApp");
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();
    app.MapHealthChecks("/health");
    
    // Map SignalR hubs
    app.MapHub<MeAndMyDog.API.Hubs.MessagingHub>("/hubs/messaging");

    #endregion

    #region Database Migration and Seeding
    
    // Ensure database is created and migrations are applied
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            Log.Information("Checking database connection and applying migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }

    #endregion

    Log.Information("MeAndMyDoggyV2 API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}

