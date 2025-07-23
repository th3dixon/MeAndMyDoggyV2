# Product Requirements Document: Account Settings

## Executive Summary

The Account Settings functionality provides users with comprehensive control over their account security, privacy, notifications, billing, and system preferences. This feature serves both Pet Owners and Service Providers, offering persona-specific settings while maintaining a unified experience. Account Settings focuses on configuration and security aspects, complementing the public-facing My Profile feature that handles personal information and public presentation.

### Key Objectives
- Enable users to manage authentication and security settings
- Provide granular control over notifications and communications
- Facilitate billing and subscription management
- Support business-specific settings for Service Providers
- Ensure data privacy and GDPR compliance
- Deliver a consistent, intuitive settings interface

## User Personas & Use Cases

### Pet Owner
**Primary Needs:**
- Secure account access
- Notification preferences for pet care reminders
- Privacy control over personal data
- Simple billing management
- Data export capabilities

**Use Cases:**
1. Enable two-factor authentication after security breach news
2. Configure quiet hours for notifications during sleep
3. Update payment method when card expires
4. Export pet health data for veterinarian
5. Manage connected family accounts

### Service Provider
**Primary Needs:**
- Enhanced security for business operations
- Professional notification management
- Business billing and invoicing
- API access for integrations
- Team member access control

**Use Cases:**
1. Set up business notifications for new bookings
2. Configure API access for calendar integration
3. Manage subscription upgrade to Premium plan
4. Add team members with limited permissions
5. Configure automated billing receipts

## Functional Requirements

### 1. Login & Security

#### 1.1 Password Management
- **Change Password**
  - Current password verification
  - Password strength indicator
  - Confirmation field validation
  - Success notification with security email

- **Password Requirements**
  - Minimum 8 characters
  - Mix of uppercase, lowercase, numbers, special characters
  - Prevent common passwords
  - No reuse of last 5 passwords

#### 1.2 Two-Factor Authentication (2FA)
- **Setup Methods**
  - SMS verification
  - Authenticator app (TOTP)
  - Email verification (backup)
  
- **Management**
  - Enable/disable 2FA
  - Generate backup codes
  - View trusted devices
  - Revoke device access

#### 1.3 Session Management
- **Active Sessions**
  - List all active sessions
  - Show device type, location, last activity
  - Individual session termination
  - "Sign out all devices" option

- **Login History**
  - Last 30 days of login attempts
  - Success/failure status
  - IP address and location
  - Suspicious activity alerts

#### 1.4 Security Keys
- **API Keys** (Service Providers only)
  - Generate new keys
  - Set permissions/scopes
  - Expiration dates
  - Usage statistics
  - Revoke keys

### 2. Notifications & Communications

#### 2.1 Notification Categories
- **System Notifications**
  - Account security alerts
  - Payment confirmations
  - System maintenance

- **Pet Care** (Pet Owners)
  - Appointment reminders
  - Medication alerts
  - Health check reminders
  - Emergency notifications

- **Business** (Service Providers)
  - New booking requests
  - Payment received
  - Review notifications
  - Calendar updates

#### 2.2 Delivery Channels
- **Configuration per Category**
  - Push notifications (mobile)
  - Email notifications
  - SMS notifications
  - In-app notifications

- **Channel Settings**
  - Primary email address
  - SMS phone number
  - Push notification permissions
  - Email frequency (instant/digest)

#### 2.3 Quiet Hours
- **Time-based Rules**
  - Start/end time configuration
  - Timezone selection
  - Day of week selection
  - Override for emergencies

#### 2.4 Communication Preferences
- **Marketing Communications**
  - Newsletter subscription
  - Product updates
  - Partner offers
  - Research participation

### 3. Privacy Settings

#### 3.1 Profile Visibility
- **Public Information Control**
  - Profile photo visibility
  - Location precision (exact/area/city)
  - Contact information display
  - Activity status

#### 3.2 Data Sharing
- **Third-party Integrations**
  - Connected services list
  - Permission management
  - Data sharing scope
  - Revoke access

#### 3.3 Search & Discovery
- **Discoverability Options**
  - Appear in search results
  - Profile suggestions
  - Friend recommendations
  - Location-based discovery

### 4. Billing & Subscriptions

#### 4.1 Payment Methods
- **Card Management**
  - Add/remove cards
  - Set default payment
  - Update expiration
  - Billing address

- **Alternative Payments**
  - PayPal integration
  - Apple Pay/Google Pay
  - Direct debit (UK)

#### 4.2 Subscription Management
- **Current Plan**
  - Plan details display
  - Features comparison
  - Upgrade/downgrade options
  - Cancellation process

- **Billing History**
  - Invoice list (24 months)
  - PDF download
  - Email receipts
  - Tax documentation

#### 4.3 Promo Codes & Credits
- **Redemption**
  - Enter promo codes
  - View active credits
  - Credit history
  - Expiration dates

### 5. Business Settings (Service Providers)

#### 5.1 Business Information
- **Tax Settings**
  - VAT/Tax ID
  - Business registration
  - Invoice customization
  - Tax exemption status

#### 5.2 Team Management
- **User Roles**
  - Admin access
  - Staff accounts
  - Read-only access
  - Custom permissions

#### 5.3 Booking Configuration
- **Availability Rules**
  - Business hours
  - Holiday calendar
  - Booking buffer time
  - Cancellation policy

#### 5.4 Payment Processing
- **Payout Settings**
  - Bank account details
  - Payout schedule
  - Currency preferences
  - Transaction fees display

### 6. Data Management

#### 6.1 Data Export
- **Export Options**
  - Personal information (JSON/CSV)
  - Pet profiles
  - Health records
  - Message history
  - Transaction history

#### 6.2 Account Deletion
- **Process**
  - Deletion request
  - 30-day grace period
  - Data retention policy
  - Permanent deletion confirmation

#### 6.3 Data Portability
- **GDPR Compliance**
  - Machine-readable format
  - Complete data package
  - Third-party transfer
  - Deletion after transfer

### 7. Integrations & API Access

#### 7.1 Calendar Integration
- **Supported Platforms**
  - Google Calendar
  - Outlook/Office 365
  - Apple Calendar
  - CalDAV support

#### 7.2 Connected Apps
- **OAuth Management**
  - Authorized applications
  - Permission scopes
  - Last access time
  - Revoke authorization

#### 7.3 Webhooks (Service Providers)
- **Event Subscriptions**
  - Booking events
  - Payment events
  - Review events
  - Custom endpoints

## Technical Requirements

### API Endpoints

#### Authentication & Security
```
POST   /api/v1/account/password/change
POST   /api/v1/account/2fa/enable
POST   /api/v1/account/2fa/disable
GET    /api/v1/account/2fa/backup-codes
POST   /api/v1/account/2fa/backup-codes/regenerate
GET    /api/v1/account/sessions
DELETE /api/v1/account/sessions/{sessionId}
GET    /api/v1/account/login-history
POST   /api/v1/account/api-keys
DELETE /api/v1/account/api-keys/{keyId}
```

#### Notifications
```
GET    /api/v1/account/notifications/preferences
PUT    /api/v1/account/notifications/preferences
GET    /api/v1/account/notifications/categories
PUT    /api/v1/account/notifications/categories/{categoryId}
PUT    /api/v1/account/notifications/quiet-hours
```

#### Privacy
```
GET    /api/v1/account/privacy/settings
PUT    /api/v1/account/privacy/visibility
GET    /api/v1/account/privacy/connected-apps
DELETE /api/v1/account/privacy/connected-apps/{appId}
```

#### Billing
```
GET    /api/v1/account/billing/payment-methods
POST   /api/v1/account/billing/payment-methods
DELETE /api/v1/account/billing/payment-methods/{methodId}
GET    /api/v1/account/billing/subscription
PUT    /api/v1/account/billing/subscription
GET    /api/v1/account/billing/invoices
GET    /api/v1/account/billing/invoices/{invoiceId}
```

#### Data Management
```
POST   /api/v1/account/data/export
GET    /api/v1/account/data/export/{exportId}/status
GET    /api/v1/account/data/export/{exportId}/download
POST   /api/v1/account/delete
POST   /api/v1/account/delete/cancel
```

### Database Schema Updates

#### UserSettings Extension
```sql
-- Additional settings categories
ALTER TABLE UserSettings ADD CONSTRAINT CK_Category 
CHECK (Category IN ('Security', 'Notifications', 'Privacy', 'Billing', 'Business', 'Integration'));

-- Notification preferences
CREATE TABLE NotificationChannelPreferences (
    Id NVARCHAR(450) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL,
    ChannelType NVARCHAR(20) NOT NULL, -- 'push', 'email', 'sms', 'inapp'
    IsEnabled BIT NOT NULL DEFAULT 1,
    Settings NVARCHAR(MAX), -- JSON for channel-specific settings
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NOT NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- API Keys
CREATE TABLE ApiKeys (
    Id NVARCHAR(450) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    KeyHash NVARCHAR(500) NOT NULL,
    Permissions NVARCHAR(MAX), -- JSON array of permissions
    ExpiresAt DATETIMEOFFSET,
    LastUsedAt DATETIMEOFFSET,
    CreatedAt DATETIMEOFFSET NOT NULL,
    RevokedAt DATETIMEOFFSET,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
```

## Security Requirements

### Authentication
- **Password Policy**
  - Enforce strong passwords
  - Account lockout after 5 failed attempts
  - Password reset via email verification
  - Session timeout after 30 minutes inactivity

### Authorization
- **Role-based Access**
  - User can only modify own settings
  - Service providers access business settings
  - Admin override capabilities
  - Audit trail for sensitive changes

### Data Protection
- **Encryption**
  - Settings data encrypted at rest
  - API keys hashed with bcrypt
  - Payment tokens never stored
  - PII data encryption

### Compliance
- **GDPR Requirements**
  - Explicit consent tracking
  - Data export within 30 days
  - Right to deletion
  - Data minimization

## UI/UX Requirements

### Layout Structure
```
Account Settings
├── Login & Security
│   ├── Password
│   ├── Two-Factor Authentication
│   ├── Active Sessions
│   └── API Keys*
├── Notifications
│   ├── Categories
│   ├── Channels
│   ├── Quiet Hours
│   └── Email Preferences
├── Privacy
│   ├── Profile Visibility
│   ├── Data Sharing
│   └── Connected Apps
├── Billing & Subscription
│   ├── Payment Methods
│   ├── Subscription Plan
│   ├── Billing History
│   └── Promo Codes
├── Business Settings*
│   ├── Tax Information
│   ├── Team Members
│   ├── Booking Rules
│   └── Payouts
└── Data & Storage
    ├── Export Data
    ├── Delete Account
    └── Storage Usage

* Service Provider only
```

### Mobile Wireframe (ASCII)
```
┌─────────────────────────────┐
│ ← Account Settings          │
├─────────────────────────────┤
│                             │
│ 🔐 Login & Security      >  │
│ ─────────────────────────   │
│ Manage your password and    │
│ security settings           │
│                             │
│ 🔔 Notifications         >  │
│ ─────────────────────────   │
│ Control how we contact you  │
│                             │
│ 🔒 Privacy              >  │
│ ─────────────────────────   │
│ Manage your privacy         │
│                             │
│ 💳 Billing              >  │
│ ─────────────────────────   │
│ Payment and subscriptions   │
│                             │
│ 💼 Business Settings    >  │
│ ─────────────────────────   │
│ Manage your business        │
│                             │
│ 📦 Data Management      >  │
│ ─────────────────────────   │
│ Export or delete your data  │
│                             │
└─────────────────────────────┘
```

### Desktop Settings Panel
```
┌────────────────────────────────────────────────────────┐
│ Account Settings                                    X  │
├────────────────┬───────────────────────────────────────┤
│                │  Password & Security                   │
│ Login &        │ ────────────────────────────────────  │
│ Security    ✓  │                                        │
│                │  Current Password                       │
│ Notifications  │  [••••••••••••••••]                    │
│                │                                        │
│ Privacy        │  New Password                          │
│                │  [                ]  💪 Strong         │
│ Billing        │                                        │
│                │  Confirm Password                      │
│ Business       │  [                ]                    │
│                │                                        │
│ Data           │  [Update Password]                     │
│                │                                        │
│                │  ─────────────────────────────────     │
│                │                                        │
│                │  🔐 Two-Factor Authentication          │
│                │     ◯ Disabled  ● Enabled              │
│                │                                        │
│                │  [Configure 2FA]                       │
│                │                                        │
└────────────────┴───────────────────────────────────────┘
```

### Interaction Patterns
- **Progressive Disclosure**: Show advanced options only when needed
- **Inline Validation**: Real-time feedback for form inputs
- **Confirmation Dialogs**: For destructive actions
- **Success Feedback**: Clear confirmation of saved changes
- **Loading States**: Show progress for async operations

## Success Metrics

### User Engagement
- **Adoption Rate**: 60% of users configure at least one setting
- **Security Features**: 30% enable 2FA within 6 months
- **Notification Management**: 50% customize notification preferences

### Technical Performance
- **API Response Time**: < 200ms for setting retrieval
- **Save Success Rate**: > 99.9% for setting updates
- **Data Export Time**: < 5 minutes for complete export

### Business Impact
- **Support Tickets**: 25% reduction in password-related tickets
- **User Retention**: 15% higher for users with customized settings
- **Premium Conversions**: 20% of providers upgrade via settings

### Security Metrics
- **2FA Adoption**: Track monthly growth
- **Suspicious Login Blocks**: Monitor prevented breaches
- **API Key Usage**: Track integration adoption

## Implementation Notes

### Phase 1: Core Settings (Weeks 1-4)
- Password management
- Basic notification preferences
- Privacy settings
- Payment method management

### Phase 2: Advanced Security (Weeks 5-6)
- Two-factor authentication
- Session management
- Login history
- Security alerts

### Phase 3: Business Features (Weeks 7-8)
- Service provider settings
- Team management
- API key generation
- Webhook configuration

### Phase 4: Data & Compliance (Weeks 9-10)
- Data export functionality
- Account deletion flow
- GDPR compliance features
- Advanced integrations

### Technical Considerations
- **State Management**: Use Alpine.js for reactive UI
- **Validation**: Client and server-side validation
- **Caching**: Cache settings for performance
- **Audit Logging**: Track all setting changes
- **Feature Flags**: Gradual rollout of new features

### Integration Points
- **Authentication System**: ASP.NET Core Identity
- **Payment Processing**: Stripe/PayPal APIs
- **Notification Service**: Azure Notification Hubs
- **Calendar APIs**: Google, Outlook OAuth
- **Export Service**: Background job processing

### Accessibility Requirements
- **WCAG 2.1 AA Compliance**
- Keyboard navigation support
- Screen reader announcements
- High contrast mode support
- Focus indicators

### Testing Strategy
- **Unit Tests**: Setting validation logic
- **Integration Tests**: API endpoint testing
- **E2E Tests**: Critical user flows
- **Security Testing**: Penetration testing
- **Performance Testing**: Load testing for exports

## Appendix: Setting Categories Reference

### Pet Owner Settings
```json
{
  "security": {
    "password": "encrypted",
    "twoFactor": {
      "enabled": true,
      "method": "authenticator"
    }
  },
  "notifications": {
    "appointments": {
      "email": true,
      "push": true,
      "sms": false
    },
    "petHealth": {
      "email": true,
      "push": true,
      "reminderTime": "09:00"
    }
  },
  "privacy": {
    "profileVisibility": "registered",
    "showLocation": "area",
    "allowMessages": "contacts"
  }
}
```

### Service Provider Settings
```json
{
  "business": {
    "taxId": "GB123456789",
    "invoicePrefix": "INV",
    "autoInvoice": true
  },
  "bookings": {
    "bufferTime": 30,
    "advanceBooking": 90,
    "cancellationHours": 24
  },
  "team": {
    "members": [
      {
        "email": "staff@example.com",
        "role": "staff",
        "permissions": ["view_bookings", "message_clients"]
      }
    ]
  },
  "integrations": {
    "googleCalendar": {
      "enabled": true,
      "calendarId": "primary",
      "syncDirection": "bidirectional"
    }
  }
}
```

---

*Last Updated: [Current Date]*
*Version: 1.0*
*Status: Draft for Review*