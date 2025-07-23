using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Diagnostics controller for debugging routing issues
/// </summary>
[AllowAnonymous]
public class DiagnosticsController : Controller
{
    private readonly ILogger<DiagnosticsController> _logger;
    private readonly IEnumerable<EndpointDataSource> _endpointSources;

    public DiagnosticsController(ILogger<DiagnosticsController> logger, IEnumerable<EndpointDataSource> endpointSources)
    {
        _logger = logger;
        _endpointSources = endpointSources;
    }

    /// <summary>
    /// Show all registered routes
    /// </summary>
    /// <returns>View with route information</returns>
    [HttpGet]
    [Route("Diagnostics/Routes")]
    public IActionResult Routes()
    {
        var routes = new List<object>();
        
        foreach (var endpointSource in _endpointSources)
        {
            foreach (var endpoint in endpointSource.Endpoints)
            {
                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    var controllerActionDescriptor = endpoint.Metadata
                        .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                    
                    if (controllerActionDescriptor != null)
                    {
                        routes.Add(new
                        {
                            Pattern = routeEndpoint.RoutePattern.RawText,
                            Controller = controllerActionDescriptor.ControllerName,
                            Action = controllerActionDescriptor.ActionName,
                            HttpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? new[] { "GET" },
                            Order = routeEndpoint.Order,
                            DisplayName = endpoint.DisplayName
                        });
                    }
                }
            }
        }
        
        // Sort routes by pattern for easier reading
        routes = routes.OrderBy(r => ((dynamic)r).Pattern).ToList();
        
        return Json(new 
        { 
            totalRoutes = routes.Count,
            routes = routes,
            profileRoutes = routes.Where(r => ((dynamic)r).Controller == "Profile").ToList()
        });
    }

    /// <summary>
    /// Show all registered controllers
    /// </summary>
    /// <returns>JSON with controller information</returns>
    [HttpGet]
    [Route("Diagnostics/Controllers")]
    public IActionResult Controllers()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var controllerTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Controller)) && !t.IsAbstract)
            .OrderBy(t => t.Name)
            .Select(t => new 
            {
                Name = t.Name,
                Namespace = t.Namespace,
                Actions = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.IsPublic && !m.IsSpecialName && m.DeclaringType == t)
                    .Select(m => new 
                    {
                        Name = m.Name,
                        ReturnType = m.ReturnType.Name,
                        Parameters = m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}").ToList(),
                        Attributes = m.GetCustomAttributes().Select(a => a.GetType().Name).ToList()
                    })
                    .ToList(),
                Attributes = t.GetCustomAttributes().Select(a => a.GetType().Name).ToList()
            })
            .ToList();

        return Json(new 
        { 
            totalControllers = controllerTypes.Count,
            controllers = controllerTypes,
            profileController = controllerTypes.FirstOrDefault(c => c.Name == "ProfileController")
        });
    }

    /// <summary>
    /// Test profile route resolution
    /// </summary>
    /// <returns>Redirect test results</returns>
    [HttpGet]
    [Route("Diagnostics/TestProfile")]
    public IActionResult TestProfile()
    {
        var results = new List<object>();
        
        // Test various profile URLs
        var testUrls = new[] { "/Profile", "/Profile/Index", "/profile", "/profile/index" };
        
        foreach (var url in testUrls)
        {
            var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Routing.IRoutingFeature>();
            results.Add(new 
            {
                Url = url,
                Resolved = feature?.RouteData?.Values?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>()
            });
        }
        
        return Json(new 
        { 
            currentPath = Request.Path,
            testResults = results,
            routeData = new 
            {
                values = HttpContext.GetRouteData().Values,
                dataTokens = HttpContext.GetRouteData().DataTokens
            }
        });
    }
}