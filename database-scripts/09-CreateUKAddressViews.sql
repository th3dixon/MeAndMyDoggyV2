-- =============================================
-- MeAndMyDoggy UK Address Lookup Views
-- =============================================
-- Optimized views for efficient address querying and autocomplete

-- 1. Complete Address View
CREATE OR ALTER VIEW [dbo].[vw_CompleteAddresses]
AS
SELECT 
    a.AddressId,
    a.BuildingNumber,
    a.BuildingName,
    a.SubBuilding,
    s.StreetName,
    s.StreetType,
    c.CityName,
    c.CityType,
    co.CountyName,
    ct.CountryName,
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
    -- Formatted full address
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
    -- Search-friendly version (lowercase, no spaces)
    LOWER(REPLACE(
        CONCAT(
            ISNULL(a.BuildingNumber, ''),
            ISNULL(a.BuildingName, ''),
            ISNULL(a.SubBuilding, ''),
            s.StreetName,
            c.CityName,
            p.Postcode
        ), ' ', ''
    )) AS SearchKey
FROM [dbo].[Addresses] a
INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId
INNER JOIN [dbo].[Cities] c ON a.CityId = c.CityId
INNER JOIN [dbo].[Counties] co ON a.CountyId = co.CountyId
INNER JOIN [dbo].[Countries] ct ON co.CountryId = ct.CountryId
INNER JOIN [dbo].[Postcodes] p ON a.PostcodeId = p.PostcodeId
WHERE a.IsActive = 1 AND s.IsActive = 1 AND c.IsActive = 1 AND p.IsActive = 1;
GO

-- 2. Postcode Summary View
CREATE OR ALTER VIEW [dbo].[vw_PostcodeSummary]
AS
SELECT 
    p.PostcodeId,
    p.PostcodeFormatted,
    p.Postcode,
    p.OutwardCode,
    p.InwardCode,
    p.PostcodeArea,
    p.PostcodeDistrict,
    p.PostcodeSector,
    pa.AreaName,
    pa.Region,
    p.Latitude,
    p.Longitude,
    p.IsActive,
    (SELECT COUNT(DISTINCT AddressId) FROM [dbo].[Addresses] WHERE PostcodeId = p.PostcodeId) AS AddressCount,
    (SELECT COUNT(DISTINCT StreetId) FROM [dbo].[Streets] WHERE PostcodeId = p.PostcodeId) AS StreetCount,
    (SELECT STRING_AGG(CityName, ', ') FROM (SELECT DISTINCT c.CityName FROM [dbo].[Streets] s INNER JOIN [dbo].[Cities] c ON s.CityId = c.CityId WHERE s.PostcodeId = p.PostcodeId) AS DistinctCities) AS Cities
FROM [dbo].[Postcodes] p
LEFT JOIN [dbo].[PostcodeAreas] pa ON p.PostcodeArea = pa.PostcodeArea
WHERE p.IsActive = 1;
GO

-- 3. City Address Summary View
CREATE OR ALTER VIEW [dbo].[vw_CityAddressSummary]
AS
SELECT 
    c.CityId,
    c.CityName,
    c.CityType,
    co.CountyName,
    ct.CountryName,
    c.Latitude AS CityLatitude,
    c.Longitude AS CityLongitude,
    (SELECT COUNT(DISTINCT StreetId) FROM [dbo].[Streets] WHERE CityId = c.CityId) AS StreetCount,
    (SELECT COUNT(DISTINCT a.AddressId) FROM [dbo].[Addresses] a INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId WHERE s.CityId = c.CityId) AS AddressCount,
    (SELECT COUNT(DISTINCT p.PostcodeId) FROM [dbo].[Postcodes] p INNER JOIN [dbo].[Streets] s ON p.PostcodeId = s.PostcodeId WHERE s.CityId = c.CityId) AS PostcodeCount,
    (SELECT STRING_AGG(PostcodeArea, ', ') FROM (SELECT DISTINCT p.PostcodeArea FROM [dbo].[Postcodes] p INNER JOIN [dbo].[Streets] s ON p.PostcodeId = s.PostcodeId WHERE s.CityId = c.CityId) AS DistinctAreas) AS PostcodeAreas
FROM [dbo].[Cities] c
INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
INNER JOIN [dbo].[Countries] ct ON co.CountryId = ct.CountryId
WHERE c.IsActive = 1;
GO

-- 4. Street Directory View
CREATE OR ALTER VIEW [dbo].[vw_StreetDirectory]
AS
SELECT 
    s.StreetId,
    s.StreetName,
    s.StreetType,
    c.CityName,
    co.CountyName,
    p.PostcodeFormatted AS PrimaryPostcode,
    p.PostcodeArea,
    p.PostcodeDistrict,
    (SELECT COUNT(DISTINCT AddressId) FROM [dbo].[Addresses] WHERE StreetId = s.StreetId) AS PropertyCount,
    (SELECT MIN(CAST(BuildingNumber AS INT)) FROM [dbo].[Addresses] WHERE StreetId = s.StreetId AND TRY_CAST(BuildingNumber AS INT) IS NOT NULL) AS LowestNumber,
    (SELECT MAX(CAST(BuildingNumber AS INT)) FROM [dbo].[Addresses] WHERE StreetId = s.StreetId AND TRY_CAST(BuildingNumber AS INT) IS NOT NULL) AS HighestNumber,
    (SELECT STRING_AGG(PostcodeFormatted, ', ') FROM (SELECT DISTINCT p2.PostcodeFormatted FROM [dbo].[Addresses] a2 INNER JOIN [dbo].[Postcodes] p2 ON a2.PostcodeId = p2.PostcodeId WHERE a2.StreetId = s.StreetId) AS DistinctPostcodes) AS AllPostcodes
FROM [dbo].[Streets] s
INNER JOIN [dbo].[Cities] c ON s.CityId = c.CityId
INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId
LEFT JOIN [dbo].[Postcodes] p ON s.PostcodeId = p.PostcodeId
WHERE s.IsActive = 1;
GO

-- 5. Address Search View (Optimized for autocomplete)
CREATE OR ALTER VIEW [dbo].[vw_AddressSearch]
AS
SELECT TOP 100 PERCENT
    CacheId,
    SearchKey,
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
    UseCount,
    -- Additional search tokens for better matching
    CONCAT(
        PostcodeFormatted, ' ',
        City, ' ',
        AddressLine1, ' ',
        REPLACE(PostcodeFormatted, ' ', '')
    ) AS SearchTokens
FROM [dbo].[AddressLookupCache]
ORDER BY SearchRank DESC, UseCount DESC;
GO

-- 6. Postcode Area Statistics View
CREATE OR ALTER VIEW [dbo].[vw_PostcodeAreaStats]
AS
SELECT 
    pa.PostcodeArea,
    pa.AreaName,
    pa.Region,
    pa.CenterLatitude,
    pa.CenterLongitude,
    COUNT(DISTINCT p.PostcodeId) AS TotalPostcodes,
    COUNT(DISTINCT p.PostcodeId) AS ActivePostcodes,
    COUNT(DISTINCT ps.PostcodeSector) AS TotalSectors,
    COUNT(DISTINCT CONCAT(c.CityId, '|', p.PostcodeId)) AS CitiesServed,
    MIN(p.Latitude) AS MinLatitude,
    MAX(p.Latitude) AS MaxLatitude,
    MIN(p.Longitude) AS MinLongitude,
    MAX(p.Longitude) AS MaxLongitude
FROM [dbo].[PostcodeAreas] pa
LEFT JOIN [dbo].[Postcodes] p ON pa.PostcodeArea = p.PostcodeArea
LEFT JOIN [dbo].[PostcodeSectors] ps ON pa.PostcodeArea = ps.PostcodeArea
LEFT JOIN [dbo].[Streets] s ON p.PostcodeId = s.PostcodeId
LEFT JOIN [dbo].[Cities] c ON s.CityId = c.CityId
WHERE p.IsActive = 1
GROUP BY 
    pa.PostcodeArea, pa.AreaName, pa.Region, 
    pa.CenterLatitude, pa.CenterLongitude;
GO

-- 7. Address Hierarchy View (for navigation/drill-down)
CREATE OR ALTER VIEW [dbo].[vw_AddressHierarchy]
AS
SELECT 
    ct.CountryId,
    ct.CountryName,
    co.CountyId,
    co.CountyName,
    c.CityId,
    c.CityName,
    c.CityType,
    p.PostcodeArea,
    p.PostcodeDistrict,
    p.PostcodeSector,
    p.PostcodeFormatted,
    s.StreetId,
    s.StreetName,
    s.StreetType,
    COUNT(DISTINCT a.AddressId) AS AddressCount
FROM [dbo].[Countries] ct
INNER JOIN [dbo].[Counties] co ON ct.CountryId = co.CountryId
INNER JOIN [dbo].[Cities] c ON co.CountyId = c.CountyId
INNER JOIN [dbo].[Streets] s ON c.CityId = s.CityId
INNER JOIN [dbo].[Postcodes] p ON s.PostcodeId = p.PostcodeId
LEFT JOIN [dbo].[Addresses] a ON s.StreetId = a.StreetId AND p.PostcodeId = a.PostcodeId
WHERE ct.IsActive = 1 AND c.IsActive = 1 AND s.IsActive = 1 AND p.IsActive = 1
GROUP BY 
    ct.CountryId, ct.CountryName, co.CountyId, co.CountyName,
    c.CityId, c.CityName, c.CityType, p.PostcodeArea, p.PostcodeDistrict,
    p.PostcodeSector, p.PostcodeFormatted, s.StreetId, s.StreetName, s.StreetType;
GO

-- Create indexed view for most common searches
CREATE OR ALTER VIEW [dbo].[vw_CommonAddressSearches]
WITH SCHEMABINDING
AS
SELECT 
    alc.PostcodeFormatted,
    alc.City,
    alc.SearchRank,
    COUNT_BIG(*) AS EntryCount
FROM [dbo].[AddressLookupCache] alc
GROUP BY alc.PostcodeFormatted, alc.City, alc.SearchRank;
GO

-- Create unique clustered index on the indexed view
CREATE UNIQUE CLUSTERED INDEX [IX_vw_CommonAddressSearches] 
ON [dbo].[vw_CommonAddressSearches] ([PostcodeFormatted], [City], [SearchRank]);
GO

PRINT 'UK Address views created successfully';