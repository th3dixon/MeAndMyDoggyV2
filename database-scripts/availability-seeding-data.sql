-- =============================================
-- Provider Availability Seeding Data Script
-- Description: Insert sample availability data for testing
-- Based on AvailabilitySlot entity from Entity Framework
-- =============================================

-- Ensure we're working with test data
-- CAUTION: This script assumes you have test providers set up
-- Modify provider IDs to match your actual test data

-- First, let's create some variables for test providers
-- You'll need to replace these with actual ServiceProvider IDs from your database
DECLARE @ServiceProviderId1 NVARCHAR(50) = 'test-provider-1'  -- Replace with actual ID
DECLARE @ServiceProviderId2 NVARCHAR(50) = 'test-provider-2'  -- Replace with actual ID
DECLARE @ServiceProviderId3 NVARCHAR(50) = 'test-provider-3'  -- Replace with actual ID

-- Clear existing test availability data (optional - remove if you want to keep existing data)
-- DELETE FROM AvailabilitySlots WHERE ServiceProviderId IN (@ServiceProviderId1, @ServiceProviderId2, @ServiceProviderId3)

-- =============================================
-- Provider 1: Happy Paws Pet Care (Full availability)
-- =============================================
-- Morning slots (9:00 AM - 12:00 PM) for next 30 days
DECLARE @StartDate DATE = CAST(GETDATE() AS DATE)
DECLARE @EndDate DATE = DATEADD(DAY, 30, @StartDate)
DECLARE @CurrentDate DATE = @StartDate

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Monday to Friday - Full availability
    IF DATEPART(WEEKDAY, @CurrentDate) BETWEEN 2 AND 6  -- Monday to Friday
    BEGIN
        -- Morning slots (1 hour each)
        INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
        VALUES 
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 9, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 12, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        
        -- Afternoon slots (1 hour each)
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 14, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        
        -- Extended slots (2 hour blocks)
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 13, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 30, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    END
    
    -- Saturday - Limited availability
    IF DATEPART(WEEKDAY, @CurrentDate) = 7  -- Saturday
    BEGIN
        INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
        VALUES 
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 12, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 14, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
END

-- =============================================
-- Provider 2: Professional Dog Walkers (Limited availability)
-- =============================================
SET @CurrentDate = @StartDate

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Monday, Wednesday, Friday only
    IF DATEPART(WEEKDAY, @CurrentDate) IN (2, 4, 6)  -- Mon, Wed, Fri
    BEGIN
        INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
        VALUES 
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 8, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 9, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 9, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 18, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 18, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 19, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    END
    
    -- Tuesday, Thursday - Group walks only
    IF DATEPART(WEEKDAY, @CurrentDate) IN (3, 5)  -- Tue, Thu
    BEGIN
        INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
        VALUES 
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 18, 30, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
END

-- =============================================
-- Provider 3: Pampered Pets Grooming (Grooming specialist)
-- =============================================
SET @CurrentDate = @StartDate

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Tuesday to Saturday
    IF DATEPART(WEEKDAY, @CurrentDate) BETWEEN 3 AND 7  -- Tuesday to Saturday
    BEGIN
        INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
        VALUES 
        -- Extended service slots (2 hours each)
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 9, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 13, 30, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 14, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 30, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        
        -- Standard service slots (1 hour each)
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 9, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 10, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 11, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 14, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 15, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        
        -- Short service slots (30 minutes each)
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 13, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 14, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 16, 30, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 0, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),
        (NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 17, 30, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
END

-- =============================================
-- Add some blocked/unavailable slots for realistic testing
-- =============================================

-- Block some random dates for vacation/maintenance
-- Add some blocked/unavailable slots for realistic testing
DECLARE @VacationDate1 DATE = DATEADD(DAY, 15, @StartDate)
DECLARE @MaintenanceDate2 DATE = DATEADD(DAY, 10, @StartDate)
DECLARE @MaintenanceDate3 DATE = DATEADD(DAY, 20, @StartDate)

INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
VALUES 
-- Provider 1 has a vacation day (full day unavailable)
(NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@VacationDate1), MONTH(@VacationDate1), DAY(@VacationDate1), 0, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@VacationDate1), MONTH(@VacationDate1), DAY(@VacationDate1), 23, 59, 0, 0, 0, 0, 0), 0, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),

-- Provider 2 has maintenance morning
(NEWID(), @ServiceProviderId2, DATETIMEOFFSETFROMPARTS(YEAR(@MaintenanceDate2), MONTH(@MaintenanceDate2), DAY(@MaintenanceDate2), 8, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@MaintenanceDate2), MONTH(@MaintenanceDate2), DAY(@MaintenanceDate2), 12, 0, 0, 0, 0, 0, 0), 0, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()),

-- Provider 3 has equipment maintenance
(NEWID(), @ServiceProviderId3, DATETIMEOFFSETFROMPARTS(YEAR(@MaintenanceDate3), MONTH(@MaintenanceDate3), DAY(@MaintenanceDate3), 9, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@MaintenanceDate3), MONTH(@MaintenanceDate3), DAY(@MaintenanceDate3), 14, 0, 0, 0, 0, 0, 0), 0, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())

-- =============================================
-- Emergency availability slots (24/7 providers)
-- =============================================

-- Add emergency slots for Provider 1 (premium provider)
SET @CurrentDate = @StartDate
WHILE @CurrentDate <= @EndDate
BEGIN
    -- Emergency slots available 24/7 but limited
    INSERT INTO AvailabilitySlots (Id, ServiceProviderId, StartTime, EndTime, IsAvailable, RecurrenceRule, CreatedAt, UpdatedAt)
    VALUES 
    (NEWID(), @ServiceProviderId1, DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 0, 0, 0, 0, 0, 0, 0), DATETIMEOFFSETFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate), DAY(@CurrentDate), 23, 59, 0, 0, 0, 0, 0), 1, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
END

-- =============================================
-- Instructions for customization:
-- =============================================
/*
1. Replace @ServiceProviderId1, @ServiceProviderId2, @ServiceProviderId3 with actual provider IDs from your database
2. All table and column names now match the AvailabilitySlot Entity Framework entity
3. Adjust time slots and availability patterns as needed
4. Consider adding indexes for performance:
   CREATE INDEX IX_AvailabilitySlots_Search ON AvailabilitySlots (ServiceProviderId, StartTime, IsAvailable)
5. Run this script in a test environment first
6. Uses DATETIMEOFFSETFROMPARTS and SYSDATETIMEOFFSET for proper timezone handling
*/

PRINT 'Availability seeding data inserted successfully!'
PRINT 'Remember to update provider IDs to match your actual test data!'