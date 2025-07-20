# Service Discovery Page - Implementation Guide

## Overview
This guide explains how to implement the service discovery page based on the comprehensive prototype.

## Key Features to Implement

### 1. Two-Tier Service Selection
- **Main Service Dropdown**: Triggers sub-service options dynamically
- **Sub-Services**: Appear based on main service selection
- **Visual Feedback**: Selected services highlighted with orange border

**Implementation Notes:**
```javascript
// Service data structure
const serviceCategories = {
    'dog-walking': {
        name: 'Dog Walking',
        icon: 'fa-dog',
        subServices: [
            { id: 'quick-walk', name: 'Quick Walk (15-30 min)', basePrice: 15 },
            { id: 'standard-walk', name: 'Standard Walk (30-60 min)', basePrice: 25 },
            { id: 'extended-walk', name: 'Extended Walk (60+ min)', basePrice: 35 }
        ]
    }
    // ... other services
};
```

### 2. Calendar Integration (FullCalendar)

**Installation:**
```bash
npm install @fullcalendar/core @fullcalendar/daygrid @fullcalendar/timegrid @fullcalendar/interaction
```

**Key Features:**
- Color-coded availability (green/yellow/red)
- Click to select time slots
- Provider availability overlay
- Date range selection for filtering

### 3. Google Maps Integration

**Required APIs:**
- Maps JavaScript API
- Places API (for address autocomplete)

**Implementation Steps:**
1. Get API key from Google Cloud Console
2. Add script tag with API key
3. Initialize map with custom markers
4. Implement marker clustering for many providers
5. Sync hover/click events with provider list

### 4. Provider Search & Filtering

**Search Parameters:**
- Location (postcode/address)
- Service type and sub-services
- Date range availability
- Price range (slider)
- Distance radius
- Provider ratings
- Special requirements (emergency, insurance, etc.)

**API Endpoint:**
```
POST /api/v1/providersearch/search
{
    "location": "SW1A 1AA",
    "serviceTypes": ["dog-walking", "pet-sitting"],
    "subServices": ["standard-walk", "overnight-care"],
    "dateFrom": "2024-01-15",
    "dateTo": "2024-01-20",
    "maxDistance": 5,
    "minRating": 4.0,
    "priceRange": { "min": 15, "max": 50 }
}
```

### 5. Modal Implementations

#### Availability Modal
- Shows provider's calendar with available slots
- Allows date/time selection
- Updates based on service type selected

#### Contact Modal
- Pre-filled message templates
- Pet profile selector
- Attachment upload
- Character counter

#### Services Modal
- Tabbed interface by service category
- Detailed pricing tables
- Package deals highlighted
- Individual service booking buttons

#### Booking Modal
- Service selection summary
- Pet information form
- Special requirements text area
- Price calculation
- Payment method selection

### 6. Pagination

**Features:**
- Page size selector (10, 20, 50, 100)
- Page navigation with numbers
- Previous/Next buttons
- Results count display
- URL parameter sync

**Implementation:**
```javascript
// Alpine.js pagination data
pagination: {
    currentPage: 1,
    totalPages: 10,
    resultsPerPage: 20,
    totalResults: 198,
    
    goToPage(page) {
        this.currentPage = page;
        this.updateResults();
        this.updateURL();
    }
}
```

### 7. Premium vs Regular Providers

**Visual Distinctions:**
- Gold badge with crown icon
- Glow effect (box-shadow)
- Priority positioning in results
- Larger map markers
- "Featured" tag in cards

### 8. Performance Optimizations

1. **Lazy Loading**
   - Load map only when visible
   - Defer calendar initialization
   - Progressive image loading

2. **Caching**
   - Cache provider data for session
   - Store filter preferences
   - Cache geocoding results

3. **Debouncing**
   - Search input (300ms delay)
   - Map bounds changes
   - Filter updates

### 9. Responsive Design

**Breakpoints:**
- Mobile: < 768px (stacked layout)
- Tablet: 768px - 1024px (compact sidebar)
- Desktop: > 1024px (full layout)

**Mobile Specific:**
- Bottom sheet for filters
- Swipeable provider cards
- Full-screen map toggle
- Touch-optimized controls

### 10. Accessibility

- ARIA labels for all interactive elements
- Keyboard navigation support
- Screen reader announcements
- Focus management in modals
- High contrast mode support

## Database Queries

### Main Search Query
```sql
EXEC sp_SearchProviders 
    @Postcode = 'SW1A 1AA',
    @ServiceTypes = 'dog-walking,pet-sitting',
    @MaxDistance = 5,
    @MinRating = 4.0,
    @PageSize = 20,
    @PageNumber = 1
```

### Availability Check
```sql
EXEC sp_GetProviderAvailability
    @ProviderId = 123,
    @StartDate = '2024-01-15',
    @EndDate = '2024-01-20',
    @ServiceType = 'dog-walking'
```

## State Management

Use Alpine.js for reactive state:
```javascript
Alpine.data('serviceDiscovery', () => ({
    // Search state
    searchParams: {
        location: '',
        services: [],
        dateRange: null,
        filters: {}
    },
    
    // Results state
    providers: [],
    loading: false,
    
    // UI state
    activeView: 'list',
    selectedProvider: null,
    activeModal: null,
    
    // Methods
    async search() { /* ... */ },
    showModal(type, provider) { /* ... */ },
    updateFilters() { /* ... */ }
}));
```

## Error Handling

1. **Network Errors**: Show retry button
2. **No Results**: Show helpful suggestions
3. **Invalid Postcode**: Inline validation
4. **API Errors**: User-friendly messages
5. **Map Load Failure**: Fallback to list view

## Testing Checklist

- [ ] Search functionality with various postcodes
- [ ] Filter combinations
- [ ] Modal interactions
- [ ] Map marker interactions
- [ ] Calendar availability selection
- [ ] Booking flow completion
- [ ] Responsive design on all devices
- [ ] Keyboard navigation
- [ ] Screen reader compatibility
- [ ] Performance with many providers

## Next Steps

1. Set up Google Maps API key
2. Implement backend search endpoints
3. Create provider availability system
4. Build booking workflow
5. Add real-time messaging
6. Implement payment integration
7. Add analytics tracking