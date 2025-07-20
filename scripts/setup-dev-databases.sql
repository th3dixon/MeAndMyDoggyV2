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