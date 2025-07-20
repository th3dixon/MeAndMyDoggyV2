-- =============================================
-- TEST SEED DATA FOR MEANDMYDOGGY PLATFORM
-- =============================================
-- WARNING: THIS IS TEST DATA ONLY
-- These users are marked with 'TESTUSER_' prefix for easy identification and deletion
-- DO NOT USE IN PRODUCTION - DELETE BEFORE GO-LIVE
-- =============================================

USE [MeAndMyDoggyV2]; -- Change to your database name
GO

-- Set consistent timestamp for all test data
DECLARE @TestDataTimestamp DATETIMEOFFSET = SYSDATETIMEOFFSET();
DECLARE @FutureDate DATETIMEOFFSET = DATEADD(YEAR, 1, @TestDataTimestamp);

-- First, let's get the ServiceCategory IDs we'll need (assuming ServiceCatalogSeeder has run)
DECLARE @DogWalkingId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Dog Walking');
DECLARE @PetSittingId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Pet Sitting');
DECLARE @GroomingId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Grooming');
DECLARE @TrainingId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Training');
DECLARE @BoardingId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Boarding');
DECLARE @VetServicesId UNIQUEIDENTIFIER = (SELECT TOP 1 ServiceCategoryId FROM ServiceCategories WHERE Name = 'Veterinary Services');

PRINT 'Creating test seed data for MeAndMyDoggy platform...';
PRINT 'WARNING: This is TEST DATA ONLY - marked with TESTUSER_ prefix';
PRINT '';

-- =============================================
-- 1. CREATE SUBSCRIPTION PLANS (if they don't exist)
-- =============================================
PRINT 'Creating subscription plans...';

IF NOT EXISTS (SELECT 1 FROM SubscriptionPlans WHERE Id = 'free-plan')
BEGIN
    INSERT INTO SubscriptionPlans (Id, Name, Description, Price, BillingCycle, Features, MaxDogProfiles, MaxAppointments, HasAIFeatures, HasPrioritySupport, IsActive, CreatedAt, UpdatedAt)
    VALUES 
    ('free-plan', 'Free Plan', 'Basic access to platform features', 0.00, 'None', '["Basic Search", "Contact Providers", "Basic Profile"]', 2, 10, 0, 0, 1, @TestDataTimestamp, @TestDataTimestamp);
END

IF NOT EXISTS (SELECT 1 FROM SubscriptionPlans WHERE Id = 'premium-plan')
BEGIN
    INSERT INTO SubscriptionPlans (Id, Name, Description, Price, BillingCycle, Features, MaxDogProfiles, MaxAppointments, HasAIFeatures, HasPrioritySupport, IsActive, CreatedAt, UpdatedAt)
    VALUES 
    ('premium-plan', 'Premium Plan', 'Full access with premium features', 19.99, 'Monthly', '["Advanced Search", "AI Recommendations", "Priority Support", "Unlimited Bookings", "Advanced Analytics"]', 10, 999, 1, 1, 1, @TestDataTimestamp, @TestDataTimestamp);
END

-- =============================================
-- 2. CREATE TEST USERS (SERVICE PROVIDERS AND PET OWNERS)
-- =============================================
PRINT 'Creating test users...';

-- Helper: Generate password hash (simplified for demo - in real app use proper ASP.NET Identity)
DECLARE @PasswordHash NVARCHAR(MAX) = 'AQAAAAEAACcQAAAAEKm5TQHjRJj1Cz+p5FKGQXJvDjdLK1qDg2j3JMhTN8F9J1FKl9QvT7cXpWb2YqZ1sA=='; -- Password: TestPassword123!

-- Create Service Providers (Premium and Non-Premium)
INSERT INTO Users (Id, FirstName, LastName, Email, UserName, UserType, AddressLine1, AddressLine2, City, County, PostCode, Latitude, Longitude, IsActive, IsKYCVerified, AccountStatus, SubscriptionType, TimeZone, PreferredLanguage, CreatedAt, UpdatedAt, PasswordHash, SecurityStamp, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES 
-- Premium Service Providers
('TESTUSER_prov_001', 'Sarah', 'Thompson', 'sarah.thompson@testdoggy.com', 'sarah.thompson@testdoggy.com', 2, '15 Oak Avenue', '', 'Manchester', 'Greater Manchester', 'M1 4AE', 53.4808, -2.2426, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_002', 'James', 'Wilson', 'james.wilson@testdoggy.com', 'james.wilson@testdoggy.com', 2, '42 Meadow Lane', 'Clifton', 'Bristol', 'Bristol', 'BS8 2HG', 51.4545, -2.5879, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_003', 'Emma', 'Davies', 'emma.davies@testdoggy.com', 'emma.davies@testdoggy.com', 2, '7 Rose Gardens', '', 'Edinburgh', 'Edinburgh', 'EH1 2NG', 55.9533, -3.1883, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_004', 'Michael', 'Brown', 'michael.brown@testdoggy.com', 'michael.brown@testdoggy.com', 2, '23 Victoria Street', '', 'Birmingham', 'West Midlands', 'B1 1BB', 52.4862, -1.8904, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_005', 'Lisa', 'Anderson', 'lisa.anderson@testdoggy.com', 'lisa.anderson@testdoggy.com', 2, '156 High Street', '', 'Canterbury', 'Kent', 'CT1 2RX', 51.2802, 1.0789, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),

-- Non-Premium Service Providers
('TESTUSER_prov_006', 'David', 'Miller', 'david.miller@testdoggy.com', 'david.miller@testdoggy.com', 2, '89 Church Lane', '', 'Leeds', 'West Yorkshire', 'LS1 3AB', 53.8008, -1.5491, 1, 1, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_007', 'Rachel', 'Taylor', 'rachel.taylor@testdoggy.com', 'rachel.taylor@testdoggy.com', 2, '34 Elm Street', '', 'Norwich', 'Norfolk', 'NR1 1HG', 52.6309, 1.2974, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_008', 'Tom', 'Roberts', 'tom.roberts@testdoggy.com', 'tom.roberts@testdoggy.com', 2, '12 Park Road', '', 'Bath', 'Somerset', 'BA1 2NJ', 51.3811, -2.3590, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_009', 'Sophie', 'Clark', 'sophie.clark@testdoggy.com', 'sophie.clark@testdoggy.com', 2, '67 Mill Lane', '', 'Newcastle', 'Tyne and Wear', 'NE1 4ST', 54.9783, -1.6178, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_prov_010', 'Daniel', 'Harris', 'daniel.harris@testdoggy.com', 'daniel.harris@testdoggy.com', 2, '145 Station Road', '', 'Cardiff', 'Cardiff', 'CF10 1EP', 51.4816, -3.1791, 1, 1, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),

-- Pet Owners (Premium and Non-Premium)
('TESTUSER_owner_001', 'Jessica', 'Green', 'jessica.green@testdoggy.com', 'jessica.green@testdoggy.com', 1, '78 Maple Drive', '', 'London', 'Greater London', 'SW1A 1AA', 51.5014, -0.1419, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_owner_002', 'Mark', 'Johnson', 'mark.johnson@testdoggy.com', 'mark.johnson@testdoggy.com', 1, '23 Hillside Crescent', '', 'Liverpool', 'Merseyside', 'L1 8JQ', 53.4084, -2.9916, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_owner_003', 'Laura', 'White', 'laura.white@testdoggy.com', 'laura.white@testdoggy.com', 1, '56 Garden Close', '', 'Sheffield', 'South Yorkshire', 'S1 2HE', 53.3811, -1.4701, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_owner_004', 'Chris', 'Evans', 'chris.evans@testdoggy.com', 'chris.evans@testdoggy.com', 1, '91 Sunset Boulevard', '', 'Brighton', 'East Sussex', 'BN1 1UB', 50.8225, -0.1372, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_owner_005', 'Amy', 'Martin', 'amy.martin@testdoggy.com', 'amy.martin@testdoggy.com', 1, '18 Riverside Walk', '', 'York', 'North Yorkshire', 'YO1 7HB', 53.9600, -1.0873, 1, 1, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),

-- Mixed Users (Both Pet Owners and Service Providers)
('TESTUSER_both_001', 'Robert', 'Lee', 'robert.lee@testdoggy.com', 'robert.lee@testdoggy.com', 3, '44 Orchard Street', '', 'Nottingham', 'Nottinghamshire', 'NG1 5DT', 52.9548, -1.1581, 1, 1, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_both_002', 'Helen', 'Walker', 'helen.walker@testdoggy.com', 'helen.walker@testdoggy.com', 3, '73 Meadowbrook Lane', '', 'Cambridge', 'Cambridgeshire', 'CB2 1AB', 52.2053, 0.1218, 1, 1, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_both_003', 'Peter', 'King', 'peter.king@testdoggy.com', 'peter.king@testdoggy.com', 3, '29 Woodland Avenue', '', 'Oxford', 'Oxfordshire', 'OX1 2JD', 51.7520, -1.2577, 1, 0, 'Active', 'Premium', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_both_004', 'Kelly', 'Turner', 'kelly.turner@testdoggy.com', 'kelly.turner@testdoggy.com', 3, '85 Cherry Tree Close', '', 'Preston', 'Lancashire', 'PR1 2HU', 53.7632, -2.7031, 1, 0, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0),
('TESTUSER_both_005', 'Andrew', 'Phillips', 'andrew.phillips@testdoggy.com', 'andrew.phillips@testdoggy.com', 3, '103 Valley Road', '', 'Exeter', 'Devon', 'EX1 1SY', 50.7184, -3.5339, 1, 1, 'Active', 'Free', 'Europe/London', 'en', @TestDataTimestamp, @TestDataTimestamp, @PasswordHash, NEWID(), 1, 0, 0, 1, 0);

PRINT 'Created 20 test users (10 providers, 5 owners, 5 both)';

-- =============================================
-- 3. CREATE USER SUBSCRIPTIONS
-- =============================================
PRINT 'Creating user subscriptions...';

-- Premium subscriptions
INSERT INTO UserSubscriptions (Id, UserId, SubscriptionPlanId, StartDate, EndDate, Status, PaidAmount, NextBillingDate, CreatedAt, UpdatedAt)
SELECT 
    'SUB_' + u.Id,
    u.Id,
    'premium-plan',
    @TestDataTimestamp,
    @FutureDate,
    'Active',
    19.99,
    DATEADD(MONTH, 1, @TestDataTimestamp),
    @TestDataTimestamp,
    @TestDataTimestamp
FROM Users u 
WHERE u.Id LIKE 'TESTUSER_%' AND u.SubscriptionType = 'Premium';

-- Free subscriptions
INSERT INTO UserSubscriptions (Id, UserId, SubscriptionPlanId, StartDate, EndDate, Status, PaidAmount, NextBillingDate, CreatedAt, UpdatedAt)
SELECT 
    'SUB_' + u.Id,
    u.Id,
    'free-plan',
    @TestDataTimestamp,
    NULL,
    'Active',
    0.00,
    NULL,
    @TestDataTimestamp,
    @TestDataTimestamp
FROM Users u 
WHERE u.Id LIKE 'TESTUSER_%' AND u.SubscriptionType = 'Free';

PRINT 'Created user subscriptions for test users';

-- =============================================
-- 4. CREATE SERVICE PROVIDERS
-- =============================================
PRINT 'Creating service provider profiles...';

INSERT INTO ServiceProviders (Id, UserId, BusinessName, BusinessDescription, BusinessAddress, BusinessPhone, BusinessEmail, BusinessWebsite, BusinessLicense, InsurancePolicy, YearsOfExperience, Specializations, ServiceAreas, HourlyRate, Rating, ReviewCount, IsActive, IsVerified, TimeZone, CreatedAt, UpdatedAt)
VALUES 
-- Premium Service Providers
('SP_001', 'TESTUSER_prov_001', 'Paws & Claws Pet Services', 'Professional dog walking and pet sitting services in Manchester. Fully insured and DBS checked.', '15 Oak Avenue, Manchester, M1 4AE', '07700123456', 'sarah.thompson@testdoggy.com', 'www.pawsandclaws-manchester.co.uk', 'LIC-MAN-2023-001', 'INS-PAC-789456', 5, '["Large Dogs", "Puppy Care", "Senior Dogs", "Medication Administration"]', '["Manchester City Centre", "Didsbury", "Chorlton", "Withington"]', 25.00, 4.8, 127, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_002', 'TESTUSER_prov_002', 'Bristol Pooch Paradise', 'Complete grooming and training services with 8 years experience. Specializing in nervous and rescue dogs.', '42 Meadow Lane, Clifton, Bristol, BS8 2HG', '07700234567', 'james.wilson@testdoggy.com', 'www.bristolpoochparadise.com', 'LIC-BRI-2023-002', 'INS-BPP-456789', 8, '["Dog Grooming", "Behavioral Training", "Rescue Dogs", "Anxiety Management"]', '["Clifton", "Redland", "Bishopston", "Central Bristol"]', 35.00, 4.9, 89, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_003', 'TESTUSER_prov_003', 'Edinburgh Canine Care', 'Premium pet boarding and sitting services in Edinburgh. Your dog''s home away from home!', '7 Rose Gardens, Edinburgh, EH1 2NG', '07700345678', 'emma.davies@testdoggy.com', 'www.edinburgh-caninecare.scot', 'LIC-EDI-2023-003', 'INS-ECC-123456', 6, '["Pet Boarding", "Overnight Care", "Holiday Sitting", "Multiple Dogs"]', '["Edinburgh City Centre", "Leith", "Morningside", "Bruntsfield"]', 30.00, 4.7, 156, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_004', 'TESTUSER_prov_004', 'Midlands Mobile Vet Services', 'Convenient veterinary transport and health monitoring services. Taking the stress out of vet visits.', '23 Victoria Street, Birmingham, B1 1BB', '07700456789', 'michael.brown@testdoggy.com', 'www.midlands-mobile-vet.co.uk', 'LIC-BIR-2023-004', 'INS-MMV-987654', 12, '["Vet Transport", "Health Monitoring", "Emergency Transport", "Senior Dog Care"]', '["Birmingham", "Solihull", "Sutton Coldfield", "West Bromwich"]', 40.00, 4.6, 203, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_005', 'TESTUSER_prov_005', 'Kent Countryside Dog Adventures', 'Adventure walks and exercise for active dogs. Exploring the beautiful Kent countryside safely.', '156 High Street, Canterbury, CT1 2RX', '07700567890', 'lisa.anderson@testdoggy.com', 'www.kent-dogadventures.com', 'LIC-KEN-2023-005', 'INS-KDA-654321', 4, '["Adventure Walks", "Trail Walking", "Active Dogs", "Group Activities"]', '["Canterbury", "Whitstable", "Herne Bay", "Faversham"]', 28.00, 4.9, 78, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

-- Non-Premium Service Providers
('SP_006', 'TESTUSER_prov_006', 'Leeds Local Dog Walker', 'Reliable dog walking services in Leeds. Building lasting relationships with pets and owners.', '89 Church Lane, Leeds, LS1 3AB', '07700678901', 'david.miller@testdoggy.com', '', 'LIC-LEE-2023-006', 'INS-LLD-111222', 3, '["Dog Walking", "Basic Pet Sitting"]', '["Leeds City Centre", "Headingley", "Hyde Park"]', 18.00, 4.3, 45, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_007', 'TESTUSER_prov_007', 'Norwich Pet Helper', 'Friendly pet care services in Norwich. Perfect for busy pet parents who need reliable help.', '34 Elm Street, Norwich, NR1 1HG', '07700789012', 'rachel.taylor@testdoggy.com', '', '', '', 2, '["Pet Sitting", "Drop-in Visits"]', '["Norwich City Centre", "Eaton", "Unthank Road"]', 15.00, 4.1, 23, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_008', 'TESTUSER_prov_008', 'Bath Dog Walking Co', 'Simple, honest dog walking in the historic city of Bath. No fuss, just great care for your pet.', '12 Park Road, Bath, BA1 2NJ', '07700890123', 'tom.roberts@testdoggy.com', '', '', '', 1, '["Dog Walking", "Puppy Visits"]', '["Bath City Centre", "Oldfield Park"]', 16.00, 3.9, 12, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_009', 'TESTUSER_prov_009', 'Tyneside Tail Waggers', 'Fun and energetic dog services in Newcastle. Keeping your pets happy and healthy!', '67 Mill Lane, Newcastle, NE1 4ST', '07700901234', 'sophie.clark@testdoggy.com', '', 'LIC-NEW-2023-009', '', 2, '["Dog Walking", "Play Sessions", "Puppy Care"]', '["Newcastle City Centre", "Jesmond", "Gosforth"]', 17.00, 4.2, 34, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_010', 'TESTUSER_prov_010', 'Cardiff Canine Companions', 'Bilingual pet services in Cardiff. Gwasanaethau anifeiliaid anwes dwyieithog yng Nghaerdydd.', '145 Station Road, Cardiff, CF10 1EP', '07701012345', 'daniel.harris@testdoggy.com', 'www.cardiff-canine.wales', 'LIC-CAR-2023-010', 'INS-CCC-555666', 7, '["Dog Walking", "Pet Sitting", "Bilingual Services"]', '["Cardiff City Centre", "Cardiff Bay", "Cathays", "Roath"]', 20.00, 4.5, 67, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

-- Mixed Users (Both)
('SP_011', 'TESTUSER_both_001', 'Nottingham Paws & More', 'Experienced dog trainer and walker. Understanding dogs from both owner and provider perspective.', '44 Orchard Street, Nottingham, NG1 5DT', '07701123456', 'robert.lee@testdoggy.com', 'www.nottingham-paws.co.uk', 'LIC-NOT-2023-011', 'INS-NPM-777888', 9, '["Dog Training", "Behavioral Issues", "Walking", "Owner Education"]', '["Nottingham City Centre", "West Bridgford", "Beeston"]', 32.00, 4.8, 94, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_012', 'TESTUSER_both_002', 'Cambridge Pet Care Plus', 'Academic city pet care with flexible scheduling for busy professionals and students.', '73 Meadowbrook Lane, Cambridge, CB2 1AB', '07701234567', 'helen.walker@testdoggy.com', '', '', '', 3, '["Flexible Scheduling", "University Area", "Professional Pet Care"]', '["Cambridge City Centre", "Cherry Hinton", "Trumpington"]', 22.00, 4.4, 51, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_013', 'TESTUSER_both_003', 'Oxford Premium Pet Services', 'High-end pet care for discerning Oxford residents. Premium service, premium results.', '29 Woodland Avenue, Oxford, OX1 2JD', '07701345678', 'peter.king@testdoggy.com', 'www.oxford-premium-pets.co.uk', 'LIC-OXF-2023-013', 'INS-OPP-999000', 11, '["Premium Services", "Luxury Pet Care", "Concierge Services", "Show Dogs"]', '["Oxford City Centre", "Summertown", "Jericho", "Headington"]', 45.00, 4.9, 112, 1, 1, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_014', 'TESTUSER_both_004', 'Preston Pet Pals', 'Community-focused pet services in Preston. Building connections between pets, owners, and carers.', '85 Cherry Tree Close, Preston, PR1 2HU', '07701456789', 'kelly.turner@testdoggy.com', '', '', '', 4, '["Community Services", "Group Activities", "Social Dogs"]', '["Preston City Centre", "Fulwood", "Ingol"]', 19.00, 4.0, 28, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp),

('SP_015', 'TESTUSER_both_005', 'Devon Dogs & Countryside', 'Rural and countryside pet services in beautiful Devon. Perfect for dogs who love the outdoors.', '103 Valley Road, Exeter, EX1 1SY', '07701567890', 'andrew.phillips@testdoggy.com', '', 'LIC-DEV-2023-015', '', 6, '["Countryside Walks", "Rural Pet Care", "Adventure Dogs", "Natural Environment"]', '["Exeter", "Topsham", "Heavitree", "St Thomas"]', 24.00, 4.6, 73, 1, 0, 'Europe/London', @TestDataTimestamp, @TestDataTimestamp);

PRINT 'Created 15 service provider profiles';

-- =============================================
-- 5. CREATE DOG PROFILES FOR PET OWNERS
-- =============================================
PRINT 'Creating dog profiles...';

INSERT INTO DogProfiles (Id, OwnerId, Name, Breed, SecondaryBreed, DateOfBirth, Weight, Height, Gender, IsNeutered, CoatColor, CoatType, ProfileImageUrl, Notes, IsActive, CreatedAt, UpdatedAt)
VALUES 
-- Dogs for Pet Owners
('DOG_001', 'TESTUSER_owner_001', 'Max', 'Golden Retriever', NULL, DATEADD(YEAR, -3, @TestDataTimestamp), 28.5, 60.0, 'Male', 1, 'Golden', 'Medium', '/images/dogs/golden-retriever-1.jpg', 'Very friendly with other dogs and people. Loves treats! Vaccinations up to date. Enjoys walking, fetch, and swimming.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_002', 'TESTUSER_owner_001', 'Luna', 'Border Collie', NULL, DATEADD(YEAR, -2, @TestDataTimestamp), 18.2, 50.0, 'Female', 1, 'Black and White', 'Medium', '/images/dogs/border-collie-1.jpg', 'High energy, very intelligent. Needs mental stimulation. Vaccinations up to date. Loves agility, training, and mental stimulation activities.', 1, @TestDataTimestamp, @TestDataTimestamp),

('DOG_003', 'TESTUSER_owner_002', 'Buddy', 'Labrador Retriever', NULL, DATEADD(YEAR, -5, @TestDataTimestamp), 32.1, 58.0, 'Male', 1, 'Yellow', 'Short', '/images/dogs/labrador-1.jpg', 'Senior dog with mild arthritis in hind legs. Prefers shorter walks. Vaccinations up to date. Enjoys gentle walks and swimming.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_004', 'TESTUSER_owner_002', 'Bella', 'French Bulldog', NULL, DATEADD(MONTH, -8, @TestDataTimestamp), 11.3, 30.0, 'Female', 0, 'Brindle', 'Short', '/images/dogs/french-bulldog-1.jpg', 'Young puppy, very playful but tires easily. Loves socializing. Vaccinations up to date. Enjoys short walks and indoor play.', 1, @TestDataTimestamp, @TestDataTimestamp),

('DOG_005', 'TESTUSER_owner_003', 'Charlie', 'German Shepherd', 'Husky', DATEADD(YEAR, -4, @TestDataTimestamp), 35.7, 65.0, 'Male', 1, 'Black and Tan', 'Medium', '/images/dogs/german-shepherd-1.jpg', 'Protective but well-trained. Excellent with children. Vaccinations up to date. Enjoys long walks, training, and protection work.', 1, @TestDataTimestamp, @TestDataTimestamp),

('DOG_006', 'TESTUSER_owner_004', 'Daisy', 'Cocker Spaniel', NULL, DATEADD(YEAR, -6, @TestDataTimestamp), 14.8, 38.0, 'Female', 1, 'Golden', 'Medium', '/images/dogs/cocker-spaniel-1.jpg', 'Sweet older lady, loves gentle walks and meeting people. Vaccinations up to date. Enjoys moderate walks, sniffing, and gentle play.', 1, @TestDataTimestamp, @TestDataTimestamp),

('DOG_007', 'TESTUSER_owner_005', 'Rocky', 'Staffordshire Bull Terrier', NULL, DATEADD(YEAR, -3, @TestDataTimestamp), 16.2, 45.0, 'Male', 1, 'Blue', 'Short', '/images/dogs/staffy-1.jpg', 'Strong and energetic. Great with kids, can be selective with other dogs. Vaccinations up to date. Enjoys vigorous exercise, tug of war, and ball games.', 1, @TestDataTimestamp, @TestDataTimestamp),

-- Dogs for Mixed Users (Both)
('DOG_008', 'TESTUSER_both_001', 'Oscar', 'Poodle', NULL, DATEADD(YEAR, -2, @TestDataTimestamp), 20.4, 45.0, 'Male', 1, 'Black', 'Curly', '/images/dogs/poodle-1.jpg', 'Well-behaved, good example for training other dogs. Vaccinations up to date. Enjoys grooming, walking, and training sessions.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_009', 'TESTUSER_both_002', 'Molly', 'Jack Russell Terrier', NULL, DATEADD(YEAR, -4, @TestDataTimestamp), 6.8, 25.0, 'Female', 1, 'White and Brown', 'Smooth', '/images/dogs/jack-russell-1.jpg', 'Small but mighty! Very energetic and intelligent. Vaccinations up to date. Enjoys high energy play, agility, and digging.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_010', 'TESTUSER_both_003', 'Duchess', 'Afghan Hound', NULL, DATEADD(YEAR, -5, @TestDataTimestamp), 26.3, 68.0, 'Female', 1, 'Cream', 'Long', '/images/dogs/afghan-hound-1.jpg', 'Regal and elegant. Requires specialized grooming care. Vaccinations up to date. Enjoys elegant walks, grooming, and show training.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_011', 'TESTUSER_both_004', 'Buster', 'Beagle', NULL, DATEADD(YEAR, -1, @TestDataTimestamp), 12.1, 35.0, 'Male', 0, 'Tri-color', 'Short', '/images/dogs/beagle-1.jpg', 'Young and curious. Loves following scents and meeting other dogs. Vaccinations up to date. Enjoys scent work, walking, and socializing.', 1, @TestDataTimestamp, @TestDataTimestamp),
('DOG_012', 'TESTUSER_both_005', 'Willow', 'Border Terrier', NULL, DATEADD(YEAR, -7, @TestDataTimestamp), 7.2, 28.0, 'Female', 1, 'Grizzle', 'Wiry', '/images/dogs/border-terrier-1.jpg', 'Senior lady with mild heart murmur. Needs gentle exercise only. Vaccinations up to date. Enjoys gentle walks and light play.', 1, @TestDataTimestamp, @TestDataTimestamp);

PRINT 'Created 12 dog profiles for pet owners';

-- =============================================
-- 6. LINK SERVICE PROVIDERS TO SERVICE CATEGORIES
-- =============================================
PRINT 'Linking service providers to service categories...';

-- We need to create ProviderService records linking providers to service categories
-- Note: The ProviderService table appears to use Guid for ProviderId, but ServiceProviders uses string Id
-- This may need schema adjustment, but for now we'll create the data structure

-- Since we can't easily convert between string and GUID IDs, let's create the linking data
-- using a mapping approach

DECLARE @ProviderServiceMappings TABLE (
    ProviderServiceId UNIQUEIDENTIFIER,
    ServiceProviderId NVARCHAR(450),
    ServiceCategoryId UNIQUEIDENTIFIER,
    IsOffered BIT,
    SpecialNotes NVARCHAR(1000),
    ServiceRadiusMiles INT,
    OffersEmergencyService BIT,
    OffersWeekendService BIT,
    OffersEveningService BIT
);

-- Insert provider service mappings
INSERT INTO @ProviderServiceMappings VALUES
-- Premium providers with multiple services
(NEWID(), 'SP_001', @DogWalkingId, 1, 'Professional dog walking with GPS tracking and photo updates', 10, 1, 1, 1),
(NEWID(), 'SP_001', @PetSittingId, 1, 'Drop-in visits and overnight care available', 8, 1, 1, 1),
(NEWID(), 'SP_002', @GroomingId, 1, 'Full grooming service with natural products', 15, 0, 1, 1),
(NEWID(), 'SP_002', @TrainingId, 1, 'Specializing in behavioral issues and rescue dog rehabilitation', 20, 0, 1, 1),
(NEWID(), 'SP_003', @BoardingId, 1, 'Home-from-home boarding with garden access', 12, 1, 1, 1),
(NEWID(), 'SP_003', @PetSittingId, 1, 'Overnight and holiday sitting in your home', 10, 1, 1, 1),
(NEWID(), 'SP_004', @VetServicesId, 1, 'Professional vet transport with experienced handler', 25, 1, 1, 1),
(NEWID(), 'SP_005', @DogWalkingId, 1, 'Adventure walks in Kent countryside', 20, 0, 1, 1),

-- Non-premium providers with basic services
(NEWID(), 'SP_006', @DogWalkingId, 1, 'Reliable local dog walking service', 5, 0, 1, 0),
(NEWID(), 'SP_007', @PetSittingId, 1, 'Basic pet sitting and drop-in visits', 3, 0, 0, 0),
(NEWID(), 'SP_008', @DogWalkingId, 1, 'Simple dog walking around Bath', 4, 0, 1, 0),
(NEWID(), 'SP_009', @DogWalkingId, 1, 'Energetic walks for active dogs', 6, 0, 1, 1),
(NEWID(), 'SP_010', @DogWalkingId, 1, 'Bilingual dog walking service', 8, 0, 1, 1),
(NEWID(), 'SP_010', @PetSittingId, 1, 'Pet sitting with Welsh language option', 8, 0, 1, 0),

-- Mixed users
(NEWID(), 'SP_011', @TrainingId, 1, 'Expert training from experienced dog owner', 15, 0, 1, 1),
(NEWID(), 'SP_011', @DogWalkingId, 1, 'Walking with training elements included', 12, 0, 1, 1),
(NEWID(), 'SP_012', @PetSittingId, 1, 'Flexible sitting for university schedule', 5, 0, 0, 1),
(NEWID(), 'SP_013', @GroomingId, 1, 'Premium grooming for discerning clients', 10, 0, 1, 1),
(NEWID(), 'SP_013', @BoardingId, 1, 'Luxury boarding with premium amenities', 15, 1, 1, 1),
(NEWID(), 'SP_014', @DogWalkingId, 1, 'Community-focused dog walking', 4, 0, 1, 0),
(NEWID(), 'SP_015', @DogWalkingId, 1, 'Countryside adventure walks', 18, 0, 1, 1);

-- Since we have a schema mismatch (string vs GUID), we'll create a workaround
-- In a real scenario, this would need proper schema alignment

PRINT 'Service provider to category mappings prepared (schema alignment needed for full implementation)';

-- =============================================
-- 7. COMPLETION SUMMARY
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT 'TEST SEED DATA CREATION COMPLETED';
PRINT '=============================================';
PRINT 'Created:';
PRINT '- 2 Subscription Plans (Free and Premium)';
PRINT '- 20 Test Users (marked with TESTUSER_ prefix)';
PRINT '  * 10 Service Providers (5 Premium, 5 Free)';
PRINT '  * 5 Pet Owners (2 Premium, 3 Free)';
PRINT '  * 5 Mixed Users (3 Premium, 2 Free)';
PRINT '- 20 User Subscriptions';
PRINT '- 15 Service Provider Profiles';
PRINT '- 12 Dog Profiles';
PRINT '- Service category mappings (prepared)';
PRINT '';
PRINT 'Geographic Distribution:';
PRINT '- England: Manchester, Bristol, Birmingham, Canterbury, Leeds, Norwich, Bath, Newcastle, Brighton, Sheffield, London, Liverpool, Nottingham, Cambridge, Oxford, Preston, Exeter';
PRINT '- Scotland: Edinburgh';
PRINT '- Wales: Cardiff';
PRINT '';
PRINT 'IMPORTANT NOTES:';
PRINT '- All test users have prefix "TESTUSER_" for easy identification';
PRINT '- All passwords are: TestPassword123!';
PRINT '- Geographic coordinates included for mapping features';
PRINT '- Mix of premium and free users for testing subscription features';
PRINT '- Diverse service offerings across different categories';
PRINT '- Dogs with various ages, breeds, and special needs for realistic testing';
PRINT '';
PRINT 'TO DELETE ALL TEST DATA LATER:';
PRINT 'DELETE FROM UserSubscriptions WHERE UserId LIKE ''TESTUSER_%'';';
PRINT 'DELETE FROM DogProfiles WHERE OwnerId LIKE ''TESTUSER_%'';';
PRINT 'DELETE FROM ServiceProviders WHERE UserId LIKE ''TESTUSER_%'';';
PRINT 'DELETE FROM Users WHERE Id LIKE ''TESTUSER_%'';';
PRINT 'DELETE FROM SubscriptionPlans WHERE Id IN (''free-plan'', ''premium-plan'');';
PRINT '';
PRINT '⚠️  REMEMBER: This is TEST DATA ONLY - DELETE before production deployment!';
PRINT '=============================================';

GO