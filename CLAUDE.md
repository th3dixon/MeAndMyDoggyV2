# Claude Development Knowledge Base

## API Configuration

### Backend API Port
- **Production API Port**: `63343`
- **API Base URL**: `https://localhost:63343/`
- **Configuration Location**: `Program.cs` - HttpClient "API" configuration
- **Fallback Configuration**: If `ApiSettings:BaseUrl` not set in appsettings, defaults to `https://localhost:63343/`

### API Endpoints Used
- **Service Categories (Public)**: `api/v1/ServiceCatalog/public/categories`
- **Provider Search**: `api/v1/ProviderSearch/*` 
- **Location Services**: `api/v1/Location/*`

## Frontend Architecture

### Home Page Search Flow
1. **Service Selection**: Visual icons for service types (no text search)
2. **Postcode Entry**: Manual input or "Detect" button with geolocation
3. **Search Execution**: Redirects to `/Search` page with URL parameters
4. **Fallback Services**: Default service list loads immediately, API services load in background

### Search Page Integration
- Loads URL parameters from home page redirect
- Uses same SearchController endpoints
- Alpine.js + Google Maps integration
- Mobile-responsive with bottom navigation

### Error Handling Patterns
- API failures gracefully fallback to default data
- Location detection has multiple fallback strategies
- User feedback via modal dialogs
- Console logging for development debugging

## Key Technologies
- **Backend**: ASP.NET Core MVC
- **Frontend**: Alpine.js, Tailwind CSS, Google Maps API
- **Authentication**: ASP.NET Core Identity + JWT
- **Database**: Entity Framework Core with SQL Server

## Development Standards
- **ALWAYS CHECK ENTITIES FIRST**: Before using any entity properties in code, read the actual entity file to verify what properties exist. This prevents hallucinations about entity structure and ensures accurate code implementation
- **ALPINE.JS EVENT BINDING**: In Razor views (.cshtml), use `@@click` instead of `@click` for Alpine.js event handlers. The `@` symbol is reserved for Razor syntax, so Alpine.js directives must be escaped with `@@` to prevent conflicts
- Always implement fallback data for offline development
- Use public API endpoints for unauthenticated pages
- Provide clear user feedback for all interactions
- Follow mobile-first responsive design
- Implement proper error boundaries and graceful degradation

## Code Quality & Validation

### Automated Code Validation Hooks
The project includes automated code validation hooks to maintain coding standards:

#### Available Hooks
- **`/hooks/code-validation-hook.py`**: Main validation script that checks:
  - Single class per file enforcement
  - XML documentation requirements
  - No hardcoded secrets or test data
  - No console.log statements in production code
  - No incomplete implementations (TODO/FIXME/NotImplementedException)
  - No inline CSS or JavaScript in .cshtml files
  - No inline style attributes or event handlers

#### Running Code Validation
- **Manual Execution**: 
  - Windows: `hooks\run-validation.bat`
  - PowerShell: `hooks\run-validation.ps1`
  - Direct: `python hooks\code-validation-hook.py`

#### Validation Reports
- Generated as `CODE_VALIDATION_REPORT.md` in project root
- Includes compliance score (0-100)
- Categorizes violations by severity (Error/Warning/Info)
- Provides specific suggestions for fixes

#### Integration with Development Workflow
- **Recommended**: Run validation after completing high-priority tasks
- **Pre-commit**: Consider running before major commits
- **CI/CD**: Can be integrated into build pipelines
- **Quality Gates**: Fails with exit code 1 if critical errors found

#### Coding Standards Enforced
1. **Architecture**: One public class per file
2. **Documentation**: XML comments for public classes
3. **Security**: No hardcoded secrets or credentials
4. **Production Readiness**: No debug statements or incomplete code
5. **Code Quality**: Proper error handling and implementation completion
6. **Separation of Concerns**: 
   - No inline CSS in .cshtml files (use separate .css files in wwwroot/css/)
   - No inline JavaScript in .cshtml files (use separate .js files in wwwroot/js/)
   - No inline style attributes (use CSS classes instead)
   - No inline event handlers (use addEventListener or framework event bindings)

### Quality Metrics
- **Target Compliance Score**: 90%+
- **Zero Tolerance**: Hardcoded secrets, incomplete implementations
- **Acceptable**: Minor documentation warnings

Last Updated: 2025-01-23