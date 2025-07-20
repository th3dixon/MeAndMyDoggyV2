# Enhanced Service Provider Discovery - Updated Specification

## Overview
The service discovery page is the core of the MeAndMyDoggy platform, enabling users to find, filter, and connect with pet service providers. This enhanced version provides comprehensive filtering, mapping, and provider interaction capabilities.

## Layout Structure

### Full-Width Layout
- **Left Panel (60%)**: Search filters and results
- **Right Panel (40%)**: Google Map and calendar
- **No sidebar navigation**: Full screen real estate utilization

### Component Hierarchy
```
Header (Search bar + Quick filters)
├── Advanced Filters Panel
├── Search Results (Left)
│   ├── Results Header (count, sorting)
│   ├── Provider Cards (Premium first, then Free)
│   └── Pagination
└── Map & Calendar (Right)
    ├── Google Map (60% height)
    └── Mobiscroll Calendar (40% height)
```

## Enhanced Filter System

### Primary Service Types
- Dog Walking
- Pet Sitting
- Grooming
- Training
- Veterinary Services
- Pet Boarding

### Sub-Service Types (Context-Dependent)
**Dog Walking:**
- Quick Walk (15-30 min)
- Standard Walk (30-60 min)
- Extended Walk (60+ min)
- Group Walk
- Solo Walk

**Pet Sitting:**
- Drop-in Visits
- Half-day Sitting
- Full-day Sitting
- Overnight Care
- Holiday Care

**Grooming:**
- Basic Wash & Dry
- Full Grooming
- Nail Trimming
- Ear Cleaning
- Teeth Cleaning

**Training:**
- Basic Obedience
- Behavioral Training
- Puppy Training
- Advanced Training
- Agility Training

### Extended Filters
1. **Dog Size**: Small, Medium, Large, Giant
2. **Number of Dogs**: 1, 2, 3, 4+
3. **Date Range**: Start date - End date picker
4. **Distance**: 1, 3, 5, 10+ miles
5. **Price Range**: £0-£50+ per service
6. **Rating**: 4+, 4.5+, 5 stars
7. **Availability**: Weekdays, Weekends, Evenings, Emergency

## Search Results Enhancement

### Provider Card Layout
```
[Premium Badge] Provider Name                    [★ 4.9]
Photo | Location • Response Time
      | Services: Walking, Sitting
      | From £15/service
      | [Availability] [Contact] [Services] [Book Now]
```

### Provider Status Hierarchy
1. **Premium Providers**: Top of results, special badge
2. **Free Providers**: Below premium, standard display

### Action Buttons
1. **Availability**: Shows calendar modal with provider's available dates
2. **Contact**: Opens messaging interface
3. **Services**: Shows popup with detailed services and pricing
4. **Book Now**: Direct booking flow

## Map Integration

### Google Map Features
- Real-time provider locations
- Cluster markers for density
- Click to highlight provider
- Zoom controls
- Search area visualization

### Map-Results Synchronization
- Hover provider card → highlight map marker
- Click map marker → highlight provider card
- Map bounds change → update search results

## Calendar Integration

### Mobiscroll Calendar
- Provider availability visualization
- Date range selection
- Multi-provider comparison
- Booking slot indicators

### Calendar Features
- Available dates (green)
- Partially booked (yellow)
- Fully booked (red)
- User selected dates (blue)

## URL State Management

### Query Parameters
```
?service=walking
&subservice=standard-walk
&dogs=2
&size=medium
&startdate=2024-01-15
&enddate=2024-01-22
&distance=5
&price=15-30
&rating=4.5
&page=1
```

### State Synchronization
- Filter changes → Update URL
- URL load → Apply filters
- Shareable links maintain all filter state

## Interactive Features

### Filter Interactions
- **Hover States**: Subtle background change (not white/invisible)
- **Active States**: Orange background with white text
- **Multi-select**: Checkboxes for compatible filters
- **Dependency**: Sub-services update based on main service

### Search Result Interactions
- **Lazy Loading**: Load more results on scroll
- **Pagination**: Traditional pagination with URL state
- **Sorting**: Distance, Rating, Price, Availability
- **Real-time Updates**: Filters apply without page reload

## Modal Windows

### Availability Modal
- Calendar view of provider availability
- Available time slots
- Booking quick-action
- Close/overlay interaction

### Contact Modal
- Pre-filled message form
- Service context included
- Send message action
- Provider response expectations

### Services Modal
- Tabbed interface by service type
- Pricing table
- Service descriptions
- Add to booking actions

## Responsive Design

### Mobile (320px-768px)
- Stacked layout (map below results)
- Collapsible filters
- Swipe gestures
- Touch-optimized buttons

### Tablet (768px-1024px)
- Side-by-side layout
- Expandable map
- Touch and mouse support

### Desktop (1024px+)
- Full layout as specified
- Hover interactions
- Keyboard navigation
- Multi-monitor support

## Performance Considerations

### Loading States
- Skeleton loading for provider cards
- Map loading indicators
- Filter application feedback
- Pagination loading

### Data Management
- Efficient filtering algorithms
- Map marker clustering
- Calendar data caching
- Search result pagination

## Accessibility Features

### WCAG 2.1 AA Compliance
- Screen reader support
- Keyboard navigation
- High contrast mode
- Focus indicators
- Alternative text for images

### Usability Enhancements
- Clear filter labels
- Helpful error messages
- Loading progress indicators
- Success/failure feedback

## Technical Implementation Notes

### Dependencies
- Google Maps JavaScript API
- Mobiscroll Calendar component
- Existing Tailwind CSS framework
- Alpine.js for interactions

### Data Structure
```javascript
// Provider object
{
  id: number,
  name: string,
  type: 'premium' | 'free',
  rating: number,
  location: { lat: number, lng: number },
  services: Array<{
    type: string,
    subTypes: Array<string>,
    pricing: Array<{ service: string, price: number }>
  }>,
  availability: Array<{ date: string, slots: Array<string> }>
}
```

# Service Provider Registration Requirements

## Service Selection During Registration

### Step 4: Service & Pricing Setup (Service Providers Only)

**Required for Service Provider registration (UserType = 1 or 2)**

#### Service Categories (Must select at least one)
- **Dog Walking**
- **Pet Sitting** 
- **Grooming**
- **Training**
- **Veterinary Services**
- **Pet Boarding**

#### Sub-Service Selection (Must select at least one per selected category)

**Dog Walking Sub-Services:**
- Quick Walk (15-30 min) - Rate: £__ per walk
- Standard Walk (30-60 min) - Rate: £__ per walk  
- Extended Walk (60+ min) - Rate: £__ per walk
- Group Walk - Rate: £__ per walk
- Solo Walk - Rate: £__ per walk

**Pet Sitting Sub-Services:**
- Drop-in Visits - Rate: £__ per visit
- Half-day Sitting - Rate: £__ per day
- Full-day Sitting - Rate: £__ per day
- Overnight Care - Rate: £__ per night
- Holiday Care - Rate: £__ per day

**Grooming Sub-Services:**
- Basic Wash & Dry - Rate: £__ per session
- Full Grooming - Rate: £__ per session
- Nail Trimming - Rate: £__ per session
- Ear Cleaning - Rate: £__ per session
- Teeth Cleaning - Rate: £__ per session

**Training Sub-Services:**
- Basic Obedience - Rate: £__ per session
- Behavioral Training - Rate: £__ per session
- Puppy Training - Rate: £__ per session
- Advanced Training - Rate: £__ per session
- Agility Training - Rate: £__ per session

**Veterinary Services Sub-Services:**
- Vet Transport - Rate: £__ per trip
- Health Monitoring - Rate: £__ per visit
- Medicine Administration - Rate: £__ per visit
- Basic First Aid - Rate: £__ per incident

**Pet Boarding Sub-Services:**
- Daycare - Rate: £__ per day
- Overnight Boarding - Rate: £__ per night
- Weekend Boarding - Rate: £__ per weekend
- Holiday Boarding - Rate: £__ per day

#### Validation Rules
1. **At least one service category must be selected**
2. **At least one sub-service must be selected for each chosen category**
3. **All selected sub-services must have a rate entered (£0.01 minimum)**
4. **Rates must be in GBP (£) with maximum 2 decimal places**
5. **Rates must be reasonable (£1-£500 per service)**

#### Registration Flow Update
- **Step 1**: Account Type Selection
- **Step 2**: Personal Information  
- **Step 3**: Address Information
- **Step 4**: Service & Pricing Setup (Service Providers Only)
- **Step 5**: Review and Submit

#### Database Requirements
**ServiceProviderServices Table:**
```sql
CREATE TABLE ServiceProviderServices (
    Id uniqueidentifier PRIMARY KEY,
    ServiceProviderId uniqueidentifier FOREIGN KEY,
    ServiceCategoryId int FOREIGN KEY,
    SubServiceId int FOREIGN KEY,
    PriceInPence int NOT NULL,
    IsActive bit DEFAULT 1,
    CreatedAt datetime2 DEFAULT GETDATE(),
    UpdatedAt datetime2 DEFAULT GETDATE()
);
```

**ServiceCategories Table:**
```sql
CREATE TABLE ServiceCategories (
    Id int PRIMARY KEY IDENTITY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(500),
    IconClass nvarchar(50),
    IsActive bit DEFAULT 1
);
```

**SubServices Table:**
```sql
CREATE TABLE SubServices (
    Id int PRIMARY KEY IDENTITY,
    ServiceCategoryId int FOREIGN KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(500),
    IsActive bit DEFAULT 1
);
```

This enhanced specification provides a comprehensive framework for building a best-in-class pet service discovery experience that meets all user requirements while maintaining excellent usability and performance.