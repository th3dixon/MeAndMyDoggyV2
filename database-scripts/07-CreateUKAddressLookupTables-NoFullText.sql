-- =============================================
-- MeAndMyDoggy UK Address Lookup Database Schema
-- Version without Full-Text Search
-- =============================================
-- This script creates the tables required for UK address lookup functionality
-- Including postcodes, streets, cities, and optimized lookup structures
-- WITHOUT Full-Text Search requirements

-- Drop existing tables if they exist (for development)
IF OBJECT_ID('dbo.AddressLookupCache', 'U') IS NOT NULL DROP TABLE [dbo].[AddressLookupCache];
IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL DROP TABLE [dbo].[Addresses];
IF OBJECT_ID('dbo.Streets', 'U') IS NOT NULL DROP TABLE [dbo].[Streets];
IF OBJECT_ID('dbo.PostcodeSectors', 'U') IS NOT NULL DROP TABLE [dbo].[PostcodeSectors];
IF OBJECT_ID('dbo.PostcodeAreas', 'U') IS NOT NULL DROP TABLE [dbo].[PostcodeAreas];
IF OBJECT_ID('dbo.Postcodes', 'U') IS NOT NULL DROP TABLE [dbo].[Postcodes];
IF OBJECT_ID('dbo.Cities', 'U') IS NOT NULL DROP TABLE [dbo].[Cities];
IF OBJECT_ID('dbo.Counties', 'U') IS NOT NULL DROP TABLE [dbo].[Counties];
IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL DROP TABLE [dbo].[Countries];

-- 1. Countries Table (for future expansion)
CREATE TABLE [dbo].[Countries] (
    [CountryId] int IDENTITY(1,1) NOT NULL,
    [CountryCode] nvarchar(2) NOT NULL,
    [CountryName] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([CountryId]),
    CONSTRAINT [UX_Countries_Code] UNIQUE ([CountryCode]),
    INDEX [IX_Countries_Name] NONCLUSTERED ([CountryName])
);

-- 2. Counties Table
CREATE TABLE [dbo].[Counties] (
    [CountyId] int IDENTITY(1,1) NOT NULL,
    [CountryId] int NOT NULL,
    [CountyName] nvarchar(100) NOT NULL,
    [CountyCode] nvarchar(10) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Counties] PRIMARY KEY CLUSTERED ([CountyId]),
    CONSTRAINT [FK_Counties_Countries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Countries] ([CountryId]),
    INDEX [IX_Counties_Name] NONCLUSTERED ([CountyName]),
    INDEX [IX_Counties_Country] NONCLUSTERED ([CountryId])
);

-- 3. Cities Table
CREATE TABLE [dbo].[Cities] (
    [CityId] int IDENTITY(1,1) NOT NULL,
    [CountyId] int NOT NULL,
    [CityName] nvarchar(100) NOT NULL,
    [CityType] nvarchar(20) NULL, -- 'City', 'Town', 'Village', etc.
    [Latitude] decimal(9,6) NULL,
    [Longitude] decimal(9,6) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED ([CityId]),
    CONSTRAINT [FK_Cities_Counties] FOREIGN KEY ([CountyId]) REFERENCES [dbo].[Counties] ([CountyId]),
    INDEX [IX_Cities_Name] NONCLUSTERED ([CityName]),
    INDEX [IX_Cities_County] NONCLUSTERED ([CountyId]),
    INDEX [IX_Cities_Location] NONCLUSTERED ([Latitude], [Longitude]) WHERE [Latitude] IS NOT NULL
);

-- 4. Postcodes Table (Main postcode lookup)
CREATE TABLE [dbo].[Postcodes] (
    [PostcodeId] int IDENTITY(1,1) NOT NULL,
    [Postcode] nvarchar(8) NOT NULL, -- Full postcode without spaces (e.g., 'SW1A1AA')
    [PostcodeFormatted] nvarchar(10) NOT NULL, -- Formatted with space (e.g., 'SW1A 1AA')
    [OutwardCode] nvarchar(4) NOT NULL, -- First part (e.g., 'SW1A')
    [InwardCode] nvarchar(3) NOT NULL, -- Second part (e.g., '1AA')
    [PostcodeArea] nvarchar(2) NOT NULL, -- Area (e.g., 'SW')
    [PostcodeDistrict] nvarchar(4) NOT NULL, -- District (e.g., 'SW1')
    [PostcodeSector] nvarchar(6) NOT NULL, -- Sector (e.g., 'SW1A 1')
    [Latitude] decimal(9,6) NOT NULL,
    [Longitude] decimal(9,6) NOT NULL,
    [Easting] int NULL,
    [Northing] int NULL,
    [GridReference] nvarchar(10) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [DateIntroduced] date NULL,
    [DateTerminated] date NULL,
    
    CONSTRAINT [PK_Postcodes] PRIMARY KEY CLUSTERED ([PostcodeId]),
    CONSTRAINT [UX_Postcodes_Postcode] UNIQUE ([Postcode]),
    INDEX [IX_Postcodes_Formatted] NONCLUSTERED ([PostcodeFormatted]),
    INDEX [IX_Postcodes_Outward] NONCLUSTERED ([OutwardCode]),
    INDEX [IX_Postcodes_Area] NONCLUSTERED ([PostcodeArea]),
    INDEX [IX_Postcodes_District] NONCLUSTERED ([PostcodeDistrict]),
    INDEX [IX_Postcodes_Sector] NONCLUSTERED ([PostcodeSector]),
    INDEX [IX_Postcodes_Location] NONCLUSTERED ([Latitude], [Longitude]),
    INDEX [IX_Postcodes_Active] NONCLUSTERED ([IsActive]) INCLUDE ([PostcodeFormatted], [Latitude], [Longitude])
);

-- 5. Postcode Areas Table (for area-level lookups)
CREATE TABLE [dbo].[PostcodeAreas] (
    [PostcodeAreaId] int IDENTITY(1,1) NOT NULL,
    [PostcodeArea] nvarchar(2) NOT NULL,
    [AreaName] nvarchar(100) NOT NULL,
    [Region] nvarchar(50) NULL,
    [CenterLatitude] decimal(9,6) NULL,
    [CenterLongitude] decimal(9,6) NULL,
    
    CONSTRAINT [PK_PostcodeAreas] PRIMARY KEY CLUSTERED ([PostcodeAreaId]),
    CONSTRAINT [UX_PostcodeAreas_Area] UNIQUE ([PostcodeArea]),
    INDEX [IX_PostcodeAreas_Name] NONCLUSTERED ([AreaName])
);

-- 6. Postcode Sectors Table (for sector-level lookups)
CREATE TABLE [dbo].[PostcodeSectors] (
    [PostcodeSectorId] int IDENTITY(1,1) NOT NULL,
    [PostcodeSector] nvarchar(6) NOT NULL,
    [PostcodeDistrict] nvarchar(4) NOT NULL,
    [PostcodeArea] nvarchar(2) NOT NULL,
    [SectorName] nvarchar(100) NULL,
    [CenterLatitude] decimal(9,6) NULL,
    [CenterLongitude] decimal(9,6) NULL,
    
    CONSTRAINT [PK_PostcodeSectors] PRIMARY KEY CLUSTERED ([PostcodeSectorId]),
    CONSTRAINT [UX_PostcodeSectors_Sector] UNIQUE ([PostcodeSector]),
    INDEX [IX_PostcodeSectors_District] NONCLUSTERED ([PostcodeDistrict]),
    INDEX [IX_PostcodeSectors_Area] NONCLUSTERED ([PostcodeArea])
);

-- 7. Streets Table
CREATE TABLE [dbo].[Streets] (
    [StreetId] int IDENTITY(1,1) NOT NULL,
    [CityId] int NOT NULL,
    [StreetName] nvarchar(200) NOT NULL,
    [StreetType] nvarchar(20) NULL, -- 'Road', 'Street', 'Avenue', 'Lane', etc.
    [PostcodeId] int NULL, -- Primary postcode for this street
    [IsActive] bit NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Streets] PRIMARY KEY CLUSTERED ([StreetId]),
    CONSTRAINT [FK_Streets_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId]),
    CONSTRAINT [FK_Streets_Postcodes] FOREIGN KEY ([PostcodeId]) REFERENCES [dbo].[Postcodes] ([PostcodeId]),
    INDEX [IX_Streets_Name] NONCLUSTERED ([StreetName]),
    INDEX [IX_Streets_City] NONCLUSTERED ([CityId]),
    INDEX [IX_Streets_Postcode] NONCLUSTERED ([PostcodeId])
);

-- 8. Addresses Table (Individual addresses)
CREATE TABLE [dbo].[Addresses] (
    [AddressId] int IDENTITY(1,1) NOT NULL,
    [BuildingNumber] nvarchar(20) NULL,
    [BuildingName] nvarchar(100) NULL,
    [SubBuilding] nvarchar(100) NULL, -- Flat/Unit number
    [StreetId] int NOT NULL,
    [PostcodeId] int NOT NULL,
    [CityId] int NOT NULL,
    [CountyId] int NOT NULL,
    [UPRN] bigint NULL, -- Unique Property Reference Number
    [IsResidential] bit NOT NULL DEFAULT 1,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedDate] datetime2(7) NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] datetime2(7) NULL,
    
    CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([AddressId]),
    CONSTRAINT [FK_Addresses_Streets] FOREIGN KEY ([StreetId]) REFERENCES [dbo].[Streets] ([StreetId]),
    CONSTRAINT [FK_Addresses_Postcodes] FOREIGN KEY ([PostcodeId]) REFERENCES [dbo].[Postcodes] ([PostcodeId]),
    CONSTRAINT [FK_Addresses_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId]),
    CONSTRAINT [FK_Addresses_Counties] FOREIGN KEY ([CountyId]) REFERENCES [dbo].[Counties] ([CountyId]),
    INDEX [IX_Addresses_Street] NONCLUSTERED ([StreetId]),
    INDEX [IX_Addresses_Postcode] NONCLUSTERED ([PostcodeId]),
    INDEX [IX_Addresses_City] NONCLUSTERED ([CityId]),
    INDEX [IX_Addresses_UPRN] NONCLUSTERED ([UPRN]) WHERE [UPRN] IS NOT NULL,
    INDEX [IX_Addresses_Building] NONCLUSTERED ([BuildingNumber], [BuildingName])
);

-- 9. Address Lookup Cache Table (for fast autocomplete)
-- Modified to use regular indexes instead of Full-Text
CREATE TABLE [dbo].[AddressLookupCache] (
    [CacheId] int IDENTITY(1,1) NOT NULL,
    [SearchKey] nvarchar(100) NOT NULL, -- Normalized search key
    [DisplayText] nvarchar(500) NOT NULL, -- Full formatted address
    [AddressLine1] nvarchar(200) NOT NULL,
    [AddressLine2] nvarchar(200) NULL,
    [City] nvarchar(100) NOT NULL,
    [County] nvarchar(100) NOT NULL,
    [PostcodeFormatted] nvarchar(10) NOT NULL,
    [PostcodeId] int NOT NULL,
    [AddressId] int NULL,
    [Latitude] decimal(9,6) NOT NULL,
    [Longitude] decimal(9,6) NOT NULL,
    [SearchRank] int NOT NULL DEFAULT 0, -- For ordering results
    [LastUsed] datetime2(7) NULL,
    [UseCount] int NOT NULL DEFAULT 0,
    
    CONSTRAINT [PK_AddressLookupCache] PRIMARY KEY CLUSTERED ([CacheId]),
    CONSTRAINT [FK_AddressLookupCache_Postcodes] FOREIGN KEY ([PostcodeId]) REFERENCES [dbo].[Postcodes] ([PostcodeId]),
    CONSTRAINT [FK_AddressLookupCache_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    INDEX [IX_AddressLookupCache_SearchKey] NONCLUSTERED ([SearchKey]),
    INDEX [IX_AddressLookupCache_Postcode] NONCLUSTERED ([PostcodeFormatted]),
    INDEX [IX_AddressLookupCache_City] NONCLUSTERED ([City]),
    INDEX [IX_AddressLookupCache_Rank] NONCLUSTERED ([SearchRank] DESC, [UseCount] DESC),
    -- Additional indexes for LIKE searches
    INDEX [IX_AddressLookupCache_DisplayText] NONCLUSTERED ([DisplayText]),
    INDEX [IX_AddressLookupCache_AddressLine1] NONCLUSTERED ([AddressLine1])
);

-- Create additional indexes for text searching without Full-Text
-- These support efficient LIKE queries
CREATE INDEX [IX_Streets_NameSearch] ON [dbo].[Streets] ([StreetName]) INCLUDE ([CityId], [PostcodeId]);
CREATE INDEX [IX_Cities_NameSearch] ON [dbo].[Cities] ([CityName]) INCLUDE ([CountyId]);

-- Create statistics for better query performance
CREATE STATISTICS [STAT_Postcodes_Area_District] ON [dbo].[Postcodes]([PostcodeArea], [PostcodeDistrict]);
CREATE STATISTICS [STAT_Addresses_Postcode_City] ON [dbo].[Addresses]([PostcodeId], [CityId]);
CREATE STATISTICS [STAT_Streets_City_Name] ON [dbo].[Streets]([CityId], [StreetName]);

PRINT 'UK Address Lookup tables created successfully (without Full-Text Search)';
PRINT '';
PRINT 'Note: This version uses regular indexes instead of Full-Text Search.';
PRINT 'Search queries will use LIKE operators instead of CONTAINS/FREETEXT.';
PRINT 'Performance may be slightly lower for complex text searches.';