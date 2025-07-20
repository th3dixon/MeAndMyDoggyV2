-- Data Integrity Test Script for MeAndMyDoggyV2
-- Run this before and after migrations to ensure data consistency

DECLARE @Results TABLE (
    TestName NVARCHAR(100),
    Status NVARCHAR(20),
    Details NVARCHAR(MAX)
);

-- Test 1: User count and basic integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'User Count and Integrity',
    CASE WHEN COUNT(*) > 0 THEN 'PASS' ELSE 'FAIL' END,
    'Total Users: ' + CAST(COUNT(*) AS NVARCHAR) + 
    ', Active Users: ' + CAST(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS NVARCHAR)
FROM AspNetUsers;

-- Test 2: Identity relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Identity Relationships',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned UserRoles: ' + CAST(COUNT(*) AS NVARCHAR)
FROM AspNetUserRoles ur
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = ur.UserId)
   OR NOT EXISTS (SELECT 1 FROM AspNetRoles r WHERE r.Id = ur.RoleId);

-- Test 3: DogProfiles integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'DogProfile Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned DogProfiles: ' + CAST(COUNT(*) AS NVARCHAR)
FROM DogProfiles dp
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = dp.OwnerId);

-- Test 4: ServiceProvider relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'ServiceProvider Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'ServiceProviders without Users: ' + CAST(COUNT(*) AS NVARCHAR)
FROM ServiceProviders sp
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = sp.UserId);

-- Test 5: Service catalog relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Service Catalog Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Orphaned SubServices: ' + CAST(COUNT(*) AS NVARCHAR)
FROM SubServices ss
WHERE NOT EXISTS (SELECT 1 FROM ServiceCategories sc WHERE sc.ServiceCategoryId = ss.ServiceCategoryId);

-- Test 6: Booking relationships
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Booking Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Bookings with invalid references: ' + CAST(COUNT(*) AS NVARCHAR)
FROM Bookings b
WHERE NOT EXISTS (SELECT 1 FROM ServiceProviders sp WHERE sp.ServiceProviderId = b.ServiceProviderId)
   OR NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = b.CustomerId);

-- Test 7: Message/Conversation integrity
INSERT INTO @Results (TestName, Status, Details)
SELECT 
    'Messaging Integrity',
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END,
    'Messages without valid conversations: ' + CAST(COUNT(*) AS NVARCHAR)
FROM Messages m
WHERE NOT EXISTS (SELECT 1 FROM Conversations c WHERE c.ConversationId = m.ConversationId);

-- Display results
SELECT 
    TestName,
    Status,
    Details,
    CASE 
        WHEN Status = 'PASS' THEN '✓'
        ELSE '✗'
    END AS Result
FROM @Results
ORDER BY 
    CASE WHEN Status = 'FAIL' THEN 0 ELSE 1 END,
    TestName;

-- Summary
DECLARE @PassCount INT = (SELECT COUNT(*) FROM @Results WHERE Status = 'PASS');
DECLARE @TotalCount INT = (SELECT COUNT(*) FROM @Results);

SELECT 
    '=== DATA INTEGRITY TEST SUMMARY ===' AS Summary,
    CAST(@PassCount AS NVARCHAR) + ' / ' + CAST(@TotalCount AS NVARCHAR) + ' tests passed' AS Result,
    CASE 
        WHEN @PassCount = @TotalCount THEN 'ALL TESTS PASSED'
        ELSE 'SOME TESTS FAILED - REVIEW DETAILS ABOVE'
    END AS Status;