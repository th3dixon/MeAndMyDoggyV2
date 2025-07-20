-- =============================================
-- MeAndMyDoggy UK Address Sample Data
-- =============================================
-- This script populates the address tables with sample UK address data
-- Includes major cities, popular postcodes, and realistic street names

-- 1. Insert Countries
INSERT INTO [dbo].[Countries] ([CountryCode], [CountryName])
VALUES 
    ('GB', 'United Kingdom'),
    ('IE', 'Ireland'); -- For future expansion

-- 2. Insert Counties (Major UK Counties)
DECLARE @UK_ID int = (SELECT CountryId FROM Countries WHERE CountryCode = 'GB');

INSERT INTO [dbo].[Counties] ([CountryId], [CountyName], [CountyCode])
VALUES 
    (@UK_ID, 'Greater London', 'LDN'),
    (@UK_ID, 'Greater Manchester', 'GTM'),
    (@UK_ID, 'West Midlands', 'WMD'),
    (@UK_ID, 'West Yorkshire', 'WYK'),
    (@UK_ID, 'Kent', 'KNT'),
    (@UK_ID, 'Essex', 'ESX'),
    (@UK_ID, 'Surrey', 'SRY'),
    (@UK_ID, 'Hampshire', 'HAM'),
    (@UK_ID, 'Merseyside', 'MSY'),
    (@UK_ID, 'South Yorkshire', 'SYK'),
    (@UK_ID, 'Tyne and Wear', 'TWR'),
    (@UK_ID, 'Lancashire', 'LAN'),
    (@UK_ID, 'Nottinghamshire', 'NTT'),
    (@UK_ID, 'Derbyshire', 'DBY'),
    (@UK_ID, 'Norfolk', 'NFK'),
    (@UK_ID, 'Suffolk', 'SFK'),
    (@UK_ID, 'Cambridgeshire', 'CAM'),
    (@UK_ID, 'Oxfordshire', 'OXF'),
    (@UK_ID, 'Devon', 'DEV'),
    (@UK_ID, 'Cornwall', 'CON');

-- 3. Insert Cities
DECLARE @London_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'Greater London');
DECLARE @Manchester_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'Greater Manchester');
DECLARE @WestMid_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'West Midlands');
DECLARE @Yorkshire_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'West Yorkshire');
DECLARE @Kent_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'Kent');
DECLARE @Surrey_ID int = (SELECT CountyId FROM Counties WHERE CountyName = 'Surrey');

INSERT INTO [dbo].[Cities] ([CountyId], [CityName], [CityType], [Latitude], [Longitude])
VALUES 
    -- London Areas
    (@London_ID, 'Westminster', 'City', 51.4975, -0.1357),
    (@London_ID, 'Camden', 'Borough', 51.5290, -0.1255),
    (@London_ID, 'Islington', 'Borough', 51.5416, -0.1022),
    (@London_ID, 'Hackney', 'Borough', 51.5450, -0.0553),
    (@London_ID, 'Tower Hamlets', 'Borough', 51.5203, -0.0293),
    (@London_ID, 'Greenwich', 'Borough', 51.4934, 0.0098),
    (@London_ID, 'Kensington and Chelsea', 'Borough', 51.4991, -0.1938),
    (@London_ID, 'Hammersmith and Fulham', 'Borough', 51.4990, -0.2291),
    (@London_ID, 'Wandsworth', 'Borough', 51.4571, -0.1818),
    (@London_ID, 'Lambeth', 'Borough', 51.4571, -0.1231),
    
    -- Manchester Areas
    (@Manchester_ID, 'Manchester', 'City', 53.4808, -2.2426),
    (@Manchester_ID, 'Salford', 'City', 53.4875, -2.2901),
    (@Manchester_ID, 'Bolton', 'Town', 53.5779, -2.4282),
    (@Manchester_ID, 'Bury', 'Town', 53.5933, -2.2966),
    (@Manchester_ID, 'Oldham', 'Town', 53.5409, -2.1113),
    
    -- Birmingham Area
    (@WestMid_ID, 'Birmingham', 'City', 52.4862, -1.8904),
    (@WestMid_ID, 'Coventry', 'City', 52.4068, -1.5197),
    (@WestMid_ID, 'Wolverhampton', 'City', 52.5865, -2.1296),
    
    -- Yorkshire
    (@Yorkshire_ID, 'Leeds', 'City', 53.8008, -1.5491),
    (@Yorkshire_ID, 'Bradford', 'City', 53.7960, -1.7594),
    (@Yorkshire_ID, 'Wakefield', 'City', 53.6833, -1.4977),
    
    -- Kent
    (@Kent_ID, 'Canterbury', 'City', 51.2802, 1.0789),
    (@Kent_ID, 'Maidstone', 'Town', 51.2704, 0.5227),
    (@Kent_ID, 'Rochester', 'City', 51.3885, 0.5067),
    
    -- Surrey
    (@Surrey_ID, 'Guildford', 'Town', 51.2365, -0.5703),
    (@Surrey_ID, 'Woking', 'Town', 51.3190, -0.5590),
    (@Surrey_ID, 'Epsom', 'Town', 51.3362, -0.2689);

-- 4. Insert Postcode Areas
INSERT INTO [dbo].[PostcodeAreas] ([PostcodeArea], [AreaName], [Region], [CenterLatitude], [CenterLongitude])
VALUES 
    -- London
    ('EC', 'East Central London', 'London', 51.5209, -0.0968),
    ('WC', 'West Central London', 'London', 51.5246, -0.1340),
    ('E', 'East London', 'London', 51.5320, -0.0427),
    ('W', 'West London', 'London', 51.5136, -0.2288),
    ('N', 'North London', 'London', 51.5904, -0.1042),
    ('NW', 'North West London', 'London', 51.5345, -0.1764),
    ('SE', 'South East London', 'London', 51.4520, 0.0293),
    ('SW', 'South West London', 'London', 51.4419, -0.1687),
    
    -- Major Cities
    ('M', 'Manchester', 'North West', 53.4794, -2.2453),
    ('B', 'Birmingham', 'West Midlands', 52.4831, -1.8936),
    ('L', 'Liverpool', 'North West', 53.4094, -2.9785),
    ('LS', 'Leeds', 'Yorkshire', 53.7997, -1.5492),
    ('G', 'Glasgow', 'Scotland', 55.8609, -4.2514),
    ('EH', 'Edinburgh', 'Scotland', 55.9533, -3.1883),
    ('BS', 'Bristol', 'South West', 51.4538, -2.5973),
    ('OX', 'Oxford', 'South East', 51.7519, -1.2578),
    ('CB', 'Cambridge', 'East', 52.2053, 0.1218),
    ('BA', 'Bath', 'South West', 51.3814, -2.3596),
    ('YO', 'York', 'Yorkshire', 53.9591, -1.0815),
    ('CT', 'Canterbury', 'South East', 51.2802, 1.0789),
    ('BN', 'Brighton', 'South East', 50.8225, -0.1372),
    ('PO', 'Portsmouth', 'South East', 50.8198, -1.0880);

-- 5. Insert Postcodes (Sample postcodes for major areas)
INSERT INTO [dbo].[Postcodes] 
    ([Postcode], [PostcodeFormatted], [OutwardCode], [InwardCode], [PostcodeArea], [PostcodeDistrict], [PostcodeSector], [Latitude], [Longitude])
VALUES 
    -- Westminster/Central London
    ('SW1A1AA', 'SW1A 1AA', 'SW1A', '1AA', 'SW', 'SW1', 'SW1A 1', 51.5014, -0.1419), -- Buckingham Palace
    ('SW1A2AA', 'SW1A 2AA', 'SW1A', '2AA', 'SW', 'SW1', 'SW1A 2', 51.5007, -0.1246), -- Westminster
    ('WC2N5DU', 'WC2N 5DU', 'WC2N', '5DU', 'WC', 'WC2', 'WC2N 5', 51.5074, -0.1278), -- Trafalgar Square
    ('EC1A1BB', 'EC1A 1BB', 'EC1A', '1BB', 'EC', 'EC1', 'EC1A 1', 51.5185, -0.1064), -- City of London
    ('W1A1AA', 'W1A 1AA', 'W1A', '1AA', 'W', 'W1', 'W1A 1', 51.5142, -0.1494), -- Oxford Street
    
    -- North London
    ('N16AY', 'N1 6AY', 'N1', '6AY', 'N', 'N1', 'N1 6', 51.5494, -0.0867), -- Islington
    ('N19GU', 'N1 9GU', 'N1', '9GU', 'N', 'N1', 'N1 9', 51.5381, -0.1031), -- King''s Cross
    ('NW11EJ', 'NW1 1EJ', 'NW1', '1EJ', 'NW', 'NW1', 'NW1 1', 51.5324, -0.1430), -- Regent''s Park
    ('NW31BF', 'NW3 1BF', 'NW3', '1BF', 'NW', 'NW3', 'NW3 1', 51.5507, -0.1765), -- Hampstead
    
    -- East London
    ('E14AB', 'E1 4AB', 'E1', '4AB', 'E', 'E1', 'E1 4', 51.5152, -0.0722), -- Whitechapel
    ('E145JP', 'E14 5JP', 'E14', '5JP', 'E', 'E14', 'E14 5', 51.5107, -0.0193), -- Canary Wharf
    ('E201JN', 'E20 1JN', 'E20', '1JN', 'E', 'E20', 'E20 1', 51.5433, -0.0077), -- Olympic Park
    
    -- South London
    ('SE19PZ', 'SE1 9PZ', 'SE1', '9PZ', 'SE', 'SE1', 'SE1 9', 51.5033, -0.1195), -- London Bridge
    ('SE109FR', 'SE10 9FR', 'SE10', '9FR', 'SE', 'SE10', 'SE10 9', 51.4827, 0.0096), -- Greenwich
    ('SW111AA', 'SW11 1AA', 'SW11', '1AA', 'SW', 'SW11', 'SW11 1', 51.4631, -0.1677), -- Battersea
    
    -- Manchester
    ('M11AD', 'M1 1AD', 'M1', '1AD', 'M', 'M1', 'M1 1', 53.4776, -2.2382), -- Manchester City Centre
    ('M21HS', 'M2 1HS', 'M2', '1HS', 'M', 'M2', 'M2 1', 53.4798, -2.2452), -- Manchester Deansgate
    ('M32AZ', 'M3 2AZ', 'M3', '2AZ', 'M', 'M3', 'M3 2', 53.4839, -2.2508), -- Salford Quays
    ('M156AZ', 'M15 6AZ', 'M15', '6AZ', 'M', 'M15', 'M15 6', 53.4631, -2.2413), -- Old Trafford
    
    -- Birmingham
    ('B11AA', 'B1 1AA', 'B1', '1AA', 'B', 'B1', 'B1 1', 52.4778, -1.9026), -- Birmingham Centre
    ('B24NT', 'B2 4NT', 'B2', '4NT', 'B', 'B2', 'B2 4', 52.4796, -1.8987), -- Birmingham New Street
    ('B156UA', 'B15 6UA', 'B15', '6UA', 'B', 'B15', 'B15 6', 52.4719, -1.9301), -- Edgbaston
    
    -- Leeds
    ('LS11AZ', 'LS1 1AZ', 'LS1', '1AZ', 'LS', 'LS1', 'LS1 1', 53.7958, -1.5436), -- Leeds City Centre
    ('LS22JT', 'LS2 2JT', 'LS2', '2JT', 'LS', 'LS2', 'LS2 2', 53.8008, -1.5491), -- Leeds University
    
    -- Brighton
    ('BN11AF', 'BN1 1AF', 'BN1', '1AF', 'BN', 'BN1', 'BN1 1', 50.8229, -0.1363), -- Brighton Centre
    ('BN21AA', 'BN2 1AA', 'BN2', '1AA', 'BN', 'BN2', 'BN2 1', 50.8238, -0.1203), -- Brighton Marina
    
    -- Cambridge
    ('CB11AA', 'CB1 1AA', 'CB1', '1AA', 'CB', 'CB1', 'CB1 1', 52.2011, 0.1279), -- Cambridge Centre
    ('CB22QR', 'CB2 2QR', 'CB2', '2QR', 'CB', 'CB2', 'CB2 2', 52.2025, 0.1198), -- Cambridge University
    
    -- Oxford
    ('OX11AA', 'OX1 1AA', 'OX1', '1AA', 'OX', 'OX1', 'OX1 1', 51.7520, -1.2577), -- Oxford Centre
    ('OX12JD', 'OX1 2JD', 'OX1', '2JD', 'OX', 'OX1', 'OX1 2', 51.7548, -1.2544); -- Oxford University

-- 6. Insert Postcode Sectors
INSERT INTO [dbo].[PostcodeSectors] ([PostcodeSector], [PostcodeDistrict], [PostcodeArea], [SectorName])
SELECT DISTINCT 
    [PostcodeSector], 
    [PostcodeDistrict], 
    [PostcodeArea],
    [PostcodeDistrict] + ' Sector ' + RIGHT([PostcodeSector], 1)
FROM [dbo].[Postcodes];

-- 7. Insert Streets (Common UK street names for each city)
DECLARE @Westminster_ID int = (SELECT CityId FROM Cities WHERE CityName = 'Westminster');
DECLARE @Camden_ID int = (SELECT CityId FROM Cities WHERE CityName = 'Camden');
DECLARE @Manchester_City_ID int = (SELECT CityId FROM Cities WHERE CityName = 'Manchester');
DECLARE @Birmingham_ID int = (SELECT CityId FROM Cities WHERE CityName = 'Birmingham');
DECLARE @Leeds_ID int = (SELECT CityId FROM Cities WHERE CityName = 'Leeds');

-- Westminster Streets
INSERT INTO [dbo].[Streets] ([CityId], [StreetName], [StreetType], [PostcodeId])
VALUES 
    (@Westminster_ID, 'Downing Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A2AA')),
    (@Westminster_ID, 'Whitehall', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A2AA')),
    (@Westminster_ID, 'The Mall', 'Road', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A1AA')),
    (@Westminster_ID, 'Victoria Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A1AA')),
    (@Westminster_ID, 'Oxford Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA')),
    (@Westminster_ID, 'Regent Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA')),
    (@Westminster_ID, 'Bond Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA')),
    (@Westminster_ID, 'Park Lane', 'Lane', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA'));

-- Camden Streets
INSERT INTO [dbo].[Streets] ([CityId], [StreetName], [StreetType], [PostcodeId])
VALUES 
    (@Camden_ID, 'Camden High Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'NW11EJ')),
    (@Camden_ID, 'Chalk Farm Road', 'Road', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'NW11EJ')),
    (@Camden_ID, 'Kentish Town Road', 'Road', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'NW11EJ')),
    (@Camden_ID, 'Hampstead Road', 'Road', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'NW11EJ'));

-- Manchester Streets
INSERT INTO [dbo].[Streets] ([CityId], [StreetName], [StreetType], [PostcodeId])
VALUES 
    (@Manchester_City_ID, 'Market Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M11AD')),
    (@Manchester_City_ID, 'Deansgate', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M21HS')),
    (@Manchester_City_ID, 'Oxford Road', 'Road', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M11AD')),
    (@Manchester_City_ID, 'Portland Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M11AD')),
    (@Manchester_City_ID, 'Piccadilly', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M11AD'));

-- Birmingham Streets
INSERT INTO [dbo].[Streets] ([CityId], [StreetName], [StreetType], [PostcodeId])
VALUES 
    (@Birmingham_ID, 'New Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'B24NT')),
    (@Birmingham_ID, 'Corporation Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'B11AA')),
    (@Birmingham_ID, 'High Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'B11AA')),
    (@Birmingham_ID, 'Broad Street', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'B156UA'));

-- Leeds Streets
INSERT INTO [dbo].[Streets] ([CityId], [StreetName], [StreetType], [PostcodeId])
VALUES 
    (@Leeds_ID, 'Briggate', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'LS11AZ')),
    (@Leeds_ID, 'The Headrow', 'Street', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'LS11AZ')),
    (@Leeds_ID, 'Park Lane', 'Lane', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'LS11AZ')),
    (@Leeds_ID, 'Woodhouse Lane', 'Lane', (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'LS22JT'));

-- 8. Insert Sample Addresses
-- Westminster Addresses
DECLARE @DowningStreet_ID int = (SELECT StreetId FROM Streets WHERE StreetName = 'Downing Street');
DECLARE @OxfordStreet_ID int = (SELECT TOP 1 StreetId FROM Streets WHERE StreetName = 'Oxford Street');
DECLARE @RegentStreet_ID int = (SELECT StreetId FROM Streets WHERE StreetName = 'Regent Street');

INSERT INTO [dbo].[Addresses] 
    ([BuildingNumber], [BuildingName], [SubBuilding], [StreetId], [PostcodeId], [CityId], [CountyId])
VALUES 
    -- Downing Street
    ('10', NULL, NULL, @DowningStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A2AA'), @Westminster_ID, @London_ID),
    ('11', NULL, NULL, @DowningStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'SW1A2AA'), @Westminster_ID, @London_ID),
    
    -- Oxford Street shops
    ('300', 'Oxford Circus House', NULL, @OxfordStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA'), @Westminster_ID, @London_ID),
    ('400', 'Selfridges', NULL, @OxfordStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA'), @Westminster_ID, @London_ID),
    
    -- Regent Street
    ('235', 'Liberty London', NULL, @RegentStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'W1A1AA'), @Westminster_ID, @London_ID);

-- Add more sample addresses for other cities
DECLARE @MarketStreet_ID int = (SELECT StreetId FROM Streets WHERE StreetName = 'Market Street' AND CityId = @Manchester_City_ID);
DECLARE @Deansgate_ID int = (SELECT StreetId FROM Streets WHERE StreetName = 'Deansgate');

INSERT INTO [dbo].[Addresses] 
    ([BuildingNumber], [BuildingName], [SubBuilding], [StreetId], [PostcodeId], [CityId], [CountyId])
VALUES 
    -- Manchester addresses
    ('1', 'Arndale House', 'Unit 10', @MarketStreet_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M11AD'), @Manchester_City_ID, @Manchester_ID),
    ('100', 'Deansgate Square', 'Flat 15A', @Deansgate_ID, 
     (SELECT PostcodeId FROM Postcodes WHERE Postcode = 'M21HS'), @Manchester_City_ID, @Manchester_ID);

-- 9. Populate Address Lookup Cache with formatted addresses
INSERT INTO [dbo].[AddressLookupCache] 
    ([SearchKey], [DisplayText], [AddressLine1], [AddressLine2], [City], [County], [PostcodeFormatted], [PostcodeId], [AddressId], [Latitude], [Longitude], [SearchRank])
SELECT 
    LOWER(REPLACE(
        ISNULL(a.BuildingNumber + ' ', '') + 
        ISNULL(a.BuildingName + ' ', '') + 
        s.StreetName + ' ' + 
        c.CityName + ' ' + 
        p.PostcodeFormatted, ' ', '')), -- SearchKey
    ISNULL(a.BuildingNumber + ' ', '') + 
        ISNULL(a.BuildingName + ', ', '') + 
        s.StreetName + ', ' + 
        c.CityName + ', ' + 
        p.PostcodeFormatted, -- DisplayText
    ISNULL(a.BuildingNumber + ' ', '') + 
        ISNULL(a.BuildingName + ' ', '') + 
        s.StreetName, -- AddressLine1
    ISNULL(a.SubBuilding, NULL), -- AddressLine2
    c.CityName, -- City
    co.CountyName, -- County
    p.PostcodeFormatted, -- PostcodeFormatted
    p.PostcodeId, -- PostcodeId
    a.AddressId, -- AddressId
    p.Latitude, -- Latitude
    p.Longitude, -- Longitude
    CASE 
        WHEN c.CityName IN ('Westminster', 'Manchester', 'Birmingham', 'Leeds') THEN 100
        WHEN c.CityName IN ('Camden', 'Islington', 'Liverpool', 'Bristol') THEN 90
        ELSE 80
    END -- SearchRank
FROM [dbo].[Addresses] a
INNER JOIN [dbo].[Streets] s ON a.StreetId = s.StreetId
INNER JOIN [dbo].[Cities] c ON a.CityId = c.CityId
INNER JOIN [dbo].[Counties] co ON a.CountyId = co.CountyId
INNER JOIN [dbo].[Postcodes] p ON a.PostcodeId = p.PostcodeId;

-- Add postcode-only entries to cache for postcode lookups
INSERT INTO [dbo].[AddressLookupCache] 
    ([SearchKey], [DisplayText], [AddressLine1], [AddressLine2], [City], [County], [PostcodeFormatted], [PostcodeId], [AddressId], [Latitude], [Longitude], [SearchRank])
SELECT DISTINCT
    LOWER(REPLACE(p.PostcodeFormatted, ' ', '')), -- SearchKey
    c.CityName + ', ' + p.PostcodeFormatted, -- DisplayText
    c.CityName, -- AddressLine1
    NULL, -- AddressLine2
    c.CityName, -- City
    co.CountyName, -- County
    p.PostcodeFormatted, -- PostcodeFormatted
    p.PostcodeId, -- PostcodeId
    NULL, -- AddressId (no specific address)
    p.Latitude, -- Latitude
    p.Longitude, -- Longitude
    50 -- SearchRank (lower than specific addresses)
FROM [dbo].[Postcodes] p
INNER JOIN [dbo].[Streets] s ON s.PostcodeId = p.PostcodeId
INNER JOIN [dbo].[Cities] c ON s.CityId = c.CityId
INNER JOIN [dbo].[Counties] co ON c.CountyId = co.CountyId;

-- Update statistics
UPDATE STATISTICS [dbo].[Countries];
UPDATE STATISTICS [dbo].[Counties];
UPDATE STATISTICS [dbo].[Cities];
UPDATE STATISTICS [dbo].[Postcodes];
UPDATE STATISTICS [dbo].[PostcodeAreas];
UPDATE STATISTICS [dbo].[PostcodeSectors];
UPDATE STATISTICS [dbo].[Streets];
UPDATE STATISTICS [dbo].[Addresses];
UPDATE STATISTICS [dbo].[AddressLookupCache];

PRINT 'UK Address sample data inserted successfully';
PRINT 'Total Postcodes: ' + CAST((SELECT COUNT(*) FROM Postcodes) AS varchar(10));
PRINT 'Total Streets: ' + CAST((SELECT COUNT(*) FROM Streets) AS varchar(10));
PRINT 'Total Addresses: ' + CAST((SELECT COUNT(*) FROM Addresses) AS varchar(10));
PRINT 'Total Cache Entries: ' + CAST((SELECT COUNT(*) FROM AddressLookupCache) AS varchar(10));