-- =============================================
-- Cleanup Unused Address Infrastructure
-- Remove all address-related views, procedures, and tables that are no longer needed
-- Keep only postcode-related tables which are still used
-- =============================================

PRINT 'Starting cleanup of unused address infrastructure...';

-- Step 1: Drop address-related views (keep postcode views if any exist)
PRINT 'Dropping address-related views...';

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_AddressSearch'))
BEGIN
    DROP VIEW [dbo].[vw_AddressSearch];
    PRINT 'Dropped view: vw_AddressSearch';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.vw_CommonAddressSearches') AND name = 'IX_vw_CommonAddressSearches')
BEGIN
    DROP INDEX [IX_vw_CommonAddressSearches] ON [dbo].[vw_CommonAddressSearches];
    PRINT 'Dropped index: IX_vw_CommonAddressSearches';
END

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_CommonAddressSearches'))
BEGIN
    DROP VIEW [dbo].[vw_CommonAddressSearches];
    PRINT 'Dropped view: vw_CommonAddressSearches';
END

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_CompleteAddresses'))
BEGIN
    DROP VIEW [dbo].[vw_CompleteAddresses];
    PRINT 'Dropped view: vw_CompleteAddresses';
END

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_StreetDirectory'))
BEGIN
    DROP VIEW [dbo].[vw_StreetDirectory];
    PRINT 'Dropped view: vw_StreetDirectory';
END

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID('dbo.vw_AddressHierarchy'))
BEGIN
    DROP VIEW [dbo].[vw_AddressHierarchy];
    PRINT 'Dropped view: vw_AddressHierarchy';
END

-- Step 2: Drop address-related stored procedures
PRINT 'Dropping address-related stored procedures...';

IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID('dbo.sp_AddressAutocomplete'))
BEGIN
    DROP PROCEDURE [dbo].[sp_AddressAutocomplete];
    PRINT 'Dropped procedure: sp_AddressAutocomplete';
END

IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID('dbo.sp_RefreshAddressCache'))
BEGIN
    DROP PROCEDURE [dbo].[sp_RefreshAddressCache];
    PRINT 'Dropped procedure: sp_RefreshAddressCache';
END

IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID('dbo.sp_UpsertAddress'))
BEGIN
    DROP PROCEDURE [dbo].[sp_UpsertAddress];
    PRINT 'Dropped procedure: sp_UpsertAddress';
END

IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID('dbo.sp_LookupPostcode'))
BEGIN
    DROP PROCEDURE [dbo].[sp_LookupPostcode];
    PRINT 'Dropped procedure: sp_LookupPostcode';
END

-- Step 3: Drop address cache table (not needed anymore)
PRINT 'Dropping address cache table...';

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.AddressLookupCache'))
BEGIN
    DROP TABLE [dbo].[AddressLookupCache];
    PRINT 'Dropped table: AddressLookupCache';
END

-- Step 4: Drop address-related tables (keep postcode tables)
PRINT 'Dropping address and street tables...';

-- Drop dependent tables first (foreign key constraints)
IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.Addresses'))
BEGIN
    DROP TABLE [dbo].[Addresses];
    PRINT 'Dropped table: Addresses';
END

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.Streets'))
BEGIN
    DROP TABLE [dbo].[Streets];
    PRINT 'Dropped table: Streets';
END

-- Step 5: Keep postcode-related tables (these are still used)
PRINT 'Keeping postcode-related tables:';
PRINT '- Postcodes (still used)';
PRINT '- PostcodeAreas (still used)';
PRINT '- PostcodeSectors (still used)';
PRINT '- Cities (still used)';
PRINT '- Counties (still used)';
PRINT '- Countries (still used)';

-- Step 6: Remove any address-related indexes that might still exist
PRINT 'Cleaning up any remaining address-related indexes...';

-- Check for any indexes on dropped tables (should be automatically dropped)
-- This is just for verification

-- Step 7: Update statistics on remaining tables
PRINT 'Updating statistics on remaining tables...';

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.Postcodes'))
BEGIN
    UPDATE STATISTICS [dbo].[Postcodes];
    PRINT 'Updated statistics: Postcodes';
END

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.PostcodeAreas'))
BEGIN
    UPDATE STATISTICS [dbo].[PostcodeAreas];
    PRINT 'Updated statistics: PostcodeAreas';
END

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.PostcodeSectors'))
BEGIN
    UPDATE STATISTICS [dbo].[PostcodeSectors];
    PRINT 'Updated statistics: PostcodeSectors';
END

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('dbo.Cities'))
BEGIN
    UPDATE STATISTICS [dbo].[Cities];
    PRINT 'Updated statistics: Cities';
END

-- Step 8: Show summary of what remains
PRINT 'Cleanup completed successfully!';
PRINT '';
PRINT 'Summary of remaining address-related objects:';

SELECT 
    'Table' as ObjectType,
    TABLE_NAME as ObjectName,
    'Active' as Status
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME IN ('Postcodes', 'PostcodeAreas', 'PostcodeSectors', 'Cities', 'Counties', 'Countries')
UNION ALL
SELECT 
    'View' as ObjectType,
    TABLE_NAME as ObjectName,
    'Active' as Status
FROM INFORMATION_SCHEMA.VIEWS 
WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME LIKE '%Postcode%'
UNION ALL
SELECT 
    'Procedure' as ObjectType,
    ROUTINE_NAME as ObjectName,
    'Active' as Status
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' 
    AND ROUTINE_TYPE = 'PROCEDURE'
    AND ROUTINE_NAME LIKE '%Postcode%'
ORDER BY ObjectType, ObjectName;

PRINT '';
PRINT 'Address autocomplete now uses hybrid API approach:';
PRINT '- Postcodes: Postcodes.io API (free)';
PRINT '- Place names: Postcodes.io API (free)';
PRINT '- Full addresses: Google Places API (paid)';
PRINT '- Server-side endpoint: /Search/GetAddressSuggestions';