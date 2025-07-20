# MeAndMyDoggy V2 - Comprehensive Technical Specification

## Table of Contents
1. [System Architecture Overview](#system-architecture-overview)
2. [Database Schema Design](#database-schema-design)
3. [Component Technical Specifications](#component-technical-specifications)
4. [API Specifications](#api-specifications)
5. [Security Architecture](#security-architecture)
6. [Performance & Caching Strategy](#performance--caching-strategy)
7. [Integration Patterns](#integration-patterns)

## System Architecture Overview

### Technology Stack
- **Backend**: ASP.NET Core 9.0 with Entity Framework Core 9.0
- **Database**: SQL Server (Azure SQL Database in production)
- **Frontend**: ASP.NET Core MVC with Razor Pages + Vue.js 3.5.13
- **Real-time**: SignalR Core
- **Cloud Platform**: Microsoft Azure
- **Authentication**: ASP.NET Core Identity + JWT Bearer tokens
- **AI Integration**: Google Gemini API

### Architecture Pattern
- Clean Architecture with Domain-Driven Design principles
- CQRS pattern for complex operations
- Repository and Unit of Work patterns
- API Gateway pattern for microservices future-proofing

### Deployment Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                        Azure CDN                             │
├─────────────────────────────────────────────────────────────┤
│                    Application Gateway                       │
├─────────────────┬─────────────────┬────────────────────────┤
│   Web App       │    API App      │   SignalR Service      │
│  (Frontend)     │   (Backend)     │   (Real-time)          │
├─────────────────┴─────────────────┴────────────────────────┤
│                    Azure SQL Database                        │
├─────────────────────────────────────────────────────────────┤
│  Blob Storage  │  Key Vault  │  App Insights  │  Redis     │
└─────────────────────────────────────────────────────────────┘
```

## Database Schema Design

### Core Tables

#### 1. Identity & Authentication Tables

```sql
-- AspNetUsers (Extended from Identity)
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    -- Extended Properties
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [UserType] INT NOT NULL, -- 0: PetOwner, 1: ServiceProvider, 2: Both
    [AddressId] INT NULL,
    [ProfileImageUrl] NVARCHAR(500) NULL,
    [DateOfBirth] DATE NULL,
    [RegistrationDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActiveDate] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedDate] DATETIME2 NULL,
    [PreferredLanguage] NVARCHAR(10) NOT NULL DEFAULT 'en-GB',
    [TimeZone] NVARCHAR(50) NOT NULL DEFAULT 'GMT Standard Time',
    [NotificationPreferences] NVARCHAR(MAX) NULL, -- JSON
    [PrivacySettings] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_AspNetUsers_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [Addresses]([Id])
);

-- Addresses
CREATE TABLE [dbo].[Addresses] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AddressLine1] NVARCHAR(200) NOT NULL,
    [AddressLine2] NVARCHAR(200) NULL,
    [City] NVARCHAR(100) NOT NULL,
    [County] NVARCHAR(100) NULL,
    [PostCode] NVARCHAR(20) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL DEFAULT 'United Kingdom',
    [Latitude] DECIMAL(9, 6) NULL,
    [Longitude] DECIMAL(9, 6) NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [VerificationDate] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

#### 2. Dog Profile Management Tables

```sql
-- DogProfiles
CREATE TABLE [dbo].[DogProfiles] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OwnerId] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Breed] NVARCHAR(100) NULL,
    [MixedBreeds] NVARCHAR(500) NULL, -- JSON array for mixed breeds
    [DateOfBirth] DATE NULL,
    [Gender] INT NOT NULL, -- 0: Male, 1: Female
    [IsNeutered] BIT NOT NULL DEFAULT 0,
    [Weight] DECIMAL(5, 2) NULL, -- in kg
    [Size] INT NULL, -- 0: Small, 1: Medium, 2: Large, 3: Giant
    [Color] NVARCHAR(100) NULL,
    [MicrochipNumber] NVARCHAR(50) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Temperament] NVARCHAR(500) NULL, -- JSON array
    [EnergyLevel] INT NULL, -- 1-5 scale
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ProfileImageUrl] NVARCHAR(500) NULL,
    [CoverImageUrl] NVARCHAR(500) NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [Visibility] INT NOT NULL DEFAULT 0, -- 0: Private, 1: Friends, 2: Public
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogProfiles_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id])
);

-- DogMedicalRecords
CREATE TABLE [dbo].[DogMedicalRecords] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [RecordType] INT NOT NULL, -- 0: Vaccination, 1: Surgery, 2: Medication, 3: Allergy, 4: Condition, 5: Other
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Date] DATE NOT NULL,
    [VeterinarianName] NVARCHAR(200) NULL,
    [VeterinaryClinic] NVARCHAR(200) NULL,
    [NextDueDate] DATE NULL,
    [Cost] DECIMAL(10, 2) NULL,
    [Documents] NVARCHAR(MAX) NULL, -- JSON array of document URLs
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogMedicalRecords_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id])
);

-- DogPhotos
CREATE TABLE [dbo].[DogPhotos] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [PhotoUrl] NVARCHAR(500) NOT NULL,
    [ThumbnailUrl] NVARCHAR(500) NULL,
    [Caption] NVARCHAR(500) NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [IsProfilePhoto] BIT NOT NULL DEFAULT 0,
    [IsCoverPhoto] BIT NOT NULL DEFAULT 0,
    [UploadedBy] NVARCHAR(450) NOT NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Likes] INT NOT NULL DEFAULT 0,
    [Views] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_DogPhotos_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id]),
    CONSTRAINT [FK_DogPhotos_AspNetUsers] FOREIGN KEY ([UploadedBy]) REFERENCES [AspNetUsers]([Id])
);

-- DogActivities
CREATE TABLE [dbo].[DogActivities] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [ActivityType] INT NOT NULL, -- 0: Walk, 1: Play, 2: Training, 3: Grooming, 4: Vet Visit, 5: Other
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [StartTime] DATETIME2 NOT NULL,
    [EndTime] DATETIME2 NULL,
    [Duration] INT NULL, -- in minutes
    [Distance] DECIMAL(5, 2) NULL, -- in km
    [Location] NVARCHAR(500) NULL,
    [LocationCoordinates] GEOGRAPHY NULL,
    [Photos] NVARCHAR(MAX) NULL, -- JSON array
    [Notes] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogActivities_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id]),
    CONSTRAINT [FK_DogActivities_AspNetUsers] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);
```

#### 3. Service Provider Tables

```sql
-- ServiceProviders
CREATE TABLE [dbo].[ServiceProviders] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL UNIQUE,
    [BusinessName] NVARCHAR(200) NOT NULL,
    [BusinessType] INT NOT NULL, -- 0: Individual, 1: Company
    [CompanyNumber] NVARCHAR(50) NULL,
    [VATNumber] NVARCHAR(50) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [YearsInBusiness] INT NULL,
    [ServiceAreas] NVARCHAR(MAX) NULL, -- JSON array of postcodes/areas
    [ServiceRadius] INT NULL, -- in miles
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [VerificationDate] DATETIME2 NULL,
    [VerificationDocuments] NVARCHAR(MAX) NULL, -- JSON array
    [SubscriptionTier] INT NOT NULL DEFAULT 0, -- 0: Free, 1: Premium
    [SubscriptionStartDate] DATETIME2 NULL,
    [SubscriptionEndDate] DATETIME2 NULL,
    [Rating] DECIMAL(3, 2) NOT NULL DEFAULT 0.00,
    [ReviewCount] INT NOT NULL DEFAULT 0,
    [ResponseTime] INT NULL, -- average in hours
    [AcceptanceRate] DECIMAL(5, 2) NULL, -- percentage
    [CompletionRate] DECIMAL(5, 2) NULL, -- percentage
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsSuspended] BIT NOT NULL DEFAULT 0,
    [SuspensionReason] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServiceProviders_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- ServiceCategories
CREATE TABLE [dbo].[ServiceCategories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Icon] NVARCHAR(50) NULL,
    [ParentCategoryId] INT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServiceCategories_ParentCategory] FOREIGN KEY ([ParentCategoryId]) REFERENCES [ServiceCategories]([Id])
);

-- Services
CREATE TABLE [dbo].[Services] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [CategoryId] INT NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Duration] INT NULL, -- in minutes
    [PriceType] INT NOT NULL, -- 0: Fixed, 1: Hourly, 2: Daily, 3: Custom
    [BasePrice] DECIMAL(10, 2) NOT NULL,
    [AdditionalPricing] NVARCHAR(MAX) NULL, -- JSON for complex pricing
    [MaxDogs] INT NOT NULL DEFAULT 1,
    [IncludedServices] NVARCHAR(MAX) NULL, -- JSON array
    [Requirements] NVARCHAR(MAX) NULL, -- JSON array
    [CancellationPolicy] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Services_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Services_ServiceCategories] FOREIGN KEY ([CategoryId]) REFERENCES [ServiceCategories]([Id])
);

-- ServiceProviderAvailability
CREATE TABLE [dbo].[ServiceProviderAvailability] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [DayOfWeek] INT NOT NULL, -- 0: Sunday - 6: Saturday
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [IsAvailable] BIT NOT NULL DEFAULT 1,
    [EffectiveFrom] DATE NOT NULL,
    [EffectiveTo] DATE NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServiceProviderAvailability_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- ServiceProviderUnavailability
CREATE TABLE [dbo].[ServiceProviderUnavailability] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [Reason] NVARCHAR(500) NULL,
    [IsRecurring] BIT NOT NULL DEFAULT 0,
    [RecurrencePattern] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServiceProviderUnavailability_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);
```

#### 4. Booking & Transaction Tables

```sql
-- Bookings
CREATE TABLE [dbo].[Bookings] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BookingReference] NVARCHAR(50) NOT NULL UNIQUE,
    [ServiceId] INT NOT NULL,
    [ProviderId] INT NOT NULL,
    [CustomerId] NVARCHAR(450) NOT NULL,
    [Status] INT NOT NULL, -- 0: Pending, 1: Confirmed, 2: InProgress, 3: Completed, 4: Cancelled, 5: Disputed
    [StartDateTime] DATETIME2 NOT NULL,
    [EndDateTime] DATETIME2 NOT NULL,
    [Duration] INT NOT NULL, -- in minutes
    [TotalPrice] DECIMAL(10, 2) NOT NULL,
    [DepositAmount] DECIMAL(10, 2) NULL,
    [DiscountAmount] DECIMAL(10, 2) NULL,
    [FinalPrice] DECIMAL(10, 2) NOT NULL,
    [PaymentStatus] INT NOT NULL, -- 0: Pending, 1: DepositPaid, 2: FullyPaid, 3: Refunded
    [SpecialInstructions] NVARCHAR(MAX) NULL,
    [CancellationReason] NVARCHAR(500) NULL,
    [CancelledBy] NVARCHAR(450) NULL,
    [CancelledAt] DATETIME2 NULL,
    [CompletedAt] DATETIME2 NULL,
    [CustomerNotes] NVARCHAR(MAX) NULL,
    [ProviderNotes] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Bookings_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([Id]),
    CONSTRAINT [FK_Bookings_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Bookings_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Bookings_CancelledBy] FOREIGN KEY ([CancelledBy]) REFERENCES [AspNetUsers]([Id])
);

-- BookingDogs
CREATE TABLE [dbo].[BookingDogs] (
    [BookingId] INT NOT NULL,
    [DogProfileId] INT NOT NULL,
    [SpecialRequirements] NVARCHAR(MAX) NULL,
    PRIMARY KEY ([BookingId], [DogProfileId]),
    CONSTRAINT [FK_BookingDogs_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id]),
    CONSTRAINT [FK_BookingDogs_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id])
);

-- Payments
CREATE TABLE [dbo].[Payments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PaymentReference] NVARCHAR(50) NOT NULL UNIQUE,
    [BookingId] INT NOT NULL,
    [Amount] DECIMAL(10, 2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'GBP',
    [PaymentMethod] INT NOT NULL, -- 0: Card, 1: BankTransfer, 2: PayPal, 3: Other
    [PaymentProvider] NVARCHAR(50) NULL, -- Stripe, PayPal, etc.
    [ProviderTransactionId] NVARCHAR(200) NULL,
    [Status] INT NOT NULL, -- 0: Pending, 1: Processing, 2: Completed, 3: Failed, 4: Refunded
    [FailureReason] NVARCHAR(500) NULL,
    [ProcessedAt] DATETIME2 NULL,
    [RefundAmount] DECIMAL(10, 2) NULL,
    [RefundedAt] DATETIME2 NULL,
    [RefundReason] NVARCHAR(500) NULL,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Payments_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id])
);

-- Invoices
CREATE TABLE [dbo].[Invoices] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [InvoiceNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [BookingId] INT NULL,
    [ProviderId] INT NOT NULL,
    [CustomerId] NVARCHAR(450) NOT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Sent, 2: Paid, 3: Overdue, 4: Cancelled
    [IssueDate] DATE NOT NULL,
    [DueDate] DATE NOT NULL,
    [SubTotal] DECIMAL(10, 2) NOT NULL,
    [TaxAmount] DECIMAL(10, 2) NOT NULL,
    [TotalAmount] DECIMAL(10, 2) NOT NULL,
    [PaidAmount] DECIMAL(10, 2) NOT NULL DEFAULT 0,
    [Notes] NVARCHAR(MAX) NULL,
    [Terms] NVARCHAR(MAX) NULL,
    [PaidAt] DATETIME2 NULL,
    [SentAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Invoices_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id]),
    CONSTRAINT [FK_Invoices_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Invoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers]([Id])
);

-- InvoiceItems
CREATE TABLE [dbo].[InvoiceItems] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [InvoiceId] INT NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Quantity] DECIMAL(10, 2) NOT NULL,
    [UnitPrice] DECIMAL(10, 2) NOT NULL,
    [TaxRate] DECIMAL(5, 2) NOT NULL DEFAULT 20.00, -- UK VAT
    [DiscountAmount] DECIMAL(10, 2) NULL,
    [LineTotal] DECIMAL(10, 2) NOT NULL,
    [ServiceId] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices]([Id]),
    CONSTRAINT [FK_InvoiceItems_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([Id])
);
```

#### 5. Reviews & Ratings Tables

```sql
-- Reviews
CREATE TABLE [dbo].[Reviews] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BookingId] INT NOT NULL,
    [ReviewerId] NVARCHAR(450) NOT NULL,
    [RevieweeId] NVARCHAR(450) NOT NULL,
    [ReviewType] INT NOT NULL, -- 0: CustomerToProvider, 1: ProviderToCustomer
    [Rating] INT NOT NULL, -- 1-5
    [Title] NVARCHAR(200) NULL,
    [Comment] NVARCHAR(MAX) NULL,
    [ServiceQuality] INT NULL, -- 1-5
    [Communication] INT NULL, -- 1-5
    [Punctuality] INT NULL, -- 1-5
    [ValueForMoney] INT NULL, -- 1-5
    [WouldRecommend] BIT NULL,
    [Response] NVARCHAR(MAX) NULL,
    [RespondedAt] DATETIME2 NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0, -- Verified booking completion
    [IsReported] BIT NOT NULL DEFAULT 0,
    [ReportReason] NVARCHAR(500) NULL,
    [IsPublished] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Reviews_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id]),
    CONSTRAINT [FK_Reviews_Reviewers] FOREIGN KEY ([ReviewerId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Reviews_Reviewees] FOREIGN KEY ([RevieweeId]) REFERENCES [AspNetUsers]([Id])
);

-- ReviewPhotos
CREATE TABLE [dbo].[ReviewPhotos] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReviewId] INT NOT NULL,
    [PhotoUrl] NVARCHAR(500) NOT NULL,
    [Caption] NVARCHAR(200) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ReviewPhotos_Reviews] FOREIGN KEY ([ReviewId]) REFERENCES [Reviews]([Id])
);
```

#### 6. Messaging & Communication Tables

```sql
-- Conversations
CREATE TABLE [dbo].[Conversations] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ConversationType] INT NOT NULL, -- 0: Direct, 1: Group, 2: Booking
    [BookingId] INT NULL,
    [Subject] NVARCHAR(200) NULL,
    [LastMessageId] INT NULL,
    [LastMessageAt] DATETIME2 NULL,
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Conversations_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id])
);

-- ConversationParticipants
CREATE TABLE [dbo].[ConversationParticipants] (
    [ConversationId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [JoinedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastReadMessageId] INT NULL,
    [LastReadAt] DATETIME2 NULL,
    [IsMuted] BIT NOT NULL DEFAULT 0,
    [MutedUntil] DATETIME2 NULL,
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [UnreadCount] INT NOT NULL DEFAULT 0,
    PRIMARY KEY ([ConversationId], [UserId]),
    CONSTRAINT [FK_ConversationParticipants_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations]([Id]),
    CONSTRAINT [FK_ConversationParticipants_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- Messages
CREATE TABLE [dbo].[Messages] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ConversationId] INT NOT NULL,
    [SenderId] NVARCHAR(450) NOT NULL,
    [MessageType] INT NOT NULL, -- 0: Text, 1: Image, 2: File, 3: System
    [Content] NVARCHAR(MAX) NOT NULL,
    [Attachments] NVARCHAR(MAX) NULL, -- JSON array
    [IsEdited] BIT NOT NULL DEFAULT 0,
    [EditedAt] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [SystemMessageType] INT NULL, -- For system messages
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Messages_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations]([Id]),
    CONSTRAINT [FK_Messages_Senders] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers]([Id])
);

-- MessageReadReceipts
CREATE TABLE [dbo].[MessageReadReceipts] (
    [MessageId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [ReadAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([MessageId], [UserId]),
    CONSTRAINT [FK_MessageReadReceipts_Messages] FOREIGN KEY ([MessageId]) REFERENCES [Messages]([Id]),
    CONSTRAINT [FK_MessageReadReceipts_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);
```

#### 7. Notification Tables

```sql
-- NotificationTemplates
CREATE TABLE [dbo].[NotificationTemplates] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Category] NVARCHAR(50) NOT NULL,
    [Subject] NVARCHAR(200) NULL,
    [BodyTemplate] NVARCHAR(MAX) NOT NULL,
    [Variables] NVARCHAR(MAX) NULL, -- JSON array of available variables
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Notifications
CREATE TABLE [dbo].[Notifications] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [Type] INT NOT NULL, -- 0: System, 1: Booking, 2: Message, 3: Review, 4: Payment, 5: Other
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [Data] NVARCHAR(MAX) NULL, -- JSON for additional data
    [RelatedEntityType] NVARCHAR(50) NULL,
    [RelatedEntityId] NVARCHAR(50) NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    [IsSent] BIT NOT NULL DEFAULT 0,
    [SentAt] DATETIME2 NULL,
    [ExpiresAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- NotificationChannels
CREATE TABLE [dbo].[NotificationChannels] (
    [UserId] NVARCHAR(450) NOT NULL,
    [Channel] INT NOT NULL, -- 0: InApp, 1: Email, 2: SMS, 3: Push
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Settings] NVARCHAR(MAX) NULL, -- JSON for channel-specific settings
    PRIMARY KEY ([UserId], [Channel]),
    CONSTRAINT [FK_NotificationChannels_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);
```

#### 8. Content Management Tables

```sql
-- ContentPages
CREATE TABLE [dbo].[ContentPages] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Slug] NVARCHAR(200) NOT NULL UNIQUE,
    [Title] NVARCHAR(200) NOT NULL,
    [MetaDescription] NVARCHAR(500) NULL,
    [MetaKeywords] NVARCHAR(500) NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [ContentType] INT NOT NULL, -- 0: HTML, 1: Markdown, 2: JSON
    [Template] NVARCHAR(100) NULL,
    [ParentPageId] INT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsPublished] BIT NOT NULL DEFAULT 0,
    [PublishedAt] DATETIME2 NULL,
    [PublishedBy] NVARCHAR(450) NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [UpdatedBy] NVARCHAR(450) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ContentPages_Parent] FOREIGN KEY ([ParentPageId]) REFERENCES [ContentPages]([Id]),
    CONSTRAINT [FK_ContentPages_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_ContentPages_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_ContentPages_PublishedBy] FOREIGN KEY ([PublishedBy]) REFERENCES [AspNetUsers]([Id])
);

-- BlogPosts
CREATE TABLE [dbo].[BlogPosts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Slug] NVARCHAR(200) NOT NULL UNIQUE,
    [Title] NVARCHAR(200) NOT NULL,
    [Excerpt] NVARCHAR(500) NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [FeaturedImageUrl] NVARCHAR(500) NULL,
    [CategoryId] INT NOT NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [AuthorId] NVARCHAR(450) NOT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Published, 2: Scheduled, 3: Archived
    [PublishedAt] DATETIME2 NULL,
    [ScheduledFor] DATETIME2 NULL,
    [ViewCount] INT NOT NULL DEFAULT 0,
    [LikeCount] INT NOT NULL DEFAULT 0,
    [CommentCount] INT NOT NULL DEFAULT 0,
    [EstimatedReadTime] INT NULL, -- in minutes
    [MetaDescription] NVARCHAR(500) NULL,
    [MetaKeywords] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_BlogPosts_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [BlogCategories]([Id]),
    CONSTRAINT [FK_BlogPosts_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [AspNetUsers]([Id])
);

-- BlogCategories
CREATE TABLE [dbo].[BlogCategories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [ParentCategoryId] INT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_BlogCategories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [BlogCategories]([Id])
);

-- FAQs
CREATE TABLE [dbo].[FAQs] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CategoryId] INT NOT NULL,
    [Question] NVARCHAR(500) NOT NULL,
    [Answer] NVARCHAR(MAX) NOT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsPublished] BIT NOT NULL DEFAULT 1,
    [ViewCount] INT NOT NULL DEFAULT 0,
    [HelpfulCount] INT NOT NULL DEFAULT 0,
    [NotHelpfulCount] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_FAQs_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [FAQCategories]([Id])
);

-- FAQCategories
CREATE TABLE [dbo].[FAQCategories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Icon] NVARCHAR(50) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

#### 9. Administrative Tables

```sql
-- AdminUsers
CREATE TABLE [dbo].[AdminUsers] (
    [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [Role] INT NOT NULL, -- 0: SuperAdmin, 1: Admin, 2: Moderator, 3: Support
    [Permissions] NVARCHAR(MAX) NULL, -- JSON array of specific permissions
    [Department] NVARCHAR(100) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastLoginAt] DATETIME2 NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_AdminUsers_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_AdminUsers_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- AuditLogs
CREATE TABLE [dbo].[AuditLogs] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NULL,
    [UserType] NVARCHAR(50) NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [EntityType] NVARCHAR(100) NOT NULL,
    [EntityId] NVARCHAR(100) NULL,
    [OldValues] NVARCHAR(MAX) NULL, -- JSON
    [NewValues] NVARCHAR(MAX) NULL, -- JSON
    [Changes] NVARCHAR(MAX) NULL, -- JSON
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [AdditionalData] NVARCHAR(MAX) NULL, -- JSON
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_AuditLogs_UserId] ([UserId]),
    INDEX [IX_AuditLogs_EntityType_EntityId] ([EntityType], [EntityId]),
    INDEX [IX_AuditLogs_Timestamp] ([Timestamp])
);

-- SystemSettings
CREATE TABLE [dbo].[SystemSettings] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Category] NVARCHAR(100) NOT NULL,
    [Key] NVARCHAR(100) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [ValueType] NVARCHAR(50) NOT NULL, -- string, int, bool, json, etc.
    [Description] NVARCHAR(500) NULL,
    [IsEditable] BIT NOT NULL DEFAULT 1,
    [IsVisible] BIT NOT NULL DEFAULT 1,
    [UpdatedBy] NVARCHAR(450) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE ([Category], [Key]),
    CONSTRAINT [FK_SystemSettings_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- Reports
CREATE TABLE [dbo].[Reports] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReportType] INT NOT NULL, -- 0: User, 1: Provider, 2: Content, 3: Review, 4: Other
    [ReportedEntityType] NVARCHAR(100) NOT NULL,
    [ReportedEntityId] NVARCHAR(100) NOT NULL,
    [ReportedBy] NVARCHAR(450) NOT NULL,
    [Reason] INT NOT NULL, -- 0: Spam, 1: Inappropriate, 2: Fraud, 3: Harassment, 4: Other
    [Description] NVARCHAR(MAX) NOT NULL,
    [Evidence] NVARCHAR(MAX) NULL, -- JSON array of evidence URLs
    [Status] INT NOT NULL, -- 0: Pending, 1: UnderReview, 2: Resolved, 3: Dismissed
    [Resolution] NVARCHAR(MAX) NULL,
    [ResolvedBy] NVARCHAR(450) NULL,
    [ResolvedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Reports_ReportedBy] FOREIGN KEY ([ReportedBy]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Reports_ResolvedBy] FOREIGN KEY ([ResolvedBy]) REFERENCES [AspNetUsers]([Id])
);
```

#### 10. Analytics & Performance Tables

```sql
-- UserAnalytics
CREATE TABLE [dbo].[UserAnalytics] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [EventType] NVARCHAR(100) NOT NULL,
    [EventCategory] NVARCHAR(50) NOT NULL,
    [EventData] NVARCHAR(MAX) NULL, -- JSON
    [SessionId] NVARCHAR(100) NULL,
    [DeviceType] NVARCHAR(50) NULL,
    [Browser] NVARCHAR(50) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [Location] NVARCHAR(200) NULL,
    [Referrer] NVARCHAR(500) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_UserAnalytics_UserId_Timestamp] ([UserId], [Timestamp]),
    INDEX [IX_UserAnalytics_EventType] ([EventType]),
    INDEX [IX_UserAnalytics_Timestamp] ([Timestamp])
);

-- PerformanceMetrics
CREATE TABLE [dbo].[PerformanceMetrics] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MetricType] NVARCHAR(100) NOT NULL,
    [MetricName] NVARCHAR(200) NOT NULL,
    [Value] DECIMAL(18, 4) NOT NULL,
    [Unit] NVARCHAR(20) NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_PerformanceMetrics_MetricType_Timestamp] ([MetricType], [Timestamp]),
    INDEX [IX_PerformanceMetrics_Timestamp] ([Timestamp])
);

-- SearchHistory
CREATE TABLE [dbo].[SearchHistory] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NULL,
    [SearchType] INT NOT NULL, -- 0: Provider, 1: Service, 2: Location, 3: Blog
    [SearchQuery] NVARCHAR(500) NOT NULL,
    [Filters] NVARCHAR(MAX) NULL, -- JSON
    [ResultCount] INT NOT NULL,
    [ClickedResults] NVARCHAR(MAX) NULL, -- JSON array
    [SessionId] NVARCHAR(100) NULL,
    [SearchedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_SearchHistory_UserId] ([UserId]),
    INDEX [IX_SearchHistory_SearchType] ([SearchType]),
    INDEX [IX_SearchHistory_SearchedAt] ([SearchedAt])
);
```

### Database Indexes Strategy

```sql
-- Performance-critical indexes
CREATE INDEX [IX_Bookings_Status_StartDateTime] ON [Bookings]([Status], [StartDateTime]);
CREATE INDEX [IX_Bookings_ProviderId_Status] ON [Bookings]([ProviderId], [Status]);
CREATE INDEX [IX_Bookings_CustomerId_Status] ON [Bookings]([CustomerId], [Status]);

CREATE INDEX [IX_Services_ProviderId_IsActive] ON [Services]([ProviderId], [IsActive]);
CREATE INDEX [IX_Services_CategoryId_IsActive] ON [Services]([CategoryId], [IsActive]);

CREATE INDEX [IX_ServiceProviders_ServiceAreas] ON [ServiceProviders]([ServiceAreas]);
CREATE INDEX [IX_ServiceProviders_Rating_IsActive] ON [ServiceProviders]([Rating], [IsActive]);

CREATE INDEX [IX_DogProfiles_OwnerId_IsActive] ON [DogProfiles]([OwnerId], [IsActive]);

CREATE INDEX [IX_Messages_ConversationId_CreatedAt] ON [Messages]([ConversationId], [CreatedAt]);

CREATE INDEX [IX_Notifications_UserId_IsRead_CreatedAt] ON [Notifications]([UserId], [IsRead], [CreatedAt]);

-- Full-text search indexes
CREATE FULLTEXT CATALOG [FT_Catalog];
CREATE FULLTEXT INDEX ON [ServiceProviders]([BusinessName], [Description]) KEY INDEX [PK_ServiceProviders] ON [FT_Catalog];
CREATE FULLTEXT INDEX ON [Services]([Name], [Description]) KEY INDEX [PK_Services] ON [FT_Catalog];
CREATE FULLTEXT INDEX ON [BlogPosts]([Title], [Content], [Excerpt]) KEY INDEX [PK_BlogPosts] ON [FT_Catalog];
```

## Component Technical Specifications

### 1. Enhanced Dog Profile Management

#### Architecture
- **Pattern**: Component-based architecture with Vue.js frontend
- **State Management**: Pinia for complex state management
- **File Storage**: Azure Blob Storage for photos and documents
- **AI Integration**: Google Gemini for breed identification and health recommendations

#### Key Technical Features
- **Photo Management**: 
  - Azure Blob Storage with CDN
  - Image optimization pipeline (WebP conversion, multiple sizes)
  - Maximum 50 photos per dog profile
- **Medical Records**:
  - Encrypted storage for sensitive documents
  - PDF generation for vaccination certificates
  - Integration with vet APIs for record verification
- **Social Features**:
  - Real-time activity feed using SignalR
  - Redis caching for friend lists and activity streams

### 2. Service Provider Discovery

#### Architecture
- **Search Engine**: Elasticsearch for advanced search capabilities
- **Geolocation**: Azure Maps for location-based searches
- **Caching**: Redis for search results and provider data

#### Key Technical Features
- **Search Algorithm**:
  ```csharp
  // Weighted scoring algorithm
  Score = (RatingWeight * Rating) + 
          (ProximityWeight * (1 / Distance)) + 
          (ResponseTimeWeight * (1 / AvgResponseTime)) + 
          (ReviewCountWeight * Log(ReviewCount + 1))
  ```
- **Real-time Availability**: SignalR for live availability updates
- **Smart Filtering**: Machine learning for personalized recommendations

### 3. Real-time Messaging

#### Architecture
- **Protocol**: SignalR Core with WebSocket fallback
- **Message Queue**: Azure Service Bus for reliability
- **Storage**: Cosmos DB for message history

#### Key Technical Features
- **Encryption**: End-to-end encryption for sensitive messages
- **Offline Support**: Message queue for offline delivery
- **File Sharing**: Azure Blob Storage with 10MB limit per file
- **Read Receipts**: Real-time delivery and read status

### 4. Financial Management System

#### Architecture
- **Payment Processing**: Stripe integration
- **Accounting**: Double-entry bookkeeping system
- **Reporting**: SSRS for financial reports

#### Key Technical Features
- **Transaction Processing**:
  - ACID compliance for all financial operations
  - Audit trail for every transaction
  - PCI DSS compliance
- **Invoice Generation**:
  - PDF generation with custom templates
  - Automated recurring invoices
  - Multi-currency support (future)

### 5. Mobile-First Dashboard

#### Architecture
- **Frontend**: Vue.js with responsive design
- **PWA**: Progressive Web App capabilities
- **Performance**: Service Worker for offline functionality

#### Key Technical Features
- **Personalization**:
  - Machine learning for content recommendations
  - Customizable widget system
  - A/B testing framework
- **Performance Optimization**:
  - Lazy loading for components
  - Virtual scrolling for large lists
  - Image optimization with responsive images

## API Specifications

### Authentication Endpoints

```yaml
/api/v1/auth:
  /register:
    POST:
      body:
        email: string
        password: string
        firstName: string
        lastName: string
        userType: enum [PetOwner, ServiceProvider, Both]
      responses:
        201: User created successfully
        400: Validation errors
        409: Email already exists
  
  /login:
    POST:
      body:
        email: string
        password: string
      responses:
        200: 
          accessToken: string
          refreshToken: string
          expiresIn: number
        401: Invalid credentials
  
  /refresh:
    POST:
      body:
        refreshToken: string
      responses:
        200: New tokens
        401: Invalid refresh token
```

### Dog Profile Endpoints

```yaml
/api/v1/dogs:
  /:
    GET:
      description: Get user's dog profiles
      auth: required
      responses:
        200: Array of dog profiles
    
    POST:
      description: Create new dog profile
      auth: required
      body:
        name: string
        breed: string
        dateOfBirth: date
        gender: enum [Male, Female]
        # ... other properties
      responses:
        201: Dog profile created
  
  /{id}:
    GET:
      description: Get specific dog profile
      auth: required
      responses:
        200: Dog profile details
        404: Dog not found
    
    PUT:
      description: Update dog profile
      auth: required
      body: DogProfileUpdateDto
      responses:
        200: Updated profile
    
    DELETE:
      description: Soft delete dog profile
      auth: required
      responses:
        204: Deleted successfully
  
  /{id}/photos:
    POST:
      description: Upload dog photos
      auth: required
      contentType: multipart/form-data
      body:
        files: array[file]
        captions: array[string]
      responses:
        201: Photos uploaded
  
  /{id}/medical-records:
    GET:
      description: Get medical records
      auth: required
      responses:
        200: Array of medical records
    
    POST:
      description: Add medical record
      auth: required
      body: MedicalRecordDto
      responses:
        201: Record created
```

### Service Provider Endpoints

```yaml
/api/v1/providers:
  /search:
    GET:
      description: Search service providers
      parameters:
        q: string (query)
        lat: number
        lng: number
        radius: number (miles)
        category: string
        minRating: number
        availability: date
        page: number
        pageSize: number
      responses:
        200: 
          results: array[ProviderSearchResult]
          totalCount: number
          facets: object
  
  /{id}:
    GET:
      description: Get provider details
      responses:
        200: Provider details with services
  
  /{id}/availability:
    GET:
      description: Get provider availability
      parameters:
        startDate: date
        endDate: date
      responses:
        200: Availability calendar
  
  /{id}/reviews:
    GET:
      description: Get provider reviews
      parameters:
        page: number
        pageSize: number
        sort: enum [newest, oldest, rating]
      responses:
        200: Paginated reviews
```

### Booking Endpoints

```yaml
/api/v1/bookings:
  /:
    GET:
      description: Get user's bookings
      auth: required
      parameters:
        status: enum [all, pending, confirmed, completed]
        role: enum [customer, provider]
      responses:
        200: Array of bookings
    
    POST:
      description: Create new booking
      auth: required
      body:
        serviceId: number
        startDateTime: datetime
        endDateTime: datetime
        dogIds: array[number]
        specialInstructions: string
      responses:
        201: Booking created
  
  /{id}:
    GET:
      description: Get booking details
      auth: required
      responses:
        200: Booking details
    
    PUT:
      description: Update booking
      auth: required
      body: BookingUpdateDto
      responses:
        200: Updated booking
  
  /{id}/confirm:
    POST:
      description: Provider confirms booking
      auth: required (provider)
      responses:
        200: Booking confirmed
  
  /{id}/cancel:
    POST:
      description: Cancel booking
      auth: required
      body:
        reason: string
      responses:
        200: Booking cancelled
  
  /{id}/complete:
    POST:
      description: Mark booking as completed
      auth: required (provider)
      responses:
        200: Booking completed
```

### Messaging Endpoints (SignalR Hubs)

```csharp
public class ChatHub : Hub
{
    // Connection management
    public override async Task OnConnectedAsync()
    public override async Task OnDisconnectedAsync(Exception exception)
    
    // Messaging
    public async Task SendMessage(SendMessageDto message)
    public async Task MarkAsRead(int messageId)
    public async Task StartTyping(int conversationId)
    public async Task StopTyping(int conversationId)
    
    // Groups
    public async Task JoinConversation(int conversationId)
    public async Task LeaveConversation(int conversationId)
}
```

## Security Architecture

### Authentication & Authorization
- **JWT Bearer Tokens**: 
  - Access Token: 15 minutes expiry
  - Refresh Token: 30 days expiry
  - Stored in httpOnly cookies
- **Role-Based Access Control (RBAC)**:
  - Roles: PetOwner, ServiceProvider, Admin, SuperAdmin
  - Policy-based authorization for fine-grained control
- **Multi-Factor Authentication**: 
  - TOTP support
  - SMS backup codes

### Data Protection
- **Encryption**:
  - At-rest: AES-256 for sensitive data
  - In-transit: TLS 1.3
  - Database: Transparent Data Encryption (TDE)
- **Personal Data**:
  - GDPR compliance
  - Right to erasure implementation
  - Data portability API
- **API Security**:
  - Rate limiting: 100 requests/minute per user
  - API key authentication for external integrations
  - CORS policy configuration

### Compliance
- **PCI DSS**: Level 4 compliance for payment processing
- **GDPR**: Full compliance with data protection regulations
- **Audit Logging**: Comprehensive audit trail for all operations

## Performance & Caching Strategy

### Caching Layers
1. **Browser Cache**:
   - Static assets: 1 year
   - API responses: Cache-Control headers
2. **CDN (Azure CDN)**:
   - Images and static files
   - Geographic distribution
3. **Application Cache (Redis)**:
   - Session data: 30 minutes
   - Search results: 5 minutes
   - User profiles: 10 minutes
   - Provider availability: 1 minute
4. **Database Cache**:
   - Query result caching
   - Compiled query plans

### Performance Targets
- **Page Load Time**: < 2 seconds
- **API Response Time**: < 200ms (p95)
- **Search Response Time**: < 500ms
- **Real-time Message Delivery**: < 100ms

### Optimization Strategies
- **Database**:
  - Proper indexing strategy
  - Query optimization
  - Connection pooling
- **Application**:
  - Async/await patterns
  - Lazy loading
  - Response compression
- **Frontend**:
  - Code splitting
  - Tree shaking
  - Image optimization

## Integration Patterns

### External Services
1. **Payment Processing (Stripe)**:
   - Webhook integration for events
   - Strong Customer Authentication (SCA)
   - Subscription management
2. **Email Service (SendGrid)**:
   - Transactional emails
   - Marketing campaigns
   - Bounce handling
3. **SMS Service (Twilio)**:
   - Appointment reminders
   - Two-factor authentication
   - Emergency notifications
4. **Maps Service (Azure Maps)**:
   - Geocoding
   - Distance calculations
   - Service area visualization

### Internal Communication
- **Event-Driven Architecture**:
  - Azure Service Bus for async messaging
  - Event sourcing for audit trail
  - CQRS for read/write separation
- **API Gateway Pattern**:
  - Single entry point
  - Request routing
  - Cross-cutting concerns

### Data Synchronization
- **Change Data Capture (CDC)**:
  - Real-time data sync
  - Audit log population
- **ETL Processes**:
  - Daily analytics aggregation
  - Data warehouse population
  - Report generation

## Monitoring & Observability

### Application Insights Integration
```csharp
services.AddApplicationInsightsTelemetry();
services.Configure<TelemetryConfiguration>(config =>
{
    config.TelemetryInitializers.Add(new CloudRoleNameInitializer());
});
```

### Key Metrics
- **Business Metrics**:
  - Daily active users
  - Booking conversion rate
  - Average booking value
  - Provider utilization rate
- **Technical Metrics**:
  - Request rate
  - Error rate
  - Response time
  - Resource utilization

### Alerting Rules
- Error rate > 1% for 5 minutes
- Response time > 1s for 10 minutes
- Database connection failures
- Payment processing failures

## Deployment & DevOps

### CI/CD Pipeline
```yaml
trigger:
  - main
  - develop

stages:
  - stage: Build
    jobs:
      - job: BuildAndTest
        steps:
          - task: DotNetCoreCLI@2
            displayName: 'Restore'
          - task: DotNetCoreCLI@2
            displayName: 'Build'
          - task: DotNetCoreCLI@2
            displayName: 'Test'
          - task: DotNetCoreCLI@2
            displayName: 'Publish'
  
  - stage: Deploy_Staging
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/develop')
    jobs:
      - deployment: DeployToStaging
  
  - stage: Deploy_Production
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/main')
    jobs:
      - deployment: DeployToProduction
```

### Infrastructure as Code
- **Terraform** for Azure resource provisioning
- **Helm Charts** for Kubernetes deployments
- **Azure Resource Manager (ARM)** templates for legacy resources

### Backup & Disaster Recovery
- **Database Backups**:
  - Full backup: Daily
  - Differential: Every 4 hours
  - Transaction log: Every 15 minutes
  - Retention: 30 days
- **Blob Storage**:
  - Geo-redundant storage
  - Soft delete enabled
  - Versioning enabled
- **Disaster Recovery**:
  - RTO: 4 hours
  - RPO: 15 minutes
  - Automated failover