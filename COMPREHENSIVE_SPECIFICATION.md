# MeAndMyDoggy - Comprehensive Platform Specification

## Executive Summary

MeAndMyDoggy is a comprehensive pet services platform built with ASP.NET Core 9.0, designed to connect pet owners with service providers while offering advanced features like AI-powered health recommendations, real-time messaging, and integrated payment processing. The platform follows modern architectural patterns with a focus on scalability, security, and user experience.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Technology Stack](#technology-stack)
3. [Azure Configuration & Infrastructure](#azure-configuration--infrastructure)
4. [Authentication & Authorization](#authentication--authorization)
5. [Core Features](#core-features)
6. [Database Design](#database-design)
7. [External Integrations](#external-integrations)
8. [Payment System](#payment-system)
9. [UI/UX Architecture](#uiux-architecture)
10. [API Design](#api-design)
11. [Security Considerations](#security-considerations)
12. [Development Setup](#development-setup)

## Architecture Overview

### Solution Structure

```
MeAndMyDog.sln
├── src/
│   ├── API/MeAndMyDog.API/          # Consolidated API (Port 7010)
│   │   ├── Controllers/              # 90+ feature-based controllers
│   │   ├── Services/                 # Business logic layer
│   │   ├── Data/                     # EF Core DbContext
│   │   ├── Entities/                 # Domain entities
│   │   ├── DTOs/                     # Data transfer objects
│   │   ├── Middleware/               # Custom middleware
│   │   ├── Hubs/                     # SignalR real-time
│   │   └── Migrations/               # Database migrations
│   │
│   ├── Web/MeAndMyDog.WebApp/       # MVC Frontend (Port 56682)
│   │   ├── Controllers/              # MVC controllers
│   │   ├── Views/                    # Razor views
│   │   ├── wwwroot/                  # Static assets
│   │   └── Components/               # Vue.js components
│   │
│   └── BuildingBlocks/               # Shared components
│       ├── MeAndMyDog.SharedKernel/  # Domain primitives
│       └── MeAndMyDog.BlobStorage/   # Storage abstraction
│
└── tests/                            # Test projects
    └── E2E/                          # Playwright E2E tests
```

### Architectural Patterns

1. **Clean Architecture** - Clear separation of concerns
2. **Repository Pattern** - Data access abstraction
3. **Service Layer Pattern** - Business logic encapsulation
4. **CQRS-like Organization** - Feature-based controller structure
5. **Microservices-Ready** - API versioning and feature isolation
6. **Real-time Communication** - SignalR integration
7. **Configuration Management** - Environment-specific settings

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server (Azure VM)
- **Caching**: Redis (StackExchange.Redis)
- **Real-time**: SignalR
- **Authentication**: ASP.NET Core Identity + JWT
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: MVC with Razor Pages
- **CSS**: Tailwind CSS 3.4.16
- **JavaScript**: Alpine.js 3.14.1, Vue.js 3.5.13
- **UI Components**: Mobiscroll, GrapesJS
- **Icons**: Font Awesome
- **Build Tool**: Vite

### Infrastructure
- **Cloud**: Microsoft Azure
- **Storage**: Azure Blob Storage
- **Secrets**: Azure Key Vault
- **Monitoring**: Application Insights
- **Logging**: Serilog

## Azure Configuration & Infrastructure

### Azure Key Vault Integration

```csharp
// Key Vault Configuration
var keyVaultName = "meandmydoggyvault";
var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";

builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential(),
    new PrefixKeyVaultSecretManager("MeAndMyDog"));
```

### Key Vault Secrets Structure
- **Connection Strings**: `MeAndMyDog--ConnectionStrings--DefaultConnection`
- **API Keys**: `MeAndMyDog--ApiKeys--{ServiceName}`
- **Payment Secrets**: `MeAndMyDog--Payment--{Provider}--{Key}`
- **External Services**: `MeAndMyDog--ExternalServices--{Service}--{Key}`

### Database Connection

#### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:senseilive.uksouth.cloudapp.azure.com,1433;Database=MeAndMyDog;User Id=sa;Password=***;MultipleActiveResultSets=True;TrustServerCertificate=True;Connection Timeout=30;"
  }
}
```

#### Production
- Connection string stored in Azure Key Vault
- Retrieved via: `ConnectionStrings-DefaultConnection`
- Fallback to environment variable: `MEANDMYDOG_ConnectionStrings__DefaultConnection`

### Azure Services Used
1. **Azure Key Vault** - Secure configuration storage
2. **Azure Blob Storage** - Media file storage
3. **Azure SQL Database** - Primary database
4. **Application Insights** - Performance monitoring
5. **Azure VM** - Database hosting (development)

## Authentication & Authorization

### JWT Configuration
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
        };
    });
```

### User Roles
1. **User** - Base authenticated user
2. **Admin** - Administrative access
3. **PetOwner** - Pet owner features
4. **ServiceProvider** - Service provider features

### Two-Factor Authentication
- TOTP implementation with OtpNet
- QR code generation for authenticator apps
- Recovery codes (10 by default)
- Database fields: `TotpSecret`, `IsTwoFactorEnabled`

### Security Features
- Email confirmation required
- Account lockout (5 attempts, 30-minute lockout)
- IP blocking for suspicious activity
- Refresh tokens (7-day expiry)
- Password requirements (8+ chars, mixed case, digits, symbols)

## Core Features

### 1. Dog Profile Management
- Complete dog profiles with photos/videos
- Medical records and health tracking
- Pedigree and breeding information
- Social features and friendships
- Activity tracking

### 2. Pet Services Marketplace
- Service provider listings
- Advanced search and filtering
- Booking system with recurring options
- Review and rating system
- KYC verification for providers

### 3. AI-Powered Features
- **Gemini AI Integration** for health recommendations
- Predictive health insights
- Image moderation (feature flag controlled)
- Behavioral analysis

### 4. Real-time Communication
- Direct messaging with SignalR
- Video calling capabilities
- Push notifications
- Email notifications
- SMS alerts (infrastructure ready)

### 5. Payment & Billing
- Multi-provider support (Santander, Stripe, PayPal)
- Subscription management (Free/Premium)
- Stored payment methods
- Invoice generation
- Refund processing

### 6. Community Features
- Forums and discussions
- Dog meetups and playdates
- Lost & found pets
- Walking routes sharing
- Social media integration

### 7. Business Tools
- Service provider dashboard
- Appointment scheduling
- Invoice management
- Expense tracking
- Analytics and reporting

### 8. Content Management
- CMS for dynamic pages
- Blog functionality
- Widget system
- SEO optimization
- Multi-language support (infrastructure)

## Database Design

### Core Entities

#### User & Authentication
- `ApplicationUser` - Identity user
- `UserProfile` - Extended profile
- `RefreshToken` - JWT refresh tokens
- `UserActivity` - Activity tracking

#### Dog Management
- `DogProfile` - Main dog entity
- `DogMedicalRecord` - Health records
- `DogBehaviorProfile` - Behavior tracking
- `DogBreed` - Breed information

#### Pet Services
- `PetServiceProfile` - Provider profiles
- `PetServiceBooking` - Bookings
- `PetServiceReview` - Reviews
- `ServiceType` - Service categories

#### Payment & Financial
- `PaymentRecord` - Transactions
- `Subscription` - User subscriptions
- `Invoice` - Billing records
- `StoredPaymentMethod` - Saved cards

#### Communication
- `UnifiedMessage` - Messaging system
- `VideoCall` - Call records
- `PushNotificationSubscription` - Push subscriptions

### Relationships
- User → Many Dogs (One-to-Many)
- Dog → Many Medical Records (One-to-Many)
- Service Provider → Many Bookings (One-to-Many)
- Booking → Many Dogs (Many-to-Many via junction table)

## External Integrations

### Payment Providers
1. **Santander**
   - API Key: Stored in Key Vault
   - Webhook URL: `/api/v1/webhooks/santander`
   - Features: Subscriptions, refunds, tokenization

2. **Stripe**
   - API Key: Stored in Key Vault
   - Webhook Secret: Stored in Key Vault
   - Features: Checkout, subscriptions, payment methods

3. **PayPal**
   - Client ID/Secret: Stored in Key Vault
   - Sandbox mode for development

### AI & Analytics
1. **Google Gemini AI**
   - Model: gemini-1.5-flash
   - Use: Health recommendations, content generation

2. **Google Maps**
   - API Key: Stored in configuration
   - Use: Location services, geocoding

### Third-Party Services
1. **SendGrid** - Email delivery
2. **WebPush** - Browser notifications
3. **Awin** - Affiliate marketing
4. **Expedia/Vrbo** - Travel services
5. **Didit** - KYC verification

### Kennel Club APIs
- AKC (American Kennel Club)
- KC (The Kennel Club UK)
- CKC (Canadian Kennel Club)
- FCI (International)

## Payment System

### Subscription Tiers
```csharp
public enum SubscriptionType
{
    Free,
    Premium // £15-19.99/month depending on provider
}
```

### Payment Features
1. **Subscription Management**
   - Auto-renewal
   - Upgrade/downgrade with proration
   - Grace periods for failed payments
   - Scheduled plan changes

2. **Payment Methods**
   - Card storage with tokenization
   - Multiple payment methods per user
   - Default payment method selection
   - Automatic expiry monitoring

3. **Webhooks**
   - Signature verification
   - Event processing
   - Retry logic with exponential backoff
   - Comprehensive logging

4. **Analytics**
   - Revenue tracking
   - Conversion funnels
   - Payment method distribution
   - Real-time dashboards

## UI/UX Architecture

### Design System
- **Framework**: Tailwind CSS with custom configuration
- **Theme**: Dark Gold luxury theme
- **Components**: Reusable component library
- **Accessibility**: WCAG compliance

### Frontend Technologies
1. **Alpine.js** - Lightweight interactivity
2. **Vue.js 3** - Complex components
3. **SignalR** - Real-time updates
4. **PWA** - Offline capabilities

### Key UI Features
- Responsive design (mobile-first)
- Dark mode support
- Touch gestures
- Virtual scrolling
- Progressive loading

### Component Library
- Alerts & notifications
- Modal dialogs
- Form components
- Data tables
- Charts & analytics

## API Design

### Versioning Strategy
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

### Controller Organization
- Feature-based structure
- RESTful endpoints
- Consistent naming conventions
- Comprehensive error handling

### API Features
1. **Rate Limiting** - Configurable per endpoint
2. **Caching** - Response caching
3. **Compression** - Gzip/Brotli
4. **CORS** - Configurable origins
5. **Swagger** - Interactive documentation

## Security Considerations

### Application Security
1. **Authentication**: JWT with refresh tokens
2. **Authorization**: Role-based access control
3. **Data Protection**: Encryption at rest and in transit
4. **Input Validation**: Comprehensive validation
5. **XSS Protection**: Content Security Policy
6. **CSRF Protection**: Anti-forgery tokens

### Infrastructure Security
1. **Secrets Management**: Azure Key Vault
2. **HTTPS Enforcement**: SSL/TLS only
3. **Security Headers**: HSTS, X-Frame-Options
4. **Rate Limiting**: DDoS protection
5. **Audit Logging**: Comprehensive activity logs

## Development Setup

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019+
- Node.js 18+
- Redis (optional for caching)

### Configuration Steps

1. **Clone Repository**
```bash
git clone [repository-url]
cd MeAndMyDoggy
```

2. **Setup User Secrets**
```bash
# API Project
cd src/API/MeAndMyDog.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[your-connection-string]"

# WebApp Project
cd src/Web/MeAndMyDog.WebApp
dotnet user-secrets init
```

3. **Database Setup**
```bash
cd src/API/MeAndMyDog.API
dotnet ef database update
```

4. **Run Applications**
```bash
# Terminal 1 - API
dotnet run --project src/API/MeAndMyDog.API

# Terminal 2 - WebApp
dotnet run --project src/Web/MeAndMyDog.WebApp
```

### Test Users
| Email | Password | Roles |
|-------|----------|-------|
| testuser@example.com | TestUser123! | User, PetOwner |
| admin@example.com | AdminUser123! | User, Admin |
| provider@example.com | Provider123! | User, ServiceProvider |

### Development URLs
- **API**: https://localhost:7010
- **WebApp**: https://localhost:56682
- **Swagger**: https://localhost:7010/swagger

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

## Monitoring & Analytics

### Application Insights
- Performance tracking
- Error monitoring
- User analytics
- Custom metrics

### Health Checks
- Database connectivity
- External service availability
- Redis connection
- Storage access

### Logging
- Structured logging with Serilog
- Log levels: Verbose, Debug, Information, Warning, Error, Fatal
- Centralized error tracking in database

## Deployment Considerations

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Staging/Production
- `MEANDMYDOG_ConnectionStrings__DefaultConnection`: Database connection
- `APPLICATIONINSIGHTS_CONNECTION_STRING`: App Insights
- `AZURE_CLIENT_ID`: Managed Identity (production)

### Performance Optimization
- Response compression
- Static file caching
- Database connection pooling
- Distributed caching with Redis
- CDN for static assets

### Scalability
- Horizontal scaling ready
- Stateless API design
- Distributed caching
- Background job processing
- Message queue ready architecture

---

This specification provides a comprehensive overview of the MeAndMyDoggy platform. For specific implementation details, refer to the source code and individual component documentation.