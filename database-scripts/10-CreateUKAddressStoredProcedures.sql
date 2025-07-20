-- =============================================
-- MeAndMyDoggy UK Address Lookup Stored Procedures
-- =============================================
-- Optimized stored procedures for address searching and autocomplete

-- 1. Postcode Lookup
CREATE OR ALTER PROCEDURE [dbo].[sp_LookupPostcode]
    @Postcode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Normalize the postcode (remove spaces, uppercase)
    DECLARE @NormalizedPostcode NVARCHAR(10) = UPPER(REPLACE(@Postcode, ' ', ''));
    
    SELECT 
        p.PostcodeId,
        p.PostcodeFormatted,
        p.Postcode,
        p.OutwardCode,
        p.InwardCode,
        p.PostcodeArea,
        p.PostcodeDistrict,
        p.PostcodeSector,
        p.Latitude,
        p.Longitude,
        pa.AreaName,
        pa.Region,
        (SELECT STRING_AGG(CityName, ', ') FROM (SELECT DISTINCT c2.CityName FROM [dbo].[Addresses] a2 INNER JOIN [dbo].[Cities] c2 ON a2.CityId = c2.CityId WHERE a2.PostcodeId = p.PostcodeId) AS DistinctCities) AS Cities,
        COUNT(DISTINCT a.AddressId) AS AddressCount
    FROM [dbo].[Postcodes] p
    LEFT JOIN [dbo].[PostcodeAreas] pa ON p.PostcodeArea = pa.PostcodeArea
    LEFT JOIN [dbo].[Addresses] a ON p.PostcodeId = a.PostcodeId
    LEFT JOIN [dbo].[Cities] c ON a.CityId = c.CityId
    WHERE p.Postcode = @NormalizedPostcode AND p.IsActive = 1
    GROUP BY 
        p.PostcodeId, p.PostcodeFormatted, p.Postcode, p.OutwardCode, p.InwardCode,
        p.PostcodeArea, p.PostcodeDistrict, p.PostcodeSector, p.Latitude, p.Longitude,
        pa.AreaName, pa.Region;
END;
GO

-- 2. Address Autocomplete Search
CREATE OR ALTER PROCEDURE [dbo].[sp_AddressAutocomplete]
    @SearchTerm NVARCHAR(100),
    @MaxResults INT = 20,
    @IncludePostcodeOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Normalize search term
    DECLARE @NormalizedSearch NVARCHAR(100) = LOWER(REPLACE(@SearchTerm, ' ', ''));
    DECLARE @SearchPattern NVARCHAR(102) = @NormalizedSearch + '%';
    
    -- Check if it's a postcode pattern
    DECLARE @IsPostcodeSearch BIT = 0;
    IF @SearchTerm LIKE '[A-Z][A-Z0-9]%' OR @SearchTerm LIKE '[A-Z][A-Z0-9] %'
        SET @IsPostcodeSearch = 1;
    
    -- Search in cache first
    SELECT TOP (@MaxResults)
        CacheId,
        DisplayText,
        AddressLine1,
        AddressLine2,
        City,
        County,
        PostcodeFormatted,
        PostcodeId,
        AddressId,
        Latitude,
        Longitude,
        SearchRank,
        'cache' AS Source
    FROM [dbo].[AddressLookupCache]
    WHERE SearchKey LIKE @SearchPattern
        OR PostcodeFormatted LIKE @SearchTerm + '%'
        OR (@IsPostcodeSearch = 0 AND 
            (AddressLine1 LIKE '%' + @SearchTerm + '%' 
             OR City LIKE @SearchTerm + '%'))
    ORDER BY 
        CASE WHEN SearchKey = @NormalizedSearch THEN 1 ELSE 0 END DESC,
        SearchRank DESC,
        UseCount DESC;
    
    -- Update usage statistics for selected results
    UPDATE [dbo].[AddressLookupCache]
    SET UseCount = UseCount + 1,
        LastUsed = GETUTCDATE()
    WHERE SearchKey LIKE @SearchPattern;
END;
GO

-- 3. Full Address Search with Distance
CREATE OR ALTER PROCEDURE [dbo].[sp_SearchAddressesNearLocation]
    @Latitude DECIMAL(9,6),
    @Longitude DECIMAL(9,6),
    @RadiusMiles DECIMAL(10,2) = 5,
    @SearchTerm NVARCHAR(100) = NULL,
    @MaxResults INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Convert radius to kilometers (1 mile = 1.60934 km)
    DECLARE @RadiusKm DECIMAL(10,2) = @RadiusMiles * 1.60934;
    
    -- Earth's radius in kilometers
    DECLARE @EarthRadiusKm DECIMAL(10,2) = 6371;
    
    SELECT TOP (@MaxResults)
        a.AddressId,
        CONCAT(
            ISNULL(a.BuildingNumber + ' ', ''),
            ISNULL(a.BuildingName + ', ', ''),
            ISNULL(a.SubBuilding + ', ', ''),
            s.StreetName,
            ISNULL(' ' + s.StreetType, ''),
            ', ',
            c.CityName,
            ', ',
            co.CountyName,
            ', ',
            p.PostcodeFormatted
        ) AS FullAddress,
        a.BuildingNumber,
        a.BuildingName,
        a.SubBuilding,
        s.StreetName,
        s.StreetType,
        c.CityName,
        co.CountyName,
        p.PostcodeFormatted,
        p.Latitude,
        p.Longitude,
        -- Calculate distance using Haversine formula
        @EarthRadiusKm * 2 * ASIN(
            SQRT(
                POWER(SIN(RADIANS(@Latitude - p.Latitude) / 2), 2) +
                COS(RADIANS(@Latitude)) * COS(RADIANS(p.Latitude)) *
                POWER(SIN(RADIANS(@Longitude - p.Longitude) / 2), 2)
            )
        ) AS DistanceKm
    FROM [dbo].[Addresses] a
    INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId
    INNER JOIN [dbo].[Cities] c ON a.CityId = c.CityId
    INNER JOIN [dbo].[Counties] co ON a.CountyId = co.CountyId
    INNER JOIN [dbo].[Postcodes] p ON a.PostcodeId = p.PostcodeId
    WHERE a.IsActive = 1 
        AND p.IsActive = 1
        AND (@SearchTerm IS NULL OR 
            s.StreetName LIKE '%' + @SearchTerm + '%' OR
            c.CityName LIKE '%' + @SearchTerm + '%' OR
            p.PostcodeFormatted LIKE @SearchTerm + '%')
        -- Distance filter using Haversine
        AND @EarthRadiusKm * 2 * ASIN(
            SQRT(
                POWER(SIN(RADIANS(@Latitude - p.Latitude) / 2), 2) +
                COS(RADIANS(@Latitude)) * COS(RADIANS(p.Latitude)) *
                POWER(SIN(RADIANS(@Longitude - p.Longitude) / 2), 2)
            )
        ) <= @RadiusKm
    ORDER BY DistanceKm;
END;
GO

-- 4. Get Address by ID
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAddressById]
    @AddressId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.AddressId,
        a.BuildingNumber,
        a.BuildingName,
        a.SubBuilding,
        s.StreetId,
        s.StreetName,
        s.StreetType,
        c.CityId,
        c.CityName,
        c.CityType,
        co.CountyId,
        co.CountyName,
        ct.CountryId,
        ct.CountryName,
        ct.CountryCode,
        p.PostcodeId,
        p.PostcodeFormatted,
        p.Postcode,
        p.OutwardCode,
        p.InwardCode,
        p.PostcodeArea,
        p.PostcodeDistrict,
        p.PostcodeSector,
        p.Latitude,
        p.Longitude,
        a.UPRN,
        a.IsResidential,
        a.IsActive,
        CONCAT(
            ISNULL(a.BuildingNumber + ' ', ''),
            ISNULL(a.BuildingName + ', ', ''),
            ISNULL(a.SubBuilding + ', ', ''),
            s.StreetName,
            ISNULL(' ' + s.StreetType, ''),
            ', ',
            c.CityName,
            ', ',
            co.CountyName,
            ', ',
            p.PostcodeFormatted
        ) AS FullAddress
    FROM [dbo].[Addresses] a
    INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId
    INNER JOIN [dbo].[Cities] c ON a.CityId = c.CityId
    INNER JOIN [dbo].[Counties] co ON a.CountyId = co.CountyId
    INNER JOIN [dbo].[Countries] ct ON co.CountryId = ct.CountryId
    INNER JOIN [dbo].[Postcodes] p ON a.PostcodeId = p.PostcodeId
    WHERE a.AddressId = @AddressId;
END;
GO

-- 5. Postcode Area Search
CREATE OR ALTER PROCEDURE [dbo].[sp_SearchPostcodeAreas]
    @SearchTerm NVARCHAR(50),
    @MaxResults INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        pa.PostcodeArea,
        pa.AreaName,
        pa.Region,
        pa.CenterLatitude,
        pa.CenterLongitude,
        COUNT(DISTINCT p.PostcodeId) AS PostcodeCount,
        COUNT(DISTINCT c.CityId) AS CityCount
    FROM [dbo].[PostcodeAreas] pa
    LEFT JOIN [dbo].[Postcodes] p ON pa.PostcodeArea = p.PostcodeArea AND p.IsActive = 1
    LEFT JOIN [dbo].[Streets] s ON p.PostcodeId = s.PostcodeId
    LEFT JOIN [dbo].[Cities] c ON s.CityId = c.CityId
    WHERE pa.PostcodeArea LIKE @SearchTerm + '%'
        OR pa.AreaName LIKE '%' + @SearchTerm + '%'
        OR pa.Region LIKE '%' + @SearchTerm + '%'
    GROUP BY 
        pa.PostcodeArea, pa.AreaName, pa.Region,
        pa.CenterLatitude, pa.CenterLongitude
    ORDER BY 
        CASE WHEN pa.PostcodeArea = @SearchTerm THEN 1 ELSE 0 END DESC,
        PostcodeCount DESC;
END;
GO

-- 6. City Search
CREATE OR ALTER PROCEDURE [dbo].[sp_SearchCities]
    @SearchTerm NVARCHAR(100),
    @CountyId INT = NULL,
    @MaxResults INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        c.CityId,
        c.CityName,
        c.CityType,
        co.CountyId,
        co.CountyName,
        ct.CountryName,
        c.Latitude,
        c.Longitude,
        COUNT(DISTINCT s.StreetId) AS StreetCount,
        COUNT(DISTINCT a.AddressId) AS AddressCount
    FROM [dbo].[Cities] c
    INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
    INNER JOIN [dbo].[Countries] ct ON co.CountryId = ct.CountryId
    LEFT JOIN [dbo].[Streets] s ON c.CityId = s.CityId
    LEFT JOIN [dbo].[Addresses] a ON s.StreetId = a.StreetId
    WHERE c.IsActive = 1
        AND c.CityName LIKE @SearchTerm + '%'
        AND (@CountyId IS NULL OR c.CountyId = @CountyId)
    GROUP BY 
        c.CityId, c.CityName, c.CityType,
        co.CountyId, co.CountyName, ct.CountryName,
        c.Latitude, c.Longitude
    ORDER BY 
        CASE WHEN c.CityName = @SearchTerm THEN 1 ELSE 0 END DESC,
        AddressCount DESC,
        c.CityName;
END;
GO

-- 7. Street Search
CREATE OR ALTER PROCEDURE [dbo].[sp_SearchStreets]
    @StreetName NVARCHAR(200),
    @CityId INT = NULL,
    @PostcodeArea NVARCHAR(2) = NULL,
    @MaxResults INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        s.StreetId,
        s.StreetName,
        s.StreetType,
        c.CityId,
        c.CityName,
        co.CountyName,
        p.PostcodeFormatted,
        p.PostcodeArea,
        p.PostcodeDistrict,
        COUNT(DISTINCT a.AddressId) AS AddressCount
    FROM [dbo].[Streets] s
    INNER JOIN [dbo].[Cities] c ON s.CityId = c.CityId
    INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
    LEFT JOIN [dbo].[Postcodes] p ON s.PostcodeId = p.PostcodeId
    LEFT JOIN [dbo].[Addresses] a ON s.StreetId = a.StreetId
    WHERE s.IsActive = 1
        AND s.StreetName LIKE '%' + @StreetName + '%'
        AND (@CityId IS NULL OR s.CityId = @CityId)
        AND (@PostcodeArea IS NULL OR p.PostcodeArea = @PostcodeArea)
    GROUP BY 
        s.StreetId, s.StreetName, s.StreetType,
        c.CityId, c.CityName, co.CountyName,
        p.PostcodeFormatted, p.PostcodeArea, p.PostcodeDistrict
    ORDER BY 
        CASE WHEN s.StreetName = @StreetName THEN 1 ELSE 0 END DESC,
        AddressCount DESC,
        s.StreetName;
END;
GO

-- 8. Bulk Address Insert/Update
CREATE OR ALTER PROCEDURE [dbo].[sp_UpsertAddress]
    @BuildingNumber NVARCHAR(20) = NULL,
    @BuildingName NVARCHAR(100) = NULL,
    @SubBuilding NVARCHAR(100) = NULL,
    @StreetName NVARCHAR(200),
    @StreetType NVARCHAR(20) = NULL,
    @CityName NVARCHAR(100),
    @CountyName NVARCHAR(100),
    @Postcode NVARCHAR(10),
    @UPRN BIGINT = NULL,
    @IsResidential BIT = 1,
    @AddressId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Normalize postcode
        DECLARE @NormalizedPostcode NVARCHAR(10) = UPPER(REPLACE(@Postcode, ' ', ''));
        DECLARE @FormattedPostcode NVARCHAR(10);
        
        -- Format postcode (add space)
        IF LEN(@NormalizedPostcode) = 7
            SET @FormattedPostcode = LEFT(@NormalizedPostcode, 4) + ' ' + RIGHT(@NormalizedPostcode, 3);
        ELSE IF LEN(@NormalizedPostcode) = 6
            SET @FormattedPostcode = LEFT(@NormalizedPostcode, 3) + ' ' + RIGHT(@NormalizedPostcode, 3);
        ELSE
            SET @FormattedPostcode = @Postcode;
        
        -- Get or create postcode
        DECLARE @PostcodeId INT;
        SELECT @PostcodeId = PostcodeId 
        FROM [dbo].[Postcodes] 
        WHERE Postcode = @NormalizedPostcode;
        
        IF @PostcodeId IS NULL
        BEGIN
            RAISERROR('Postcode not found in database', 16, 1);
            RETURN;
        END
        
        -- Get or create city
        DECLARE @CityId INT;
        DECLARE @CountyId INT;
        
        SELECT @CountyId = CountyId 
        FROM [dbo].[Counties] 
        WHERE CountyName = @CountyName;
        
        IF @CountyId IS NULL
        BEGIN
            RAISERROR('County not found in database', 16, 1);
            RETURN;
        END
        
        SELECT @CityId = CityId 
        FROM [dbo].[Cities] 
        WHERE CityName = @CityName AND CountyId = @CountyId;
        
        IF @CityId IS NULL
        BEGIN
            INSERT INTO [dbo].[Cities] (CountyId, CityName, CityType)
            VALUES (@CountyId, @CityName, 'Town');
            
            SET @CityId = SCOPE_IDENTITY();
        END
        
        -- Get or create street
        DECLARE @StreetId INT;
        SELECT @StreetId = StreetId 
        FROM [dbo].[Streets] 
        WHERE StreetName = @StreetName 
            AND CityId = @CityId 
            AND (StreetType = @StreetType OR (@StreetType IS NULL AND StreetType IS NULL));
        
        IF @StreetId IS NULL
        BEGIN
            INSERT INTO [dbo].[Streets] (CityId, StreetName, StreetType, PostcodeId)
            VALUES (@CityId, @StreetName, @StreetType, @PostcodeId);
            
            SET @StreetId = SCOPE_IDENTITY();
        END
        
        -- Check if address already exists
        SELECT @AddressId = AddressId
        FROM [dbo].[Addresses]
        WHERE StreetId = @StreetId
            AND PostcodeId = @PostcodeId
            AND (BuildingNumber = @BuildingNumber OR (@BuildingNumber IS NULL AND BuildingNumber IS NULL))
            AND (BuildingName = @BuildingName OR (@BuildingName IS NULL AND BuildingName IS NULL))
            AND (SubBuilding = @SubBuilding OR (@SubBuilding IS NULL AND SubBuilding IS NULL));
        
        IF @AddressId IS NULL
        BEGIN
            -- Insert new address
            INSERT INTO [dbo].[Addresses] 
                (BuildingNumber, BuildingName, SubBuilding, StreetId, PostcodeId, CityId, CountyId, UPRN, IsResidential)
            VALUES 
                (@BuildingNumber, @BuildingName, @SubBuilding, @StreetId, @PostcodeId, @CityId, @CountyId, @UPRN, @IsResidential);
            
            SET @AddressId = SCOPE_IDENTITY();
            
            -- Add to cache
            INSERT INTO [dbo].[AddressLookupCache] 
                (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
            SELECT 
                LOWER(REPLACE(
                    ISNULL(@BuildingNumber + ' ', '') + 
                    ISNULL(@BuildingName + ' ', '') + 
                    @StreetName + ' ' + 
                    @CityName + ' ' + 
                    @FormattedPostcode, ' ', '')),
                ISNULL(@BuildingNumber + ' ', '') + 
                    ISNULL(@BuildingName + ', ', '') + 
                    @StreetName + 
                    ISNULL(' ' + @StreetType, '') + 
                    ', ' + @CityName + ', ' + 
                    @CountyName + ', ' + 
                    @FormattedPostcode,
                ISNULL(@BuildingNumber + ' ', '') + 
                    ISNULL(@BuildingName + ' ', '') + 
                    @StreetName + 
                    ISNULL(' ' + @StreetType, ''),
                @SubBuilding,
                @CityName,
                @CountyName,
                @FormattedPostcode,
                @PostcodeId,
                @AddressId,
                p.Latitude,
                p.Longitude,
                50
            FROM [dbo].[Postcodes] p
            WHERE p.PostcodeId = @PostcodeId;
        END
        ELSE
        BEGIN
            -- Update existing address if needed
            UPDATE [dbo].[Addresses]
            SET UPRN = COALESCE(@UPRN, UPRN),
                IsResidential = @IsResidential,
                ModifiedDate = GETUTCDATE()
            WHERE AddressId = @AddressId;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END;
GO

-- 9. Refresh Address Cache
CREATE OR ALTER PROCEDURE [dbo].[sp_RefreshAddressCache]
    @MaxRecords INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Clear existing cache
        TRUNCATE TABLE [dbo].[AddressLookupCache];
        
        -- Repopulate with address entries
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT TOP (COALESCE(@MaxRecords, 1000000))
            LOWER(REPLACE(
                ISNULL(a.BuildingNumber + ' ', '') + 
                ISNULL(a.BuildingName + ' ', '') + 
                s.StreetName + ' ' + 
                c.CityName + ' ' + 
                p.PostcodeFormatted, ' ', '')),
            ISNULL(a.BuildingNumber + ' ', '') + 
                ISNULL(a.BuildingName + ', ', '') + 
                s.StreetName + ', ' + 
                c.CityName + ', ' + 
                p.PostcodeFormatted,
            ISNULL(a.BuildingNumber + ' ', '') + 
                ISNULL(a.BuildingName + ' ', '') + 
                s.StreetName,
            a.SubBuilding,
            c.CityName,
            co.CountyName,
            p.PostcodeFormatted,
            p.PostcodeId,
            a.AddressId,
            p.Latitude,
            p.Longitude,
            CASE 
                WHEN c.CityName IN ('Westminster', 'Manchester', 'Birmingham', 'Leeds', 'Liverpool', 'Bristol') THEN 100
                WHEN c.CityName IN ('Camden', 'Islington', 'Edinburgh', 'Glasgow', 'Cardiff') THEN 90
                ELSE 80
            END
        FROM [dbo].[Addresses] a
        INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId
        INNER JOIN [dbo].[Cities] c ON a.CityId = c.CityId
        INNER JOIN [dbo].[Counties] co ON a.CountyId = co.CountyId
        INNER JOIN [dbo].[Postcodes] p ON a.PostcodeId = p.PostcodeId
        WHERE a.IsActive = 1 AND s.IsActive = 1 AND c.IsActive = 1 AND p.IsActive = 1;
        
        -- Add postcode-only entries
        INSERT INTO [dbo].[AddressLookupCache] 
            (SearchKey, DisplayText, AddressLine1, AddressLine2, City, County, PostcodeFormatted, PostcodeId, AddressId, Latitude, Longitude, SearchRank)
        SELECT DISTINCT TOP (COALESCE(@MaxRecords, 100000))
            LOWER(REPLACE(p.PostcodeFormatted, ' ', '')),
            c.CityName + ', ' + p.PostcodeFormatted,
            c.CityName,
            NULL,
            c.CityName,
            co.CountyName,
            p.PostcodeFormatted,
            p.PostcodeId,
            NULL,
            p.Latitude,
            p.Longitude,
            40
        FROM [dbo].[Postcodes] p
        INNER JOIN [dbo].[Streets] s ON s.PostcodeId = p.PostcodeId
        INNER JOIN [dbo].[Cities] c ON s.CityId = c.CityId
        INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
        WHERE p.IsActive = 1;
        
        COMMIT TRANSACTION;
        
        -- Update statistics
        UPDATE STATISTICS [dbo].[AddressLookupCache];
        
        SELECT COUNT(*) AS CacheRecordCount FROM [dbo].[AddressLookupCache];
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END;
GO

-- 10. Get Nearby Postcodes
CREATE OR ALTER PROCEDURE [dbo].[sp_GetNearbyPostcodes]
    @PostcodeId INT,
    @RadiusMiles DECIMAL(10,2) = 3,
    @MaxResults INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Latitude DECIMAL(9,6), @Longitude DECIMAL(9,6);
    
    -- Get the source postcode coordinates
    SELECT @Latitude = Latitude, @Longitude = Longitude
    FROM [dbo].[Postcodes]
    WHERE PostcodeId = @PostcodeId;
    
    IF @Latitude IS NULL OR @Longitude IS NULL
    BEGIN
        RAISERROR('Invalid postcode or coordinates not found', 16, 1);
        RETURN;
    END
    
    -- Convert radius to kilometers
    DECLARE @RadiusKm DECIMAL(10,2) = @RadiusMiles * 1.60934;
    DECLARE @EarthRadiusKm DECIMAL(10,2) = 6371;
    
    -- Find nearby postcodes
    SELECT TOP (@MaxResults)
        p.PostcodeId,
        p.PostcodeFormatted,
        p.PostcodeArea,
        p.PostcodeDistrict,
        p.Latitude,
        p.Longitude,
        @EarthRadiusKm * 2 * ASIN(
            SQRT(
                POWER(SIN(RADIANS(@Latitude - p.Latitude) / 2), 2) +
                COS(RADIANS(@Latitude)) * COS(RADIANS(p.Latitude)) *
                POWER(SIN(RADIANS(@Longitude - p.Longitude) / 2), 2)
            )
        ) AS DistanceKm,
        @EarthRadiusKm * 2 * ASIN(
            SQRT(
                POWER(SIN(RADIANS(@Latitude - p.Latitude) / 2), 2) +
                COS(RADIANS(@Latitude)) * COS(RADIANS(p.Latitude)) *
                POWER(SIN(RADIANS(@Longitude - p.Longitude) / 2), 2)
            )
        ) / 1.60934 AS DistanceMiles
    FROM [dbo].[Postcodes] p
    WHERE p.IsActive = 1
        AND p.PostcodeId != @PostcodeId
        AND @EarthRadiusKm * 2 * ASIN(
            SQRT(
                POWER(SIN(RADIANS(@Latitude - p.Latitude) / 2), 2) +
                COS(RADIANS(@Latitude)) * COS(RADIANS(p.Latitude)) *
                POWER(SIN(RADIANS(@Longitude - p.Longitude) / 2), 2)
            )
        ) <= @RadiusKm
    ORDER BY DistanceKm;
END;
GO

PRINT 'UK Address stored procedures created successfully';