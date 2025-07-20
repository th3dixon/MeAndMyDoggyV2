-- MeAndMyDog Assign ServiceProvider Role to Existing Users
-- This script assigns the ServiceProvider role to all existing users
-- Run this script after the system roles have been created

USE [MeAndMyDog]
GO

-- Declare variables
DECLARE @ServiceProviderRoleId NVARCHAR(450)
DECLARE @UsersAssigned INT = 0
DECLARE @UsersSkipped INT = 0

-- Get ServiceProvider role ID
SELECT @ServiceProviderRoleId = Id 
FROM Roles 
WHERE Name = 'ServiceProvider'

-- Check if ServiceProvider role exists
IF @ServiceProviderRoleId IS NULL
BEGIN
    PRINT 'ERROR: ServiceProvider role not found. Please run the insert-system-roles.sql script first.'
    RETURN
END

PRINT 'ServiceProvider Role ID: ' + @ServiceProviderRoleId
PRINT 'Starting role assignment process...'
PRINT ''

-- Insert ServiceProvider role for all users who don't already have it
INSERT INTO UserRoles (UserId, RoleId)
SELECT 
    u.Id as UserId,
    @ServiceProviderRoleId as RoleId
FROM Users u
WHERE u.Id NOT IN (
    SELECT ur.UserId 
    FROM UserRoles ur 
    WHERE ur.RoleId = @ServiceProviderRoleId
)

-- Get count of users assigned
SET @UsersAssigned = @@ROWCOUNT

-- Get count of users who already had the role
SELECT @UsersSkipped = COUNT(*)
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
WHERE ur.RoleId = @ServiceProviderRoleId

-- Also assign base User role to all users if they don't have it
DECLARE @UserRoleId NVARCHAR(450)
SELECT @UserRoleId = Id FROM Roles WHERE Name = 'User'

IF @UserRoleId IS NOT NULL
BEGIN
    INSERT INTO UserRoles (UserId, RoleId)
    SELECT 
        u.Id as UserId,
        @UserRoleId as RoleId
    FROM Users u
    WHERE u.Id NOT IN (
        SELECT ur.UserId 
        FROM UserRoles ur 
        WHERE ur.RoleId = @UserRoleId
    )
    
    PRINT 'Base User role also assigned where needed'
END

-- Display results
PRINT 'Role assignment completed:'
PRINT '- Users newly assigned ServiceProvider role: ' + CAST(@UsersAssigned AS VARCHAR(10))
PRINT '- Users who already had ServiceProvider role: ' + CAST(@UsersSkipped AS VARCHAR(10))
PRINT '- Total users with ServiceProvider role: ' + CAST((@UsersAssigned + @UsersSkipped) AS VARCHAR(10))
PRINT ''

-- Verification query - show all users with their roles
SELECT 
    u.UserName,
    u.Email,
    r.Name as RoleName,
    ur.UserId,
    ur.RoleId
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE r.Name IN ('ServiceProvider', 'User', 'Admin', 'PetOwner')
ORDER BY u.UserName, r.Name

-- Summary count by role
PRINT ''
PRINT 'Role assignment summary:'
SELECT 
    r.Name as RoleName,
    COUNT(ur.UserId) as UserCount
FROM Roles r
LEFT JOIN UserRoles ur ON r.Id = ur.RoleId
WHERE r.Name IN ('Admin', 'PetOwner', 'ServiceProvider', 'User')
GROUP BY r.Name, r.Id
ORDER BY r.Name

PRINT ''
PRINT 'ServiceProvider role assignment completed successfully!'
GO