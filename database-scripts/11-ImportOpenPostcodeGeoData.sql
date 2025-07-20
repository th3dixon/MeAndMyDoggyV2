-- =============================================
-- MeAndMyDoggy - Import Open Postcode Geo Data
-- =============================================
-- This script imports UK postcode data from Open Postcode Geo CSV format
-- Download from: https://www.getthedata.com/open-postcode-geo
-- 
-- Expected CSV columns: postcode, status, usertype, easting, northing, 
-- positional_quality_indicator, country, latitude, longitude, 
-- postcode_no_space, postcode_fixed_width_seven, postcode_fixed_width_eight, 
-- postcode_area, postcode_district, postcode_sector, outcode, incode

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

-- IMPORTANT: Run this BULK INSERT with the correct file path
-- Replace 'C:\Data\OpenPostcodeGeo.csv' with your actual file path
/*
BULK INSERT [dbo].[PostcodeImportStaging]
FROM 'C:\Data\OpenPostcodeGeo.csv'
WITH (
    FIRSTROW = 2, -- Skip header row
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    TABLOCK,
    FORMAT = 'CSV'
);
*/

-- Alternative: If using SQL Server Import Wizard or SSIS, import to PostcodeImportStaging table

-- After importing, process the data into our schema
BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Processing imported postcode data...';
    
    -- 1. Ensure UK exists in Countries table
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Countries] WHERE CountryCode = 'GB')
    BEGIN
        INSERT INTO [dbo].[Countries] ([CountryCode], [CountryName])
        VALUES ('GB', 'United Kingdom');
    END
    
    DECLARE @UK_ID int = (SELECT CountryId FROM Countries WHERE CountryCode = 'GB');
    
    -- 2. Insert postcode areas if they don't exist
    INSERT INTO [dbo].[PostcodeAreas] ([PostcodeArea], [AreaName], [Region])
    SELECT DISTINCT 
        LEFT(postcode_area, 2),
        CASE LEFT(postcode_area, 2)
            WHEN 'AB' THEN 'Aberdeen'
            WHEN 'AL' THEN 'St Albans'
            WHEN 'B' THEN 'Birmingham'
            WHEN 'BA' THEN 'Bath'
            WHEN 'BB' THEN 'Blackburn'
            WHEN 'BD' THEN 'Bradford'
            WHEN 'BH' THEN 'Bournemouth'
            WHEN 'BL' THEN 'Bolton'
            WHEN 'BN' THEN 'Brighton'
            WHEN 'BR' THEN 'Bromley'
            WHEN 'BS' THEN 'Bristol'
            WHEN 'BT' THEN 'Belfast'
            WHEN 'CA' THEN 'Carlisle'
            WHEN 'CB' THEN 'Cambridge'
            WHEN 'CF' THEN 'Cardiff'
            WHEN 'CH' THEN 'Chester'
            WHEN 'CM' THEN 'Chelmsford'
            WHEN 'CO' THEN 'Colchester'
            WHEN 'CR' THEN 'Croydon'
            WHEN 'CT' THEN 'Canterbury'
            WHEN 'CV' THEN 'Coventry'
            WHEN 'CW' THEN 'Crewe'
            WHEN 'DA' THEN 'Dartford'
            WHEN 'DD' THEN 'Dundee'
            WHEN 'DE' THEN 'Derby'
            WHEN 'DG' THEN 'Dumfries'
            WHEN 'DH' THEN 'Durham'
            WHEN 'DL' THEN 'Darlington'
            WHEN 'DN' THEN 'Doncaster'
            WHEN 'DT' THEN 'Dorchester'
            WHEN 'DY' THEN 'Dudley'
            WHEN 'E' THEN 'East London'
            WHEN 'EC' THEN 'East Central London'
            WHEN 'EH' THEN 'Edinburgh'
            WHEN 'EN' THEN 'Enfield'
            WHEN 'EX' THEN 'Exeter'
            WHEN 'FK' THEN 'Falkirk'
            WHEN 'FY' THEN 'Blackpool'
            WHEN 'G' THEN 'Glasgow'
            WHEN 'GL' THEN 'Gloucester'
            WHEN 'GU' THEN 'Guildford'
            WHEN 'HA' THEN 'Harrow'
            WHEN 'HD' THEN 'Huddersfield'
            WHEN 'HG' THEN 'Harrogate'
            WHEN 'HP' THEN 'Hemel Hempstead'
            WHEN 'HR' THEN 'Hereford'
            WHEN 'HS' THEN 'Hebrides'
            WHEN 'HU' THEN 'Hull'
            WHEN 'HX' THEN 'Halifax'
            WHEN 'IG' THEN 'Ilford'
            WHEN 'IP' THEN 'Ipswich'
            WHEN 'IV' THEN 'Inverness'
            WHEN 'KA' THEN 'Kilmarnock'
            WHEN 'KT' THEN 'Kingston upon Thames'
            WHEN 'KW' THEN 'Kirkwall'
            WHEN 'KY' THEN 'Kirkcaldy'
            WHEN 'L' THEN 'Liverpool'
            WHEN 'LA' THEN 'Lancaster'
            WHEN 'LD' THEN 'Llandrindod Wells'
            WHEN 'LE' THEN 'Leicester'
            WHEN 'LL' THEN 'Llandudno'
            WHEN 'LN' THEN 'Lincoln'
            WHEN 'LS' THEN 'Leeds'
            WHEN 'LU' THEN 'Luton'
            WHEN 'M' THEN 'Manchester'
            WHEN 'ME' THEN 'Rochester'
            WHEN 'MK' THEN 'Milton Keynes'
            WHEN 'ML' THEN 'Motherwell'
            WHEN 'N' THEN 'North London'
            WHEN 'NE' THEN 'Newcastle upon Tyne'
            WHEN 'NG' THEN 'Nottingham'
            WHEN 'NN' THEN 'Northampton'
            WHEN 'NP' THEN 'Newport'
            WHEN 'NR' THEN 'Norwich'
            WHEN 'NW' THEN 'North West London'
            WHEN 'OL' THEN 'Oldham'
            WHEN 'OX' THEN 'Oxford'
            WHEN 'PA' THEN 'Paisley'
            WHEN 'PE' THEN 'Peterborough'
            WHEN 'PH' THEN 'Perth'
            WHEN 'PL' THEN 'Plymouth'
            WHEN 'PO' THEN 'Portsmouth'
            WHEN 'PR' THEN 'Preston'
            WHEN 'RG' THEN 'Reading'
            WHEN 'RH' THEN 'Redhill'
            WHEN 'RM' THEN 'Romford'
            WHEN 'S' THEN 'Sheffield'
            WHEN 'SA' THEN 'Swansea'
            WHEN 'SE' THEN 'South East London'
            WHEN 'SG' THEN 'Stevenage'
            WHEN 'SK' THEN 'Stockport'
            WHEN 'SL' THEN 'Slough'
            WHEN 'SM' THEN 'Sutton'
            WHEN 'SN' THEN 'Swindon'
            WHEN 'SO' THEN 'Southampton'
            WHEN 'SP' THEN 'Salisbury'
            WHEN 'SR' THEN 'Sunderland'
            WHEN 'SS' THEN 'Southend-on-Sea'
            WHEN 'ST' THEN 'Stoke-on-Trent'
            WHEN 'SW' THEN 'South West London'
            WHEN 'SY' THEN 'Shrewsbury'
            WHEN 'TA' THEN 'Taunton'
            WHEN 'TD' THEN 'Galashiels'
            WHEN 'TF' THEN 'Telford'
            WHEN 'TN' THEN 'Tunbridge Wells'
            WHEN 'TQ' THEN 'Torquay'
            WHEN 'TR' THEN 'Truro'
            WHEN 'TS' THEN 'Cleveland'
            WHEN 'TW' THEN 'Twickenham'
            WHEN 'UB' THEN 'Southall'
            WHEN 'W' THEN 'West London'
            WHEN 'WA' THEN 'Warrington'
            WHEN 'WC' THEN 'West Central London'
            WHEN 'WD' THEN 'Watford'
            WHEN 'WF' THEN 'Wakefield'
            WHEN 'WN' THEN 'Wigan'
            WHEN 'WR' THEN 'Worcester'
            WHEN 'WS' THEN 'Walsall'
            WHEN 'WV' THEN 'Wolverhampton'
            WHEN 'YO' THEN 'York'
            WHEN 'ZE' THEN 'Shetland'
            ELSE LEFT(postcode_area, 2) + ' Area'
        END,
        CASE 
            WHEN LEFT(postcode_area, 2) IN ('E', 'EC', 'N', 'NW', 'SE', 'SW', 'W', 'WC') THEN 'London'
            WHEN LEFT(postcode_area, 2) IN ('AB', 'DD', 'DG', 'EH', 'FK', 'G', 'HS', 'IV', 'KA', 'KW', 'KY', 'ML', 'PA', 'PH', 'TD', 'ZE') THEN 'Scotland'
            WHEN LEFT(postcode_area, 2) IN ('CF', 'LD', 'LL', 'NP', 'SA', 'SY') THEN 'Wales'
            WHEN LEFT(postcode_area, 2) IN ('BT') THEN 'Northern Ireland'
            WHEN LEFT(postcode_area, 2) IN ('B', 'CV', 'DY', 'ST', 'WS', 'WV') THEN 'West Midlands'
            WHEN LEFT(postcode_area, 2) IN ('M', 'OL', 'SK', 'WA', 'WN') THEN 'North West'
            WHEN LEFT(postcode_area, 2) IN ('BD', 'HD', 'HG', 'HX', 'LS', 'WF', 'YO') THEN 'Yorkshire'
            WHEN LEFT(postcode_area, 2) IN ('DN', 'HU', 'LN', 'NG', 'S') THEN 'Yorkshire and the Humber'
            WHEN LEFT(postcode_area, 2) IN ('DE', 'LE', 'NN') THEN 'East Midlands'
            WHEN LEFT(postcode_area, 2) IN ('AL', 'CB', 'CM', 'CO', 'EN', 'IG', 'IP', 'LU', 'NR', 'PE', 'RM', 'SG', 'SS') THEN 'East of England'
            WHEN LEFT(postcode_area, 2) IN ('BN', 'BR', 'CR', 'CT', 'DA', 'GU', 'HA', 'KT', 'ME', 'MK', 'OX', 'RG', 'RH', 'SL', 'SM', 'TN', 'TW', 'UB', 'WD') THEN 'South East'
            WHEN LEFT(postcode_area, 2) IN ('BA', 'BH', 'BS', 'DT', 'EX', 'GL', 'PL', 'PO', 'SN', 'SO', 'SP', 'TA', 'TQ', 'TR') THEN 'South West'
            WHEN LEFT(postcode_area, 2) IN ('CA', 'DH', 'DL', 'NE', 'SR', 'TS') THEN 'North East'
            ELSE 'Other'
        END
    FROM [dbo].[PostcodeImportStaging]
    WHERE postcode_area IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM [dbo].[PostcodeAreas] pa 
            WHERE pa.PostcodeArea = LEFT(postcode_area, 2)
        );
    
    -- 3. Update postcode area center coordinates
    UPDATE pa
    SET CenterLatitude = coords.AvgLat,
        CenterLongitude = coords.AvgLon
    FROM [dbo].[PostcodeAreas] pa
    INNER JOIN (
        SELECT 
            LEFT(postcode_area, 2) AS Area,
            AVG(latitude) AS AvgLat,
            AVG(longitude) AS AvgLon
        FROM [dbo].[PostcodeImportStaging]
        WHERE latitude IS NOT NULL AND longitude IS NOT NULL
        GROUP BY LEFT(postcode_area, 2)
    ) coords ON pa.PostcodeArea = coords.Area;
    
    -- 4. Insert postcodes (batch process for performance)
    DECLARE @BatchSize INT = 10000;
    DECLARE @Offset INT = 0;
    DECLARE @TotalRows INT = (SELECT COUNT(*) FROM [dbo].[PostcodeImportStaging] WHERE status = 'live');
    
    WHILE @Offset < @TotalRows
    BEGIN
        INSERT INTO [dbo].[Postcodes] 
            ([Postcode], [PostcodeFormatted], [OutwardCode], [InwardCode], 
             [PostcodeArea], [PostcodeDistrict], [PostcodeSector], 
             [Latitude], [Longitude], [Easting], [Northing], [IsActive])
        SELECT 
            postcode_no_space,
            postcode,
            outcode,
            incode,
            LEFT(postcode_area, 2),
            postcode_district,
            postcode_sector,
            latitude,
            longitude,
            easting,
            northing,
            CASE WHEN status = 'live' THEN 1 ELSE 0 END
        FROM [dbo].[PostcodeImportStaging]
        WHERE status = 'live'
            AND NOT EXISTS (
                SELECT 1 FROM [dbo].[Postcodes] p 
                WHERE p.Postcode = postcode_no_space
            )
        ORDER BY postcode_no_space
        OFFSET @Offset ROWS
        FETCH NEXT @BatchSize ROWS ONLY;
        
        SET @Offset = @Offset + @BatchSize;
        
        -- Log progress
        IF @Offset % 50000 = 0
            PRINT 'Processed ' + CAST(@Offset AS VARCHAR(10)) + ' postcodes...';
    END;
    
    -- 5. Insert postcode sectors
    INSERT INTO [dbo].[PostcodeSectors] 
        ([PostcodeSector], [PostcodeDistrict], [PostcodeArea], [SectorName])
    SELECT DISTINCT 
        p.PostcodeSector,
        p.PostcodeDistrict,
        p.PostcodeArea,
        p.PostcodeDistrict + ' Sector ' + RIGHT(p.PostcodeSector, 1)
    FROM [dbo].[Postcodes] p
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[PostcodeSectors] ps 
        WHERE ps.PostcodeSector = p.PostcodeSector
    );
    
    -- 6. Update sector center coordinates
    UPDATE ps
    SET CenterLatitude = coords.AvgLat,
        CenterLongitude = coords.AvgLon
    FROM [dbo].[PostcodeSectors] ps
    INNER JOIN (
        SELECT 
            PostcodeSector,
            AVG(Latitude) AS AvgLat,
            AVG(Longitude) AS AvgLon
        FROM [dbo].[Postcodes]
        WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL
        GROUP BY PostcodeSector
    ) coords ON ps.PostcodeSector = coords.PostcodeSector;
    
    COMMIT TRANSACTION;
    
    -- Clean up staging table
    DROP TABLE [dbo].[PostcodeImportStaging];
    
    -- Update statistics
    UPDATE STATISTICS [dbo].[Postcodes];
    UPDATE STATISTICS [dbo].[PostcodeAreas];
    UPDATE STATISTICS [dbo].[PostcodeSectors];
    
    -- Report results
    DECLARE @PostcodeCount INT = (SELECT COUNT(*) FROM [dbo].[Postcodes]);
    DECLARE @AreaCount INT = (SELECT COUNT(*) FROM [dbo].[PostcodeAreas]);
    DECLARE @SectorCount INT = (SELECT COUNT(*) FROM [dbo].[PostcodeSectors]);
    
    PRINT '';
    PRINT '=== Import Complete ===';
    PRINT 'Total Postcodes: ' + CAST(@PostcodeCount AS VARCHAR(10));
    PRINT 'Total Areas: ' + CAST(@AreaCount AS VARCHAR(10));
    PRINT 'Total Sectors: ' + CAST(@SectorCount AS VARCHAR(10));
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Error occurred during import:';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
GO

-- Create helper script for CSV import using PowerShell
PRINT '';
PRINT '-- PowerShell script to download and prepare Open Postcode Geo data:';
PRINT '-- Save this as Download-PostcodeData.ps1 and run in PowerShell';
PRINT '';
PRINT '$dataUrl = "https://www.getthedata.com/downloads/open_postcode_geo.csv.zip"';
PRINT '$downloadPath = "C:\Temp\open_postcode_geo.csv.zip"';
PRINT '$extractPath = "C:\Temp\"';
PRINT '$csvPath = "C:\Temp\open_postcode_geo.csv"';
PRINT '';
PRINT '# Download the file';
PRINT 'Invoke-WebRequest -Uri $dataUrl -OutFile $downloadPath';
PRINT '';
PRINT '# Extract the ZIP file';
PRINT 'Expand-Archive -Path $downloadPath -DestinationPath $extractPath -Force';
PRINT '';
PRINT '# Import to SQL Server using BCP';
PRINT '$connectionString = "Server=localhost;Database=MeAndMyDoggy;Trusted_Connection=true;"';
PRINT '$bcpCommand = "bcp PostcodeImportStaging in `"$csvPath`" -S localhost -d MeAndMyDoggy -T -c -t,"';
PRINT 'Invoke-Expression $bcpCommand';
PRINT '';
PRINT 'Write-Host "Data download and staging complete. Run the SQL import script to process the data."';