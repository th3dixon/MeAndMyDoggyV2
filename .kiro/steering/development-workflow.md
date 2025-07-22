---
inclusion: always
---

# MeAndMyDog Development Workflow

## Development Environment Setup

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019+ (or SQL Server Express)
- Node.js 18+ (for frontend tooling)
- Visual Studio 2022 or VS Code
- Redis (optional for caching)

### Initial Setup Commands
```bash
# Clone and restore packages
dotnet restore

# Setup user secrets for API
cd src/API/MeAndMyDog.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[connection-string]"

# Setup user secrets for WebApp
cd src/Web/MeAndMyDog.WebApp
dotnet user-secrets init

# Run database migrations
cd src/API/MeAndMyDog.API
dotnet ef database update
```

### Running the Application
```bash
# Terminal 1 - API (Port 63343)
dotnet run --project src/API/MeAndMyDog.API

# Terminal 2 - WebApp (Port 56682)
dotnet run --project src/Web/MeAndMyDog.WebApp
```

## Git Workflow

### Branch Naming Convention
- **Feature branches**: `feature/feature-name`
- **Bug fixes**: `bugfix/issue-description`
- **Hotfixes**: `hotfix/critical-issue`
- **Release branches**: `release/version-number`

### Commit Message Format
```
type(scope): description

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(auth): add two-factor authentication
fix(booking): resolve date validation issue
docs(api): update swagger documentation
```

### Pull Request Process
1. Create feature branch from `main`
2. Implement changes following coding standards
3. Write/update tests
4. Update documentation if needed
5. Create pull request with descriptive title
6. Request code review
7. Address review feedback
8. Merge after approval

## Code Review Guidelines

### What to Review
- **Functionality**: Does the code work as intended?
- **Standards**: Follows coding standards and conventions?
- **Security**: No security vulnerabilities introduced?
- **Performance**: Efficient implementation?
- **Tests**: Adequate test coverage?
- **Documentation**: Clear and up-to-date?

### Review Checklist
- [ ] Code follows naming conventions
- [ ] Proper error handling implemented
- [ ] Security best practices followed
- [ ] Performance considerations addressed
- [ ] Tests written and passing
- [ ] Documentation updated
- [ ] No hardcoded secrets or credentials

## Testing Strategy

### Unit Testing
- Test all business logic
- Mock external dependencies
- Use meaningful test names
- Follow AAA pattern (Arrange, Act, Assert)
- Aim for 80%+ code coverage

### Integration Testing
- Test API endpoints
- Test database interactions
- Test external service integrations
- Use test databases
- Clean up test data after tests

### E2E Testing
- Test critical user journeys
- Use Playwright for browser automation
- Test across different browsers
- Include mobile testing
- Run in CI/CD pipeline

## Database Management

### Entity Framework Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/API/MeAndMyDog.API

# Update database
dotnet ef database update --project src/API/MeAndMyDog.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/API/MeAndMyDog.API
```

### Migration Best Practices
- Use descriptive migration names
- Review generated migration scripts
- Test migrations on development database
- Include rollback scripts for production
- Never modify existing migrations

### Database Seeding
- Use data seeding for reference data
- Create test users for development
- Include sample data for testing
- Use consistent seed data across environments

## Configuration Management

### User Secrets (Development)
```bash
# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=MeAndMyDog;..."

# Set JWT secret
dotnet user-secrets set "Jwt:SecretKey" "your-secret-key-here"

# Set external API keys
dotnet user-secrets set "ExternalServices:GoogleMaps:ApiKey" "your-api-key"
```

### Azure Key Vault (Production)
- Store all secrets in Azure Key Vault
- Use managed identity for authentication
- Follow naming convention: `MeAndMyDog--Section--Key`
- Never commit secrets to source control

### Environment Variables
- Use for environment-specific settings
- Prefix with `MEANDMYDOG_` for clarity
- Document all required environment variables
- Provide default values where appropriate

## Debugging Guidelines

### Logging Standards
- Use structured logging with Serilog
- Include correlation IDs for request tracing
- Log at appropriate levels (Debug, Info, Warning, Error)
- Include relevant context in log messages
- Never log sensitive information

### Error Handling
- Use try-catch blocks appropriately
- Log errors with full context
- Return meaningful error messages to users
- Use custom exceptions for business logic
- Implement global exception handling

### Performance Monitoring
- Use Application Insights for monitoring
- Monitor API response times
- Track database query performance
- Monitor memory usage and garbage collection
- Set up alerts for critical metrics

## Deployment Process

### Build Process
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build --configuration Release

# Run tests
dotnet test

# Publish API
dotnet publish src/API/MeAndMyDog.API --configuration Release --output ./publish/api

# Publish WebApp
dotnet publish src/Web/MeAndMyDog.WebApp --configuration Release --output ./publish/webapp
```

### Pre-deployment Checklist
- [ ] All tests passing
- [ ] Code reviewed and approved
- [ ] Database migrations ready
- [ ] Configuration updated
- [ ] Secrets configured in Key Vault
- [ ] Performance testing completed
- [ ] Security scan passed

### Deployment Steps
1. Deploy database migrations
2. Deploy API application
3. Deploy WebApp application
4. Update configuration
5. Verify deployment
6. Monitor for issues

## Feature Flags

### Current Feature Toggles
```json
{
  "AIHealthRecommendations": 100,
  "RealTimeMessaging": 100,
  "PremiumFeatures": 100,
  "AdvancedSearch": 100,
  "CommunityForums": 100,
  "VideoChat": 50,
  "PushNotifications": 75,
  "DarkMode": 100,
  "NewPaymentFlow": 10,
  "EnhancedSecurity": 25,
  "MaintenanceMode": 0
}
```

### Feature Flag Usage
- Use for gradual rollouts
- Enable A/B testing
- Quick rollback capability
- Environment-specific features
- Maintenance mode activation

## Monitoring and Alerting

### Key Metrics to Monitor
- API response times
- Error rates
- Database performance
- Memory usage
- User activity
- Payment processing

### Alert Thresholds
- API response time > 2 seconds
- Error rate > 5%
- Database connection failures
- Memory usage > 80%
- Disk space < 20%

### Health Checks
- Database connectivity
- External service availability
- Redis connection
- Storage access
- Authentication service

## Documentation Standards

### Code Documentation
- Use XML documentation comments for public APIs
- Document complex business logic
- Include examples for public methods
- Keep documentation up-to-date
- Use clear, concise language

### API Documentation
- Use Swagger/OpenAPI for API documentation
- Include request/response examples
- Document error responses
- Provide authentication details
- Keep documentation current with code changes

### Architecture Documentation
- Document system architecture
- Include deployment diagrams
- Document integration points
- Maintain decision records
- Update with significant changes