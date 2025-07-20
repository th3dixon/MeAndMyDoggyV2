# Database Migration Testing Setup for MeAndMyDoggyV2

## Overview

This document provides a comprehensive guide for setting up a development environment to test database migrations for the MeAndMyDoggyV2 project. The project uses:

- **Database Provider**: SQL Server (Azure SQL Database in production)
- **ORM**: Entity Framework Core 9.0
- **Authentication**: ASP.NET Identity with custom ApplicationUser and ApplicationRole
- **Current State**: 3 migrations already applied

## Current Database Configuration

### Connection String (Production)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:senseilive.uksouth.cloudapp.azure.com,1433;Initial Catalog=MeAndMyDoggy;Persist Security Info=False;User ID=DojoAdmin;Password=K1lledkenny#1;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
}
```

### Existing Migrations
1. `20250719231810_InitialComprehensiveMigration`
2. `20250720033925_CompleteDbContextWithAllEntities`
3. `20250720080002_SearchFunctionality`

## Local Development Environment Setup

### Prerequisites

1. **SQL Server Developer Edition** (2019 or later) or **SQL Server Express**
   - Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Alternative: Docker container for SQL Server

2. **.NET 9.0 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0

3. **Entity Framework Core CLI Tools**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

### Option 1: Using Docker for SQL Server

Create a `docker-compose.dev.yml` file in the project root:

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: meandmydoggy-sqlserver-dev
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
      - ./database-scripts:/docker-entrypoint-initdb.d
    networks:
      - meandmydoggy-network

  sqlserver-test:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: meandmydoggy-sqlserver-test
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
      - MSSQL_PID=Developer
    ports:
      - "1434:1433"
    volumes:
      - sqlserver-test-data:/var/opt/mssql
    networks:
      - meandmydoggy-network

volumes:
  sqlserver-data:
  sqlserver-test-data:

networks:
  meandmydoggy-network:
    driver: bridge
```

Start the containers:
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### Option 2: Using Local SQL Server Installation

1. Install SQL Server Developer Edition
2. Enable TCP/IP in SQL Server Configuration Manager
3. Create databases for development and testing

## Database Setup Scripts

### 1. Create Development Databases

Create file `scripts/setup-dev-databases.sql`:

```sql
-- Create development database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MeAndMyDoggy_Dev')
BEGIN
    CREATE DATABASE MeAndMyDoggy_Dev;
END
GO

-- Create test database for migration testing
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MeAndMyDoggy_Test')
BEGIN
    CREATE DATABASE MeAndMyDoggy_Test;
END
GO

-- Create staging database for rollback testing
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MeAndMyDoggy_Staging')
BEGIN
    CREATE DATABASE MeAndMyDoggy_Staging;
END
GO

-- Grant permissions (adjust user as needed)
USE MeAndMyDoggy_Dev;
GO
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'DojoAdmin')
BEGIN
    CREATE USER DojoAdmin FOR LOGIN DojoAdmin;
    ALTER ROLE db_owner ADD MEMBER DojoAdmin;
END
GO

USE MeAndMyDoggy_Test;
GO
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'DojoAdmin')
BEGIN
    CREATE USER DojoAdmin FOR LOGIN DojoAdmin;
    ALTER ROLE db_owner ADD MEMBER DojoAdmin;
END
GO

USE MeAndMyDoggy_Staging;
GO
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'DojoAdmin')
BEGIN
    CREATE USER DojoAdmin FOR LOGIN DojoAdmin;
    ALTER ROLE db_owner ADD MEMBER DojoAdmin;
END
GO
```

### 2. Environment-Specific Configuration

Create `src/API/MeAndMyDog.API/appsettings.Testing.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MeAndMyDoggy_Test;User ID=sa;Password=YourStrong@Password123;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Warning"
    }
  }
}
```

Create `src/API/MeAndMyDog.API/appsettings.Local.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MeAndMyDoggy_Dev;User ID=sa;Password=YourStrong@Password123;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
  },
  "Azure": {
    "StorageConnectionString": "UseDevelopmentStorage=true"
  }
}
```

## Migration Testing Procedures

### 1. Create Migration Testing Script

Create `scripts/test-migrations.ps1`:

```powershell
# Migration Testing Script for MeAndMyDoggyV2

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Testing",
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/API/MeAndMyDog.API",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBackup = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$TestRollback = $false
)

$ErrorActionPreference = "Stop"

Write-Host "=== MeAndMyDoggyV2 Migration Testing ===" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Project Path: $ProjectPath" -ForegroundColor Yellow

# Function to execute SQL command
function Execute-SqlCommand {
    param(
        [string]$ConnectionString,
        [string]$Query
    )
    
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $ConnectionString
    
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $Query
        $command.ExecuteNonQuery()
    }
    finally {
        $connection.Close()
    }
}

# Function to backup database
function Backup-Database {
    param(
        [string]$ConnectionString,
        [string]$DatabaseName
    )
    
    $backupPath = "$PSScriptRoot\backups\$DatabaseName`_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
    $backupDir = Split-Path $backupPath -Parent
    
    if (!(Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
    }
    
    $query = "BACKUP DATABASE [$DatabaseName] TO DISK = '$backupPath' WITH FORMAT, INIT"
    
    Write-Host "Backing up database to: $backupPath" -ForegroundColor Yellow
    Execute-SqlCommand -ConnectionString $ConnectionString -Query $query
    
    return $backupPath
}

# Step 1: Backup current test database
if (!$SkipBackup) {
    Write-Host "`nStep 1: Backing up test database..." -ForegroundColor Green
    $connString = "Server=localhost;Database=master;User ID=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
    $backupPath = Backup-Database -ConnectionString $connString -DatabaseName "MeAndMyDoggy_Test"
}

# Step 2: List current migrations
Write-Host "`nStep 2: Listing current migrations..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    dotnet ef migrations list --no-build
}
finally {
    Pop-Location
}

# Step 3: Generate migration script
Write-Host "`nStep 3: Generating migration script..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $scriptPath = "$PSScriptRoot\migration-scripts\pending-migrations_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"
    $scriptDir = Split-Path $scriptPath -Parent
    
    if (!(Test-Path $scriptDir)) {
        New-Item -ItemType Directory -Path $scriptDir | Out-Null
    }
    
    dotnet ef migrations script --idempotent --output $scriptPath --no-build
    Write-Host "Migration script generated at: $scriptPath" -ForegroundColor Yellow
}
finally {
    Pop-Location
}

# Step 4: Apply migrations to test database
Write-Host "`nStep 4: Applying migrations to test database..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $env:ASPNETCORE_ENVIRONMENT = $Environment
    dotnet ef database update --no-build
}
finally {
    Pop-Location
}

# Step 5: Verify migration success
Write-Host "`nStep 5: Verifying migration success..." -ForegroundColor Green
$verifyQuery = @"
SELECT 
    MigrationId,
    ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId DESC
"@

$connString = "Server=localhost;Database=MeAndMyDoggy_Test;User ID=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
Execute-SqlCommand -ConnectionString $connString -Query $verifyQuery

# Step 6: Test rollback if requested
if ($TestRollback) {
    Write-Host "`nStep 6: Testing rollback..." -ForegroundColor Green
    
    # Get the previous migration name
    Push-Location $ProjectPath
    try {
        $migrations = dotnet ef migrations list --no-build | Where-Object { $_ -match '^\s*\d+_' }
        if ($migrations.Count -gt 1) {
            $previousMigration = $migrations[-2] -replace '^\s*', ''
            $previousMigration = $previousMigration.Split(' ')[0]
            
            Write-Host "Rolling back to: $previousMigration" -ForegroundColor Yellow
            dotnet ef database update $previousMigration --no-build
            
            Write-Host "Rollback completed successfully!" -ForegroundColor Green
        }
        else {
            Write-Host "No previous migration to rollback to." -ForegroundColor Yellow
        }
    }
    finally {
        Pop-Location
    }
}

Write-Host "`n=== Migration testing completed successfully! ===" -ForegroundColor Cyan
```

### 2. Create Data Integrity Test Script

Create `scripts/test-data-integrity.sql`:

```sql
-- Data Integrity Test Script for MeAndMyDoggyV2
-- Run this before and after migrations to ensure data consistency

DECLARE @Results TABLE (
    TestName NVARCHAR(100),
    Status NVARCHAR(20),
    Details NVARCHAR(MAX)
);

-- Test 1: User count and basic integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'User Count and Integrity',
    CASE WHEN COUNT(*) > 0 THEN 'PASS' ELSE 'FAIL' END,
    'Total Users: ' + CAST(COUNT(*) AS NVARCHAR) + 
    ', Active Users: ' + CAST(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS NVARCHAR)
FROM AspNetUsers;

-- Test 2: Identity relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Identity Relationships',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned UserRoles: ' + CAST(COUNT(*) AS NVARCHAR)
FROM AspNetUserRoles ur
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = ur.UserId)
   OR NOT EXISTS (SELECT 1 FROM AspNetRoles r WHERE r.Id = ur.RoleId);

-- Test 3: DogProfiles integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'DogProfile Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned DogProfiles: ' + CAST(COUNT(*) AS NVARCHAR)
FROM DogProfiles dp
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = dp.OwnerId);

-- Test 4: ServiceProvider relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'ServiceProvider Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'ServiceProviders without Users: ' + CAST(COUNT(*) AS NVARCHAR)
FROM ServiceProviders sp
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = sp.UserId);

-- Test 5: Service catalog relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Service Catalog Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned SubServices: ' + CAST(COUNT(*) AS NVARCHAR)
FROM SubServices ss
WHERE NOT EXISTS (SELECT 1 FROM ServiceCategories sc WHERE sc.ServiceCategoryId = ss.ServiceCategoryId);

-- Test 6: Booking relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Booking Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Bookings with invalid references: ' + CAST(COUNT(*) AS NVARCHAR)
FROM Bookings b
WHERE NOT EXISTS (SELECT 1 FROM ServiceProviders sp WHERE sp.ServiceProviderId = b.ServiceProviderId)
   OR NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = b.CustomerId);

-- Test 7: Message/Conversation integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Messaging Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Messages without valid conversations: ' + CAST(COUNT(*) AS NVARCHAR)
FROM Messages m
WHERE NOT EXISTS (SELECT 1 FROM Conversations c WHERE c.ConversationId = m.ConversationId);

-- Display results
SELECT 
    TestName,
    Status,
    Details,
    CASE 
        WHEN Status = 'PASS' THEN '✓'
        ELSE '✗'
    END AS Result
FROM @Results
ORDER BY 
    CASE WHEN Status = 'FAIL' THEN 0 ELSE 1 END,
    TestName;

-- Summary
DECLARE @PassCount INT = (SELECT COUNT(*) FROM @Results WHERE Status = 'PASS');
DECLARE @TotalCount INT = (SELECT COUNT(*) FROM @Results);

SELECT 
    '=== DATA INTEGRITY TEST SUMMARY ===' AS Summary,
    CAST(@PassCount AS NVARCHAR) + ' / ' + CAST(@TotalCount AS NVARCHAR) + ' tests passed' AS Result,
    CASE 
        WHEN @PassCount = @TotalCount THEN 'ALL TESTS PASSED'
        ELSE 'SOME TESTS FAILED - REVIEW DETAILS ABOVE'
    END AS Status;
```

### 3. Performance Testing Script

Create `scripts/test-performance.sql`:

```sql
-- Performance Testing Script for MeAndMyDoggyV2
-- Captures baseline metrics before and after migrations

-- Clear cache for consistent testing
DBCC DROPCLEANBUFFERS;
DBCC FREEPROCCACHE;

-- Enable statistics
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

PRINT '=== PERFORMANCE TEST SUITE ==='
PRINT ''

-- Test 1: User authentication query
PRINT '--- Test 1: User Authentication Query ---'
DECLARE @StartTime DATETIME = GETDATE();

SELECT TOP 100
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.UserType,
    u.IsKYCVerified,
    u.SubscriptionType
FROM AspNetUsers u
WHERE u.IsActive = 1
ORDER BY u.CreatedAt DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 2: Service provider search with location
PRINT '--- Test 2: Service Provider Location Search ---'
SET @StartTime = GETDATE();

SELECT TOP 50
    sp.ServiceProviderId,
    sp.BusinessName,
    u.FirstName,
    u.LastName,
    sp.Rating,
    pl.Postcode,
    pl.City,
    pl.Latitude,
    pl.Longitude
FROM ServiceProviders sp
INNER JOIN AspNetUsers u ON sp.UserId = u.Id
LEFT JOIN ProviderLocations pl ON sp.ServiceProviderId = pl.ServiceProviderId AND pl.IsPrimary = 1
WHERE sp.IsActive = 1 AND u.IsActive = 1
ORDER BY sp.Rating DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 3: Complex service catalog query
PRINT '--- Test 3: Service Catalog with Pricing ---'
SET @StartTime = GETDATE();

SELECT 
    sc.Name AS CategoryName,
    COUNT(DISTINCT ss.SubServiceId) AS SubServiceCount,
    COUNT(DISTINCT b.BookingId) AS TotalBookings,
    AVG(b.TotalPrice) AS AvgBookingPrice
FROM ServiceCategories sc
LEFT JOIN SubServices ss ON sc.ServiceCategoryId = ss.ServiceCategoryId
LEFT JOIN Bookings b ON sc.ServiceCategoryId = b.ServiceCategoryId
WHERE sc.IsActive = 1
GROUP BY sc.ServiceCategoryId, sc.Name, sc.DisplayOrder
ORDER BY sc.DisplayOrder;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 4: Message/Conversation query
PRINT '--- Test 4: Recent Conversations with Messages ---'
SET @StartTime = GETDATE();

SELECT TOP 20
    c.ConversationId,
    c.ConversationType,
    c.LastMessageAt,
    COUNT(DISTINCT m.MessageId) AS MessageCount,
    COUNT(DISTINCT cp.UserId) AS ParticipantCount
FROM Conversations c
INNER JOIN Messages m ON c.ConversationId = m.ConversationId
INNER JOIN ConversationParticipants cp ON c.ConversationId = cp.ConversationId
WHERE c.IsActive = 1
GROUP BY c.ConversationId, c.ConversationType, c.LastMessageAt
ORDER BY c.LastMessageAt DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 5: Appointment scheduling query
PRINT '--- Test 5: Appointment Availability Check ---'
SET @StartTime = GETDATE();

SELECT TOP 100
    a.AppointmentId,
    a.ServiceType,
    a.StartTime,
    a.EndTime,
    sp.BusinessName,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    dp.Name AS DogName
FROM Appointments a
INNER JOIN ServiceProviders sp ON a.ServiceProviderId = sp.ServiceProviderId
INNER JOIN AspNetUsers u ON a.PetOwnerId = u.Id
LEFT JOIN DogProfiles dp ON a.DogId = dp.DogId
WHERE a.StartTime >= DATEADD(DAY, -30, GETDATE())
  AND a.Status = 'Scheduled'
ORDER BY a.StartTime;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Analyze index usage
PRINT '=== INDEX USAGE ANALYSIS ==='
SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    CAST(s.last_user_seek AS DATE) AS LastSeek,
    CAST(s.last_user_scan AS DATE) AS LastScan
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE s.database_id = DB_ID()
  AND OBJECT_NAME(s.object_id) IN (
    'AspNetUsers', 'ServiceProviders', 'DogProfiles', 
    'Appointments', 'Messages', 'Conversations',
    'ServiceCategories', 'Bookings', 'ProviderLocations'
  )
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
```

## Rollback Testing Procedures

### 1. Create Rollback Test Script

Create `scripts/test-rollback.ps1`:

```powershell
# Rollback Testing Script for MeAndMyDoggyV2

param(
    [Parameter(Mandatory=$true)]
    [string]$TargetMigration,
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/API/MeAndMyDog.API"
)

Write-Host "=== Testing Rollback to Migration: $TargetMigration ===" -ForegroundColor Cyan

# Step 1: Create database snapshot
Write-Host "`nStep 1: Creating database snapshot..." -ForegroundColor Green
$snapshotQuery = @"
CREATE DATABASE MeAndMyDoggy_Test_Snapshot ON
(NAME = 'MeAndMyDoggy_Test', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\MeAndMyDoggy_Test_Snapshot.ss')
AS SNAPSHOT OF MeAndMyDoggy_Test;
"@

try {
    Invoke-Sqlcmd -ServerInstance "localhost" -Database "master" -Query $snapshotQuery
    Write-Host "Snapshot created successfully" -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not create snapshot. Proceeding without snapshot." -ForegroundColor Yellow
}

# Step 2: Run data integrity tests before rollback
Write-Host "`nStep 2: Running pre-rollback data integrity tests..." -ForegroundColor Green
Invoke-Sqlcmd -ServerInstance "localhost" -Database "MeAndMyDoggy_Test" -InputFile "$PSScriptRoot\test-data-integrity.sql"

# Step 3: Perform rollback
Write-Host "`nStep 3: Rolling back to migration: $TargetMigration..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $env:ASPNETCORE_ENVIRONMENT = "Testing"
    dotnet ef database update $TargetMigration --no-build
}
finally {
    Pop-Location
}

# Step 4: Run data integrity tests after rollback
Write-Host "`nStep 4: Running post-rollback data integrity tests..." -ForegroundColor Green
Invoke-Sqlcmd -ServerInstance "localhost" -Database "MeAndMyDoggy_Test" -InputFile "$PSScriptRoot\test-data-integrity.sql"

# Step 5: Test application functionality
Write-Host "`nStep 5: Testing application functionality..." -ForegroundColor Green
Write-Host "Please run the application and verify:"
Write-Host "  - User authentication works"
Write-Host "  - Service provider search works"
Write-Host "  - Booking creation works"
Write-Host "  - Messaging works"

Write-Host "`n=== Rollback testing completed ===" -ForegroundColor Cyan
```

### 2. Create Automated Integration Tests

Create `tests/MeAndMyDog.API.MigrationTests/MigrationIntegrationTests.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeAndMyDog.API.MigrationTests
{
    [TestFixture]
    public class MigrationIntegrationTests
    {
        private ApplicationDbContext _context;
        private string _connectionString;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Testing.json")
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [Test]
        public async Task Migration_ShouldMaintainUserData()
        {
            // Arrange
            var userCountBefore = await _context.Users.CountAsync();

            // Act - This assumes migration has been applied
            await _context.Database.EnsureCreatedAsync();

            // Assert
            var userCountAfter = await _context.Users.CountAsync();
            Assert.That(userCountAfter, Is.GreaterThanOrEqualTo(userCountBefore));
        }

        [Test]
        public async Task Migration_ShouldMaintainServiceProviderRelationships()
        {
            // Arrange & Act
            var serviceProviders = await _context.ServiceProviders
                .Include(sp => sp.User)
                .Include(sp => sp.Services)
                .Include(sp => sp.Reviews)
                .Take(10)
                .ToListAsync();

            // Assert
            Assert.That(serviceProviders, Is.Not.Empty);
            foreach (var sp in serviceProviders)
            {
                Assert.That(sp.User, Is.Not.Null);
                Assert.That(sp.UserId, Is.Not.EqualTo(Guid.Empty));
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainDogProfileIntegrity()
        {
            // Arrange & Act
            var dogProfiles = await _context.DogProfiles
                .Include(dp => dp.Owner)
                .Include(dp => dp.MedicalRecords)
                .Take(10)
                .ToListAsync();

            // Assert
            foreach (var dog in dogProfiles)
            {
                Assert.That(dog.Owner, Is.Not.Null);
                Assert.That(dog.OwnerId, Is.Not.Empty);
                Assert.That(dog.Name, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainBookingRelationships()
        {
            // Arrange & Act
            var bookings = await _context.Bookings
                .Include(b => b.ServiceProvider)
                .Include(b => b.Customer)
                .Include(b => b.Dog)
                .Where(b => b.Status == "Scheduled")
                .Take(10)
                .ToListAsync();

            // Assert
            foreach (var booking in bookings)
            {
                Assert.That(booking.ServiceProvider, Is.Not.Null);
                Assert.That(booking.Customer, Is.Not.Null);
                Assert.That(booking.BookingReference, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainMessagingIntegrity()
        {
            // Arrange & Act
            var conversations = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                .Where(c => c.IsActive)
                .Take(5)
                .ToListAsync();

            // Assert
            foreach (var conversation in conversations)
            {
                Assert.That(conversation.Participants, Is.Not.Empty);
                Assert.That(conversation.CreatedBy, Is.Not.Empty);
            }
        }

        [Test]
        [TestCase("AspNetUsers")]
        [TestCase("ServiceProviders")]
        [TestCase("DogProfiles")]
        [TestCase("Bookings")]
        [TestCase("Messages")]
        public async Task Migration_ShouldMaintainTableIndexes(string tableName)
        {
            // Arrange
            var query = @"
                SELECT i.name AS IndexName
                FROM sys.indexes i
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                WHERE t.name = @p0 AND i.type > 0";

            // Act
            var indexes = await _context.Database
                .SqlQueryRaw<string>(query, tableName)
                .ToListAsync();

            // Assert
            Assert.That(indexes, Is.Not.Empty, $"Table {tableName} should have indexes");
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }

    // Helper class for raw SQL queries
    public class IndexInfo
    {
        public string IndexName { get; set; }
    }
}
```

## Migration Testing Checklist

### Pre-Migration Checklist
- [ ] Backup production database
- [ ] Backup test database
- [ ] Document current schema version
- [ ] Run data integrity tests
- [ ] Capture performance baselines
- [ ] Review migration scripts
- [ ] Test in isolated environment first

### Migration Execution Checklist
- [ ] Apply migrations to test database
- [ ] Verify all tables created/modified correctly
- [ ] Check foreign key constraints
- [ ] Verify indexes are present
- [ ] Run data integrity tests
- [ ] Execute performance tests
- [ ] Test application functionality

### Post-Migration Checklist
- [ ] Compare performance metrics
- [ ] Verify no data loss
- [ ] Test all CRUD operations
- [ ] Verify authentication works
- [ ] Check real-time features (SignalR)
- [ ] Document any issues
- [ ] Update migration log

### Rollback Testing Checklist
- [ ] Create database snapshot
- [ ] Test rollback procedure
- [ ] Verify data integrity after rollback
- [ ] Test application with rolled-back schema
- [ ] Document rollback time
- [ ] Verify no orphaned data
- [ ] Update rollback procedures

## Troubleshooting Common Issues

### Issue: Connection Timeout
```sql
-- Increase connection timeout in connection string
Server=localhost;Database=MeAndMyDoggy_Test;...;Connection Timeout=60;
```

### Issue: Migration Fails Due to Existing Data
```csharp
// In migration Up() method, handle existing data
migrationBuilder.Sql(@"
    IF EXISTS (SELECT 1 FROM [table_name] WHERE [condition])
    BEGIN
        -- Handle existing data
    END
");
```

### Issue: Index Already Exists
```csharp
// Check before creating index
migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_IndexName')
    BEGIN
        CREATE INDEX IX_IndexName ON TableName (ColumnName);
    END
");
```

## Best Practices

1. **Always test in isolation first**
   - Use a separate test database
   - Never test directly on production

2. **Document everything**
   - Keep migration logs
   - Document any manual steps
   - Track performance metrics

3. **Use idempotent scripts**
   - Migrations should be re-runnable
   - Check for existence before creating

4. **Test rollbacks**
   - Every migration should have a rollback plan
   - Test the rollback procedure

5. **Monitor after deployment**
   - Watch performance metrics
   - Monitor error logs
   - Be ready to rollback if needed

## Conclusion

This comprehensive setup provides a robust framework for testing database migrations in the MeAndMyDoggyV2 project. By following these procedures, you can ensure that migrations are applied safely and can be rolled back if necessary, maintaining data integrity throughout the process.