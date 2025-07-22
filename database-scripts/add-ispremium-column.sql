-- =============================================
-- Add IsPremium Column to ServiceProviders Table
-- Description: Adds premium status tracking for service providers
-- =============================================

-- Check if the column already exists before adding it
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ServiceProviders' 
    AND COLUMN_NAME = 'IsPremium'
)
BEGIN
    -- Add IsPremium column
    ALTER TABLE ServiceProviders 
    ADD IsPremium BIT NOT NULL DEFAULT 0;
    
    PRINT 'IsPremium column added successfully to ServiceProviders table';
END
ELSE
BEGIN
    PRINT 'IsPremium column already exists in ServiceProviders table';
END

-- Add index for performance on premium filtering
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_ServiceProviders_IsPremium'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_ServiceProviders_IsPremium
    ON ServiceProviders (IsPremium DESC, IsVerified DESC, CreatedAt DESC)
    INCLUDE (Id, BusinessName, Rating, ReviewCount, IsActive);
    
    PRINT 'Premium filtering index created successfully';
END

-- Add audit columns for premium subscription tracking (optional)
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ServiceProviders' 
    AND COLUMN_NAME = 'PremiumStartDate'
)
BEGIN
    ALTER TABLE ServiceProviders 
    ADD PremiumStartDate DATETIME2 NULL,
        PremiumEndDate DATETIME2 NULL,
        PremiumSubscriptionId NVARCHAR(50) NULL;
    
    PRINT 'Premium subscription tracking columns added successfully';
END

-- Set some test providers to premium status (optional - for testing)
-- Replace these IDs with actual provider IDs from your test data
/*
UPDATE ServiceProviders 
SET IsPremium = 1,
    PremiumStartDate = GETDATE(),
    PremiumEndDate = DATEADD(MONTH, 1, GETDATE())
WHERE Id IN (
    'test-provider-1',  -- Replace with actual provider ID
    'test-provider-3'   -- Replace with actual provider ID
);
*/

-- =============================================
-- DOCUMENTATION FOR FUTURE PREMIUM SUBSCRIPTION IMPLEMENTATION
-- =============================================

/*
PREMIUM SUBSCRIPTION FUNCTIONALITY TO BE IMPLEMENTED:

1. SUBSCRIPTION PLANS TABLE
   Create a table to define different premium subscription plans:
   
   CREATE TABLE PremiumSubscriptionPlans (
       Id NVARCHAR(50) PRIMARY KEY,
       PlanName NVARCHAR(100) NOT NULL,
       Description NVARCHAR(500),
       MonthlyPrice DECIMAL(10,2) NOT NULL,
       AnnualPrice DECIMAL(10,2),
       Features NVARCHAR(MAX), -- JSON array of features
       MaxListings INT,
       PrioritySupport BIT DEFAULT 0,
       AdvancedAnalytics BIT DEFAULT 0,
       CustomBranding BIT DEFAULT 0,
       IsActive BIT DEFAULT 1,
       CreatedAt DATETIME2 DEFAULT GETDATE(),
       UpdatedAt DATETIME2 DEFAULT GETDATE()
   );

2. SUBSCRIPTION TRANSACTIONS TABLE
   Track all premium subscription payments and status:
   
   CREATE TABLE PremiumSubscriptions (
       Id NVARCHAR(50) PRIMARY KEY,
       ProviderId NVARCHAR(50) NOT NULL,
       PlanId NVARCHAR(50) NOT NULL,
       StripeSubscriptionId NVARCHAR(100), -- For Stripe integration
       Status NVARCHAR(20) NOT NULL, -- active, canceled, past_due, unpaid
       StartDate DATETIME2 NOT NULL,
       EndDate DATETIME2 NOT NULL,
       AutoRenew BIT DEFAULT 1,
       PaymentAmount DECIMAL(10,2) NOT NULL,
       Currency NVARCHAR(3) DEFAULT 'GBP',
       PaymentFrequency NVARCHAR(20), -- monthly, annual
       CreatedAt DATETIME2 DEFAULT GETDATE(),
       UpdatedAt DATETIME2 DEFAULT GETDATE(),
       
       FOREIGN KEY (ProviderId) REFERENCES ServiceProviders(Id),
       FOREIGN KEY (PlanId) REFERENCES PremiumSubscriptionPlans(Id)
   );

3. BUSINESS LOGIC TO IMPLEMENT:

   A. SUBSCRIPTION ACTIVATION:
      - When user subscribes to premium plan
      - Set ServiceProviders.IsPremium = 1
      - Set ServiceProviders.PremiumStartDate = subscription start
      - Set ServiceProviders.PremiumEndDate = subscription end
      - Create record in PremiumSubscriptions table
      
   B. SUBSCRIPTION RENEWAL:
      - Check expiring subscriptions daily (background job)
      - Process automatic renewals via Stripe webhooks
      - Update ServiceProviders.PremiumEndDate on successful renewal
      
   C. SUBSCRIPTION CANCELLATION:
      - Allow immediate cancellation or at end of period
      - Set ServiceProviders.IsPremium = 0 when subscription expires
      - Update PremiumSubscriptions.Status = 'canceled'
      
   D. SUBSCRIPTION EXPIRY:
      - Daily background job to check expired subscriptions
      - Automatically set IsPremium = 0 for expired subscriptions
      - Send notifications before expiry (7 days, 1 day)

4. API ENDPOINTS TO CREATE:

   POST /api/v1/providers/{providerId}/premium/subscribe
   POST /api/v1/providers/{providerId}/premium/cancel
   GET  /api/v1/providers/{providerId}/premium/status
   GET  /api/v1/premium/plans
   PUT  /api/v1/providers/{providerId}/premium/plan/{planId}

5. STRIPE INTEGRATION:

   A. WEBHOOK HANDLERS:
      - invoice.payment_succeeded
      - invoice.payment_failed
      - customer.subscription.updated
      - customer.subscription.deleted
      
   B. PAYMENT FLOW:
      - Create Stripe customer for provider
      - Create Stripe subscription with selected plan
      - Handle webhooks to update database status
      - Process refunds and prorations

6. PREMIUM FEATURES TO IMPLEMENT:

   A. SEARCH RANKING:
      - Premium providers appear first in search results ✅ (Already implemented)
      - Higher visibility in map view
      
   B. ENHANCED LISTINGS:
      - More photos (10 vs 3 for free)
      - Custom business description
      - Video introduction
      - Custom service descriptions
      
   C. ADVANCED FEATURES:
      - Priority customer support
      - Advanced booking management
      - Revenue analytics dashboard
      - Customer relationship management
      - Marketing tools (discounts, promotions)
      
   D. COMMUNICATION:
      - Instant messaging with customers
      - SMS notifications
      - Email marketing tools

7. SCHEDULED TASKS TO IMPLEMENT:

   - Daily: Check and process expired subscriptions
   - Daily: Send expiry notifications (7 days, 1 day before)
   - Monthly: Generate premium subscription reports
   - Weekly: Process failed payment retries

8. CONFIGURATION SETTINGS:

   Add to appsettings.json:
   {
     "PremiumSubscription": {
       "StripeApiKey": "sk_test_...",
       "StripePublishableKey": "pk_test_...",
       "WebhookSecret": "whsec_...",
       "GracePeriodDays": 3,
       "NotificationDays": [7, 3, 1],
       "DefaultCurrency": "GBP"
     }
   }

9. DATABASE TRIGGERS (OPTIONAL):

   CREATE TRIGGER TR_UpdateProviderPremiumStatus
   ON PremiumSubscriptions
   AFTER INSERT, UPDATE
   AS
   BEGIN
       UPDATE sp
       SET IsPremium = CASE 
           WHEN ps.Status = 'active' AND ps.EndDate > GETDATE() THEN 1 
           ELSE 0 
       END,
       PremiumEndDate = ps.EndDate,
       UpdatedAt = GETDATE()
       FROM ServiceProviders sp
       INNER JOIN inserted ps ON sp.Id = ps.ProviderId;
   END

10. TESTING CHECKLIST:

    □ Subscription creation flow
    □ Payment processing
    □ Webhook handling
    □ Subscription renewal
    □ Subscription cancellation
    □ Expiry handling
    □ Search ranking priority
    □ Premium feature access
    □ Notification system
    □ Admin dashboard
    □ Billing reports
    □ Error handling
    □ Security testing
*/

-- =============================================
-- IMMEDIATE UPDATES FOR CURRENT SEARCH FUNCTIONALITY
-- =============================================

-- Update the search ordering logic to use IsPremium
-- This should be implemented in your provider search stored procedure or Entity Framework query

/*
EXAMPLE ORDERING LOGIC:
ORDER BY 
    CASE WHEN IsPremium = 1 THEN 0 ELSE 1 END,  -- Premium providers first
    CASE WHEN IsVerified = 1 THEN 0 ELSE 1 END, -- Then verified providers
    Rating DESC,                                 -- Then by rating
    ReviewCount DESC,                           -- Then by review count
    DistanceMiles ASC,                         -- Then by distance
    BusinessName ASC                           -- Finally by name

ENTITY FRAMEWORK EXAMPLE:
.OrderBy(p => p.IsPremium ? 0 : 1)
.ThenBy(p => p.IsVerified ? 0 : 1)
.ThenByDescending(p => p.Rating)
.ThenByDescending(p => p.ReviewCount)
.ThenBy(p => p.DistanceMiles)
.ThenBy(p => p.BusinessName)
*/

PRINT '==============================================';
PRINT 'IsPremium column setup completed successfully!';
PRINT 'Remember to:';
PRINT '1. Update your Entity Framework model to include IsPremium';
PRINT '2. Run Entity Framework migrations if using Code First';
PRINT '3. Update search queries to use IsPremium for ordering';
PRINT '4. Review the premium subscription documentation above';
PRINT '==============================================';