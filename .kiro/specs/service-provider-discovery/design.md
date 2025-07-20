# Service Provider Discovery - Design Document

## Overview

The Service Provider Discovery system creates a comprehensive marketplace experience that connects pet owners with qualified service providers through intelligent search, detailed profiles, and seamless booking. The design emphasizes location-based discovery, transparent pricing, and mobile-first interactions that make finding and booking pet services as simple as ordering food delivery.

## Architecture

### Enhanced Component Architecture
```
ServiceDiscoveryContainer (Full Width Layout - w-full max-w-none)
├── SearchHeader (bg-white border-b border-gray-200 sticky top-0 z-40)
│   ├── LocationSelector (with GPS detection)
│   ├── SearchInput (with autocomplete and fuzzy matching)
│   └── FilterToggle (mobile only)
├── EnhancedFilterBar (Horizontal, Always Visible, bg-gray-50 p-4 border-b border-gray-200)
│   ├── ServiceTypeFilter (Primary categories with fixed hover: hover:bg-primary-50 hover:text-primary-700)
│   ├── SubServiceTypeFilter (Dynamic dropdown based on selected service type)
│   ├── DogSizeFilter (Small, Medium, Large, Extra Large with dog icons)
│   ├── NumberOfDogsFilter (1, 2, 3, 4+ with counter badges)
│   ├── DateRangeFilter (Mobiscroll date range picker integration)
│   ├── PriceRangeFilter (£ range slider with currency formatting)
│   ├── RatingFilter (Star rating selector 1-5 stars)
│   ├── AvailabilityFilter (Available now, This week, Custom dates)
│   └── ClearFiltersButton (Clear all active filters)
├── MainContentArea (Full Width Grid Layout - grid grid-cols-12 gap-6 px-4 sm:px-6 lg:px-8)
│   ├── ResultsSection (col-span-12 lg:col-span-7)
│   │   ├── ResultsHeader (flex justify-between items-center mb-4)
│   │   │   ├── SortOptions (Premium first toggle + sort dropdown)
│   │   │   ├── ResultsCount (with active filter summary chips)
│   │   │   └── ViewToggle (List/Grid view icons)
│   │   └── ProviderList (space-y-4 with infinite scroll)
│   │       └── EnhancedProviderCard (bg-white rounded-xl shadow-sm border border-gray-200 p-6)
│   │           ├── PremiumBadge (bg-primary-500 text-white px-3 py-1 rounded-full text-sm font-medium)
│   │           ├── ProviderHeader (flex items-start justify-between)
│   │           │   ├── ProviderInfo
│   │           │   │   ├── ProfileImage (w-16 h-16 rounded-full)
│   │           │   │   ├── ProviderName (text-xl font-semibold)
│   │           │   │   ├── BusinessName (text-gray-600)
│   │           │   │   └── VerificationBadges (flex gap-2)
│   │           │   └── FavoriteButton (heart icon toggle)
│   │           ├── LocationInfo (text-gray-500 flex items-center gap-2)
│   │           ├── ServiceSummary (flex flex-wrap gap-2 mt-3)
│   │           ├── RatingDisplay (flex items-center gap-2)
│   │           ├── PriceRange (text-lg font-medium text-primary-600)
│   │           ├── AvailabilityIndicator (bg-green-100 text-green-800 px-2 py-1 rounded)
│   │           └── ActionButtons (grid grid-cols-4 gap-2 mt-4)
│   │               ├── AvailabilityButton (bg-blue-50 hover:bg-blue-100 text-blue-700 px-4 py-2 rounded-lg flex items-center gap-2)
│   │               ├── ContactButton (bg-green-50 hover:bg-green-100 text-green-700 px-4 py-2 rounded-lg flex items-center gap-2)
│   │               ├── ServicesButton (bg-purple-50 hover:bg-purple-100 text-purple-700 px-4 py-2 rounded-lg flex items-center gap-2)
│   │               └── BookButton (bg-primary-500 hover:bg-primary-600 text-white px-4 py-2 rounded-lg font-medium)
│   └── MapAndCalendarSection (col-span-12 lg:col-span-5)
│       ├── GoogleMapContainer (h-96 rounded-xl overflow-hidden shadow-sm border border-gray-200)
│       │   ├── InteractiveMap (Google Maps with custom styling)
│       │   ├── MapControls (absolute top-4 right-4 z-10)
│       │   ├── ProviderMarkers (Premium: gold markers, Free: blue markers)
│       │   └── MarkerClustering (for dense areas)
│       └── MobiscrollCalendar (mt-6 bg-white rounded-xl shadow-sm border border-gray-200 p-4)
│           ├── CalendarHeader (Provider availability legend)
│           ├── MobiscrollDatePicker (integrated with availability data)
│           └── AvailabilityLegend (Available, Busy, Unavailable color coding)
├── AvailabilityModal (Mobiscroll calendar modal - max-w-2xl)
│   ├── ModalHeader (Provider info with close button)
│   ├── CalendarView (Mobiscroll calendar with time slots)
│   ├── TimeSlotGrid (Available slots for selected date)
│   ├── ServiceSelector (If provider offers multiple services)
│   └── BookFromCalendarButton (Primary CTA)
├── ContactModal (Message composition modal - max-w-lg)
│   ├── ModalHeader (Provider name and profile image)
│   ├── MessageForm (textarea with character counter)
│   ├── MessageTemplates (Quick message options)
│   ├── AttachmentUpload (Dog photos/medical records)
│   ├── DogProfileSelector (Select which dogs this is about)
│   └── SendMessageButton (with delivery confirmation)
├── ServicesModal (Comprehensive service listing - max-w-4xl)
│   ├── ModalHeader (Provider name, rating, and close button)
│   ├── ServiceCategories (Tabbed interface for service types)
│   ├── ServicesList (Expandable service cards)
│   │   ├── ServiceCard
│   │   │   ├── ServiceName (text-lg font-semibold)
│   │   │   ├── ServiceDescription (text-gray-600)
│   │   │   ├── Duration (with clock icon)
│   │   │   ├── BasePrice (£X.XX in large text)
│   │   │   └── SubServicesList (nested expandable list)
│   │   └── AddOnsSection (Additional services and extras)
│   ├── RatesTable (Comprehensive pricing table)
│   │   ├── TableHeader (Service | Duration | Price | Book)
│   │   └── TableRows (with individual booking buttons)
│   └── BookServiceButton (Sticky bottom CTA)
└── URLParameterManager (Handles all filter state in URL)
    ├── FilterStateSync (Sync filters with URL params)
    ├── ShareableURLs (Generate shareable filtered URLs)
    └── BrowserHistory (Handle back/forward navigation)
```

### Search Architecture
```
Search Query → Search Service → Elasticsearch/Database
                            ↓
Location Service → Geospatial Indexing → Filtered Results
                            ↓
Recommendation Engine → Personalization → Ranked Results
                            ↓
Cache Layer → Response Optimization → Client Display
```

### Booking Architecture
```
Booking Request → Validation Service → Availability Check
                                   ↓
Payment Processing → Provider Notification → Confirmation
                                   ↓
Calendar Integration → Reminder System → Follow-up
```

## Components and Interfaces

### Enhanced Search and Filter Components
```typescript
interface EnhancedSearchFilters {
  location: LocationFilter
  serviceTypes: string[] // Primary service categories
  subServiceTypes: string[] // Dynamic based on selected service types
  dogSize: DogSize[] // Small, Medium, Large, Extra Large
  numberOfDogs: NumberRange // 1, 2, 3, 4+
  dateRange: DateRangeFilter // Start and end dates for availability
  priceRange: PriceRange
  availability: AvailabilityFilter
  rating: number
  specialties: string[]
  distance: number
  sortBy: SortOption
  isPremiumFirst: boolean // Premium providers prioritized
}

interface LocationFilter {
  address: string
  coordinates: Coordinates
  radius: number // in miles
  useCurrentLocation: boolean
}

interface DateRangeFilter {
  startDate: Date | null
  endDate: Date | null
  isFlexible: boolean // Allow +/- 1 day flexibility
}

interface DogSize {
  id: string
  name: 'Small' | 'Medium' | 'Large' | 'Extra Large'
  weightRange: string // e.g., "0-25 lbs"
  icon: string
}

interface NumberRange {
  min: number
  max: number | null // null for "4+"
  display: string // "1", "2", "3", "4+"
}

interface ServiceTypeHierarchy {
  id: string
  name: string
  icon: string
  subServices: SubServiceType[]
  popularSubServices: string[] // Most commonly searched
}

interface SubServiceType {
  id: string
  name: string
  parentServiceId: string
  description: string
  averagePrice: number
  duration: number
}

// URL Parameter Management
interface URLSearchParams {
  location?: string
  lat?: number
  lng?: number
  radius?: number
  services?: string // comma-separated service type IDs
  subServices?: string // comma-separated sub-service IDs
  dogSize?: string // comma-separated size IDs
  dogCount?: string // number or "4+"
  startDate?: string // ISO date string
  endDate?: string // ISO date string
  minPrice?: number
  maxPrice?: number
  rating?: number
  sort?: string
  premium?: boolean
  view?: 'list' | 'grid' | 'map'
}

interface ServiceProvider {
  id: string
  name: string
  businessName: string
  profileImage: string
  rating: number
  reviewCount: number
  services: Service[]
  location: Location
  availability: AvailabilitySlot[]
  pricing: PricingInfo
  certifications: Certification[]
  specialties: string[]
  responseTime: string
  distance?: number
}

interface Service {
  id: string
  name: string
  description: string
  basePrice: number
  duration: number
  category: ServiceCategory
  requirements: string[]
  addOns: AddOnService[]
}
```

### Provider Profile Components
```typescript
interface ProviderProfile extends ServiceProvider {
  bio: string
  experience: string
  photos: ProviderPhoto[]
  reviews: Review[]
  verificationStatus: VerificationStatus
  insurance: InsuranceInfo
  policies: ServicePolicies
  calendar: AvailabilityCalendar
  contactInfo: ContactInfo
}

interface Review {
  id: string
  userId: string
  userName: string
  rating: number
  comment: string
  photos: string[]
  serviceType: string
  date: Date
  verified: boolean
  helpful: number
}

interface AvailabilityCalendar {
  timeZone: string
  workingHours: WorkingHours
  availableSlots: AvailabilitySlot[]
  blockedDates: Date[]
  recurringAvailability: RecurringSchedule[]
}
```

### Booking Components
```typescript
interface BookingRequest {
  providerId: string
  serviceId: string
  dogIds: string[]
  scheduledDate: Date
  duration: number
  specialInstructions: string
  addOns: string[]
  totalPrice: number
  paymentMethodId: string
}

interface BookingConfirmation {
  bookingId: string
  status: BookingStatus
  provider: ServiceProvider
  service: Service
  scheduledDate: Date
  totalPrice: number
  cancellationPolicy: CancellationPolicy
  contactInfo: ContactInfo
}

interface PaymentInfo {
  subtotal: number
  fees: Fee[]
  taxes: number
  total: number
  paymentMethod: PaymentMethod
  refundPolicy: RefundPolicy
}
```

### Map Integration Components
```typescript
interface MapView {
  center: Coordinates
  zoom: number
  providers: MapProvider[]
  selectedProvider?: string
  onProviderSelect: (providerId: string) => void
  onMapMove: (bounds: MapBounds) => void
}

interface MapProvider {
  id: string
  coordinates: Coordinates
  name: string
  rating: number
  priceRange: string
  services: string[]
  availability: 'available' | 'busy' | 'unavailable'
}
```

## Data Models

### Enhanced Service Provider Model
```typescript
interface EnhancedServiceProvider {
  // Basic Information
  id: string
  userId: string
  businessName: string
  displayName: string
  profileImage: string
  coverImages: string[]
  
  // Location and Service Area
  primaryLocation: Location
  serviceAreas: ServiceArea[]
  mobilityOptions: MobilityOption[]
  
  // Services and Pricing
  services: EnhancedService[]
  pricingTiers: PricingTier[]
  specialOffers: SpecialOffer[]
  
  // Credentials and Verification
  certifications: Certification[]
  insurance: InsuranceInfo
  backgroundCheck: BackgroundCheckStatus
  verificationBadges: VerificationBadge[]
  
  // Reviews and Ratings
  overallRating: number
  ratingBreakdown: RatingBreakdown
  reviews: Review[]
  responseRate: number
  responseTime: string
  
  // Availability and Booking
  calendar: ProviderCalendar
  bookingSettings: BookingSettings
  cancellationPolicy: CancellationPolicy
  
  // Business Information
  businessHours: BusinessHours
  contactMethods: ContactMethod[]
  languages: string[]
  establishedDate: Date
  
  // Specializations
  breedSpecialties: string[]
  serviceSpecialties: string[]
  specialNeeds: SpecialNeedsCapability[]
  
  // Performance Metrics
  completedBookings: number
  repeatCustomers: number
  averageRating: number
  joinDate: Date
  lastActive: Date
}
```

### Search and Recommendation Models
```typescript
interface SearchQuery {
  query: string
  location: LocationQuery
  filters: SearchFilters
  sort: SortCriteria
  pagination: PaginationInfo
  userContext: UserContext
}

interface SearchResult {
  providers: ServiceProvider[]
  totalCount: number
  facets: SearchFacets
  suggestions: SearchSuggestion[]
  mapBounds: MapBounds
  searchId: string
}

interface RecommendationContext {
  userId: string
  dogProfiles: DogProfile[]
  bookingHistory: BookingHistory[]
  preferences: UserPreferences
  location: Location
  searchHistory: SearchHistory[]
}
```

### Booking and Payment Models
```typescript
interface BookingFlow {
  sessionId: string
  currentStep: BookingStep
  selectedProvider: ServiceProvider
  selectedService: Service
  selectedDogs: DogProfile[]
  schedulingInfo: SchedulingInfo
  paymentInfo: PaymentInfo
  specialRequests: SpecialRequest[]
  totalPrice: PriceBreakdown
}

interface PriceBreakdown {
  basePrice: number
  addOns: AddOnPrice[]
  fees: ServiceFee[]
  taxes: TaxInfo[]
  discounts: Discount[]
  total: number
  currency: string
}
```

## Error Handling

### Search Error Handling
- **Location Errors**: Graceful fallback to manual location entry
- **No Results**: Helpful suggestions and alternative searches
- **Filter Conflicts**: Smart filter adjustment recommendations
- **Map Loading**: Fallback to list view with location information

### Booking Error Handling
- **Availability Conflicts**: Real-time availability updates and alternatives
- **Payment Failures**: Multiple payment method options and retry mechanisms
- **Provider Unavailability**: Automatic rebooking suggestions
- **Network Issues**: Offline booking queue with sync when online

### Profile Loading Errors
- **Image Loading**: Progressive image loading with placeholders
- **Review Loading**: Graceful degradation with cached reviews
- **Calendar Loading**: Fallback to contact provider directly
- **Data Inconsistency**: Clear error messages with refresh options

## Testing Strategy

### Search Functionality Testing
- **Location-based Search**: GPS accuracy and radius calculations
- **Filter Combinations**: All filter permutations and edge cases
- **Performance Testing**: Search response times under load
- **Mobile Search**: Touch interactions and responsive design

### Booking Flow Testing
- **End-to-end Booking**: Complete booking process validation
- **Payment Integration**: All payment methods and error scenarios
- **Calendar Integration**: Availability accuracy and conflicts
- **Notification Testing**: Email and SMS delivery verification

### Map Integration Testing
- **Map Performance**: Smooth zooming and panning with many markers
- **Marker Clustering**: Proper clustering and expansion behavior
- **Mobile Map**: Touch gestures and responsive map controls
- **Offline Maps**: Cached map functionality without internet

### Provider Profile Testing
- **Profile Completeness**: All profile sections and data validation
- **Review System**: Review submission, moderation, and display
- **Photo Gallery**: Image loading, optimization, and lightbox
- **Responsive Design**: Profile display across all device sizes

## Performance Considerations

### Search Performance
- **Elasticsearch Integration**: Fast full-text and geospatial search
- **Result Caching**: Cache popular searches and location-based results
- **Progressive Loading**: Load search results incrementally
- **Search Optimization**: Debounced search with intelligent prefetching

### Map Performance
- **Marker Optimization**: Efficient marker rendering and clustering
- **Tile Caching**: Cache map tiles for offline functionality
- **Viewport Optimization**: Only load providers in visible area
- **Mobile Performance**: Optimized touch interactions and smooth animations

### Booking Performance
- **Real-time Availability**: WebSocket updates for calendar changes
- **Payment Processing**: Fast, secure payment with minimal redirects
- **Form Optimization**: Smart form validation and auto-completion
- **Mobile Booking**: Streamlined mobile booking flow

## Security Considerations

### Payment Security
- **PCI Compliance**: Secure payment processing with tokenization
- **Fraud Prevention**: Advanced fraud detection and prevention
- **Data Encryption**: End-to-end encryption for sensitive data
- **Secure Storage**: Encrypted storage of payment information

### Provider Verification
- **Background Checks**: Integration with verification services
- **Credential Validation**: Automated certificate verification
- **Review Authenticity**: Anti-fraud measures for fake reviews
- **Identity Verification**: Multi-factor provider identity confirmation

### User Privacy
- **Location Privacy**: Opt-in location sharing with granular controls
- **Data Protection**: GDPR compliance for user data handling
- **Communication Security**: Secure messaging between users and providers
- **Profile Privacy**: Granular privacy controls for user information