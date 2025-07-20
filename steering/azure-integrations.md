---
inclusion: always
---

# MeAndMyDog Azure & External Integrations

## Azure Configuration

### Azure Key Vault Setup
**Key Vault Name**: `meandmydoggyvault`
**URL**: `https://meandmydoggyvault.vault.azure.net/`

### Key Vault Secrets Structure
All secrets follow the pattern: `MeAndMyDog--Section--Key`

**Connection Strings**
- `MeAndMyDog--ConnectionStrings--DefaultConnection`
- `MeAndMyDog--ConnectionStrings--Redis`

**Authentication**
- `MeAndMyDog--Jwt--SecretKey`
- `MeAndMyDog--Jwt--Issuer`
- `MeAndMyDog--Jwt--Audience`

**Payment Providers**
- `MeAndMyDog--Payment--Santander--ApiKey`
- `MeAndMyDog--Payment--Stripe--SecretKey`
- `MeAndMyDog--Payment--Stripe--WebhookSecret`
- `MeAndMyDog--Payment--PayPal--ClientId`
- `MeAndMyDog--Payment--PayPal--ClientSecret`

**External Services**
- `MeAndMyDog--ExternalServices--GoogleMaps--ApiKey`
- `MeAndMyDog--ExternalServices--Gemini--ApiKey`
- `MeAndMyDog--ExternalServices--SendGrid--ApiKey`

### Key Vault Configuration Code
```csharp
// Program.cs - Key Vault Integration
var keyVaultName = "meandmydoggyvault";
var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";

builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential(),
    new PrefixKeyVaultSecretManager("MeAndMyDog"));
```

## Database Configuration

### Development Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:senseilive.uksouth.cloudapp.azure.com,1433;Database=MeAndMyDog;User Id=sa;Password=***;MultipleActiveResultSets=True;TrustServerCertificate=True;Connection Timeout=30;"
  }
}
```

### Production Database
- Connection string retrieved from Azure Key Vault
- Fallback to environment variable: `MEANDMYDOG_ConnectionStrings__DefaultConnection`
- Use Azure SQL Database for production
- Enable automatic backups and point-in-time restore

### Database Security
- Use Azure AD authentication where possible
- Enable Transparent Data Encryption (TDE)
- Configure firewall rules appropriately
- Monitor with Azure SQL Analytics
- Enable threat detection

## Azure Services Integration

### Azure Blob Storage
**Purpose**: Media file storage (dog photos, videos, documents)

**Configuration**:
```csharp
services.AddScoped<IBlobStorageService, BlobStorageService>();
services.Configure<BlobStorageOptions>(options =>
{
    options.ConnectionString = configuration.GetConnectionString("AzureStorage");
    options.ContainerName = "meandmydog-media";
});
```

**Container Structure**:
- `dog-profiles/` - Dog profile images
- `medical-records/` - Medical documents
- `service-providers/` - Provider photos and documents
- `user-uploads/` - General user uploads

### Application Insights
**Purpose**: Performance monitoring and error tracking

**Configuration**:
```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = configuration.GetConnectionString("ApplicationInsights");
});
```

**Custom Metrics**:
- User registration rates
- Booking completion rates
- Payment success rates
- API response times
- Error rates by endpoint

### Azure Service Bus (Future)
**Purpose**: Asynchronous message processing

**Use Cases**:
- Email notifications
- Push notifications
- Background job processing
- Event-driven architecture

## Payment Provider Integrations

### Santander Payment Integration
**Environment**: Sandbox/Production
**Authentication**: API Key stored in Key Vault

**Configuration**:
```csharp
services.Configure<SantanderPaymentOptions>(options =>
{
    options.ApiKey = configuration["Payment:Santander:ApiKey"];
    options.BaseUrl = configuration["Payment:Santander:BaseUrl"];
    options.WebhookUrl = configuration["Payment:Santander:WebhookUrl"];
});
```

**Webhook Endpoint**: `/api/v1/webhooks/santander`

### Stripe Integration
**Environment**: Test/Live mode based on configuration

**Configuration**:
```csharp
services.Configure<StripeOptions>(options =>
{
    options.SecretKey = configuration["Payment:Stripe:SecretKey"];
    options.WebhookSecret = configuration["Payment:Stripe:WebhookSecret"];
});
```

**Features**:
- Payment processing
- Subscription management
- Stored payment methods
- Webhook handling

**Webhook Endpoint**: `/api/v1/webhooks/stripe`

### PayPal Integration
**Environment**: Sandbox/Live

**Configuration**:
```csharp
services.Configure<PayPalOptions>(options =>
{
    options.ClientId = configuration["Payment:PayPal:ClientId"];
    options.ClientSecret = configuration["Payment:PayPal:ClientSecret"];
    options.Environment = configuration["Payment:PayPal:Environment"];
});
```

## AI & External Service Integrations

### Google Gemini AI
**Model**: gemini-1.5-flash
**Purpose**: Health recommendations, content generation

**Configuration**:
```csharp
services.Configure<GeminiOptions>(options =>
{
    options.ApiKey = configuration["ExternalServices:Gemini:ApiKey"];
    options.Model = "gemini-1.5-flash";
});
```

**Use Cases**:
- Dog health recommendations
- Behavioral analysis
- Content moderation
- Image analysis

### Google Maps API
**Purpose**: Location services, geocoding

**Configuration**:
```csharp
services.Configure<GoogleMapsOptions>(options =>
{
    options.ApiKey = configuration["ExternalServices:GoogleMaps:ApiKey"];
});
```

**Features**:
- Address validation
- Distance calculations
- Map display
- Location search

### SendGrid Email Service
**Purpose**: Transactional emails

**Configuration**:
```csharp
services.Configure<SendGridOptions>(options =>
{
    options.ApiKey = configuration["ExternalServices:SendGrid:ApiKey"];
    options.FromEmail = configuration["ExternalServices:SendGrid:FromEmail"];
    options.FromName = configuration["ExternalServices:SendGrid:FromName"];
});
```

**Email Templates**:
- Welcome emails
- Booking confirmations
- Password reset
- Payment receipts
- Notifications

## Third-Party Service Integrations

### Kennel Club APIs
**Purpose**: Breed information and pedigree data

**Supported APIs**:
- AKC (American Kennel Club)
- KC (The Kennel Club UK)
- CKC (Canadian Kennel Club)
- FCI (International)

### Awin Affiliate Marketing
**Purpose**: Pet product recommendations

**Configuration**:
```csharp
services.Configure<AwinOptions>(options =>
{
    options.PublisherId = configuration["ExternalServices:Awin:PublisherId"];
    options.ApiKey = configuration["ExternalServices:Awin:ApiKey"];
});
```

### Didit KYC Verification
**Purpose**: Service provider verification

**Configuration**:
```csharp
services.Configure<DiditOptions>(options =>
{
    options.ApiKey = configuration["ExternalServices:Didit:ApiKey"];
    options.BaseUrl = configuration["ExternalServices:Didit:BaseUrl"];
});
```

## Security & Compliance

### Data Protection
- GDPR compliance for UK/EU users
- Data encryption at rest and in transit
- Regular security audits
- Secure API endpoints
- Input validation and sanitization

### Authentication Security
- JWT token expiration (15 minutes)
- Refresh token rotation
- Account lockout policies
- IP-based blocking
- Two-factor authentication

### API Security
- Rate limiting per endpoint
- CORS configuration
- API versioning
- Request/response logging
- Error handling without information disclosure

## Monitoring & Alerting

### Azure Monitor Integration
**Metrics to Track**:
- Application performance
- Database performance
- Storage usage
- Payment processing
- User activity

**Alert Rules**:
- High error rates
- Slow response times
- Database connection issues
- Payment failures
- Storage quota limits

### Health Checks
```csharp
services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddAzureBlobStorage(storageConnectionString)
    .AddApplicationInsightsPublisher()
    .AddCheck<PaymentServiceHealthCheck>("payment-service")
    .AddCheck<ExternalApiHealthCheck>("external-apis");
```

## Environment Configuration

### Development Environment
- Use user secrets for sensitive data
- Local SQL Server or Azure SQL
- Sandbox/test modes for external services
- Detailed logging enabled
- Feature flags for testing

### Staging Environment
- Mirror production configuration
- Use staging databases
- Test external service integrations
- Performance testing
- Security scanning

### Production Environment
- Azure Key Vault for all secrets
- Production databases with backups
- Live external service integrations
- Optimized logging levels
- Monitoring and alerting enabled

## Deployment Considerations

### Azure App Service Configuration
- Use deployment slots for zero-downtime deployments
- Configure auto-scaling rules
- Enable Application Insights
- Set up custom domains and SSL certificates
- Configure backup and restore policies

### Database Deployment
- Use Entity Framework migrations
- Test migrations in staging first
- Plan for rollback scenarios
- Monitor migration performance
- Backup before major changes

### Configuration Management
- Use Azure App Configuration for feature flags
- Environment-specific appsettings files
- Secure connection string management
- Configuration validation on startup
- Hot-reload capabilities where appropriate