-- =============================================
-- Create Postcode Import Staging Table
-- =============================================
-- Run this before importing CSV data

-- Create staging table for bulk import
IF OBJECT_ID('dbo.PostcodeImportStaging', 'U') IS NOT NULL 
    DROP TABLE [dbo].[PostcodeImportStaging];

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

PRINT 'Staging table created successfully.';
PRINT 'Now you can import the CSV data using:';
PRINT '  1. SQL Server Import Wizard';
PRINT '  2. BULK INSERT command';
PRINT '  3. BCP utility';
PRINT '';
PRINT 'After importing data to this staging table, run the 11-ImportOpenPostcodeGeoData.sql script.';