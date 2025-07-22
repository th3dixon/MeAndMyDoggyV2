-- =============================================
-- Premium Providers Ordering SQL Script
-- Description: Update provider search queries to show premium providers first
-- =============================================

-- This script provides SQL modifications for your provider search stored procedures or queries
-- to ensure premium/verified providers appear first in search results

-- Option 1: If you have a stored procedure for provider search, modify the ORDER BY clause
-- Replace your existing ORDER BY with this ordering logic:

/*
Example ORDER BY clause for provider search:
ORDER BY 
    CASE WHEN IsVerified = 1 OR IsPremium = 1 THEN 0 ELSE 1 END,  -- Premium providers first
    Rating DESC,                                                   -- Then by rating
    ReviewCount DESC,                                             -- Then by review count
    DistanceMiles ASC,                                           -- Then by distance
    BusinessName ASC                                             -- Finally by name
*/

-- Option 2: If you're using Entity Framework, you can apply this ordering in your C# code:
-- .OrderBy(p => p.IsVerified ? 0 : 1)
-- .ThenByDescending(p => p.Rating)
-- .ThenByDescending(p => p.ReviewCount)
-- .ThenBy(p => p.DistanceMiles)
-- .ThenBy(p => p.BusinessName)

-- Option 3: Raw SQL query example for provider search:
/*
SELECT 
    p.Id,
    p.BusinessName,
    p.Rating,
    p.ReviewCount,
    p.IsVerified,
    p.IsPremium,
    p.ProfileImageUrl,
    -- Calculate distance (example using geography types)
    GEOGRAPHY::Point(p.Latitude, p.Longitude, 4326).STDistance(
        GEOGRAPHY::Point(@SearchLatitude, @SearchLongitude, 4326)
    ) / 1609.344 AS DistanceMiles,
    -- Other fields...
FROM Providers p
WHERE 
    p.IsActive = 1
    AND p.IsVerified = 1
    -- Add your other search filters here
ORDER BY 
    CASE WHEN p.IsVerified = 1 OR p.IsPremium = 1 THEN 0 ELSE 1 END,
    p.Rating DESC,
    p.ReviewCount DESC,
    GEOGRAPHY::Point(p.Latitude, p.Longitude, 4326).STDistance(
        GEOGRAPHY::Point(@SearchLatitude, @SearchLongitude, 4326)
    ) ASC,
    p.BusinessName ASC
*/

-- Option 4: Update existing index for better performance with premium ordering
-- Create a composite index that supports premium-first ordering:
/*
CREATE NONCLUSTERED INDEX IX_Providers_PremiumOrdering
ON Providers (IsVerified DESC, IsPremium DESC, Rating DESC, ReviewCount DESC)
INCLUDE (Id, BusinessName, ProfileImageUrl, Latitude, Longitude, IsActive)
*/

-- =============================================
-- Notes:
-- =============================================
-- 1. Replace field names with your actual database column names
-- 2. Adjust the distance calculation based on your database setup (SQL Server geography, PostgreSQL PostGIS, etc.)
-- 3. Add any additional search filters you need (service categories, price ranges, etc.)
-- 4. Consider creating appropriate indexes for performance optimization
-- 5. Test the query performance with your actual data volume