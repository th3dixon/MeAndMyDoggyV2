using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeAndMyDog.WebApp.Hubs;
using MeAndMyDog.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Use PascalCase for JSON serialization (matches C# DTOs)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });

// Add HTTP context accessor for API services
builder.Services.AddHttpContextAccessor();

// Add HttpClient for API calls
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:63343/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Authentication (Cookie-based for Web app)
builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.ReturnUrlParameter = "returnUrl";
});

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SignalR
builder.Services.AddSignalR();

// Add API-based services (no direct database access)
builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
builder.Services.AddScoped<IRoleNavigationService, ApiRoleNavigationService>();

// Add SignalR notification services
builder.Services.AddScoped<MeAndMyDog.WebApp.Hubs.IProviderDashboardNotificationService, MeAndMyDog.WebApp.Hubs.ProviderDashboardNotificationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add request logging middleware for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("=== Incoming Request ===");
    logger.LogInformation("Path: {Path}", context.Request.Path);
    logger.LogInformation("Method: {Method}", context.Request.Method);
    logger.LogInformation("QueryString: {QueryString}", context.Request.QueryString);
    logger.LogInformation("IsAuthenticated: {IsAuthenticated}", context.User.Identity?.IsAuthenticated ?? false);
    
    if (context.Request.Path.StartsWithSegments("/Profile", StringComparison.OrdinalIgnoreCase))
    {
        logger.LogWarning("!!! Profile request detected !!!");
        logger.LogWarning("Full Path: {FullPath}", context.Request.Path.Value);
    }
    
    await next();
    
    logger.LogInformation("Response Status: {StatusCode}", context.Response.StatusCode);
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Provider dashboard routes
app.MapControllerRoute(
    name: "provider",
    pattern: "provider/{action=Dashboard}",
    defaults: new { controller = "ProviderDashboard" });

// Role switcher routes
app.MapControllerRoute(
    name: "roleswitcher",
    pattern: "switch-role/{action=Dashboard}",
    defaults: new { controller = "RoleSwitcher" });

// API routes
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}");

// Default MVC routes (fallback for server-side rendering)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SignalR Hubs
app.MapHub<ProviderDashboardHub>("/hubs/provider-dashboard");

// MVC routes

app.Run();
