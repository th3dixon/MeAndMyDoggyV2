-- =============================================
-- Fix sp_RefreshAddressCache to work with available data
-- Uses populated tables: Postcodes, PostcodeAreas, PostcodeSectors, Cities
-- Skips empty tables: Addresses, Streets
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_RefreshAddressCache]
    @MaxRecords INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Drop indexed view that prevents truncation
        IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.vw_CommonAddressSearches') AND name = 'IX_vw_CommonAddressSearches')
            DROP INDEX [IX_vw_CommonAddressSearches] ON [dbo].[vw_CommonAddressSearches];
        
        IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_CommonAddressSearches'))
            DROP VIEW [dbo].[vw_CommonAddressSearches];
        
        -- Clear existing cache
        TRUNCATE TABLE [dbo].[AddressLookupCache];
        
        -- Strategy 1: Pure postcode entries with area information
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT TOP (COALESCE(@MaxRecords, 500000))
            LOWER(REPLACE(p.PostcodeFormatted, ' ', '')) AS SearchKey,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName + ', ' + p.PostcodeFormatted
                ELSE p.PostcodeFormatted + ' Area'
            END AS DisplayText,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName
                ELSE p.PostcodeArea + ' Area'
            END AS AddressLine1,
            NULL AS AddressLine2,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName
                ELSE p.PostcodeArea + ' Area'
            END AS City,
            ISNULL(pa.Region, 'Unknown Region') AS County,
            p.PostcodeFormatted,
            p.PostcodeId,
            NULL AS AddressId, -- No specific address
            p.Latitude,
            p.Longitude,
            60 AS SearchRank -- Medium priority for postcode areas
        FROM [dbo].[Postcodes] p
        LEFT JOIN [dbo].[PostcodeAreas] pa ON p.PostcodeArea = pa.PostcodeArea
        WHERE p.IsActive = 1;
        
        -- Strategy 2: Add major cities as searchable entries (higher priority)
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT DISTINCT TOP (COALESCE(@MaxRecords, 100000))
            LOWER(REPLACE(c.CityName, ' ', '')) AS SearchKey,
            c.CityName + ', ' + co.CountyName AS DisplayText,
            c.CityName AS AddressLine1,
            NULL AS AddressLine2,
            c.CityName AS City,
            co.CountyName AS County,
            NULL AS PostcodeFormatted, -- No specific postcode
            NULL AS PostcodeId,
            NULL AS AddressId,
            c.Latitude,
            c.Longitude,
            CASE 
                WHEN c.CityName IN ('London', 'Birmingham', 'Manchester', 'Glasgow', 'Liverpool', 'Leeds', 'Sheffield', 'Edinburgh', 'Bristol', 'Cardiff') THEN 100
                WHEN c.CityName IN ('Leicester', 'Coventry', 'Bradford', 'Belfast', 'Nottingham', 'Plymouth', 'Southampton', 'Reading', 'Derby', 'Luton') THEN 90
                ELSE 80
            END AS SearchRank
        FROM [dbo].[Cities] c
        INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
        WHERE c.IsActive = 1;
        
        -- Strategy 3: Add postcode sectors for more granular search
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT DISTINCT TOP (COALESCE(@MaxRecords, 200000))
            LOWER(REPLACE(ps.PostcodeSector, ' ', '')) AS SearchKey,
            ps.PostcodeSector + ' Sector' + 
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN ', ' + pa.AreaName
                ELSE ''
            END AS DisplayText,
            ps.PostcodeSector + ' Sector' AS AddressLine1,
            NULL AS AddressLine2,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName
                ELSE ps.PostcodeArea + ' Area'
            END AS City,
            ISNULL(pa.Region, 'Unknown Region') AS County,
            ps.PostcodeSector AS PostcodeFormatted,
            NULL AS PostcodeId,
            NULL AS AddressId,
            ps.CenterLatitude AS Latitude,
            ps.CenterLongitude AS Longitude,
            50 AS SearchRank -- Lower priority for sectors
        FROM [dbo].[PostcodeSectors] ps
        LEFT JOIN [dbo].[PostcodeAreas] pa ON ps.PostcodeArea = pa.PostcodeArea
        WHERE ps.IsActive = 1 AND ps.CenterLatitude IS NOT NULL AND ps.CenterLongitude IS NOT NULL;
        
        -- Strategy 4: Add postcode areas as broad search entries
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT TOP (COALESCE(@MaxRecords, 50000))
            LOWER(REPLACE(pa.PostcodeArea, ' ', '')) AS SearchKey,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName + ' (' + pa.PostcodeArea + ')'
                ELSE pa.PostcodeArea + ' Area'
            END AS DisplayText,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName
                ELSE pa.PostcodeArea + ' Area'
            END AS AddressLine1,
            NULL AS AddressLine2,
            CASE 
                WHEN pa.AreaName IS NOT NULL AND pa.AreaName != ''
                THEN pa.AreaName
                ELSE pa.PostcodeArea + ' Area'
            END AS City,
            ISNULL(pa.Region, 'Unknown Region') AS County,
            pa.PostcodeArea AS PostcodeFormatted,
            NULL AS PostcodeId,
            NULL AS AddressId,
            pa.CenterLatitude AS Latitude,
            pa.CenterLongitude AS Longitude,
            70 AS SearchRank -- Medium-high priority for areas
        FROM [dbo].[PostcodeAreas] pa
        WHERE pa.CenterLatitude IS NOT NULL AND pa.CenterLongitude IS NOT NULL;
        
        -- Recreate the indexed view and index
        EXEC('CREATE VIEW [dbo].[vw_CommonAddressSearches]
        WITH SCHEMABINDING
        AS
        SELECT 
            alc.PostcodeFormatted,
            alc.City,
            alc.SearchRank,
            COUNT_BIG(*) AS EntryCount
        FROM [dbo].[AddressLookupCache] alc
        GROUP BY alc.PostcodeFormatted, alc.City, alc.SearchRank');
        
        CREATE UNIQUE CLUSTERED INDEX [IX_vw_CommonAddressSearches] 
        ON [dbo].[vw_CommonAddressSearches] ([PostcodeFormatted], [City], [SearchRank]);
        
        COMMIT TRANSACTION;
        
        -- Update statistics
        UPDATE STATISTICS [dbo].[AddressLookupCache];
        UPDATE STATISTICS [dbo].[vw_CommonAddressSearches];
        
        -- Return summary of cache population
        SELECT 
            COUNT(*) AS TotalCacheRecords,
            COUNT(CASE WHEN SearchRank = 100 THEN 1 END) AS MajorCities,
            COUNT(CASE WHEN SearchRank = 90 THEN 1 END) AS SecondaryCities,
            COUNT(CASE WHEN SearchRank = 80 THEN 1 END) AS OtherCities,
            COUNT(CASE WHEN SearchRank = 70 THEN 1 END) AS PostcodeAreas,
            COUNT(CASE WHEN SearchRank = 60 THEN 1 END) AS Postcodes,
            COUNT(CASE WHEN SearchRank = 50 THEN 1 END) AS PostcodeSectors
        FROM [dbo].[AddressLookupCache];
        
        PRINT 'AddressLookupCache populated successfully using available geographic data';
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Show error details
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState,
            ERROR_MESSAGE() AS ErrorMessage;
        
        THROW;
    END CATCH
END;
GO

-- Execute the updated procedure
PRINT 'Executing sp_RefreshAddressCache to populate cache with available data...';
EXEC sp_RefreshAddressCache;
GO

-- Test the cache with the example postcode
PRINT 'Testing address autocomplete with PO13 0JX...';
EXEC sp_AddressAutocomplete @SearchTerm = 'PO13 0JX', @MaxResults = 10;
GO

-- Test with partial searches
PRINT 'Testing address autocomplete with PO13...';
EXEC sp_AddressAutocomplete @SearchTerm = 'PO13', @MaxResults = 10;
GO

PRINT 'Testing address autocomplete with London...';
EXEC sp_AddressAutocomplete @SearchTerm = 'London', @MaxResults = 10;
GO

PRINT 'Address lookup cache fix completed successfully!';