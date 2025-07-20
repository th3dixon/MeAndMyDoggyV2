-- Performance Testing Script for MeAndMyDoggyV2
-- Captures baseline metrics before and after migrations

-- Clear cache for consistent testing
DBCC DROPCLEANBUFFERS;
DBCC FREEPROCCACHE;

-- Enable statistics
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

PRINT '=== PERFORMANCE TEST SUITE ==='
PRINT ''

-- Test 1: User authentication query
PRINT '--- Test 1: User Authentication Query ---'
DECLARE @StartTime DATETIME = GETDATE();

SELECT TOP 100
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.UserType,
    u.IsKYCVerified,
    u.SubscriptionType
FROM AspNetUsers u
WHERE u.IsActive = 1
ORDER BY u.CreatedAt DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 2: Service provider search with location
PRINT '--- Test 2: Service Provider Location Search ---'
SET @StartTime = GETDATE();

SELECT TOP 50
    sp.ServiceProviderId,
    sp.BusinessName,
    u.FirstName,
    u.LastName,
    sp.Rating,
    pl.Postcode,
    pl.City,
    pl.Latitude,
    pl.Longitude
FROM ServiceProviders sp
INNER JOIN AspNetUsers u ON sp.UserId = u.Id
LEFT JOIN ProviderLocations pl ON sp.ServiceProviderId = pl.ServiceProviderId AND pl.IsPrimary = 1
WHERE sp.IsActive = 1 AND u.IsActive = 1
ORDER BY sp.Rating DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 3: Complex service catalog query
PRINT '--- Test 3: Service Catalog with Pricing ---'
SET @StartTime = GETDATE();

SELECT 
    sc.Name AS CategoryName,
    COUNT(DISTINCT ss.SubServiceId) AS SubServiceCount,
    COUNT(DISTINCT b.BookingId) AS TotalBookings,
    AVG(b.TotalPrice) AS AvgBookingPrice
FROM ServiceCategories sc
LEFT JOIN SubServices ss ON sc.ServiceCategoryId = ss.ServiceCategoryId
LEFT JOIN Bookings b ON sc.ServiceCategoryId = b.ServiceCategoryId
WHERE sc.IsActive = 1
GROUP BY sc.ServiceCategoryId, sc.Name, sc.DisplayOrder
ORDER BY sc.DisplayOrder;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 4: Message/Conversation query
PRINT '--- Test 4: Recent Conversations with Messages ---'
SET @StartTime = GETDATE();

SELECT TOP 20
    c.ConversationId,
    c.ConversationType,
    c.LastMessageAt,
    COUNT(DISTINCT m.MessageId) AS MessageCount,
    COUNT(DISTINCT cp.UserId) AS ParticipantCount
FROM Conversations c
INNER JOIN Messages m ON c.ConversationId = m.ConversationId
INNER JOIN ConversationParticipants cp ON c.ConversationId = cp.ConversationId
WHERE c.IsActive = 1
GROUP BY c.ConversationId, c.ConversationType, c.LastMessageAt
ORDER BY c.LastMessageAt DESC;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Test 5: Appointment scheduling query
PRINT '--- Test 5: Appointment Availability Check ---'
SET @StartTime = GETDATE();

SELECT TOP 100
    a.AppointmentId,
    a.ServiceType,
    a.StartTime,
    a.EndTime,
    sp.BusinessName,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    dp.Name AS DogName
FROM Appointments a
INNER JOIN ServiceProviders sp ON a.ServiceProviderId = sp.ServiceProviderId
INNER JOIN AspNetUsers u ON a.PetOwnerId = u.Id
LEFT JOIN DogProfiles dp ON a.DogId = dp.DogId
WHERE a.StartTime >= DATEADD(DAY, -30, GETDATE())
  AND a.Status = 'Scheduled'
ORDER BY a.StartTime;

PRINT 'Execution Time: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, GETDATE()) AS NVARCHAR) + ' ms'
PRINT ''

-- Analyze index usage
PRINT '=== INDEX USAGE ANALYSIS ==='
SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    CAST(s.last_user_seek AS DATE) AS LastSeek,
    CAST(s.last_user_scan AS DATE) AS LastScan
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE s.database_id = DB_ID()
  AND OBJECT_NAME(s.object_id) IN (
    'AspNetUsers', 'ServiceProviders', 'DogProfiles', 
    'Appointments', 'Messages', 'Conversations',
    'ServiceCategories', 'Bookings', 'ProviderLocations'
  )
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;