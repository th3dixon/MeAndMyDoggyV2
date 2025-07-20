-- =============================================
-- Enable Full-Text Search for SQL Server
-- =============================================

-- Method 1: Check if Full-Text Search is installed
SELECT 
    SERVERPROPERTY('IsFullTextInstalled') AS IsFullTextInstalled,
    SERVERPROPERTY('Edition') AS SQLServerEdition,
    SERVERPROPERTY('ProductVersion') AS Version;

-- Method 2: Enable Full-Text on the database (if already installed)
-- This will fail if Full-Text Search is not installed
BEGIN TRY
    EXEC sp_fulltext_database 'enable';
    PRINT 'Full-Text Search enabled on current database';
END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT 'Full-Text Search may not be installed. Please install it using SQL Server Installation Center.';
END CATCH;

-- Method 3: List available Full-Text components
SELECT * FROM sys.fulltext_languages;
SELECT * FROM sys.fulltext_catalogs;

-- Instructions for installing Full-Text Search:
/*
If Full-Text Search is not installed, you need to:

1. For SQL Server (not Express):
   - Run SQL Server Installation Center
   - Click "New SQL Server stand-alone installation or add features"
   - Select your instance
   - In Feature Selection, check "Full-Text and Semantic Extractions for Search"
   - Complete the installation

2. For SQL Server Express:
   - Download SQL Server Express with Advanced Services
   - This version includes Full-Text Search
   - Run the installer and ensure Full-Text Search is selected

3. For Azure SQL Database:
   - Full-Text Search is automatically available
   - No installation needed

4. For SQL Server in Docker/Linux:
   - Use the Developer or Standard edition image
   - Full-Text Search should be included
*/