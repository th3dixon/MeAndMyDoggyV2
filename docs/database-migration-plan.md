# MeAndMyDoggyV2 Complete Database Migration Plan

## Overview

This document outlines the systematic migration plan to implement all missing database entities for the unified MeAndMyDoggyV2 platform. The migrations are organized by dependency order and functional groups to ensure referential integrity and minimize deployment risks.

## Current State Analysis

**Existing Tables (5):**
- Users (AspNetCore Identity)
- Roles 
- UserRoles
- Basic messaging infrastructure
- ServiceProviders (partial)

**Required Tables (35+):**
- Complete RBAC system
- Pet management entities
- Messaging system enhancements
- KYC verification workflow
- Calendar and appointments
- AI integration tables
- Subscription and billing
- Security and audit logs

## Migration Phase Structure

### Phase 1: Foundation & Security (Migrations 001-008)
**Priority:** CRITICAL - Required for basic platform security
**Timeline:** Week 1
**Dependencies:** None (extends existing AspNetCore Identity)

```
Migration 001: Enhanced User Management
Migration 002: RBAC System (Permissions)
Migration 003: User Sessions & Security
Migration 004: System Configuration
Migration 005: Audit Logging
Migration 006: KYC Verification Foundation
Migration 007: File Storage Management
Migration 008: User Settings & Preferences
```

### Phase 2: Core Business Entities (Migrations 009-016)
**Priority:** HIGH - Core platform functionality
**Timeline:** Week 2
**Dependencies:** Phase 1 complete

```
Migration 009: Pet Profiles (DogProfiles)
Migration 010: Pet Medical Records
Migration 011: Service Catalog
Migration 012: Service Provider Enhancements
Migration 013: Reviews & Ratings
Migration 014: Subscription Plans
Migration 015: User Subscriptions
Migration 016: Location & Address Management
```

### Phase 3: Communication & Interaction (Migrations 017-024)
**Priority:** HIGH - Real-time features
**Timeline:** Week 3
**Dependencies:** Phase 1 & 2 complete

```
Migration 017: Enhanced Messaging System
Migration 018: Message Attachments & Media
Migration 019: Message Reactions & Read Receipts
Migration 020: Video Call Sessions
Migration 021: Notifications System
Migration 022: Push Notification Settings
Migration 023: User Presence & Activity
Migration 024: Conversation Management Enhancements
```

### Phase 4: Scheduling & Appointments (Migrations 025-030)
**Priority:** MEDIUM - Booking functionality
**Timeline:** Week 4
**Dependencies:** Phase 1, 2 & 3 complete

```
Migration 025: Availability Management
Migration 026: Appointment System
Migration 027: Calendar Integration
Migration 028: Booking Rules & Constraints
Migration 029: Appointment History & Status
Migration 030: Service Delivery Tracking
```

### Phase 5: AI & Analytics (Migrations 031-036)
**Priority:** MEDIUM - Advanced features
**Timeline:** Week 5
**Dependencies:** All previous phases

```
Migration 031: AI Health Recommendations
Migration 032: AI Content Moderation
Migration 033: AI Usage Tracking
Migration 034: User Analytics & Behavior
Migration 035: Business Intelligence
Migration 036: Performance Metrics
```

### Phase 6: Advanced Features (Migrations 037-042)
**Priority:** LOW - Enhancement features
**Timeline:** Week 6
**Dependencies:** All previous phases

```
Migration 037: Emergency Contacts
Migration 038: Service Provider Verification
Migration 039: Payment Tracking (Reference Only)
Migration 040: Expense Management
Migration 041: Reporting & Dashboard Data
Migration 042: System Health Monitoring
```

## Detailed Migration Specifications

### Phase 1: Foundation & Security

#### Migration 001: Enhanced User Management
```sql
-- Extends existing AspNetUsers table
ALTER TABLE Users ADD COLUMN FirstName NVARCHAR(100);
ALTER TABLE Users ADD COLUMN LastName NVARCHAR(100);
ALTER TABLE Users ADD COLUMN ProfileImageUrl NVARCHAR(500);
ALTER TABLE Users ADD COLUMN DateOfBirth DATE;
ALTER TABLE Users ADD COLUMN Gender NVARCHAR(20);
ALTER TABLE Users ADD COLUMN TimeZone NVARCHAR(50) NOT NULL DEFAULT 'UTC';
ALTER TABLE Users ADD COLUMN PreferredLanguage NVARCHAR(10) NOT NULL DEFAULT 'en';
ALTER TABLE Users ADD COLUMN IsEmailNotificationsEnabled BIT NOT NULL DEFAULT 1;
ALTER TABLE Users ADD COLUMN IsSmsNotificationsEnabled BIT NOT NULL DEFAULT 0;
ALTER TABLE Users ADD COLUMN LastSeenAt DATETIMEOFFSET;
ALTER TABLE Users ADD COLUMN AccountStatus NVARCHAR(20) NOT NULL DEFAULT 'Active';
-- Active, Suspended, Pending, Deactivated

-- Create indexes for enhanced fields
CREATE INDEX IX_Users_Name ON Users (FirstName, LastName);
CREATE INDEX IX_Users_LastSeen ON Users (LastSeenAt DESC);
CREATE INDEX IX_Users_AccountStatus ON Users (AccountStatus);
```

#### Migration 002: RBAC System (Permissions)
```sql
-- Permissions table
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    Category NVARCHAR(100) NOT NULL,
    IsSystemPermission BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    
    INDEX IX_Permissions_Category (Category),
    INDEX IX_Permissions_Name (Name)
);

-- Role-Permission mapping
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    GrantedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    GrantedBy NVARCHAR(450), -- UserId who granted permission
    
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE,
    FOREIGN KEY (GrantedBy) REFERENCES Users(Id)
);

-- Enhanced Roles table
ALTER TABLE Roles ADD COLUMN Level INT NOT NULL DEFAULT 1;
ALTER TABLE Roles ADD COLUMN IsSystemRole BIT NOT NULL DEFAULT 0;
ALTER TABLE Roles ADD COLUMN MaxUsers INT NULL;

-- Enhanced UserRoles table
ALTER TABLE UserRoles ADD COLUMN AssignedBy NVARCHAR(450) NOT NULL;
ALTER TABLE UserRoles ADD COLUMN IsActive BIT NOT NULL DEFAULT 1;
ALTER TABLE UserRoles ADD COLUMN ExpiresAt DATETIMEOFFSET NULL;
ALTER TABLE UserRoles ADD COLUMN Notes NVARCHAR(500);

-- Add foreign key for AssignedBy
ALTER TABLE UserRoles ADD CONSTRAINT FK_UserRoles_AssignedBy 
    FOREIGN KEY (AssignedBy) REFERENCES Users(Id);

-- Insert default permissions
INSERT INTO Permissions (Name, Description, Category, IsSystemPermission) VALUES
('users.view', 'View user profiles', 'User Management', 1),
('users.manage', 'Manage user accounts', 'User Management', 1),
('services.book', 'Book services', 'Service Booking', 1),
('services.provide', 'Provide services', 'Service Provision', 1),
('messaging.access', 'Access messaging system', 'Communication', 1),
('kyc.review', 'Review KYC submissions', 'Verification', 1),
('ai.access', 'Access AI features', 'AI Features', 1),
('calendar.manage', 'Manage calendar', 'Calendar', 1),
('system.admin', 'System administration', 'Administration', 1);

-- Insert default role-permission mappings
INSERT INTO RolePermissions (RoleId, PermissionId) 
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'User' AND p.Name = 'users.view';

INSERT INTO RolePermissions (RoleId, PermissionId) 
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'PetOwner' AND p.Name IN ('users.view', 'services.book', 'messaging.access', 'ai.access');

INSERT INTO RolePermissions (RoleId, PermissionId) 
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'ServiceProvider' AND p.Name IN ('users.view', 'services.provide', 'messaging.access', 'calendar.manage', 'ai.access');

INSERT INTO RolePermissions (RoleId, PermissionId) 
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'Admin' AND p.Category IN ('User Management', 'Verification', 'Administration');
```

#### Migration 003: User Sessions & Security
```sql
CREATE TABLE UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(450) NOT NULL,
    SessionToken NVARCHAR(500) NOT NULL UNIQUE,
    RefreshToken NVARCHAR(500) NOT NULL UNIQUE,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIMEOFFSET NOT NULL,
    LastActivityAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    IPAddress NVARCHAR(45),
    UserAgent NVARCHAR(500),
    DeviceInfo NVARCHAR(200),
    Location NVARCHAR(200), -- City, Country
    IsActive BIT NOT NULL DEFAULT 1,
    EndedAt DATETIMEOFFSET NULL,
    EndReason NVARCHAR(50), -- Logout, Expired, Revoked, Replaced
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    INDEX IX_UserSessions_User (UserId),
    INDEX IX_UserSessions_Token (SessionToken),
    INDEX IX_UserSessions_RefreshToken (RefreshToken),
    INDEX IX_UserSessions_ExpiresAt (ExpiresAt),
    INDEX IX_UserSessions_LastActivity (LastActivityAt DESC)
);

CREATE TABLE LoginAttempts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL,
    IPAddress NVARCHAR(45) NOT NULL,
    UserAgent NVARCHAR(500),
    Success BIT NOT NULL,
    FailureReason NVARCHAR(100), -- InvalidCredentials, AccountLocked, etc.
    AttemptedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    
    INDEX IX_LoginAttempts_Email_Time (Email, AttemptedAt DESC),
    INDEX IX_LoginAttempts_IP_Time (IPAddress, AttemptedAt DESC),
    INDEX IX_LoginAttempts_Success (Success)
);
```

#### Migration 004: System Configuration
```sql
CREATE TABLE SystemSettings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SettingKey NVARCHAR(200) NOT NULL UNIQUE,
    SettingValue NVARCHAR(MAX) NOT NULL,
    DataType NVARCHAR(20) NOT NULL, -- String, Integer, Boolean, JSON, Decimal
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsUserEditable BIT NOT NULL DEFAULT 0,
    IsEncrypted BIT NOT NULL DEFAULT 0,
    ValidationRule NVARCHAR(500), -- Regex or JSON schema
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy NVARCHAR(450),
    
    FOREIGN KEY (UpdatedBy) REFERENCES Users(Id),
    INDEX IX_SystemSettings_Category (Category),
    INDEX IX_SystemSettings_Key (SettingKey)
);

-- Insert default system settings
INSERT INTO SystemSettings (SettingKey, SettingValue, DataType, Category, Description) VALUES
('Platform.MaintenanceMode', 'false', 'Boolean', 'System', 'Enable maintenance mode'),
('Platform.AllowRegistration', 'true', 'Boolean', 'Security', 'Allow new user registration'),
('Platform.RequireEmailVerification', 'true', 'Boolean', 'Security', 'Require email verification'),
('Platform.SessionTimeoutMinutes', '120', 'Integer', 'Security', 'Session timeout in minutes'),
('KYC.AutoApprovalEnabled', 'false', 'Boolean', 'KYC', 'Enable automatic KYC approval'),
('AI.DailyRequestLimit', '100', 'Integer', 'AI', 'Daily AI request limit per user'),
('Messaging.MaxFileSize', '10485760', 'Integer', 'Messaging', 'Max file size in bytes (10MB)'),
('Calendar.BookingAdvanceDays', '30', 'Integer', 'Calendar', 'Maximum days in advance for booking');
```

#### Migration 005: Audit Logging
```sql
CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(450) NULL,
    Action NVARCHAR(100) NOT NULL,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(100),
    OldValues NVARCHAR(MAX), -- JSON
    NewValues NVARCHAR(MAX), -- JSON
    IPAddress NVARCHAR(45),
    UserAgent NVARCHAR(500),
    SessionId UNIQUEIDENTIFIER NULL,
    Timestamp DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    Severity NVARCHAR(20) NOT NULL DEFAULT 'Info', -- Info, Warning, Error, Critical
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SessionId) REFERENCES UserSessions(Id),
    
    INDEX IX_AuditLogs_User_Timestamp (UserId, Timestamp DESC),
    INDEX IX_AuditLogs_EntityType (EntityType),
    INDEX IX_AuditLogs_Action (Action),
    INDEX IX_AuditLogs_Timestamp (Timestamp DESC),
    INDEX IX_AuditLogs_Severity (Severity)
);

-- Partitioning for performance (monthly partitions)
-- This would be implemented as a separate step for large-scale deployments
```

### Phase 2: Core Business Entities

#### Migration 009: Pet Profiles (DogProfiles)
```sql
CREATE TABLE DogProfiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OwnerId NVARCHAR(450) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Breed NVARCHAR(100),
    SecondaryBreed NVARCHAR(100), -- For mixed breeds
    Birthdate DATE,
    Weight DECIMAL(5,2),
    Height DECIMAL(5,2), -- in inches
    Gender NVARCHAR(10), -- Male, Female, Unknown
    IsSpayedNeutered BIT DEFAULT 0,
    CoatColor NVARCHAR(100),
    CoatType NVARCHAR(50), -- Short, Long, Curly, etc.
    EyeColor NVARCHAR(50),
    MicrochipNumber NVARCHAR(50),
    RegistrationNumber NVARCHAR(100), -- AKC or other registration
    
    -- Emergency & Veterinary Contacts
    EmergencyContactName NVARCHAR(200),
    EmergencyContactPhone NVARCHAR(50),
    EmergencyContactRelationship NVARCHAR(100),
    VeterinarianName NVARCHAR(200),
    VeterinarianPhone NVARCHAR(50),
    VeterinarianAddress NVARCHAR(500),
    
    -- Health & Behavior
    EnergyLevel NVARCHAR(20), -- Low, Medium, High
    SocializationLevel NVARCHAR(20), -- Shy, Friendly, Very Social
    TrainingLevel NVARCHAR(20), -- None, Basic, Advanced
    SpecialNeeds NVARCHAR(MAX),
    Medications NVARCHAR(MAX),
    Allergies NVARCHAR(MAX),
    BehaviorNotes NVARCHAR(MAX),
    DietaryRestrictions NVARCHAR(MAX),
    
    -- Media & Documentation
    ProfileImageUrl NVARCHAR(500),
    AdditionalPhotos NVARCHAR(MAX), -- JSON array of URLs
    
    -- Metadata
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    INDEX IX_DogProfiles_Owner (OwnerId),
    INDEX IX_DogProfiles_Name (Name),
    INDEX IX_DogProfiles_Breed (Breed),
    INDEX IX_DogProfiles_Active (IsActive),
    INDEX IX_DogProfiles_Owner_Active (OwnerId, IsActive)
);

-- Full-text search capability
CREATE FULLTEXT INDEX ON DogProfiles (Name, Breed, SecondaryBreed, SpecialNeeds, BehaviorNotes)
KEY INDEX PK__DogProfiles;
```

## Migration Execution Strategy

### Pre-Migration Checklist
1. **Backup Strategy**
   - Full database backup before each phase
   - Transaction log backup every 15 minutes during migration
   - Point-in-time recovery verification

2. **Environment Preparation**
   - Test migrations in development environment
   - Validate migrations in staging environment
   - Performance testing with sample data

3. **Rollback Planning**
   - Rollback scripts for each migration
   - Data validation queries
   - Application compatibility verification

### Migration Execution Order

```bash
# Phase 1: Foundation (Week 1)
dotnet ef migrations add 001_EnhancedUserManagement
dotnet ef migrations add 002_RBACSystem
dotnet ef migrations add 003_UserSessions
dotnet ef migrations add 004_SystemConfiguration
dotnet ef migrations add 005_AuditLogging
dotnet ef migrations add 006_KYCFoundation
dotnet ef migrations add 007_FileStorage
dotnet ef migrations add 008_UserSettings

# Deploy Phase 1
dotnet ef database update --target-migration 008_UserSettings

# Phase 2: Core Business (Week 2)
dotnet ef migrations add 009_DogProfiles
dotnet ef migrations add 010_MedicalRecords
dotnet ef migrations add 011_ServiceCatalog
dotnet ef migrations add 012_ServiceProviderEnhancements
dotnet ef migrations add 013_ReviewsRatings
dotnet ef migrations add 014_SubscriptionPlans
dotnet ef migrations add 015_UserSubscriptions
dotnet ef migrations add 016_LocationManagement

# Continue for all phases...
```

### Performance Considerations

1. **Index Creation Strategy**
   - Create indexes after data migration for better performance
   - Use ONLINE index creation where possible
   - Monitor index fragmentation

2. **Data Migration**
   - Batch processing for large data sets
   - Progress monitoring and logging
   - Parallel processing where safe

3. **Constraint Application**
   - Add foreign key constraints after data validation
   - Implement check constraints last
   - Validate data integrity before constraint activation

### Monitoring & Validation

1. **Migration Health Checks**
   - Row count validation
   - Data integrity verification
   - Performance baseline comparison

2. **Application Integration Testing**
   - API endpoint functionality
   - Entity Framework model validation
   - User interface compatibility

3. **Rollback Triggers**
   - Migration time exceeds 2x estimate
   - Data corruption detected
   - Application functionality broken

## Risk Mitigation

### High-Risk Migrations
1. **RBAC System (Migration 002)** - Affects all user access
2. **Dog Profiles (Migration 009)** - Core business entity
3. **Enhanced Messaging (Migration 017)** - Real-time functionality

### Mitigation Strategies
1. **Blue-Green Deployment** for high-risk migrations
2. **Feature Flags** to disable new functionality during migration
3. **Phased User Rollout** to verify functionality
4. **24/7 Monitoring** during migration periods

## Success Criteria

### Phase Completion Criteria
- [ ] All migrations execute successfully
- [ ] Data integrity validation passes
- [ ] Performance benchmarks met
- [ ] Application functionality verified
- [ ] User acceptance testing completed

### Overall Success Metrics
- **Zero Data Loss** - All existing data preserved
- **Performance Maintained** - <5% degradation acceptable
- **Functionality Intact** - All existing features work
- **New Features Enabled** - Target functionality accessible

---

**Next Steps:**
1. Review and approve migration plan
2. Create Entity Framework Core models
3. Generate SQL migration scripts
4. Set up migration testing environment

This plan provides a systematic, risk-managed approach to implementing the complete database schema for MeAndMyDoggyV2 while maintaining system stability and data integrity.