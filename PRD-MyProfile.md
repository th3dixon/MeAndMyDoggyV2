# Product Requirements Document: My Profile

## Executive Summary

The "My Profile" feature provides a comprehensive public-facing profile system for both Pet Owners and Service Providers within the MeAndMyDoggy platform. This feature enables users to showcase their information, build trust within the community, and facilitate connections between pet owners and service providers. The profile system supports dual-role users who may be both pet owners and service providers, ensuring a seamless experience regardless of user type.

### Key Objectives
- Create a unified profile experience that adapts to user roles
- Build trust through transparent, verified information
- Enable easy discovery and connection between users
- Support professional branding for service providers
- Showcase pet profiles for pet owners

## User Personas & Use Cases

### Persona 1: Pet Owner (Sarah)
- **Background**: Dog owner seeking reliable pet services
- **Goals**: Find trustworthy service providers, showcase her pets, build connections
- **Use Cases**:
  - View and edit her public profile
  - Manage multiple pet profiles
  - Display verified status and reviews
  - Share profile with potential service providers

### Persona 2: Service Provider (Mike)
- **Background**: Professional dog walker building his business
- **Goals**: Attract clients, showcase expertise, build reputation
- **Use Cases**:
  - Create professional business profile
  - Display services, rates, and availability
  - Showcase reviews and ratings
  - Highlight certifications and experience

### Persona 3: Dual-Role User (Emma)
- **Background**: Pet owner who also offers pet-sitting services
- **Goals**: Manage both personal and professional presence
- **Use Cases**:
  - Toggle between pet owner and provider profiles
  - Maintain separate information for each role
  - Cross-promote between profiles when appropriate

## Functional Requirements

### 1. Profile Structure

#### 1.1 Pet Owner Profile
- **Basic Information**
  - Display name (from ApplicationUser.DisplayName)
  - Profile photo (ApplicationUser.ProfilePhotoUrl)
  - Location (City, County from ApplicationUser)
  - Member since date (ApplicationUser.CreatedAt)
  - Verification status (ApplicationUser.IsKYCVerified)
  - Friend code for connections (ApplicationUser.FriendCode)

- **Pet Showcase**
  - Grid/list view of pet profiles
  - Pet cards showing:
    - Pet photo (DogProfile.ProfileImageUrl)
    - Name, breed, age
    - Basic characteristics
  - Quick access to detailed pet profiles

- **Activity & Reputation**
  - Reviews given count
  - Active member badge (based on LastSeenAt)
  - Response rate (if applicable)

#### 1.2 Service Provider Profile
- **Business Information**
  - Business name (ServiceProvider.BusinessName)
  - Business description
  - Profile photo (from ApplicationUser)
  - Service areas covered
  - Years of experience
  - Specializations

- **Professional Details**
  - Services offered with pricing
  - Availability overview
  - Response time average
  - Insurance and licensing info (if verified)
  - Business contact information

- **Reputation & Reviews**
  - Overall rating (ServiceProvider.Rating)
  - Total reviews (ServiceProvider.ReviewCount)
  - Recent reviews with responses
  - Reliability score visualization
  - Premium provider badge (if applicable)

#### 1.3 Dual-Role Profile
- **Profile Selector**
  - Toggle between "Pet Owner" and "Service Provider" views
  - Clear visual distinction between modes
  - Persistent selection across sessions

- **Unified Elements**
  - Shared basic information
  - Combined verification status
  - Consolidated activity metrics

### 2. Profile Management

#### 2.1 Edit Profile - Pet Owner
- **Editable Fields**:
  - Profile photo upload
  - Bio/About section
  - Location preferences
  - Contact preferences
  - Privacy settings

- **Pet Management**:
  - Add/Edit/Remove pet profiles
  - Upload pet photos
  - Update pet information
  - Set primary pet

#### 2.2 Edit Profile - Service Provider
- **Business Information**:
  - Business details and description
  - Service offerings and pricing
  - Service areas (map-based selection)
  - Business hours and availability
  - Professional photos gallery

- **Credentials**:
  - Upload certifications
  - Add insurance information
  - Professional affiliations
  - Training and qualifications

### 3. Profile Viewing

#### 3.1 Public View
- **Accessible Information**:
  - Public profile data
  - Verified status indicators
  - Reviews and ratings
  - Service information (providers)
  - Pet profiles (owners)

- **Restricted Information**:
  - Personal contact details
  - Exact address
  - Private notes
  - Financial information

#### 3.2 Authenticated View
- **Additional Features**:
  - Contact buttons
  - Save to favorites
  - Report profile option
  - View mutual connections

### 4. Verification & Trust

#### 4.1 Verification Badges
- **Types**:
  - Email verified
  - Phone verified
  - KYC verified
  - Business verified (providers)
  - Insurance verified (providers)

#### 4.2 Trust Indicators
- Member duration
- Response rate
- Completion rate (providers)
- Review authenticity

## Technical Requirements

### 1. API Endpoints

#### Profile Retrieval
```
GET /api/v1/Profile/{userId}
GET /api/v1/Profile/me
GET /api/v1/Profile/{userId}/pets
GET /api/v1/Profile/{userId}/services
GET /api/v1/Profile/{userId}/reviews
```

#### Profile Management
```
PUT /api/v1/Profile/me
POST /api/v1/Profile/me/photo
DELETE /api/v1/Profile/me/photo
PUT /api/v1/Profile/me/settings
```

#### Service Provider Specific
```
PUT /api/v1/Profile/provider/business
POST /api/v1/Profile/provider/services
PUT /api/v1/Profile/provider/services/{serviceId}
DELETE /api/v1/Profile/provider/services/{serviceId}
POST /api/v1/Profile/provider/certifications
```

### 2. Data Transfer Objects (DTOs)

```csharp
public class UserProfileDto
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public UserType UserType { get; set; }
    public bool IsVerified { get; set; }
    public DateTimeOffset MemberSince { get; set; }
    public string FriendCode { get; set; }
    public ProfileStatsDto Stats { get; set; }
    public List<VerificationBadgeDto> Badges { get; set; }
}

public class PetOwnerProfileDto : UserProfileDto
{
    public List<PetProfileSummaryDto> Pets { get; set; }
    public int ReviewsGiven { get; set; }
    public decimal ResponseRate { get; set; }
}

public class ServiceProviderProfileDto : UserProfileDto
{
    public string BusinessName { get; set; }
    public string? BusinessDescription { get; set; }
    public List<string> ServiceAreas { get; set; }
    public int YearsOfExperience { get; set; }
    public List<string> Specializations { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public decimal ResponseTimeHours { get; set; }
    public bool IsPremium { get; set; }
    public List<ServiceOfferingDto> Services { get; set; }
    public List<ReviewDto> RecentReviews { get; set; }
}
```

### 3. Database Schema Changes

```sql
-- Add profile-specific fields to ApplicationUser
ALTER TABLE ApplicationUser ADD
    Bio NVARCHAR(1000) NULL,
    ProfileVisibility VARCHAR(20) DEFAULT 'Public',
    LastProfileUpdate DATETIMEOFFSET NULL,
    ProfileCompleteness INT DEFAULT 0;

-- Add profile view tracking
CREATE TABLE ProfileViews (
    Id NVARCHAR(450) PRIMARY KEY,
    ProfileUserId NVARCHAR(450) NOT NULL,
    ViewerUserId NVARCHAR(450) NULL,
    ViewedAt DATETIMEOFFSET NOT NULL,
    ViewerType VARCHAR(20) NOT NULL,
    FOREIGN KEY (ProfileUserId) REFERENCES ApplicationUser(Id),
    FOREIGN KEY (ViewerUserId) REFERENCES ApplicationUser(Id)
);

-- Add favorite profiles
CREATE TABLE FavoriteProfiles (
    Id NVARCHAR(450) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    FavoriteUserId NVARCHAR(450) NOT NULL,
    CreatedAt DATETIMEOFFSET NOT NULL,
    Notes NVARCHAR(500) NULL,
    FOREIGN KEY (UserId) REFERENCES ApplicationUser(Id),
    FOREIGN KEY (FavoriteUserId) REFERENCES ApplicationUser(Id)
);
```

## UI/UX Requirements

### 1. Desktop Layout

```
┌─────────────────────────────────────────────────────────────┐
│  [Back]                My Profile              [Edit]        │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────┐  Sarah Johnson                                  │
│  │         │  Pet Owner · London, UK                         │
│  │  Photo  │  Member since Jan 2024                          │
│  │         │  ⭐ Verified · 🟢 Active Now                    │
│  └─────────┘  Friend Code: ABC123XY                          │
│                                                               │
│  [About]────────────────────────────────────                 │
│  Loving dog mom to two energetic pups. Always looking        │
│  for reliable pet services in North London area.             │
│                                                               │
│  [My Pets]──────────────────────────────────                 │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐                        │
│  │ Max     │ │ Luna    │ │ + Add   │                        │
│  │ Golden  │ │ Husky   │ │  Pet    │                        │
│  │ 3 years │ │ 5 years │ │         │                        │
│  └─────────┘ └─────────┘ └─────────┘                        │
│                                                               │
│  [Activity & Stats]─────────────────────────                 │
│  📝 15 Reviews Given  ⚡ 95% Response Rate                   │
│  🏆 Trusted Member    📅 50 Bookings                        │
└─────────────────────────────────────────────────────────────┘
```

### 2. Mobile Layout

```
┌─────────────────────┐
│ < Back   My Profile │
├─────────────────────┤
│    ┌─────────┐      │
│    │         │      │
│    │  Photo  │      │
│    │         │      │
│    └─────────┘      │
│                     │
│  Sarah Johnson      │
│  Pet Owner          │
│  London, UK         │
│  ⭐ Verified        │
│                     │
│ ┌─────────────────┐ │
│ │ View as: Owner ▼│ │
│ └─────────────────┘ │
│                     │
│ [About]             │
│ Loving dog mom...   │
│                     │
│ [My Pets] (2)    >  │
│ [Reviews] (15)   >  │
│ [Stats]          >  │
│                     │
│ Friend Code:        │
│ ABC123XY [Copy]     │
└─────────────────────┘
```

### 3. Service Provider View

```
┌─────────────────────────────────────────────────────────────┐
│  Premium Provider ⭐                                          │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────┐  Mike's Dog Walking Service                     │
│  │         │  Professional Dog Walker                        │
│  │  Logo   │  ⭐⭐⭐⭐⭐ 4.8 (127 reviews)                    │
│  │         │  📍 Covers: Camden, Islington, Hackney          │
│  └─────────┘  ⚡ Responds in ~2 hours                        │
│                                                               │
│  [Services & Pricing]────────────────────────                │
│  • Dog Walking (30 min): £15                                 │
│  • Dog Walking (60 min): £25                                 │
│  • Pet Sitting (per day): £45                                │
│                                           [View All]         │
│                                                               │
│  [About the Business]────────────────────                    │
│  10+ years experience. Fully insured and DBS checked.        │
│  Specializing in energetic breeds and behavioral training.   │
│                                                               │
│  [Credentials]───────────────────────────                    │
│  ✓ Business Insurance  ✓ DBS Checked  ✓ First Aid Cert     │
│                                                               │
│  [Recent Reviews]────────────────────────                    │
│  ⭐⭐⭐⭐⭐ "Mike is fantastic with our..." - Sarah J.       │
│  ⭐⭐⭐⭐⭐ "Very reliable and professional..." - Tom K.      │
│                                           [See All Reviews]   │
└─────────────────────────────────────────────────────────────┘
```

## Mobile Responsiveness Requirements

### 1. Breakpoints
- Mobile: < 640px
- Tablet: 640px - 1024px
- Desktop: > 1024px

### 2. Mobile-Specific Features
- Swipeable photo galleries
- Collapsible sections
- Bottom sheet for actions
- Touch-optimized buttons
- Simplified navigation

### 3. Performance
- Lazy load images
- Progressive image loading
- Minimize initial payload
- Cache profile data
- Optimize for 3G connections

## Success Metrics

### 1. Engagement Metrics
- Profile completion rate > 80%
- Profile views per user per month
- Profile update frequency
- Photo upload rate

### 2. Trust Metrics
- Verification completion rate
- Friend code usage rate
- Profile report rate < 0.1%
- Fake profile detection

### 3. Business Metrics
- Profile to booking conversion rate
- Premium upgrade rate (providers)
- User retention correlation with profile completion
- Cross-role adoption rate

## Integration Note

The following sections originally designed for Account Settings should be integrated into the Profile page for a better user experience:

### 1. Profile Settings Section
This section should be integrated into the Profile editing interface and include:
- **Basic Information Editing**
  - Name (display name)
  - Phone number
  - Timezone preferences
  - Language settings
- **Profile Photo Upload Functionality**
  - Photo upload/change capabilities
  - Photo cropping and optimization
  - Avatar fallback options

### 2. Business Settings Section (Service Providers Only)
For users with service provider roles, these business-specific settings should be part of the Profile page:
- **Business Profile Information**
  - Business name and description
  - Business contact details
  - Operating entity information
- **Service Area Configuration**
  - Geographic coverage settings
  - Service radius or specific area selection
  - Multiple location management
- **Booking Rules and Policies**
  - Cancellation policies
  - Booking requirements
  - Terms of service
  - Payment policies
- **Business Hours and Availability**
  - Operating hours by day
  - Holiday schedules
  - Availability calendar integration
  - Automatic out-of-office settings

These features were originally conceived as separate Account Settings but should be seamlessly integrated into the Profile page to create a unified experience where users can manage all aspects of their public presence and business configuration in one location.

## Implementation Notes

### 1. Phase 1 - Core Profile (Week 1-2)
- Basic profile display
- Edit functionality
- Photo upload
- Pet profile integration

### 2. Phase 2 - Provider Features (Week 3-4)
- Service management
- Business information
- Review display
- Premium features

### 3. Phase 3 - Advanced Features (Week 5-6)
- Verification system
- Analytics dashboard
- Social features
- Mobile optimizations

### 4. Technical Considerations
- Use lazy loading for images
- Implement proper caching strategy
- Ensure GDPR compliance for profile data
- Add proper SEO meta tags for public profiles
- Implement rate limiting for profile views
- Use CDN for profile images
- Add proper error boundaries
- Implement offline support for own profile

### 5. Security & Privacy
- Sanitize all user inputs
- Implement proper access controls
- Hide sensitive information from public view
- Add profile privacy settings
- Log profile access for security
- Implement CAPTCHA for public views
- Add abuse reporting system

### 6. Accessibility
- WCAG 2.1 AA compliance
- Screen reader support
- Keyboard navigation
- High contrast mode support
- Proper ARIA labels
- Focus management

## Acceptance Criteria

### 1. Pet Owner Profile
- [ ] User can view their own profile
- [ ] User can edit profile information
- [ ] User can upload/change profile photo
- [ ] User can add/edit/remove pet profiles
- [ ] Verification badges display correctly
- [ ] Friend code is displayed and copyable
- [ ] Profile shows activity statistics

### 2. Service Provider Profile
- [ ] Provider can manage business information
- [ ] Provider can add/edit/remove services
- [ ] Reviews and ratings display correctly
- [ ] Service areas are clearly shown
- [ ] Premium badge displays for premium users
- [ ] Response time is calculated and shown
- [ ] Credentials can be uploaded and verified

### 3. Dual-Role Support
- [ ] Users can switch between profile types
- [ ] Information is properly segregated
- [ ] Profile type selection persists
- [ ] No data leakage between roles

### 4. Mobile Experience
- [ ] All features work on mobile devices
- [ ] Touch gestures work properly
- [ ] Performance is acceptable on 3G
- [ ] Offline viewing of own profile works

### 5. Performance
- [ ] Profile loads in < 2 seconds
- [ ] Image uploads complete in < 5 seconds
- [ ] Search indexing works properly
- [ ] No memory leaks in profile views

## Dependencies

### 1. External Services
- AWS S3 or Azure Blob Storage for images
- Google Maps API for service areas
- Email service for verifications
- SMS service for phone verification

### 2. Internal Dependencies
- Authentication system
- User management system
- Review system
- Messaging system
- Payment system (for premium features)

### 3. Team Dependencies
- UX/UI design team for mockups
- Backend team for API development
- Mobile team for app integration
- QA team for testing
- DevOps for infrastructure

## Risk Mitigation

### 1. Technical Risks
- **Risk**: Image storage costs
  - **Mitigation**: Implement image compression and limits
- **Risk**: Profile spam/abuse
  - **Mitigation**: Rate limiting and moderation tools

### 2. Business Risks
- **Risk**: Low profile completion
  - **Mitigation**: Gamification and onboarding flow
- **Risk**: Privacy concerns
  - **Mitigation**: Clear privacy controls and education

### 3. User Experience Risks
- **Risk**: Complex dual-role navigation
  - **Mitigation**: User testing and iterative design
- **Risk**: Mobile performance issues
  - **Mitigation**: Progressive enhancement approach

## Future Enhancements

### 1. Social Features
- Profile sharing to social media
- Following/Followers system
- Profile recommendations
- Community badges

### 2. Advanced Features
- Video introductions
- Virtual business cards
- Calendar integration
- AI-powered profile optimization

### 3. Analytics
- Profile view analytics
- Conversion tracking
- A/B testing framework
- Competitor analysis tools

## Conclusion

The My Profile feature is a cornerstone of the MeAndMyDoggy platform, serving as the primary way users present themselves and build trust within the community. By supporting both pet owners and service providers with tailored experiences while maintaining a cohesive design, we create a flexible system that grows with our users' needs. The implementation should prioritize mobile experience, trust-building features, and seamless role management to ensure success.