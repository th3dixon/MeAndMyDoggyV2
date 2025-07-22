-- =============================================
-- Provider Service Pricing Seeding Data Script
-- Description: Insert realistic pricing data for testing
-- Based on actual Entity Framework table structure
-- =============================================

-- Ensure we're working with test data
-- CAUTION: This script assumes you have:
-- 1. ServiceProviders records with test data
-- 2. ServiceCategories and SubServices configured
-- 3. ProviderService linking records set up
-- Modify IDs to match your actual test data

-- First, let's create some variables for test data
-- You'll need to replace these with actual IDs from your database
DECLARE @ServiceProviderId1 NVARCHAR(450) = 'test-provider-1'  -- Replace with actual ServiceProvider.Id (string)
DECLARE @ServiceProviderId2 NVARCHAR(450) = 'test-provider-2'  -- Replace with actual ServiceProvider.Id (string)
DECLARE @ServiceProviderId3 NVARCHAR(450) = 'test-provider-3'  -- Replace with actual ServiceProvider.Id (string)

-- Common service category GUIDs - replace with actual IDs from your ServiceCategories table
DECLARE @DogWalkingCategoryId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual ServiceCategoryId
DECLARE @PetSittingCategoryId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual ServiceCategoryId
DECLARE @GroomingCategoryId UNIQUEIDENTIFIER = NEWID()    -- Replace with actual ServiceCategoryId
DECLARE @TrainingCategoryId UNIQUEIDENTIFIER = NEWID()    -- Replace with actual ServiceCategoryId

-- Common sub-service GUIDs - replace with actual IDs from your SubServices table
-- Dog Walking Sub-Services
DECLARE @BasicWalkSubServiceId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual SubServiceId
DECLARE @LongWalkSubServiceId UNIQUEIDENTIFIER = NEWID()   -- Replace with actual SubServiceId
DECLARE @GroupWalkSubServiceId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual SubServiceId

-- Pet Sitting Sub-Services  
DECLARE @DaySittingSubServiceId UNIQUEIDENTIFIER = NEWID() -- Replace with actual SubServiceId
DECLARE @OvernightSubServiceId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual SubServiceId
DECLARE @WeekendSubServiceId UNIQUEIDENTIFIER = NEWID()    -- Replace with actual SubServiceId

-- Grooming Sub-Services
DECLARE @BasicGroomSubServiceId UNIQUEIDENTIFIER = NEWID() -- Replace with actual SubServiceId
DECLARE @FullGroomSubServiceId UNIQUEIDENTIFIER = NEWID()  -- Replace with actual SubServiceId
DECLARE @BathOnlySubServiceId UNIQUEIDENTIFIER = NEWID()   -- Replace with actual SubServiceId

-- Clear existing test pricing data (optional - remove if you want to keep existing data)
-- DELETE FROM ProviderServicePricing WHERE ProviderServiceId IN (
--     SELECT ps.ProviderServiceId FROM ProviderService ps 
--     WHERE ps.ProviderId IN (
--         CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
--         CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER), 
--         CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER)
--     )
-- )

-- =============================================
-- Provider 1: Happy Paws Pet Care (Premium pricing)
-- =============================================

-- Dog Walking Services for Provider 1
-- Basic Walk (30 minutes) - Provider 1
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @BasicWalkSubServiceId,
    15.00,  -- £15 per service
    1,      -- PerService (from PricingType enum)
    1,      -- Available
    2,      -- 2 hours advance booking
    14,     -- 14 days max advance booking
    1,      -- Has weekend surcharge
    10.00,  -- 10% weekend surcharge
    0,      -- No evening surcharge
    NULL,
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- Long Walk (60 minutes) - Provider 1
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @LongWalkSubServiceId,
    25.00,  -- £25 per service
    1,      -- PerService
    1,
    2,
    14,
    1,
    10.00,
    1,      -- Has evening surcharge
    15.00,  -- 15% evening surcharge
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- Pet Sitting Services for Provider 1
-- Day Sitting - Provider 1
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @DaySittingSubServiceId,
    45.00,  -- £45 per day
    3,      -- PerDay
    1,
    24,     -- 24 hours advance booking for sitting
    21,     -- 21 days max advance booking
    1,
    15.00,  -- 15% weekend surcharge
    0,
    NULL,
    'Includes feeding, walks, and companionship',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @PetSittingCategoryId
  AND ps.IsOffered = 1

-- Overnight Sitting - Provider 1  
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @OvernightSubServiceId,
    65.00,  -- £65 per night
    4,      -- PerNight
    1,
    48,     -- 48 hours advance booking for overnight
    30,     -- 30 days max advance booking
    1,
    20.00,  -- 20% weekend surcharge
    0,
    NULL,
    'Full overnight care at your home',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @PetSittingCategoryId
  AND ps.IsOffered = 1

-- Grooming Services for Provider 1
-- Full Grooming - Provider 1
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @FullGroomSubServiceId,
    75.00,  -- £75 per service
    1,      -- PerService
    1,
    24,     -- 24 hours advance booking
    14,     -- 14 days max advance booking
    1,
    10.00,
    0,
    NULL,
    'Complete grooming service with bath, cut, nails',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @GroomingCategoryId
  AND ps.IsOffered = 1

-- =============================================
-- Provider 2: Professional Dog Walkers (Mid-range pricing)
-- =============================================

-- Basic Walk - Provider 2 (lower price)
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @BasicWalkSubServiceId,
    12.00,  -- £12 per service
    1,      -- PerService
    1,
    1,      -- 1 hour advance booking
    7,      -- 7 days max advance booking
    1,
    5.00,   -- 5% weekend surcharge
    0,
    NULL,
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- Group Walk - Provider 2 (specialized service)
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @GroupWalkSubServiceId,
    8.00,   -- £8 per service (group discount)
    1,      -- PerService
    1,
    2,      -- 2 hours advance booking for group coordination
    7,
    1,
    5.00,
    0,
    NULL,
    'Socialization group walks with max 4 dogs',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- Long Walk - Provider 2
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @LongWalkSubServiceId,
    18.00,  -- £18 per service
    1,      -- PerService
    1,
    1,
    7,
    1,
    5.00,
    1,
    10.00,  -- 10% evening surcharge
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- =============================================
-- Provider 3: Pampered Pets Grooming (Specialist pricing)
-- =============================================

-- Basic Grooming - Provider 3
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @BasicGroomSubServiceId,
    45.00,  -- £45 per service
    1,      -- PerService
    1,
    12,     -- 12 hours advance booking
    21,     -- 21 days max advance booking
    1,
    15.00,  -- 15% weekend surcharge
    0,
    NULL,
    'Professional grooming with basic cut and wash',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @GroomingCategoryId
  AND ps.IsOffered = 1

-- Full Grooming - Provider 3 (premium specialist)
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @FullGroomSubServiceId,
    85.00,  -- £85 per service (specialist premium)
    1,      -- PerService
    1,
    24,     -- 24 hours advance booking for premium service
    28,     -- 28 days max advance booking
    1,
    15.00,
    0,
    NULL,
    'Premium grooming with specialty styling and treatments',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @GroomingCategoryId
  AND ps.IsOffered = 1

-- Bath Only - Provider 3
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @BathOnlySubServiceId,
    25.00,  -- £25 per service
    1,      -- PerService
    1,
    6,      -- 6 hours advance booking
    14,     -- 14 days max advance booking
    1,
    15.00,
    0,
    NULL,
    'Professional bath and dry service',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @GroomingCategoryId
  AND ps.IsOffered = 1

-- =============================================
-- Package Deal Pricing (Multi-service discounts)
-- =============================================

-- Weekly Dog Walking Package - Provider 1
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @BasicWalkSubServiceId,
    60.00,  -- £60 per week (5 walks = £12 each, discounted from £15)
    5,      -- PerWeek
    1,
    24,     -- 24 hours advance booking for packages
    14,
    0,      -- No weekend surcharge for packages
    NULL,
    0,
    NULL,
    'Weekly package: 5 walks at discounted rate',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @DogWalkingCategoryId
  AND ps.IsOffered = 1

-- Monthly Pet Sitting Package - Provider 1  
INSERT INTO ProviderServicePricing (ProviderServicePricingId, ProviderServiceId, SubServiceId, Price, PricingType, IsAvailable, MinAdvanceBookingHours, MaxAdvanceBookingDays, HasWeekendSurcharge, WeekendSurchargePercentage, HasEveningSurcharge, EveningSurchargePercentage, Notes, CreatedAt, UpdatedAt)
SELECT 
    NEWID(),
    ps.ProviderServiceId,
    @DaySittingSubServiceId,
    900.00, -- £900 per month (20 days = £45 each, bulk discount)
    6,      -- PerMonth
    1,
    168,    -- 1 week advance booking for monthly packages
    60,     -- 60 days max advance booking
    0,      -- No surcharges for packages
    NULL,
    0,
    NULL,
    'Monthly package: 20 days of pet sitting at discounted rate',
    GETDATE(),
    GETDATE()
FROM ProviderService ps
WHERE ps.ProviderId = CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER)
  AND ps.ServiceCategoryId = @PetSittingCategoryId
  AND ps.IsOffered = 1

-- =============================================
-- Instructions for customization:
-- =============================================
/*
IMPORTANT: Before running this script, you must:

1. Replace all variables at the top with actual IDs from your database:
   - @ServiceProviderId1, @ServiceProviderId2, @ServiceProviderId3 (from ServiceProviders.Id - these are NVARCHAR/string values)
   - @DogWalkingCategoryId, @PetSittingCategoryId, @GroomingCategoryId, etc. (from ServiceCategories.ServiceCategoryId)
   - @BasicWalkSubServiceId, @LongWalkSubServiceId, etc. (from SubServices.SubServiceId)

2. Ensure these records exist in your database:
   - ServiceProviders with the specified IDs
   - ServiceCategories with the specified IDs  
   - SubServices with the specified IDs
   - ProviderService linking records connecting ServiceProviders to ServiceCategories

3. Query your database to get the actual IDs:
   SELECT Id, BusinessName FROM ServiceProviders;
   SELECT ServiceCategoryId, Name FROM ServiceCategories;
   SELECT SubServiceId, Name, ServiceCategoryId FROM SubServices;
   SELECT ProviderServiceId, ProviderId, ServiceCategoryId FROM ProviderService WHERE IsOffered = 1;

4. Note the data type conversion: ServiceProvider.Id is NVARCHAR but ProviderService.ProviderId is UNIQUEIDENTIFIER
   The script uses CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER) to handle this conversion

5. Adjust prices to match your market rates

6. Consider adding indexes for performance:
   CREATE INDEX IX_ProviderServicePricing_Search ON ProviderServicePricing (ProviderServiceId, SubServiceId, IsAvailable);

7. Run this script in a test environment first

8. PricingType enum values:
   1 = PerService, 2 = PerHour, 3 = PerDay, 4 = PerNight, 5 = PerWeek, 6 = PerMonth
*/

PRINT 'Provider service pricing seeding data inserted successfully!'
PRINT 'Remember to update all ID variables with actual IDs from your database!'
PRINT 'Check that ServiceProviders, ServiceCategories, SubServices, and ProviderService records exist!'
PRINT 'Note: ServiceProvider IDs are strings but get converted to GUIDs for ProviderService.ProviderId!'