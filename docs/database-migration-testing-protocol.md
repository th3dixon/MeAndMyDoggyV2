# Database Migration Testing Protocol - User Dashboard Enhancement

## Overview

This document outlines the comprehensive testing protocol for database migrations related to the User Dashboard enhancement project. The protocol ensures safe additive schema changes, validates performance of existing queries, provides rollback procedures, and verifies data integrity throughout the migration process.

## Project Context

### Existing Database Schema
The current system utilizes Entity Framework Core 9.0 with SQL Server (Azure SQL Database) and includes:

- **Core Identity**: ASP.NET Identity with ApplicationUser, IdentityRole<Guid>
- **Service Catalog**: ServiceCategory, SubService, ProviderService, ProviderServicePricing
- **User Management**: ApplicationUser with location data (Latitude/Longitude), PostCode indexing
- **Relationships**: Complex foreign key relationships between services and providers

### Dashboard Enhancement Scope
The migration will add dashboard-specific tables for:
- Widget configuration and preferences
- User behavior analytics and personalization data
- Dashboard layout customization settings
- Real-time notification preferences
- Health and wellness tracking data
- Financial tracking and expense categorization

## 1. Migration Testing Procedures for Additive Schema Changes

### 1.1 Pre-Migration Validation

#### Database State Verification
```sql
-- Verify current schema integrity
EXEC sp_helpdb 'MeAndMyDoggyDB'

-- Check existing table constraints
SELECT 
    TABLE_NAME,
    CONSTRAINT_NAME,
    CONSTRAINT_TYPE
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;

-- Verify foreign key relationships
SELECT 
    FK.TABLE_NAME AS ForeignKeyTable,
    CU.COLUMN_NAME AS ForeignKeyColumn,
    PK.TABLE_NAME AS PrimaryKeyTable,
    PT.COLUMN_NAME AS PrimaryKeyColumn,
    FK.CONSTRAINT_NAME AS ConstraintName
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE PT ON C.UNIQUE_CONSTRAINT_NAME = PT.CONSTRAINT_NAME;
```

#### Index Analysis
```sql
-- Document existing indexes for performance baseline
SELECT 
    i.name AS IndexName,
    t.name AS TableName,
    i.type_desc AS IndexType,
    i.is_unique,
    i.is_primary_key,
    STUFF((SELECT ', ' + c.name
           FROM sys.index_columns ic
           INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
           WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
           ORDER BY ic.key_ordinal
           FOR XML PATH('')), 1, 2, '') AS IndexColumns
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type > 0
ORDER BY t.name, i.name;
```

### 1.2 Migration Script Validation

#### Test Migration Scripts in Isolation
```bash
# Create test database copy for migration validation
sqlcmd -S localhost -Q "CREATE DATABASE MeAndMyDoggyDB_MigrationTest"
sqlcmd -S localhost -Q "RESTORE DATABASE MeAndMyDoggyDB_MigrationTest FROM DISK = 'backup_path' WITH REPLACE"

# Apply Entity Framework migrations to test database
dotnet ef database update --connection "connection_string_test" --project src/API/MeAndMyDog.API
```

#### Schema Validation Post-Migration
```sql
-- Verify new tables exist with correct structure
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN (
    'DashboardWidgets',
    'UserDashboardPreferences', 
    'DashboardAnalytics',
    'WidgetConfigurations',
    'UserBehaviorTracking'
)
ORDER BY TABLE_NAME, ORDINAL_POSITION;

-- Verify foreign key constraints are properly created
SELECT 
    FK.TABLE_NAME AS ForeignKeyTable,
    CU.COLUMN_NAME AS ForeignKeyColumn,
    PK.TABLE_NAME AS PrimaryKeyTable,
    PT.COLUMN_NAME AS PrimaryKeyColumn
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE PT ON C.UNIQUE_CONSTRAINT_NAME = PT.CONSTRAINT_NAME
WHERE FK.TABLE_NAME IN ('DashboardWidgets', 'UserDashboardPreferences', 'DashboardAnalytics');
```

### 1.3 Additive Change Verification

#### Test Backward Compatibility
```csharp
// Test existing Entity Framework queries still work
public async Task TestExistingUserQueries()
{
    using var context = new AppDbContext(options);
    
    // Test existing ApplicationUser queries
    var users = await context.Users
        .Include(u => u.ServiceProvider)
        .Where(u => u.IsActive)
        .ToListAsync();
    
    Assert.NotNull(users);
    Assert.True(users.Count > 0);
    
    // Test existing ServiceCategory queries
    var serviceCategories = await context.ServiceCategories
        .Include(sc => sc.SubServices)
        .Include(sc => sc.ProviderServices)
        .Where(sc => sc.IsActive)
        .OrderBy(sc => sc.DisplayOrder)
        .ToListAsync();
    
    Assert.NotNull(serviceCategories);
    
    // Test existing complex joins
    var providerServices = await context.ProviderServices
        .Include(ps => ps.ServiceCategory)
        .Include(ps => ps.Pricing)
        .ThenInclude(p => p.SubService)
        .Where(ps => ps.IsOffered)
        .ToListAsync();
    
    Assert.NotNull(providerServices);
}
```

## 2. Performance Validation for Existing Queries

### 2.1 Query Performance Baseline

#### Establish Performance Baselines
```sql
-- Enable query execution statistics
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Test critical existing queries and record baseline metrics
-- User authentication query
SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName, u.UserType
FROM AspNetUsers u
WHERE u.Email = @email AND u.IsActive = 1;

-- Service category listing
SELECT sc.ServiceCategoryId, sc.Name, sc.Description, sc.IconClass, sc.ColorCode
FROM ServiceCategories sc
WHERE sc.IsActive = 1
ORDER BY sc.DisplayOrder;

-- Provider service search
SELECT ps.ProviderServiceId, ps.ProviderId, sc.Name as ServiceName, 
       psp.Price, psp.WeekendSurchargePercentage
FROM ProviderServices ps
INNER JOIN ServiceCategories sc ON ps.ServiceCategoryId = sc.ServiceCategoryId
INNER JOIN ProviderServicePricing psp ON ps.ProviderServiceId = psp.ProviderServiceId
WHERE ps.IsOffered = 1 AND sc.IsActive = 1;

-- User location-based queries
SELECT u.Id, u.FirstName, u.LastName, u.Latitude, u.Longitude
FROM AspNetUsers u
WHERE u.PostCode LIKE @postCodePrefix + '%' AND u.IsActive = 1;
```

#### Record Baseline Metrics
```sql
-- Query execution plan analysis
SELECT 
    query_plan,
    execution_count,
    total_worker_time,
    total_physical_reads,
    total_logical_reads,
    total_elapsed_time
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
WHERE st.text LIKE '%ServiceCategories%' OR st.text LIKE '%AspNetUsers%';
```

### 2.2 Post-Migration Performance Testing

#### Re-run Baseline Queries
```sql
-- Clear plan cache for accurate testing
DBCC FREEPROCCACHE;
DBCC DROPCLEANBUFFERS;

-- Re-execute baseline queries and compare metrics
-- Use identical queries from baseline testing
-- Record execution times, logical reads, and plan costs
```

#### New Query Integration Testing
```sql
-- Test new dashboard queries don't impact existing performance
-- Dashboard widget data query
SELECT 
    u.Id,
    u.FirstName,
    u.LastName,
    udp.WidgetLayout,
    udp.NotificationPreferences,
    COUNT(DISTINCT ps.ProviderServiceId) as ServiceCount
FROM AspNetUsers u
LEFT JOIN UserDashboardPreferences udp ON u.Id = udp.UserId
LEFT JOIN ProviderServices ps ON u.Id = ps.ProviderId
WHERE u.Id = @userId
GROUP BY u.Id, u.FirstName, u.LastName, udp.WidgetLayout, udp.NotificationPreferences;

-- Performance validation: ensure new joins don't degrade existing queries
```

### 2.3 Index Performance Validation

#### Verify Index Usage
```sql
-- Check existing index usage patterns
SELECT 
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('AspNetUsers', 'ServiceCategories', 'ProviderServices', 'SubServices')
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;

-- Monitor for index fragmentation
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    ps.avg_fragmentation_in_percent,
    ps.page_count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ps
INNER JOIN sys.indexes i ON ps.object_id = i.object_id AND ps.index_id = i.index_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE ps.avg_fragmentation_in_percent > 10
ORDER BY ps.avg_fragmentation_in_percent DESC;
```

## 3. Rollback Validation Procedures

### 3.1 Rollback Strategy Preparation

#### Document Rollback Steps
```bash
# Create rollback migration scripts
dotnet ef migrations add DashboardEnhancement --project src/API/MeAndMyDog.API
dotnet ef script-migration PreviousMigration DashboardEnhancement --project src/API/MeAndMyDog.API --output rollback-script.sql

# Test rollback in isolated environment
dotnet ef database update PreviousMigration --connection "connection_string_test" --project src/API/MeAndMyDog.API
```

#### Test Rollback Scenarios
```sql
-- 1. Test dropping new tables without affecting existing data
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DashboardWidgets')
    DROP TABLE DashboardWidgets;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDashboardPreferences')
    DROP TABLE UserDashboardPreferences;

-- 2. Verify existing data integrity after rollback
SELECT COUNT(*) FROM AspNetUsers WHERE IsActive = 1;
SELECT COUNT(*) FROM ServiceCategories WHERE IsActive = 1;
SELECT COUNT(*) FROM ProviderServices WHERE IsOffered = 1;

-- 3. Test existing application functionality
```

### 3.2 Data Consistency During Rollback

#### Verify Foreign Key Integrity
```sql
-- Check for orphaned references (should be none for additive changes)
SELECT 'AspNetUsers' as TableName, COUNT(*) as OrphanedRecords
FROM AspNetUsers u
LEFT JOIN UserDashboardPreferences udp ON u.Id = udp.UserId
WHERE udp.UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM AspNetUsers u2 WHERE u2.Id = udp.UserId);

-- Verify no cascading deletes affect existing data
SELECT 
    FK.TABLE_NAME,
    FK.CONSTRAINT_NAME,
    RC.DELETE_RULE
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON RC.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
WHERE RC.DELETE_RULE != 'NO ACTION';
```

### 3.3 Application-Level Rollback Testing

#### Entity Framework Model Validation
```csharp
public async Task TestRollbackModelCompatibility()
{
    using var context = new AppDbContext(options);
    
    try
    {
        // Test that existing models still work after rollback
        var user = await context.Users.FirstOrDefaultAsync();
        var serviceCategory = await context.ServiceCategories.FirstOrDefaultAsync();
        var providerService = await context.ProviderServices.FirstOrDefaultAsync();
        
        // Verify navigation properties still work
        var userWithProvider = await context.Users
            .Include(u => u.ServiceProvider)
            .FirstOrDefaultAsync();
            
        Assert.NotNull(userWithProvider);
    }
    catch (Exception ex)
    {
        // Log rollback compatibility issues
        Assert.Fail($"Rollback compatibility test failed: {ex.Message}");
    }
}
```

## 4. Data Integrity Verification Steps

### 4.1 Referential Integrity Validation

#### Foreign Key Constraint Testing
```sql
-- Test all foreign key relationships maintain integrity
EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

-- Verify specific dashboard-related foreign keys
SELECT 
    FK_Table = FK.TABLE_NAME,
    FK_Column = CU.COLUMN_NAME,
    PK_Table = PK.TABLE_NAME,
    PK_Column = PT.COLUMN_NAME,
    Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE PT ON C.UNIQUE_CONSTRAINT_NAME = PT.CONSTRAINT_NAME
WHERE FK.TABLE_NAME LIKE '%Dashboard%' OR PK.TABLE_NAME LIKE '%Dashboard%';
```

#### Data Type and Constraint Validation
```sql
-- Verify data types match Entity Framework expectations
SELECT 
    c.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.IS_NULLABLE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME IN (
    'DashboardWidgets',
    'UserDashboardPreferences',
    'DashboardAnalytics',
    'WidgetConfigurations'
)
ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION;

-- Check decimal precision matches AppDbContext configuration
SELECT 
    c.COLUMN_NAME,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'ProviderServicePricing'
AND c.COLUMN_NAME IN ('Price', 'WeekendSurchargePercentage', 'EveningSurchargePercentage');
```

### 4.2 Business Logic Integrity

#### User Role and Permission Validation
```sql
-- Verify user types and roles remain intact
SELECT 
    UserType,
    COUNT(*) as UserCount
FROM AspNetUsers
GROUP BY UserType;

-- Validate dashboard preferences align with user roles
SELECT 
    u.UserType,
    udp.WidgetLayout,
    COUNT(*) as Count
FROM AspNetUsers u
LEFT JOIN UserDashboardPreferences udp ON u.Id = udp.UserId
GROUP BY u.UserType, udp.WidgetLayout;
```

#### Service Catalog Integrity
```sql
-- Verify service relationships remain intact
SELECT 
    sc.Name as CategoryName,
    COUNT(DISTINCT ss.SubServiceId) as SubServiceCount,
    COUNT(DISTINCT ps.ProviderServiceId) as ProviderServiceCount
FROM ServiceCategories sc
LEFT JOIN SubServices ss ON sc.ServiceCategoryId = ss.ServiceCategoryId
LEFT JOIN ProviderServices ps ON sc.ServiceCategoryId = ps.ServiceCategoryId
WHERE sc.IsActive = 1
GROUP BY sc.ServiceCategoryId, sc.Name
ORDER BY sc.DisplayOrder;

-- Test pricing relationships
SELECT 
    COUNT(*) as TotalPricingRecords,
    COUNT(DISTINCT ProviderServiceId) as UniqueProviderServices,
    COUNT(DISTINCT SubServiceId) as UniqueSubServices
FROM ProviderServicePricing;
```

### 4.3 Location Data Integrity

#### Geographic Data Validation
```sql
-- Verify latitude/longitude precision is maintained
SELECT 
    COUNT(*) as TotalUsers,
    COUNT(Latitude) as UsersWithLatitude,
    COUNT(Longitude) as UsersWithLongitude,
    AVG(CAST(Latitude as FLOAT)) as AvgLatitude,
    AVG(CAST(Longitude as FLOAT)) as AvgLongitude
FROM AspNetUsers
WHERE IsActive = 1;

-- Validate PostCode index effectiveness
SELECT 
    PostCode,
    COUNT(*) as UserCount
FROM AspNetUsers
WHERE PostCode IS NOT NULL
GROUP BY PostCode
ORDER BY COUNT(*) DESC;
```

## 5. Dashboard-Specific Schema Testing

### 5.1 Widget Configuration Testing

#### Widget Table Structure Validation
```sql
-- Test widget configuration schema
CREATE TABLE #TestWidgetConfig (
    WidgetConfigId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    WidgetType NVARCHAR(50) NOT NULL,
    Configuration NVARCHAR(MAX), -- JSON configuration
    Position INT NOT NULL,
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Test widget configuration operations
INSERT INTO #TestWidgetConfig (UserId, WidgetType, Configuration, Position)
SELECT TOP 5 
    Id,
    'HealthSummary',
    '{"showVaccinations": true, "showMedications": true}',
    1
FROM AspNetUsers
WHERE IsActive = 1;

-- Verify JSON configuration handling
SELECT 
    WidgetType,
    JSON_VALUE(Configuration, '$.showVaccinations') as ShowVaccinations,
    JSON_VALUE(Configuration, '$.showMedications') as ShowMedications
FROM #TestWidgetConfig;

DROP TABLE #TestWidgetConfig;
```

### 5.2 User Behavior Analytics Testing

#### Analytics Data Structure
```sql
-- Test user behavior tracking schema
CREATE TABLE #TestUserBehavior (
    BehaviorId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    ActionType NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50),
    EntityId UNIQUEIDENTIFIER,
    Metadata NVARCHAR(MAX), -- JSON metadata
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Test behavior tracking operations
INSERT INTO #TestUserBehavior (UserId, ActionType, EntityType, EntityId, Metadata)
SELECT TOP 10
    u.Id,
    'DashboardView',
    'Widget',
    NEWID(),
    '{"widgetType": "HealthSummary", "duration": 15000}'
FROM AspNetUsers u
WHERE u.IsActive = 1;

-- Test analytics queries
SELECT 
    ActionType,
    COUNT(*) as ActionCount,
    COUNT(DISTINCT UserId) as UniqueUsers
FROM #TestUserBehavior
GROUP BY ActionType;

DROP TABLE #TestUserBehavior;
```

### 5.3 Integration with Existing Entities

#### Test Dashboard-User Relationships
```sql
-- Verify dashboard preferences integrate with user profiles
SELECT 
    u.Id,
    u.UserType,
    u.FirstName,
    u.LastName,
    u.PostCode,
    COUNT(ps.ProviderServiceId) as ServiceCount
FROM AspNetUsers u
LEFT JOIN ProviderServices ps ON u.Id = ps.ProviderId
WHERE u.IsActive = 1
GROUP BY u.Id, u.UserType, u.FirstName, u.LastName, u.PostCode
ORDER BY ServiceCount DESC;

-- Test dashboard widget relevance to user services
WITH UserServices AS (
    SELECT 
        u.Id as UserId,
        sc.Name as ServiceName,
        COUNT(*) as ServiceCount
    FROM AspNetUsers u
    INNER JOIN ProviderServices ps ON u.Id = ps.ProviderId
    INNER JOIN ServiceCategories sc ON ps.ServiceCategoryId = sc.ServiceCategoryId
    WHERE u.IsActive = 1 AND ps.IsOffered = 1
    GROUP BY u.Id, sc.Name
)
SELECT 
    ServiceName,
    COUNT(DISTINCT UserId) as ProvidersOffering,
    AVG(ServiceCount) as AvgServicesPerProvider
FROM UserServices
GROUP BY ServiceName
ORDER BY ProvidersOffering DESC;
```

## 6. Testing Execution Framework

### 6.1 Automated Test Suite

#### NUnit Test Framework Implementation
```csharp
[TestFixture]
public class DatabaseMigrationTests
{
    private AppDbContext _context;
    private string _testConnectionString;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_testConnectionString)
            .Options;
        _context = new AppDbContext(options);
    }

    [Test]
    public async Task Migration_ShouldPreserveExistingUserData()
    {
        // Arrange
        var userCountBefore = await _context.Users.CountAsync();
        
        // Act - Run migration
        await _context.Database.MigrateAsync();
        
        // Assert
        var userCountAfter = await _context.Users.CountAsync();
        Assert.AreEqual(userCountBefore, userCountAfter);
    }

    [Test]
    public async Task Migration_ShouldMaintainServiceCategoryRelationships()
    {
        // Arrange & Act
        var serviceCategories = await _context.ServiceCategories
            .Include(sc => sc.SubServices)
            .Include(sc => sc.ProviderServices)
            .ToListAsync();
        
        // Assert
        Assert.That(serviceCategories, Is.Not.Empty);
        Assert.That(serviceCategories.All(sc => sc.SubServices != null));
        Assert.That(serviceCategories.All(sc => sc.ProviderServices != null));
    }

    [Test]
    public async Task NewDashboardTables_ShouldHaveCorrectSchema()
    {
        // Test will be implemented based on actual dashboard entity models
        var dashboardWidgets = await _context.Set<DashboardWidget>().ToListAsync();
        Assert.That(dashboardWidgets, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}
```

### 6.2 Performance Benchmarking

#### Query Performance Tests
```csharp
[TestFixture]
public class MigrationPerformanceTests
{
    [Test]
    public async Task ExistingQueries_ShouldMaintainPerformance()
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var context = new AppDbContext(options);
        
        // Test critical user query performance
        var users = await context.Users
            .Where(u => u.IsActive)
            .Take(100)
            .ToListAsync();
            
        stopwatch.Stop();
        
        // Assert performance threshold (e.g., < 500ms)
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(500));
    }

    [Test]
    public async Task ServiceCategoryQuery_ShouldMaintainPerformance()
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var context = new AppDbContext(options);
        
        var serviceCategories = await context.ServiceCategories
            .Include(sc => sc.SubServices)
            .Where(sc => sc.IsActive)
            .OrderBy(sc => sc.DisplayOrder)
            .ToListAsync();
            
        stopwatch.Stop();
        
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000));
        Assert.That(serviceCategories, Is.Not.Empty);
    }
}
```

## 7. Production Deployment Protocol

### 7.1 Pre-Deployment Checklist

#### Environment Preparation
- [ ] Backup production database
- [ ] Verify test database migration success
- [ ] Confirm rollback procedures tested
- [ ] Validate Entity Framework migration scripts
- [ ] Check application configuration compatibility
- [ ] Verify Redis cache compatibility
- [ ] Test SignalR connection handling with new schema

#### Performance Validation
- [ ] Baseline performance metrics documented
- [ ] Load test results within acceptable parameters
- [ ] Index usage analysis completed
- [ ] Query execution plans optimized
- [ ] Memory usage impact assessed

### 7.2 Deployment Execution

#### Migration Deployment Steps
```bash
# 1. Application maintenance mode
# 2. Database backup
sqlcmd -S production_server -Q "BACKUP DATABASE MeAndMyDoggyDB TO DISK = 'backup_path'"

# 3. Run Entity Framework migrations
dotnet ef database update --connection "production_connection_string" --project src/API/MeAndMyDog.API

# 4. Verify migration success
dotnet ef migrations list --connection "production_connection_string" --project src/API/MeAndMyDog.API

# 5. Application smoke tests
# 6. Remove maintenance mode
```

### 7.3 Post-Deployment Validation

#### Immediate Verification
```sql
-- Verify new tables created successfully
SELECT name FROM sys.tables WHERE name LIKE '%Dashboard%';

-- Check foreign key constraints
SELECT name FROM sys.foreign_keys WHERE parent_object_id IN (
    SELECT object_id FROM sys.tables WHERE name LIKE '%Dashboard%'
);

-- Verify existing data integrity
SELECT COUNT(*) FROM AspNetUsers WHERE IsActive = 1;
SELECT COUNT(*) FROM ServiceCategories WHERE IsActive = 1;
```

#### Performance Monitoring
```sql
-- Monitor query performance post-deployment
SELECT 
    qs.execution_count,
    qs.total_worker_time / qs.execution_count AS avg_cpu_time,
    qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
    qs.total_logical_reads / qs.execution_count AS avg_logical_reads,
    SUBSTRING(qt.text, qs.statement_start_offset/2+1, 
              (CASE WHEN qs.statement_end_offset = -1 
                    THEN LEN(CONVERT(nvarchar(max), qt.text)) * 2 
                    ELSE qs.statement_end_offset END - qs.statement_start_offset)/2) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qt.text LIKE '%AspNetUsers%' OR qt.text LIKE '%ServiceCategories%'
ORDER BY qs.total_worker_time / qs.execution_count DESC;
```

## 8. Monitoring and Maintenance

### 8.1 Ongoing Performance Monitoring

#### Key Performance Indicators
- Query execution time for existing and new dashboard queries
- Database connection pool utilization
- Index fragmentation levels
- Foreign key constraint check performance
- SignalR connection handling with dashboard real-time updates

#### Automated Monitoring Queries
```sql
-- Daily performance check
CREATE PROCEDURE sp_DashboardPerformanceCheck
AS
BEGIN
    -- Monitor dashboard query performance
    SELECT 
        'Dashboard Query Performance' as CheckType,
        COUNT(*) as QueryCount,
        AVG(total_elapsed_time/execution_count) as AvgElapsedTime
    FROM sys.dm_exec_query_stats qs
    CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
    WHERE qt.text LIKE '%Dashboard%'
    
    -- Check index usage
    SELECT 
        'Index Usage' as CheckType,
        t.name as TableName,
        i.name as IndexName,
        s.user_seeks + s.user_scans + s.user_lookups as TotalUsage
    FROM sys.dm_db_index_usage_stats s
    INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
    INNER JOIN sys.tables t ON i.object_id = t.object_id
    WHERE t.name LIKE '%Dashboard%' OR t.name IN ('AspNetUsers', 'ServiceCategories')
    ORDER BY TotalUsage DESC;
END
```

### 8.2 Maintenance Procedures

#### Regular Maintenance Tasks
- Weekly index defragmentation analysis
- Monthly query performance review
- Quarterly database growth analysis
- Dashboard usage analytics review
- Foreign key performance validation

## Conclusion

This comprehensive migration testing protocol ensures the User Dashboard enhancement maintains system integrity, performance, and reliability while adding powerful new functionality. The protocol emphasizes additive schema changes, thorough testing at each stage, and robust rollback procedures to minimize risk during deployment.

The testing approach validates that existing functionality remains unaffected while new dashboard capabilities integrate seamlessly with the current ASP.NET Core Identity, service catalog, and real-time messaging infrastructure.

---

**Document Version**: 1.0  
**Last Updated**: 2025-01-18  
**Review Schedule**: Before each major dashboard enhancement release  
**Responsible Team**: Backend Development & Database Administration