-- MeAndMyDog System Roles Insert Script
-- This script inserts the required roles for the MeAndMyDog application
-- Run this script after the initial database migration

USE [MeAndMyDog]
GO

-- Insert system roles into Roles table
-- These roles are used throughout the application for authorization

-- Check if roles already exist to avoid duplicates
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
    PRINT 'Admin role created successfully'
END
ELSE
    PRINT 'Admin role already exists'

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'PetOwner')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'PetOwner', 'PETOWNER', NEWID())
    PRINT 'PetOwner role created successfully'
END
ELSE
    PRINT 'PetOwner role already exists'

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ServiceProvider')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'ServiceProvider', 'SERVICEPROVIDER', NEWID())
    PRINT 'ServiceProvider role created successfully'
END
ELSE
    PRINT 'ServiceProvider role already exists'

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
BEGIN
    INSERT INTO Roles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'User', 'USER', NEWID())
    PRINT 'User role created successfully'
END
ELSE
    PRINT 'User role already exists'

-- Optional: Create a super admin user (uncomment if needed)
-- Note: This should only be used for initial setup and removed in production
/*
DECLARE @AdminUserId NVARCHAR(450) = NEWID()
DECLARE @AdminRoleId NVARCHAR(450)

-- Get Admin role ID
SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Admin'

-- Create admin user (replace with actual admin details)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@meandmydog.com')
BEGIN
    INSERT INTO Users (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount
    )
    VALUES (
        @AdminUserId, 'admin@meandmydog.com', 'ADMIN@MEANDMYDOG.COM',
        'admin@meandmydog.com', 'ADMIN@MEANDMYDOG.COM',
        1, 'AQAAAAEAACcQAAAAEJ4...[HASH_PLACEHOLDER]', NEWID(), NEWID(),
        0, 0, 1, 0
    )
    
    -- Assign admin role to admin user
    INSERT INTO UserRoles (UserId, RoleId)
    VALUES (@AdminUserId, @AdminRoleId)
    
    PRINT 'Admin user created and assigned Admin role'
END
*/

-- Verify roles were created
SELECT 
    Name as RoleName,
    NormalizedName,
    Id as RoleId
FROM Roles 
WHERE Name IN ('Admin', 'PetOwner', 'ServiceProvider', 'User')
ORDER BY Name

PRINT 'System roles setup completed successfully'
GO