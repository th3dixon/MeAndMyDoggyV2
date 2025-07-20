-- =============================================
-- Bulk Import Postcodes using Format File
-- =============================================

-- Make sure the staging table exists first
IF OBJECT_ID('dbo.PostcodeImportStaging', 'U') IS NULL
BEGIN
    PRINT 'Creating staging table...';
    
    CREATE TABLE [dbo].[PostcodeImportStaging] (
        [postcode] nvarchar(10),
        [status] nvarchar(20),
        [usertype] nvarchar(20),
        [easting] int,
        [northing] int,
        [positional_quality_indicator] int,
        [country] nvarchar(50),
        [latitude] decimal(10,7),
        [longitude] decimal(10,7),
        [postcode_no_space] nvarchar(10),
        [postcode_fixed_width_seven] nvarchar(7),
        [postcode_fixed_width_eight] nvarchar(8),
        [postcode_area] nvarchar(4),
        [postcode_district] nvarchar(6),
        [postcode_sector] nvarchar(8),
        [outcode] nvarchar(4),
        [incode] nvarchar(3)
    );
END

-- Clear any existing data
TRUNCATE TABLE [dbo].[PostcodeImportStaging];

-- Attempt bulk insert with format file
BEGIN TRY
    BULK INSERT [dbo].[PostcodeImportStaging]
    FROM 'C:\Temp\open_postcode_geo.csv'
    WITH (
        FORMATFILE = 'C:\kirorepo\MeAndMyDoggyV2\MeAndMyDoggyV2\database-scripts\PostcodeImport.fmt',
        FIRSTROW = 2,
        ERRORFILE = 'C:\Temp\PostcodeImportErrors.log',
        MAXERRORS = 100,
        TABLOCK
    );
    
    PRINT 'Bulk insert completed successfully!';
    
    DECLARE @Count INT = (SELECT COUNT(*) FROM [dbo].[PostcodeImportStaging]);
    PRINT 'Total records imported: ' + CAST(@Count AS VARCHAR(20));
    
END TRY
BEGIN CATCH
    PRINT 'Error during bulk insert:';
    PRINT ERROR_MESSAGE();
    PRINT '';
    PRINT 'Alternative: Run the PowerShell script Convert-PostcodeCSVToSQL.ps1 to generate INSERT statements';
END CATCH;
GO