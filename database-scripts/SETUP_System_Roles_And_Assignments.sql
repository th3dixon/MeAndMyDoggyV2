-- =============================================
-- SYSTEM ROLES SETUP AND USER ASSIGNMENTS
-- =============================================
-- Creates system roles and assigns them to test users
-- This script should be run after the seed data script
-- =============================================

USE [MeAndMyDoggyV2]; -- Change to your database name
GO

DECLARE @TimestampNow DATETIMEOFFSET = SYSDATETIMEOFFSET();

PRINT 'Setting up system roles and user assignments...';
PRINT '';

-- =============================================
-- 1. CREATE SYSTEM ROLES
-- =============================================
PRINT 'Creating system roles...';

-- Create Admin Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, Description, IsSystemRole, CreatedAt, UpdatedAt, ConcurrencyStamp)
    VALUES (
        NEWID(), 
        'Admin', 
        'ADMIN', 
        'Administrative users with full system access', 
        1, 
        @TimestampNow, 
        @TimestampNow,
        NEWID()
    );
    PRINT '✓ Created Admin role';
END
ELSE
BEGIN
    PRINT '- Admin role already exists';
END

-- Create User Role (Base role for all users)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, Description, IsSystemRole, CreatedAt, UpdatedAt, ConcurrencyStamp)
    VALUES (
        NEWID(), 
        'User', 
        'USER', 
        'Base role for all registered users', 
        1, 
        @TimestampNow, 
        @TimestampNow,
        NEWID()
    );
    PRINT '✓ Created User role';
END
ELSE
BEGIN
    PRINT '- User role already exists';
END

-- Create PetOwner Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'PetOwner')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, Description, IsSystemRole, CreatedAt, UpdatedAt, ConcurrencyStamp)
    VALUES (
        NEWID(), 
        'PetOwner', 
        'PETOWNER', 
        'Users who own pets and seek services', 
        1, 
        @TimestampNow, 
        @TimestampNow,
        NEWID()
    );
    PRINT '✓ Created PetOwner role';
END
ELSE
BEGIN
    PRINT '- PetOwner role already exists';
END

-- Create ServiceProvider Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ServiceProvider')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, Description, IsSystemRole, CreatedAt, UpdatedAt, ConcurrencyStamp)
    VALUES (
        NEWID(), 
        'ServiceProvider', 
        'SERVICEPROVIDER', 
        'Users who provide pet services', 
        1, 
        @TimestampNow, 
        @TimestampNow,
        NEWID()
    );
    PRINT '✓ Created ServiceProvider role';
END
ELSE
BEGIN
    PRINT '- ServiceProvider role already exists';
END

PRINT '';

-- =============================================
-- 2. GET ROLE IDs FOR ASSIGNMENTS
-- =============================================
DECLARE @AdminRoleId NVARCHAR(450) = (SELECT Id FROM Roles WHERE Name = 'Admin');
DECLARE @UserRoleId NVARCHAR(450) = (SELECT Id FROM Roles WHERE Name = 'User');
DECLARE @PetOwnerRoleId NVARCHAR(450) = (SELECT Id FROM Roles WHERE Name = 'PetOwner');
DECLARE @ServiceProviderRoleId NVARCHAR(450) = (SELECT Id FROM Roles WHERE Name = 'ServiceProvider');

-- =============================================
-- 3. ASSIGN ROLES TO TEST USERS
-- =============================================
PRINT 'Assigning roles to test users...';

-- Helper table for tracking assignments
DECLARE @RoleAssignments TABLE (
    UserId NVARCHAR(450),
    RoleId NVARCHAR(450),
    RoleName NVARCHAR(50)
);

-- =============================================
-- 3.1 ASSIGN ADMIN ROLE TO ONE TEST USER (for administrative testing)
-- =============================================
-- Make the first premium service provider also an admin for testing
INSERT INTO @RoleAssignments (UserId, RoleId, RoleName)
SELECT TOP 1 'TESTUSER_prov_001', @AdminRoleId, 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = 'TESTUSER_prov_001' AND RoleId = @AdminRoleId);

-- =============================================
-- 3.2 ASSIGN USER ROLE TO ALL TEST USERS (Base role)
-- =============================================
INSERT INTO @RoleAssignments (UserId, RoleId, RoleName)
SELECT u.Id, @UserRoleId, 'User'
FROM Users u 
WHERE u.Id LIKE 'TESTUSER_%' 
  AND NOT EXISTS (SELECT 1 FROM UserRoles ur WHERE ur.UserId = u.Id AND ur.RoleId = @UserRoleId);

-- =============================================
-- 3.3 ASSIGN PETOWNER ROLE
-- =============================================
-- Pet Owners (UserType = 1) and Mixed Users (UserType = 3)
INSERT INTO @RoleAssignments (UserId, RoleId, RoleName)
SELECT u.Id, @PetOwnerRoleId, 'PetOwner'
FROM Users u 
WHERE u.Id LIKE 'TESTUSER_%' 
  AND u.UserType IN (1, 3) -- PetOwner or Both
  AND NOT EXISTS (SELECT 1 FROM UserRoles ur WHERE ur.UserId = u.Id AND ur.RoleId = @PetOwnerRoleId);

-- =============================================
-- 3.4 ASSIGN SERVICEPROVIDER ROLE
-- =============================================
-- Service Providers (UserType = 2) and Mixed Users (UserType = 3)
INSERT INTO @RoleAssignments (UserId, RoleId, RoleName)
SELECT u.Id, @ServiceProviderRoleId, 'ServiceProvider'
FROM Users u 
WHERE u.Id LIKE 'TESTUSER_%' 
  AND u.UserType IN (2, 3) -- ServiceProvider or Both
  AND NOT EXISTS (SELECT 1 FROM UserRoles ur WHERE ur.UserId = u.Id AND ur.RoleId = @ServiceProviderRoleId);

-- =============================================
-- 4. EXECUTE ROLE ASSIGNMENTS
-- =============================================
PRINT 'Executing role assignments...';

-- Insert all role assignments
INSERT INTO UserRoles (UserId, RoleId)
SELECT DISTINCT UserId, RoleId 
FROM @RoleAssignments;

-- =============================================
-- 5. SUMMARY AND VERIFICATION
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT 'ROLE ASSIGNMENT SUMMARY';
PRINT '=============================================';

-- Count assignments by role
PRINT 'Role assignment counts:';
SELECT 
    r.Name as RoleName,
    COUNT(ur.UserId) as UserCount
FROM Roles r
LEFT JOIN UserRoles ur ON r.Id = ur.RoleId
LEFT JOIN Users u ON ur.UserId = u.Id
WHERE r.IsSystemRole = 1 
  AND (u.Id LIKE 'TESTUSER_%' OR u.Id IS NULL)
GROUP BY r.Name, r.Id
ORDER BY r.Name;

PRINT '';
PRINT 'Test user role assignments:';

-- Show detailed assignments for test users
SELECT 
    u.Id as UserId,
    u.FirstName + ' ' + u.LastName as FullName,
    u.Email,
    CASE u.UserType 
        WHEN 1 THEN 'PetOwner'
        WHEN 2 THEN 'ServiceProvider' 
        WHEN 3 THEN 'Both'
        ELSE 'Unknown'
    END as UserType,
    u.SubscriptionType,
    STRING_AGG(r.Name, ', ') as AssignedRoles
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Id LIKE 'TESTUSER_%'
GROUP BY u.Id, u.FirstName, u.LastName, u.Email, u.UserType, u.SubscriptionType
ORDER BY u.UserType, u.Id;

PRINT '';
PRINT 'Admin users (for testing administrative functions):';
SELECT 
    u.FirstName + ' ' + u.LastName as AdminUser,
    u.Email,
    u.SubscriptionType
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId  
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE r.Name = 'Admin' AND u.Id LIKE 'TESTUSER_%';

PRINT '';
PRINT '=============================================';
PRINT 'ROLE SETUP COMPLETED SUCCESSFULLY';
PRINT '=============================================';
PRINT 'Summary:';
PRINT '- System roles created (Admin, User, PetOwner, ServiceProvider)';
PRINT '- All test users assigned base User role';
PRINT '- Pet owners assigned PetOwner role';
PRINT '- Service providers assigned ServiceProvider role';  
PRINT '- Mixed users assigned both PetOwner and ServiceProvider roles';
PRINT '- One test user assigned Admin role for administrative testing';
PRINT '';
PRINT 'Login Credentials for Admin Testing:';
PRINT 'Email: sarah.thompson@testdoggy.com';
PRINT 'Password: TestPassword123!';
PRINT 'Roles: Admin, User, ServiceProvider';
PRINT '';
PRINT 'IMPORTANT: These are TEST users only - delete before production!';
PRINT '=============================================';

GO