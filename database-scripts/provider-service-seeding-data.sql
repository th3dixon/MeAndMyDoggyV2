-- =============================================
-- Provider Service Seeding Data Script
-- Description: Insert linking records between ServiceProviders and ServiceCategories
-- Based on ProviderService entity from Entity Framework
-- This script must be run BEFORE the pricing-seeding-data.sql script
-- =============================================

-- Ensure we're working with test data
-- CAUTION: This script assumes you have:
-- 1. ServiceProviders records with test data
-- 2. ServiceCategories configured in your database
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
DECLARE @VeterinaryCareCategoryId UNIQUEIDENTIFIER = NEWID() -- Replace with actual ServiceCategoryId
DECLARE @TransportationCategoryId UNIQUEIDENTIFIER = NEWID() -- Replace with actual ServiceCategoryId

-- Clear existing test provider service data (optional - remove if you want to keep existing data)
-- DELETE FROM ProviderService WHERE ProviderId IN (
--     CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
--     CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER), 
--     CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER)
-- )

-- =============================================
-- Provider 1: Happy Paws Pet Care (Full Service Provider)
-- Offers: Dog Walking, Pet Sitting, Grooming, Training
-- =============================================

-- Dog Walking Services - Provider 1
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
    @DogWalkingCategoryId,
    1, -- IsOffered = true
    'Premium dog walking service with GPS tracking and photo updates. All walks include basic obedience reinforcement.',
    15, -- 15 mile service radius
    1,  -- OffersEmergencyService = true
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- Pet Sitting Services - Provider 1
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
    @PetSittingCategoryId,
    1, -- IsOffered = true
    'In-home pet sitting with overnight stays available. Includes feeding, medication administration, and companionship.',
    12, -- 12 mile service radius for sitting
    1,  -- OffersEmergencyService = true
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- Grooming Services - Provider 1
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
    @GroomingCategoryId,
    1, -- IsOffered = true
    'Professional grooming services with mobile grooming van. All breeds welcome, specializing in nervous dogs.',
    20, -- 20 mile service radius for grooming
    0,  -- OffersEmergencyService = false (grooming not typically emergency)
    1,  -- OffersWeekendService = true
    0,  -- OffersEveningService = false
    GETDATE(),
    GETDATE()
)

-- Training Services - Provider 1
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
    @TrainingCategoryId,
    1, -- IsOffered = true
    'Certified dog trainer with 10+ years experience. Specializes in puppy training and behavioral issues.',
    25, -- 25 mile service radius for training
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- =============================================
-- Provider 2: Professional Dog Walkers (Walking Specialist)
-- Offers: Dog Walking, Training (basic)
-- =============================================

-- Dog Walking Services - Provider 2
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER),
    @DogWalkingCategoryId,
    1, -- IsOffered = true
    'Specialized dog walking service with group and individual options. Experienced with reactive and high-energy dogs.',
    10, -- 10 mile service radius
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- Training Services - Provider 2 (basic training during walks)
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER),
    @TrainingCategoryId,
    1, -- IsOffered = true
    'Basic obedience training integrated with walking services. Focus on leash manners and recall training.',
    10, -- 10 mile service radius
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- =============================================
-- Provider 3: Pampered Pets Grooming (Grooming Specialist)
-- Offers: Grooming, Transportation (to grooming salon)
-- =============================================

-- Grooming Services - Provider 3
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER),
    @GroomingCategoryId,
    1, -- IsOffered = true
    'Luxury pet grooming salon with state-of-the-art equipment. Show dog preparation and breed-specific cuts available.',
    8,  -- 8 mile service radius (salon-based)
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true (Saturdays)
    0,  -- OffersEveningService = false
    GETDATE(),
    GETDATE()
)

-- Transportation Services - Provider 3 (pickup/dropoff for grooming)
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER),
    @TransportationCategoryId,
    1, -- IsOffered = true
    'Pickup and delivery service for grooming appointments. Climate-controlled vehicle with safety restraints.',
    15, -- 15 mile pickup radius
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true
    0,  -- OffersEveningService = false
    GETDATE(),
    GETDATE()
)

-- =============================================
-- Additional Service Combinations (Optional)
-- =============================================

-- Example: Provider 1 also offers Veterinary Transportation
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER),
    @TransportationCategoryId,
    1, -- IsOffered = true
    'Transportation to vet appointments, emergency clinics, and other pet-related services.',
    18, -- 18 mile service radius
    1,  -- OffersEmergencyService = true (emergency vet trips)
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- Example: Provider 2 offers Pet Sitting (limited)
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId2 AS UNIQUEIDENTIFIER),
    @PetSittingCategoryId,
    1, -- IsOffered = true
    'Daytime pet sitting and check-ins only. No overnight stays. Perfect for working pet parents.',
    8,  -- 8 mile service radius
    0,  -- OffersEmergencyService = false
    1,  -- OffersWeekendService = true
    1,  -- OffersEveningService = true
    GETDATE(),
    GETDATE()
)

-- =============================================
-- Disabled Services (for testing)
-- =============================================

-- Example: Provider 3 used to offer veterinary care but no longer does
INSERT INTO ProviderService (ProviderServiceId, ProviderId, ServiceCategoryId, IsOffered, SpecialNotes, ServiceRadiusMiles, OffersEmergencyService, OffersWeekendService, OffersEveningService, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    CAST(@ServiceProviderId3 AS UNIQUEIDENTIFIER),
    @VeterinaryCareCategoryId,
    0, -- IsOffered = false (discontinued service)
    'Veterinary care services discontinued as of 2024. Grooming and transportation services still available.',
    0,  -- No service radius when not offered
    0,  -- OffersEmergencyService = false
    0,  -- OffersWeekendService = false
    0,  -- OffersEveningService = false
    GETDATE(),
    GETDATE()
)

-- =============================================
-- Instructions for customization:
-- =============================================
/*
IMPORTANT: Before running this script, you must:

1. Replace all variables at the top with actual IDs from your database:
   - @ServiceProviderId1, @ServiceProviderId2, @ServiceProviderId3 (from ServiceProviders.Id - these are NVARCHAR/string values)
   - @DogWalkingCategoryId, @PetSittingCategoryId, @GroomingCategoryId, etc. (from ServiceCategories.ServiceCategoryId)

2. Ensure these records exist in your database:
   - ServiceProviders with the specified IDs
   - ServiceCategories with the specified IDs

3. Query your database to get the actual IDs:
   SELECT Id, BusinessName FROM ServiceProviders;
   SELECT ServiceCategoryId, Name FROM ServiceCategories;

4. Note the data type conversion: ServiceProvider.Id is NVARCHAR but ProviderService.ProviderId is UNIQUEIDENTIFIER
   The script uses CAST(@ServiceProviderId1 AS UNIQUEIDENTIFIER) to handle this conversion

5. Adjust service radius and capabilities based on your providers' actual offerings

6. Consider adding indexes for performance:
   CREATE INDEX IX_ProviderService_Search ON ProviderService (ProviderId, ServiceCategoryId, IsOffered);

7. Run this script BEFORE running the pricing-seeding-data.sql script

8. Run this script in a test environment first

9. Service capabilities explanations:
   - OffersEmergencyService: Can handle same-day/urgent requests
   - OffersWeekendService: Available on weekends
   - OffersEveningService: Available after 6 PM
   - ServiceRadiusMiles: How far from their base they will travel
*/

PRINT 'Provider service linking data inserted successfully!'
PRINT 'Remember to update all ID variables with actual IDs from your database!'
PRINT 'This script should be run BEFORE the pricing-seeding-data.sql script!'
PRINT 'Check that ServiceProviders and ServiceCategories records exist!'