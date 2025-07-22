# Premium Subscription System Implementation Guide

## Overview
This document outlines the complete implementation plan for the premium subscription system for service providers in the MeAndMyDog platform.

## Current Status
- ✅ **IsPremium column**: Database schema updated (run `add-ispremium-column.sql`)
- ✅ **Search ordering**: Premium providers appear first in search results
- ✅ **Frontend integration**: UI supports premium provider display
- ❌ **Subscription management**: Not yet implemented
- ❌ **Payment processing**: Not yet implemented
- ❌ **Premium features**: Not yet implemented

## Database Schema

### 1. Core Premium Column
```sql
-- Already implemented in add-ispremium-column.sql
ALTER TABLE ServiceProviders 
ADD IsPremium BIT NOT NULL DEFAULT 0,
    PremiumStartDate DATETIME2 NULL,
    PremiumEndDate DATETIME2 NULL,
    PremiumSubscriptionId NVARCHAR(50) NULL;
```

### 2. Premium Subscription Plans Table
```sql
CREATE TABLE PremiumSubscriptionPlans (
    Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    PlanName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    MonthlyPrice DECIMAL(10,2) NOT NULL,
    AnnualPrice DECIMAL(10,2),
    Features NVARCHAR(MAX), -- JSON array of features
    MaxListings INT DEFAULT -1, -- -1 for unlimited
    MaxPhotos INT DEFAULT 10,
    PrioritySupport BIT DEFAULT 0,
    AdvancedAnalytics BIT DEFAULT 0,
    CustomBranding BIT DEFAULT 0,
    SearchBoost INT DEFAULT 0, -- 0-100 priority boost
    IsActive BIT DEFAULT 1,
    SortOrder INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
```

### 3. Premium Subscriptions Table
```sql
CREATE TABLE PremiumSubscriptions (
    Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    ProviderId NVARCHAR(50) NOT NULL,
    PlanId NVARCHAR(50) NOT NULL,
    StripeSubscriptionId NVARCHAR(100),
    StripeCustomerId NVARCHAR(100),
    Status NVARCHAR(20) NOT NULL, -- active, canceled, past_due, unpaid, trialing
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    TrialEndDate DATETIME2 NULL,
    AutoRenew BIT DEFAULT 1,
    PaymentAmount DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'GBP',
    PaymentFrequency NVARCHAR(20), -- monthly, annual
    BillingCycle INT DEFAULT 1, -- months
    LastPaymentDate DATETIME2 NULL,
    NextPaymentDate DATETIME2 NULL,
    CancelAtPeriodEnd BIT DEFAULT 0,
    CancelReason NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (ProviderId) REFERENCES ServiceProviders(Id),
    FOREIGN KEY (PlanId) REFERENCES PremiumSubscriptionPlans(Id)
);
```

### 4. Payment History Table
```sql
CREATE TABLE PremiumPaymentHistory (
    Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    SubscriptionId NVARCHAR(50) NOT NULL,
    StripePaymentIntentId NVARCHAR(100),
    Amount DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'GBP',
    Status NVARCHAR(20) NOT NULL, -- succeeded, failed, pending, refunded
    PaymentDate DATETIME2 NOT NULL,
    FailureReason NVARCHAR(500),
    RefundAmount DECIMAL(10,2) DEFAULT 0,
    RefundDate DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (SubscriptionId) REFERENCES PremiumSubscriptions(Id)
);
```

## API Endpoints to Implement

### 1. Subscription Management Controller
```csharp
// Controllers/PremiumSubscriptionController.cs

[ApiController]
[Route("api/v1/premium")]
public class PremiumSubscriptionController : ControllerBase
{
    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    
    [HttpPost("providers/{providerId}/subscribe")]
    [Authorize]
    public async Task<IActionResult> CreateSubscription(string providerId, CreateSubscriptionRequest request)
    
    [HttpPost("providers/{providerId}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelSubscription(string providerId)
    
    [HttpGet("providers/{providerId}/subscription")]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionStatus(string providerId)
    
    [HttpPost("providers/{providerId}/upgrade")]
    [Authorize]
    public async Task<IActionResult> UpgradePlan(string providerId, UpgradePlanRequest request)
    
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleStripeWebhook()
}
```

### 2. Admin Management Controller
```csharp
// Controllers/Admin/PremiumAdminController.cs

[ApiController]
[Route("api/v1/admin/premium")]
[Authorize(Roles = "Admin")]
public class PremiumAdminController : ControllerBase
{
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetAllSubscriptions(int page = 1, int pageSize = 20)
    
    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan(CreatePlanRequest request)
    
    [HttpPut("plans/{planId}")]
    public async Task<IActionResult> UpdatePlan(string planId, UpdatePlanRequest request)
    
    [HttpGet("revenue-report")]
    public async Task<IActionResult> GetRevenueReport(DateTime startDate, DateTime endDate)
    
    [HttpPost("providers/{providerId}/grant-premium")]
    public async Task<IActionResult> GrantPremium(string providerId, int months)
}
```

## Service Classes to Implement

### 1. Premium Subscription Service
```csharp
// Services/IPremiumSubscriptionService.cs
public interface IPremiumSubscriptionService
{
    Task<PremiumSubscriptionPlans[]> GetAvailablePlansAsync();
    Task<PremiumSubscription> CreateSubscriptionAsync(string providerId, string planId, PaymentMethodDetails paymentMethod);
    Task<bool> CancelSubscriptionAsync(string subscriptionId, string reason = null);
    Task<PremiumSubscription> GetProviderSubscriptionAsync(string providerId);
    Task<bool> UpgradeSubscriptionAsync(string subscriptionId, string newPlanId);
    Task ProcessExpiredSubscriptionsAsync(); // Background job
    Task ProcessPaymentFailuresAsync(); // Background job
    Task SendExpiryNotificationsAsync(); // Background job
}
```

### 2. Stripe Integration Service
```csharp
// Services/IStripeService.cs
public interface IStripeService
{
    Task<Stripe.Customer> CreateCustomerAsync(ServiceProvider provider);
    Task<Stripe.Subscription> CreateSubscriptionAsync(string customerId, string priceId);
    Task<Stripe.Subscription> CancelSubscriptionAsync(string subscriptionId, bool immediately = false);
    Task<Stripe.PaymentIntent> ProcessPaymentAsync(string customerId, decimal amount, string currency = "gbp");
    Task<bool> HandleWebhookAsync(string json, string signature);
}
```

### 3. Premium Features Service
```csharp
// Services/IPremiumFeaturesService.cs
public interface IPremiumFeaturesService
{
    Task<bool> CanUploadPhotosAsync(string providerId, int photoCount);
    Task<bool> HasPrioritySearchAsync(string providerId);
    Task<bool> CanAccessAnalyticsAsync(string providerId);
    Task<bool> HasCustomBrandingAsync(string providerId);
    Task<int> GetMaxListingsAsync(string providerId);
    Task<PremiumFeatureUsage> GetFeatureUsageAsync(string providerId);
}
```

## Background Jobs to Implement

### 1. Subscription Management Jobs
```csharp
// Jobs/PremiumSubscriptionJobs.cs

[BackgroundJob]
public class SubscriptionExpiryJob
{
    // Run daily at 2 AM
    [RecurringJob("0 2 * * *")]
    public async Task ProcessExpiredSubscriptions()
    {
        // Find expired subscriptions
        // Set IsPremium = false
        // Send expiry notifications
        // Log activities
    }
}

[BackgroundJob]
public class SubscriptionNotificationJob
{
    // Run daily at 9 AM
    [RecurringJob("0 9 * * *")]
    public async Task SendExpiryNotifications()
    {
        // Send 7-day warning
        // Send 3-day warning
        // Send 1-day warning
        // Send expired notification
    }
}

[BackgroundJob]
public class PaymentRetryJob
{
    // Run every 6 hours
    [RecurringJob("0 */6 * * *")]
    public async Task ProcessFailedPayments()
    {
        // Retry failed payments
        // Update subscription status
        // Send payment failure notifications
    }
}
```

## Frontend Implementation

### 1. Premium Subscription Page
```typescript
// src/pages/PremiumSubscription.tsx
export default function PremiumSubscription() {
    // Display available plans
    // Handle plan selection
    // Integrate Stripe payment form
    // Show subscription status
    // Allow cancellation/upgrades
}
```

### 2. Provider Dashboard Premium Section
```typescript
// src/components/ProviderDashboard/PremiumSection.tsx
export default function PremiumSection() {
    // Show current plan status
    // Display feature usage
    // Premium analytics
    // Upgrade/downgrade options
}
```

### 3. Premium Feature Gates
```typescript
// src/hooks/usePremiumFeatures.ts
export function usePremiumFeatures(providerId: string) {
    // Check premium status
    // Return available features
    // Handle feature limitations
}
```

## Configuration

### 1. appsettings.json
```json
{
  "PremiumSubscription": {
    "Stripe": {
      "ApiKey": "sk_test_...",
      "PublishableKey": "pk_test_...",
      "WebhookSecret": "whsec_..."
    },
    "Plans": {
      "Basic": {
        "MonthlyPriceId": "price_...",
        "AnnualPriceId": "price_..."
      },
      "Professional": {
        "MonthlyPriceId": "price_...",
        "AnnualPriceId": "price_..."
      }
    },
    "Features": {
      "TrialDays": 14,
      "GracePeriodDays": 3,
      "NotificationDays": [7, 3, 1],
      "DefaultCurrency": "GBP"
    }
  }
}
```

### 2. Stripe Product Setup
```javascript
// Stripe Dashboard Product Configuration
{
  "Basic Premium": {
    "monthly": "£29.99/month",
    "annual": "£299.99/year" // Save 2 months
  },
  "Professional": {
    "monthly": "£59.99/month", 
    "annual": "£599.99/year" // Save 2 months
  },
  "Enterprise": {
    "monthly": "£149.99/month",
    "annual": "£1499.99/year" // Save 2 months
  }
}
```

## Premium Features by Plan

### Basic Premium (£29.99/month)
- ✅ Priority search ranking
- ✅ Up to 10 photos per listing
- ✅ Basic analytics
- ✅ Email support
- ✅ Premium badge
- ✅ Featured in "Premium Providers" section

### Professional (£59.99/month)
- ✅ All Basic features
- ✅ Up to 25 photos per listing
- ✅ Advanced analytics & reporting
- ✅ Priority support (phone + email)
- ✅ Custom business description
- ✅ Video introduction upload
- ✅ Social media integration
- ✅ Customer relationship management

### Enterprise (£149.99/month)
- ✅ All Professional features
- ✅ Unlimited photos
- ✅ Custom branding & styling
- ✅ API access for integrations
- ✅ Dedicated account manager
- ✅ White-label booking widget
- ✅ Advanced marketing tools
- ✅ Multi-location management

## Implementation Timeline

### Phase 1: Core Infrastructure (2-3 weeks)
1. **Week 1**: Database schema, basic API endpoints
2. **Week 2**: Stripe integration, webhook handling
3. **Week 3**: Background jobs, testing

### Phase 2: Frontend Integration (2 weeks)
1. **Week 4**: Subscription management UI
2. **Week 5**: Provider dashboard, feature gates

### Phase 3: Premium Features (3-4 weeks)
1. **Week 6-7**: Enhanced listings, analytics
2. **Week 8-9**: Advanced features, admin tools

### Phase 4: Testing & Launch (2 weeks)
1. **Week 10**: Integration testing, security audit
2. **Week 11**: Beta testing, production deployment

## Security Considerations

### 1. Payment Security
- Use Stripe's secure payment processing
- Never store credit card information
- Implement proper webhook signature verification
- Use HTTPS for all payment-related endpoints

### 2. Subscription Security
- Validate subscription status server-side
- Implement rate limiting on subscription endpoints
- Audit all subscription changes
- Secure admin endpoints with proper authorization

### 3. Feature Access Control
- Server-side validation for all premium features
- Regular subscription status checks
- Graceful degradation when subscriptions expire
- Prevent feature abuse

## Testing Strategy

### 1. Unit Tests
- Subscription service logic
- Payment processing
- Feature access validation
- Background job processing

### 2. Integration Tests
- Stripe webhook handling
- Database consistency
- API endpoint functionality
- Email notifications

### 3. End-to-End Tests
- Complete subscription flow
- Payment success/failure scenarios
- Cancellation and upgrades
- Feature access enforcement

## Monitoring & Analytics

### 1. Key Metrics
- Monthly Recurring Revenue (MRR)
- Customer Lifetime Value (CLV)
- Churn rate by plan
- Feature usage statistics
- Payment success/failure rates

### 2. Alerts
- Failed payment notifications
- Subscription cancellations
- High churn rate alerts
- System errors and exceptions

### 3. Reports
- Monthly revenue reports
- Customer acquisition costs
- Feature adoption rates
- Premium vs. free user behavior

## Launch Strategy

### 1. Soft Launch (Beta)
- Invite existing high-rated providers
- Limited feature set
- Gather feedback and iterate
- Monitor system performance

### 2. Marketing Campaign
- Email to all providers
- Social media promotion
- Special launch pricing
- Referral incentives

### 3. Success Metrics
- Target: 10% of active providers upgrade in first 3 months
- Revenue goal: £10,000 MRR within 6 months
- Feature adoption: 80% of premium users use key features
- Customer satisfaction: 4.5+ star rating for premium experience

---

## Next Steps

1. **Run the SQL script**: Execute `add-ispremium-column.sql` to add the IsPremium column
2. **Update Entity Framework models**: Add IsPremium property to ServiceProvider model
3. **Choose implementation phase**: Decide which phase to tackle first
4. **Set up Stripe account**: Configure products and pricing
5. **Review and approve**: Architecture and implementation plan

This comprehensive plan provides a roadmap for implementing a full-featured premium subscription system that will generate recurring revenue and provide value to service providers.