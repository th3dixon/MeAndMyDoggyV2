# Service Provider Discovery - Technical Specification

## Component Overview
The Service Provider Discovery system is a revenue-generating marketplace that connects pet owners with service providers through advanced search, real-time availability, smart filtering, and integrated booking capabilities.

## Database Schema

### Primary Tables
- **ServiceProviders** - Provider profiles and business info
- **Services** - Service offerings with pricing
- **ServiceCategories** - Service taxonomy
- **ServiceProviderAvailability** - Schedule management
- **ServiceAreas** - Geographic coverage
- **ProviderSearchIndex** - Optimized search data

### Additional Tables for This Component

```sql
-- ServiceAreas
CREATE TABLE [dbo].[ServiceAreas] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [AreaType] INT NOT NULL, -- 0: Postcode, 1: City, 2: Region, 3: Radius
    [AreaValue] NVARCHAR(100) NOT NULL,
    [CenterLatitude] DECIMAL(9, 6) NULL,
    [CenterLongitude] DECIMAL(9, 6) NULL,
    [RadiusKm] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServiceAreas_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    INDEX [IX_ServiceAreas_AreaValue] ([AreaValue]),
    INDEX [IX_ServiceAreas_Location] ([CenterLatitude], [CenterLongitude])
);

-- ProviderSearchIndex
CREATE TABLE [dbo].[ProviderSearchIndex] (
    [ProviderId] INT NOT NULL PRIMARY KEY,
    [SearchVector] NVARCHAR(MAX) NOT NULL, -- Full-text search content
    [LocationGeography] GEOGRAPHY NULL,
    [ServiceTypeFlags] BIGINT NOT NULL, -- Bit flags for quick filtering
    [PriceRangeMin] DECIMAL(10, 2) NOT NULL,
    [PriceRangeMax] DECIMAL(10, 2) NOT NULL,
    [Rating] DECIMAL(3, 2) NOT NULL,
    [ReviewCount] INT NOT NULL,
    [ResponseTimeHours] INT NULL,
    [CompletionRate] DECIMAL(5, 2) NULL,
    [LastActiveDate] DATETIME2 NOT NULL,
    [PopularityScore] DECIMAL(10, 2) NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ProviderSearchIndex_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- ServicePricingTiers
CREATE TABLE [dbo].[ServicePricingTiers] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ServiceId] INT NOT NULL,
    [TierName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [MinQuantity] INT NOT NULL DEFAULT 1,
    [MaxQuantity] INT NULL,
    [Price] DECIMAL(10, 2) NOT NULL,
    [DiscountPercentage] DECIMAL(5, 2) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ServicePricingTiers_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([Id])
);

-- ProviderSpecializations
CREATE TABLE [dbo].[ProviderSpecializations] (
    [ProviderId] INT NOT NULL,
    [SpecializationId] INT NOT NULL,
    [YearsExperience] INT NULL,
    [CertificationDetails] NVARCHAR(500) NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    PRIMARY KEY ([ProviderId], [SpecializationId]),
    CONSTRAINT [FK_ProviderSpecializations_Providers] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_ProviderSpecializations_Specializations] FOREIGN KEY ([SpecializationId]) REFERENCES [Specializations]([Id])
);

-- Specializations (Reference data)
CREATE TABLE [dbo].[Specializations] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Category] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [RequiresCertification] BIT NOT NULL DEFAULT 0,
    [Icon] NVARCHAR(50) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ProviderPortfolio
CREATE TABLE [dbo].[ProviderPortfolio] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [MediaType] INT NOT NULL, -- 0: Image, 1: Video
    [MediaUrl] NVARCHAR(500) NOT NULL,
    [ThumbnailUrl] NVARCHAR(500) NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ProviderPortfolio_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- SearchSessions
CREATE TABLE [dbo].[SearchSessions] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UserId] NVARCHAR(450) NULL,
    [SessionStarted] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [InitialQuery] NVARCHAR(500) NULL,
    [InitialLocation] GEOGRAPHY NULL,
    [FinalQuery] NVARCHAR(500) NULL,
    [FiltersApplied] NVARCHAR(MAX) NULL, -- JSON
    [ResultsViewed] INT NOT NULL DEFAULT 0,
    [ProvidersContacted] INT NOT NULL DEFAULT 0,
    [BookingCreated] BIT NOT NULL DEFAULT 0,
    [SessionDuration] INT NULL, -- seconds
    [DeviceType] NVARCHAR(50) NULL,
    CONSTRAINT [FK_SearchSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- ProviderPromotions
CREATE TABLE [dbo].[ProviderPromotions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [PromotionType] INT NOT NULL, -- 0: Featured, 1: Discount, 2: NewCustomer
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [DiscountPercentage] DECIMAL(5, 2) NULL,
    [DiscountAmount] DECIMAL(10, 2) NULL,
    [PromoCode] NVARCHAR(50) NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [MaxRedemptions] INT NULL,
    [CurrentRedemptions] INT NOT NULL DEFAULT 0,
    [TargetAudience] NVARCHAR(MAX) NULL, -- JSON criteria
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ProviderPromotions_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);
```

## API Endpoints

### Search & Discovery

```yaml
/api/v1/providers:
  /search:
    POST:
      description: Advanced provider search with filters
      body:
        query: string # Optional text search
        location:
          latitude: number
          longitude: number
          address: string # Alternative to coordinates
          radius: number # km, default 10
        
        services:
          categories: array[number]
          specificServices: array[number]
        
        availability:
          dateRange:
            start: datetime
            end: datetime
          timeOfDay: enum [Morning, Afternoon, Evening, Night]
          daysOfWeek: array[number]
        
        filters:
          priceRange:
            min: number
            max: number
          rating:
            min: number # 1-5
          verified: boolean
          insurance: boolean
          experience:
            min: number # years
          specializations: array[number]
          languages: array[string]
        
        sort:
          by: enum [Relevance, Distance, Price, Rating, Popularity]
          order: enum [Asc, Desc]
        
        pagination:
          page: number
          pageSize: number
      
      responses:
        200:
          results: array[ProviderSearchResult]
          totalCount: number
          facets:
            categories: array[FacetCount]
            priceRanges: array[FacetCount]
            ratings: array[FacetCount]
            distances: array[FacetCount]
          searchSessionId: string
          suggestions: array[SearchSuggestion]

  /quick-search:
    GET:
      description: Quick search for autocomplete
      parameters:
        q: string
        lat: number
        lng: number
        limit: number
      responses:
        200:
          providers: array[ProviderQuickResult]
          services: array[ServiceQuickResult]
          categories: array[CategoryQuickResult]

  /nearby:
    GET:
      description: Find nearby providers
      parameters:
        lat: number
        lng: number
        radius: number
        category: number # Optional
      responses:
        200:
          providers: array[NearbyProvider]
          mapData: object # For map rendering

  /featured:
    GET:
      description: Get featured providers
      parameters:
        location: string # Optional
        category: number # Optional
      responses:
        200:
          featured: array[FeaturedProvider]
          promotions: array[ProviderPromotion]

  /{providerId}:
    GET:
      description: Get detailed provider profile
      responses:
        200:
          provider: ProviderDetail
          services: array[ServiceDetail]
          availability: AvailabilityCalendar
          reviews: ReviewSummary
          portfolio: array[PortfolioItem]
          promotions: array[ActivePromotion]
```

### Availability & Scheduling

```yaml
/api/v1/providers/{providerId}/availability:
  /calendar:
    GET:
      description: Get availability calendar
      parameters:
        startDate: date
        endDate: date
        serviceId: number # Optional
      responses:
        200:
          availableDates: array[date]
          availableSlots: object # Date -> TimeSlot[]
          blockedDates: array[date]
          specialHours: array[SpecialHour]

  /slots:
    GET:
      description: Get available time slots for specific date
      parameters:
        date: date
        serviceId: number
        duration: number # minutes
      responses:
        200:
          slots: array[{
            startTime: datetime
            endTime: datetime
            available: boolean
            price: number # May vary by time
          }]

  /check:
    POST:
      description: Check specific slot availability
      body:
        serviceId: number
        startTime: datetime
        endTime: datetime
        dogCount: number
      responses:
        200:
          available: boolean
          conflicts: array[string]
          alternativeSlots: array[TimeSlot]
```

### Provider Profiles

```yaml
/api/v1/providers/{providerId}:
  /services:
    GET:
      description: Get provider's services
      parameters:
        category: number # Optional filter
      responses:
        200:
          services: array[ServiceDetail]
          packages: array[ServicePackage]

  /reviews:
    GET:
      description: Get provider reviews
      parameters:
        page: number
        pageSize: number
        sort: enum [Newest, Oldest, HighestRated, LowestRated]
        verified: boolean # Only verified bookings
      responses:
        200:
          reviews: array[Review]
          summary:
            averageRating: number
            totalReviews: number
            ratingDistribution: object
            highlights: array[string] # AI-generated

  /portfolio:
    GET:
      description: Get provider portfolio
      responses:
        200:
          items: array[PortfolioItem]
          categories: array[string]

  /contact:
    POST:
      description: Contact provider
      auth: required
      body:
        subject: string
        message: string
        serviceId: number # Optional
        preferredDate: date # Optional
      responses:
        200:
          conversationId: number
          messageSent: boolean
```

### Booking Integration

```yaml
/api/v1/providers/{providerId}/book:
  /instant:
    POST:
      description: Instant booking for verified providers
      auth: required
      body:
        serviceId: number
        startTime: datetime
        endTime: datetime
        dogs: array[number] # Dog IDs
        specialRequirements: string
        promoCode: string # Optional
      responses:
        201:
          bookingId: number
          status: enum [Confirmed, Pending]
          totalPrice: number
          paymentRequired: boolean

  /request:
    POST:
      description: Booking request (requires approval)
      auth: required
      body:
        serviceId: number
        preferredTimes: array[{
          startTime: datetime
          endTime: datetime
          priority: number
        }]
        dogs: array[number]
        message: string
      responses:
        201:
          requestId: number
          status: PendingApproval
          estimatedResponse: datetime
```

## Frontend Components

### Search Interface Components (Vue.js)

```typescript
// ProviderSearch.vue
interface ProviderSearchProps {
  initialLocation?: Location
  initialCategory?: number
  embedded?: boolean // For embedding in other pages
}

// Components:
// - SearchBar.vue (with autocomplete)
// - FilterPanel.vue (collapsible filters)
// - ResultsList.vue (with infinite scroll)
// - MapView.vue (interactive map)
// - SearchSuggestions.vue

// SearchBar.vue
interface SearchBarProps {
  placeholder: string
  showLocationPicker: boolean
  showCategorySelector: boolean
  onSearch: (query: SearchQuery) => void
}

// Features:
// - Autocomplete with debouncing
// - Recent searches
// - Voice search
// - Location detection
```

### Provider Profile Components

```typescript
// ProviderProfile.vue
interface ProviderProfileProps {
  providerId: number
  showBookingPanel: boolean
}

// Sections:
// - ProfileHeader.vue (photo, name, badges)
// - ServicesList.vue (with pricing)
// - AvailabilityCalendar.vue
// - ReviewsSection.vue
// - PortfolioGallery.vue
// - ContactForm.vue

// AvailabilityCalendar.vue
interface AvailabilityCalendarProps {
  providerId: number
  serviceId?: number
  onSlotSelected: (slot: TimeSlot) => void
}

// Features:
// - Month/week/day views
// - Real-time updates
// - Price variations
// - Booking integration
```

### Search Results Components

```typescript
// ProviderCard.vue
interface ProviderCardProps {
  provider: ProviderSearchResult
  viewMode: 'grid' | 'list'
  showDistance: boolean
  showAvailability: boolean
}

// Features:
// - Quick view modal
// - Favorite toggle
// - Share functionality
// - Instant booking button

// SearchFilters.vue
interface SearchFiltersProps {
  availableFilters: FilterOptions
  appliedFilters: AppliedFilters
  onFilterChange: (filters: AppliedFilters) => void
}

// Filter categories:
// - Service types
// - Price range slider
// - Availability picker
// - Ratings
// - Distance
// - Specializations
// - Languages
```

## Technical Implementation Details

### Search Engine Implementation

```csharp
public class ProviderSearchService
{
    private readonly IElasticsearchClient _elasticsearch;
    private readonly ILocationService _locationService;
    
    public async Task<SearchResult> SearchProviders(SearchRequest request)
    {
        var searchQuery = BuildSearchQuery(request);
        
        var response = await _elasticsearch.SearchAsync<ProviderDocument>(s => s
            .Index("providers")
            .Query(q => searchQuery)
            .Aggregations(a => a
                .Terms("categories", t => t.Field(f => f.Categories))
                .Range("price_ranges", r => r
                    .Field(f => f.PriceMin)
                    .Ranges(
                        r => r.To(50),
                        r => r.From(50).To(100),
                        r => r.From(100).To(200),
                        r => r.From(200)
                    ))
                .Terms("ratings", t => t.Field(f => f.Rating))
            )
            .Sort(BuildSortDescriptor(request.Sort))
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .TrackTotalHits()
        );
        
        return MapSearchResponse(response, request);
    }
    
    private QueryContainer BuildSearchQuery(SearchRequest request)
    {
        var queries = new List<QueryContainer>();
        
        // Text search
        if (!string.IsNullOrEmpty(request.Query))
        {
            queries.Add(q => q.MultiMatch(m => m
                .Query(request.Query)
                .Fields(f => f
                    .Field(p => p.BusinessName, boost: 2.0)
                    .Field(p => p.Description)
                    .Field(p => p.Services)
                    .Field(p => p.Specializations)
                )
                .Type(TextQueryType.BestFields)
                .Fuzziness(Fuzziness.Auto)
            ));
        }
        
        // Location filter
        if (request.Location != null)
        {
            queries.Add(q => q.GeoDistance(g => g
                .Field(f => f.Location)
                .Distance($"{request.Location.Radius}km")
                .Location(request.Location.Latitude, request.Location.Longitude)
            ));
        }
        
        // Availability filter
        if (request.Availability != null)
        {
            queries.Add(BuildAvailabilityQuery(request.Availability));
        }
        
        // Other filters
        if (request.Filters != null)
        {
            if (request.Filters.PriceRange != null)
            {
                queries.Add(q => q.Range(r => r
                    .Field(f => f.PriceMin)
                    .GreaterThanOrEquals(request.Filters.PriceRange.Min)
                    .LessThanOrEquals(request.Filters.PriceRange.Max)
                ));
            }
            
            if (request.Filters.Rating?.Min > 0)
            {
                queries.Add(q => q.Range(r => r
                    .Field(f => f.Rating)
                    .GreaterThanOrEquals(request.Filters.Rating.Min)
                ));
            }
        }
        
        return q => q.Bool(b => b.Must(queries));
    }
}
```

### Real-time Availability System

```csharp
public class AvailabilityService
{
    private readonly IRedisCache _cache;
    private readonly IAvailabilityRepository _repository;
    
    public async Task<AvailabilityCalendar> GetProviderAvailability(
        int providerId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var cacheKey = $"availability:{providerId}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";
        
        // Try cache first
        var cached = await _cache.GetAsync<AvailabilityCalendar>(cacheKey);
        if (cached != null) return cached;
        
        // Build availability from multiple sources
        var baseSchedule = await _repository.GetBaseSchedule(providerId);
        var bookings = await _repository.GetBookings(providerId, startDate, endDate);
        var unavailability = await _repository.GetUnavailability(providerId, startDate, endDate);
        var specialHours = await _repository.GetSpecialHours(providerId, startDate, endDate);
        
        var calendar = BuildAvailabilityCalendar(
            baseSchedule, 
            bookings, 
            unavailability, 
            specialHours, 
            startDate, 
            endDate);
        
        // Cache for 5 minutes
        await _cache.SetAsync(cacheKey, calendar, TimeSpan.FromMinutes(5));
        
        return calendar;
    }
    
    private AvailabilityCalendar BuildAvailabilityCalendar(
        WeeklySchedule baseSchedule,
        List<Booking> bookings,
        List<Unavailability> unavailability,
        List<SpecialHours> specialHours,
        DateTime startDate,
        DateTime endDate)
    {
        var calendar = new AvailabilityCalendar();
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var daySchedule = GetDaySchedule(baseSchedule, date, specialHours);
            if (daySchedule == null) continue;
            
            var slots = GenerateTimeSlots(daySchedule, 30); // 30-minute slots
            
            // Remove booked slots
            var dayBookings = bookings.Where(b => b.Date == date).ToList();
            foreach (var booking in dayBookings)
            {
                slots.RemoveAll(s => s.OverlapsWith(booking.StartTime, booking.EndTime));
            }
            
            // Remove unavailable periods
            var dayUnavailable = unavailability.Where(u => u.Date == date).ToList();
            foreach (var period in dayUnavailable)
            {
                slots.RemoveAll(s => s.OverlapsWith(period.StartTime, period.EndTime));
            }
            
            calendar.AvailableSlots[date] = slots;
        }
        
        return calendar;
    }
}
```

### Smart Ranking Algorithm

```csharp
public class ProviderRankingService
{
    public double CalculateRelevanceScore(
        ProviderDocument provider, 
        SearchRequest request)
    {
        var scores = new Dictionary<string, double>();
        
        // Text relevance (0-100)
        scores["text"] = CalculateTextRelevance(provider, request.Query);
        
        // Distance score (0-100)
        if (request.Location != null)
        {
            var distance = CalculateDistance(provider.Location, request.Location);
            scores["distance"] = Math.Max(0, 100 - (distance * 10)); // -10 points per km
        }
        
        // Rating score (0-100)
        scores["rating"] = (provider.Rating / 5.0) * 100;
        
        // Popularity score (0-100)
        scores["popularity"] = CalculatePopularityScore(provider);
        
        // Response time score (0-100)
        if (provider.ResponseTimeHours.HasValue)
        {
            scores["responseTime"] = Math.Max(0, 100 - (provider.ResponseTimeHours.Value * 5));
        }
        
        // Premium boost
        if (provider.IsPremium)
        {
            scores["premium"] = 20;
        }
        
        // Apply weights
        var weights = GetScoringWeights(request);
        var totalScore = scores.Sum(s => s.Value * weights.GetValueOrDefault(s.Key, 0));
        
        return totalScore / weights.Values.Sum();
    }
    
    private double CalculatePopularityScore(ProviderDocument provider)
    {
        var factors = new[]
        {
            provider.ReviewCount / 100.0, // More reviews = more popular
            provider.CompletionRate / 100.0, // High completion = reliable
            Math.Min(provider.BookingsLastMonth / 50.0, 1.0), // Recent activity
            provider.RepeatCustomerRate // Customer satisfaction
        };
        
        return factors.Average() * 100;
    }
}
```

### Personalization Engine

```csharp
public class SearchPersonalizationService
{
    private readonly IUserPreferenceService _preferences;
    private readonly IBookingHistoryService _history;
    
    public async Task<PersonalizedSearchModifiers> GetPersonalizedModifiers(string userId)
    {
        var preferences = await _preferences.GetUserPreferences(userId);
        var history = await _history.GetBookingHistory(userId);
        
        return new PersonalizedSearchModifiers
        {
            PreferredCategories = DeterminePreferredCategories(history),
            PriceRangeBias = CalculatePriceRangeBias(history),
            DistancePreference = preferences.MaxTravelDistance,
            PreferredProviders = GetFrequentlyBookedProviders(history),
            AvoidProviders = GetNegativelyReviewedProviders(history),
            TimePreferences = AnalyzeBookingTimes(history),
            LanguagePreferences = preferences.PreferredLanguages
        };
    }
    
    public SearchRequest ApplyPersonalization(
        SearchRequest request, 
        PersonalizedSearchModifiers modifiers)
    {
        // Boost preferred categories
        if (modifiers.PreferredCategories.Any())
        {
            request.Boosters.Add(new CategoryBooster
            {
                Categories = modifiers.PreferredCategories,
                BoostFactor = 1.5
            });
        }
        
        // Adjust price range if not specified
        if (request.Filters.PriceRange == null)
        {
            request.Filters.PriceRange = modifiers.PriceRangeBias;
        }
        
        // Pin preferred providers
        request.PinnedProviders = modifiers.PreferredProviders.Take(3).ToList();
        
        // Exclude avoided providers
        request.ExcludedProviders = modifiers.AvoidProviders;
        
        return request;
    }
}
```

## Security Considerations

### Search Security
1. **Rate Limiting**: 
   - Anonymous: 20 searches/minute
   - Authenticated: 60 searches/minute
2. **Query Sanitization**: 
   - SQL injection prevention
   - XSS protection in search terms
3. **Location Privacy**:
   - Approximate provider locations only
   - Exact address after booking

### Data Protection
```csharp
public class ProviderDataProtection
{
    public ProviderPublicProfile SanitizeForPublicView(ServiceProvider provider)
    {
        return new ProviderPublicProfile
        {
            Id = provider.Id,
            BusinessName = provider.BusinessName,
            Description = provider.Description,
            // Approximate location only
            ServiceArea = GetApproximateArea(provider.ServiceAreas),
            // Hide personal details
            ContactEmail = null,
            ContactPhone = null,
            // Show only public portfolio items
            Portfolio = provider.Portfolio.Where(p => p.IsPublic).ToList()
        };
    }
}
```

## Performance Optimization

### Search Performance

```csharp
// Elasticsearch optimization
public class SearchIndexManager
{
    public async Task RebuildIndex()
    {
        var indexName = $"providers_{DateTime.Now:yyyyMMddHHmmss}";
        
        // Create optimized index
        await _client.Indices.CreateAsync(indexName, c => c
            .Settings(s => s
                .NumberOfShards(3)
                .NumberOfReplicas(1)
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("provider_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "stop", "synonym", "stemmer")
                        )
                    )
                )
            )
            .Map<ProviderDocument>(m => m
                .Properties(p => p
                    .Text(t => t.Name(n => n.BusinessName).Analyzer("provider_analyzer"))
                    .GeoPoint(g => g.Name(n => n.Location))
                    .Keyword(k => k.Name(n => n.ServiceTypes))
                    .Number(n => n.Name(nn => nn.Rating).Type(NumberType.Float))
                )
            )
        );
        
        // Bulk index providers
        await BulkIndexProviders(indexName);
        
        // Atomic alias swap
        await _client.Indices.BulkAliasAsync(b => b
            .Remove(r => r.Index("*").Alias("providers"))
            .Add(a => a.Index(indexName).Alias("providers"))
        );
    }
}
```

### Caching Strategy

```csharp
public class SearchCacheService
{
    private readonly IDistributedCache _cache;
    
    public async Task<SearchResult> GetCachedSearchResults(SearchRequest request)
    {
        var cacheKey = GenerateCacheKey(request);
        var cached = await _cache.GetAsync<SearchResult>(cacheKey);
        
        if (cached != null)
        {
            // Update view counts asynchronously
            _ = Task.Run(() => UpdateProviderViewCounts(cached.Results));
            return cached;
        }
        
        var results = await _searchService.SearchProviders(request);
        
        // Cache popular searches longer
        var ttl = IsPopularSearch(request) 
            ? TimeSpan.FromMinutes(30) 
            : TimeSpan.FromMinutes(5);
        
        await _cache.SetAsync(cacheKey, results, ttl);
        
        return results;
    }
    
    private string GenerateCacheKey(SearchRequest request)
    {
        var normalized = NormalizeRequest(request);
        var json = JsonSerializer.Serialize(normalized);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return $"search:{Convert.ToBase64String(hash)}";
    }
}
```

## Monitoring & Analytics

### Search Analytics

```csharp
public class SearchAnalyticsService
{
    public async Task TrackSearch(SearchRequest request, SearchResult result)
    {
        var analytics = new SearchAnalytics
        {
            SessionId = request.SessionId,
            UserId = request.UserId,
            Query = request.Query,
            Filters = JsonSerializer.Serialize(request.Filters),
            ResultCount = result.TotalCount,
            ResponseTime = result.ResponseTime,
            ClickedResults = new List<string>(), // Updated later
            Timestamp = DateTime.UtcNow
        };
        
        await _repository.SaveSearchAnalytics(analytics);
        
        // Track search quality metrics
        if (result.TotalCount == 0)
        {
            await TrackZeroResults(request);
        }
    }
    
    public async Task TrackProviderView(int providerId, string searchSessionId)
    {
        await _repository.UpdateSearchSession(searchSessionId, session =>
        {
            session.ResultsViewed++;
            session.ViewedProviders.Add(providerId);
        });
    }
}
```

### Key Metrics
1. **Search Performance**:
   - Query response time
   - Zero result rate
   - Search abandonment rate
2. **User Behavior**:
   - Click-through rate
   - Booking conversion rate
   - Filter usage patterns
3. **Provider Metrics**:
   - Profile views
   - Contact rate
   - Booking rate

## Testing Strategy

### Search Testing

```csharp
[TestClass]
public class ProviderSearchTests
{
    [TestMethod]
    public async Task Search_WithLocationFilter_ReturnsNearbyProviders()
    {
        // Arrange
        var request = new SearchRequest
        {
            Location = new LocationFilter
            {
                Latitude = 51.5074,
                Longitude = -0.1278,
                Radius = 5
            }
        };
        
        // Act
        var results = await _searchService.SearchProviders(request);
        
        // Assert
        Assert.IsTrue(results.Results.All(r => 
            CalculateDistance(r.Location, request.Location) <= 5));
    }
    
    [TestMethod]
    public async Task Search_WithAvailability_ReturnsAvailableProviders()
    {
        // Test availability filtering
    }
}
```

### Load Testing
- Target: 1000 concurrent searches
- Response time: < 200ms p95
- Elasticsearch cluster sizing
- CDN configuration for assets