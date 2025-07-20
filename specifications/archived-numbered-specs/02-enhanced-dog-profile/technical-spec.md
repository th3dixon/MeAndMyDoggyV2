# Enhanced Dog Profile Management - Technical Specification

## Component Overview
The Enhanced Dog Profile Management system serves as the core data foundation for pet owners, featuring comprehensive medical records, photo galleries, activity tracking, and AI-powered health insights using Google Gemini.

## Database Schema

### Primary Tables
- **DogProfiles** - Core dog information
- **DogMedicalRecords** - Vaccination, medical history
- **DogPhotos** - Photo gallery with metadata
- **DogActivities** - Activity tracking
- **DogHealthMetrics** - Weight, measurements tracking
- **DogBehaviorProfiles** - Behavioral assessments
- **DogEmergencyContacts** - Emergency vet contacts

### Additional Tables for This Component

```sql
-- DogHealthMetrics
CREATE TABLE [dbo].[DogHealthMetrics] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [MetricType] INT NOT NULL, -- 0: Weight, 1: Height, 2: Length, 3: Temperature, 4: HeartRate
    [Value] DECIMAL(10, 2) NOT NULL,
    [Unit] NVARCHAR(20) NOT NULL,
    [MeasuredBy] NVARCHAR(450) NOT NULL,
    [MeasuredAt] DATETIME2 NOT NULL,
    [Notes] NVARCHAR(500) NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0, -- Vet verified
    [VerifiedBy] NVARCHAR(200) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogHealthMetrics_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id]),
    CONSTRAINT [FK_DogHealthMetrics_MeasuredBy] FOREIGN KEY ([MeasuredBy]) REFERENCES [AspNetUsers]([Id])
);

-- DogBehaviorProfiles
CREATE TABLE [dbo].[DogBehaviorProfiles] (
    [DogProfileId] INT NOT NULL PRIMARY KEY,
    [LastAssessmentDate] DATETIME2 NULL,
    [GoodWithChildren] INT NULL, -- 0-5 scale
    [GoodWithDogs] INT NULL, -- 0-5 scale
    [GoodWithCats] INT NULL, -- 0-5 scale
    [GoodWithStrangers] INT NULL, -- 0-5 scale
    [TrainingLevel] INT NULL, -- 0: None, 1: Basic, 2: Intermediate, 3: Advanced
    [ActivityNeeds] INT NULL, -- 1-5 scale
    [BarkingTendency] INT NULL, -- 1-5 scale
    [SeparationAnxiety] INT NULL, -- 1-5 scale
    [Aggression] INT NULL, -- 0-5 scale
    [SpecialNeeds] NVARCHAR(MAX) NULL, -- JSON array
    [Triggers] NVARCHAR(MAX) NULL, -- JSON array
    [PreferredActivities] NVARCHAR(MAX) NULL, -- JSON array
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogBehaviorProfiles_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id])
);

-- DogEmergencyContacts
CREATE TABLE [dbo].[DogEmergencyContacts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [ContactType] INT NOT NULL, -- 0: PrimaryVet, 1: EmergencyVet, 2: Other
    [Name] NVARCHAR(200) NOT NULL,
    [PhoneNumber] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(256) NULL,
    [Address] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogEmergencyContacts_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id])
);

-- DogDocuments
CREATE TABLE [dbo].[DogDocuments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [DocumentType] INT NOT NULL, -- 0: Registration, 1: Insurance, 2: Medical, 3: Other
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [FileUrl] NVARCHAR(500) NOT NULL,
    [FileName] NVARCHAR(200) NOT NULL,
    [FileSize] BIGINT NOT NULL,
    [MimeType] NVARCHAR(100) NOT NULL,
    [ExpiryDate] DATE NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [UploadedBy] NVARCHAR(450) NOT NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogDocuments_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id]),
    CONSTRAINT [FK_DogDocuments_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [AspNetUsers]([Id])
);

-- DogBreedInfo (Reference data)
CREATE TABLE [dbo].[DogBreedInfo] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BreedName] NVARCHAR(100) NOT NULL,
    [BreedGroup] NVARCHAR(100) NULL,
    [Size] INT NOT NULL, -- 0: Small, 1: Medium, 2: Large, 3: Giant
    [LifeExpectancy] NVARCHAR(50) NULL,
    [CommonHealthIssues] NVARCHAR(MAX) NULL, -- JSON array
    [GroomingNeeds] INT NULL, -- 1-5 scale
    [ExerciseNeeds] INT NULL, -- 1-5 scale
    [TrainabilityScore] INT NULL, -- 1-5 scale
    [Description] NVARCHAR(MAX) NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- DogActivityGoals
CREATE TABLE [dbo].[DogActivityGoals] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [DogProfileId] INT NOT NULL,
    [GoalType] INT NOT NULL, -- 0: DailyWalk, 1: WeeklyExercise, 2: MonthlyGrooming
    [TargetValue] DECIMAL(10, 2) NOT NULL,
    [Unit] NVARCHAR(20) NOT NULL,
    [Frequency] INT NOT NULL, -- 0: Daily, 1: Weekly, 2: Monthly
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_DogActivityGoals_DogProfiles] FOREIGN KEY ([DogProfileId]) REFERENCES [DogProfiles]([Id]),
    CONSTRAINT [FK_DogActivityGoals_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);
```

## API Endpoints

### Dog Profile Management

```yaml
/api/v1/dogs:
  /:
    GET:
      description: Get all user's dog profiles
      auth: required
      parameters:
        includeArchived: boolean
      responses:
        200:
          dogs: array[DogProfileSummary]
    
    POST:
      description: Create new dog profile
      auth: required
      body:
        name: string
        breed: string
        mixedBreeds: array[string]
        dateOfBirth: date
        gender: enum [Male, Female]
        isNeutered: boolean
        weight: number
        size: enum [Small, Medium, Large, Giant]
        microchipNumber: string
        description: string
      responses:
        201:
          dogId: number
          profileUrl: string

  /{dogId}:
    GET:
      description: Get detailed dog profile
      auth: required
      responses:
        200: Complete dog profile with all related data
    
    PUT:
      description: Update dog profile
      auth: required
      body: DogProfileUpdateDto
      responses:
        200: Updated profile
    
    DELETE:
      description: Archive dog profile
      auth: required
      responses:
        204: Archived successfully

  /{dogId}/transfer:
    POST:
      description: Transfer ownership to another user
      auth: required
      body:
        newOwnerEmail: string
        reason: string
      responses:
        200: Transfer initiated
```

### Medical Records Management

```yaml
/api/v1/dogs/{dogId}/medical:
  /records:
    GET:
      description: Get all medical records
      auth: required
      parameters:
        recordType: enum [All, Vaccination, Surgery, Medication, Allergy, Condition]
        startDate: date
        endDate: date
      responses:
        200:
          records: array[MedicalRecord]
          upcomingReminders: array[Reminder]
    
    POST:
      description: Add medical record
      auth: required
      body:
        recordType: enum
        title: string
        description: string
        date: date
        veterinarianName: string
        veterinaryClinic: string
        nextDueDate: date
        cost: number
        documents: array[file]
      responses:
        201: Record created

  /records/{recordId}:
    PUT:
      description: Update medical record
      auth: required
      body: MedicalRecordUpdateDto
      responses:
        200: Updated record
    
    DELETE:
      description: Delete medical record
      auth: required
      responses:
        204: Deleted

  /vaccinations/due:
    GET:
      description: Get vaccination due dates
      auth: required
      responses:
        200:
          overdue: array[Vaccination]
          upcoming: array[Vaccination]

  /health-metrics:
    GET:
      description: Get health metrics history
      auth: required
      parameters:
        metricType: enum [Weight, Height, Temperature]
        period: enum [Week, Month, Year, All]
      responses:
        200:
          metrics: array[HealthMetric]
          trends: object
    
    POST:
      description: Record health metric
      auth: required
      body:
        metricType: enum
        value: number
        unit: string
        notes: string
      responses:
        201: Metric recorded
```

### Photo Gallery Management

```yaml
/api/v1/dogs/{dogId}/photos:
  /:
    GET:
      description: Get dog photos
      auth: required
      parameters:
        page: number
        pageSize: number
        sortBy: enum [newest, oldest, popular]
      responses:
        200:
          photos: array[DogPhoto]
          totalCount: number
    
    POST:
      description: Upload photos
      auth: required
      contentType: multipart/form-data
      body:
        files: array[file] # Max 10 files, 10MB each
        captions: array[string]
        tags: array[array[string]]
      responses:
        201:
          uploadedPhotos: array[PhotoInfo]

  /{photoId}:
    GET:
      description: Get photo details
      auth: required
      responses:
        200: Photo with metadata
    
    PUT:
      description: Update photo metadata
      auth: required
      body:
        caption: string
        tags: array[string]
      responses:
        200: Updated photo
    
    DELETE:
      description: Delete photo
      auth: required
      responses:
        204: Deleted

  /{photoId}/set-profile:
    POST:
      description: Set as profile photo
      auth: required
      responses:
        200: Profile photo updated

  /ai-analyze:
    POST:
      description: AI analysis of dog photo
      auth: required
      body:
        photoId: number
      responses:
        200:
          breedPredictions: array[BreedPrediction]
          ageEstimate: object
          healthObservations: array[string]
```

### Activity Tracking

```yaml
/api/v1/dogs/{dogId}/activities:
  /:
    GET:
      description: Get activities
      auth: required
      parameters:
        activityType: enum [All, Walk, Play, Training, Grooming, VetVisit]
        startDate: date
        endDate: date
      responses:
        200:
          activities: array[Activity]
          summary: ActivitySummary
    
    POST:
      description: Log activity
      auth: required
      body:
        activityType: enum
        title: string
        startTime: datetime
        endTime: datetime
        duration: number # minutes
        distance: number # km
        location: string
        notes: string
      responses:
        201: Activity logged

  /goals:
    GET:
      description: Get activity goals
      auth: required
      responses:
        200:
          goals: array[ActivityGoal]
          progress: array[GoalProgress]
    
    POST:
      description: Set activity goal
      auth: required
      body:
        goalType: enum
        targetValue: number
        unit: string
        frequency: enum [Daily, Weekly, Monthly]
      responses:
        201: Goal created

  /summary:
    GET:
      description: Get activity summary
      auth: required
      parameters:
        period: enum [Day, Week, Month, Year]
      responses:
        200:
          totalActivities: number
          totalDuration: number
          totalDistance: number
          byType: object
          trends: object
```

### AI-Powered Features

```yaml
/api/v1/dogs/{dogId}/ai:
  /breed-identification:
    POST:
      description: Identify breed from photo using Google Gemini
      auth: required
      body:
        photoId: number
      responses:
        200:
          predictions: array[{
            breed: string
            confidence: number
            characteristics: object
          }]
          mixedBreedAnalysis: object

  /health-insights:
    GET:
      description: Get AI-generated health insights
      auth: required
      responses:
        200:
          insights: array[{
            category: string
            title: string
            description: string
            recommendations: array[string]
            priority: enum [Low, Medium, High]
          }]
          lastUpdated: datetime

  /behavior-assessment:
    POST:
      description: AI behavior assessment questionnaire
      auth: required
      body:
        responses: array[{
          questionId: string
          answer: any
        }]
      responses:
        200:
          behaviorProfile: object
          recommendations: array[string]
          trainingTips: array[string]
```

## Frontend Components

### Dog Profile Components (Vue.js)

```typescript
// DogProfileDashboard.vue
interface DogProfileDashboardProps {
  dogId: number
}

// Main sections:
// - ProfileHeader.vue (photo, name, basic info)
// - QuickStats.vue (age, weight, last activity)
// - HealthSummary.vue (upcoming appointments, medications)
// - RecentActivities.vue (activity feed)
// - PhotoGalleryPreview.vue (recent photos)

// DogProfileEditor.vue
interface DogProfileEditorProps {
  dog: DogProfile
  mode: 'create' | 'edit'
}

// Sections:
// - BasicInfoForm.vue
// - BreedSelector.vue (with autocomplete)
// - MedicalInfoForm.vue
// - BehaviorAssessment.vue
// - EmergencyContacts.vue
```

### Medical Records Components

```typescript
// MedicalRecordsList.vue
interface MedicalRecordsListProps {
  dogId: number
  viewMode: 'timeline' | 'category'
}

// VaccinationTracker.vue
interface VaccinationTrackerProps {
  dogId: number
  vaccines: Vaccination[]
}

// Features:
// - Visual timeline
// - Due date alerts
// - Certificate generation
// - Reminder settings
```

### Photo Gallery Components

```typescript
// PhotoGallery.vue
interface PhotoGalleryProps {
  dogId: number
  enableUpload: boolean
}

// Features:
// - Infinite scroll
// - Lightbox view
// - Batch operations
// - AI analysis trigger

// PhotoUploader.vue
interface PhotoUploaderProps {
  dogId: number
  maxFiles: number
  onUploadComplete: (photos: Photo[]) => void
}

// Features:
// - Drag & drop
// - Progress tracking
// - Auto-optimization
// - Metadata extraction
```

## Technical Implementation Details

### Photo Storage & Processing

```csharp
public class PhotoService
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IImageProcessor _imageProcessor;
    
    public async Task<PhotoUploadResult> UploadDogPhoto(
        int dogId, 
        IFormFile file, 
        string caption)
    {
        // Validate file
        if (file.Length > 10 * 1024 * 1024) // 10MB limit
            throw new ValidationException("File too large");
        
        // Generate unique filename
        var fileName = $"dogs/{dogId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        
        // Process image variants
        var variants = await _imageProcessor.ProcessImage(file.OpenReadStream(), new[]
        {
            new ImageVariant { Width = 150, Height = 150, Suffix = "_thumb" },
            new ImageVariant { Width = 800, Height = 800, Suffix = "_medium" },
            new ImageVariant { Width = 1920, Height = 1920, Suffix = "_large" },
            new ImageVariant { Format = ImageFormat.WebP, Suffix = "_webp" }
        });
        
        // Upload to blob storage
        var uploadTasks = variants.Select(v => 
            _blobStorage.UploadAsync(v.Stream, fileName + v.Suffix));
        
        await Task.WhenAll(uploadTasks);
        
        // Save metadata
        var photo = new DogPhoto
        {
            DogProfileId = dogId,
            PhotoUrl = $"{_cdnUrl}/{fileName}_large",
            ThumbnailUrl = $"{_cdnUrl}/{fileName}_thumb",
            Caption = caption,
            UploadedBy = _currentUser.Id,
            UploadedAt = DateTime.UtcNow
        };
        
        await _context.DogPhotos.AddAsync(photo);
        await _context.SaveChangesAsync();
        
        return new PhotoUploadResult { PhotoId = photo.Id, Urls = variants };
    }
}
```

### AI Integration - Google Gemini

```csharp
public class GeminiAIService
{
    private readonly GoogleAIClient _geminiClient;
    
    public async Task<BreedIdentificationResult> IdentifyBreed(string photoUrl)
    {
        var prompt = @"
            Analyze this dog photo and identify:
            1. Primary breed (with confidence percentage)
            2. Possible mixed breeds if applicable
            3. Physical characteristics observed
            4. Estimated age range
            5. Any visible health concerns
            
            Return as structured JSON.
        ";
        
        var response = await _geminiClient.GenerateContent(new GenerateContentRequest
        {
            Contents = new[]
            {
                new Content
                {
                    Parts = new[]
                    {
                        new Part { Text = prompt },
                        new Part { ImageUrl = photoUrl }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.1f,
                MaxOutputTokens = 1000,
                ResponseMimeType = "application/json"
            }
        });
        
        return JsonSerializer.Deserialize<BreedIdentificationResult>(response.Text);
    }
    
    public async Task<HealthInsights> GenerateHealthInsights(DogProfile dog, List<MedicalRecord> records)
    {
        var context = new
        {
            Dog = new { dog.Name, dog.Breed, dog.Age, dog.Weight },
            RecentRecords = records.Take(10),
            HealthMetrics = await GetRecentHealthMetrics(dog.Id)
        };
        
        var prompt = $@"
            Based on this dog's profile and medical history:
            {JsonSerializer.Serialize(context)}
            
            Provide health insights including:
            1. Overall health assessment
            2. Breed-specific concerns to monitor
            3. Preventive care recommendations
            4. Diet and exercise suggestions
            5. Red flags to watch for
        ";
        
        var response = await _geminiClient.GenerateContent(prompt);
        return ParseHealthInsights(response);
    }
}
```

### Activity Tracking & Analytics

```csharp
public class ActivityAnalyticsService
{
    public async Task<ActivitySummary> GetActivitySummary(
        int dogId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var activities = await _context.DogActivities
            .Where(a => a.DogProfileId == dogId 
                && a.StartTime >= startDate 
                && a.StartTime <= endDate)
            .ToListAsync();
        
        return new ActivitySummary
        {
            TotalActivities = activities.Count,
            TotalDuration = activities.Sum(a => a.Duration ?? 0),
            TotalDistance = activities.Sum(a => a.Distance ?? 0),
            AverageDailyActivity = CalculateDailyAverage(activities, startDate, endDate),
            MostActiveTime = DetermineActiveTime(activities),
            ActivityBreakdown = activities
                .GroupBy(a => a.ActivityType)
                .Select(g => new ActivityTypeBreakdown
                {
                    Type = g.Key,
                    Count = g.Count(),
                    TotalDuration = g.Sum(a => a.Duration ?? 0)
                })
                .ToList(),
            GoalProgress = await CalculateGoalProgress(dogId, activities)
        };
    }
    
    private async Task<List<GoalProgress>> CalculateGoalProgress(
        int dogId, 
        List<DogActivity> activities)
    {
        var goals = await _context.DogActivityGoals
            .Where(g => g.DogProfileId == dogId && g.IsActive)
            .ToListAsync();
        
        return goals.Select(goal => new GoalProgress
        {
            Goal = goal,
            CurrentValue = CalculateProgress(goal, activities),
            PercentageComplete = CalculatePercentage(goal, activities),
            IsOnTrack = DetermineIfOnTrack(goal, activities)
        }).ToList();
    }
}
```

### Medical Record Management

```csharp
public class MedicalRecordService
{
    public async Task<MedicalRecord> AddVaccinationRecord(
        int dogId, 
        VaccinationDto vaccination)
    {
        var record = new DogMedicalRecord
        {
            DogProfileId = dogId,
            RecordType = MedicalRecordType.Vaccination,
            Title = $"{vaccination.VaccineName} Vaccination",
            Description = vaccination.Notes,
            Date = vaccination.DateAdministered,
            VeterinarianName = vaccination.VeterinarianName,
            VeterinaryClinic = vaccination.ClinicName,
            NextDueDate = vaccination.NextDueDate,
            Cost = vaccination.Cost
        };
        
        await _context.DogMedicalRecords.AddAsync(record);
        
        // Schedule reminder if due date provided
        if (vaccination.NextDueDate.HasValue)
        {
            await _reminderService.ScheduleVaccinationReminder(
                dogId, 
                vaccination.VaccineName, 
                vaccination.NextDueDate.Value);
        }
        
        // Generate vaccination certificate if requested
        if (vaccination.GenerateCertificate)
        {
            var certificateUrl = await GenerateVaccinationCertificate(record);
            record.Documents = JsonSerializer.Serialize(new[] { certificateUrl });
        }
        
        await _context.SaveChangesAsync();
        return record;
    }
    
    private async Task<string> GenerateVaccinationCertificate(DogMedicalRecord record)
    {
        var dog = await _context.DogProfiles
            .Include(d => d.Owner)
            .FirstAsync(d => d.Id == record.DogProfileId);
        
        var pdfBytes = _pdfGenerator.GenerateCertificate(new VaccinationCertificateData
        {
            DogName = dog.Name,
            OwnerName = $"{dog.Owner.FirstName} {dog.Owner.LastName}",
            VaccinationType = record.Title,
            DateAdministered = record.Date,
            VeterinarianName = record.VeterinarianName,
            ClinicName = record.VeterinaryClinic,
            NextDueDate = record.NextDueDate
        });
        
        var fileName = $"certificates/{dog.Id}/vaccination_{record.Id}.pdf";
        return await _blobStorage.UploadAsync(pdfBytes, fileName);
    }
}
```

## Security Considerations

### Data Privacy
1. **Photo Privacy**:
   - Private by default
   - Granular sharing controls
   - No EXIF data exposed publicly
2. **Medical Records**:
   - Encrypted at rest
   - Access logging
   - Owner-only access by default
3. **Location Data**:
   - Optional GPS tracking
   - Anonymized in public views
   - Geofencing alerts

### Access Control
```csharp
public class DogProfileAuthorizationHandler : AuthorizationHandler<DogProfileRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DogProfileRequirement requirement)
    {
        var userId = context.User.GetUserId();
        var dogId = GetDogIdFromResource(context.Resource);
        
        var hasAccess = await _context.DogProfiles
            .AnyAsync(d => d.Id == dogId && d.OwnerId == userId);
        
        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
}
```

## Performance Optimization

### Caching Strategy
```csharp
public class DogProfileCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<DogProfile> GetDogProfileAsync(int dogId)
    {
        // L1 Cache - Memory
        if (_cache.TryGetValue($"dog:{dogId}", out DogProfile cached))
            return cached;
        
        // L2 Cache - Redis
        var redisKey = $"dog:profile:{dogId}";
        var cachedJson = await _distributedCache.GetStringAsync(redisKey);
        
        if (!string.IsNullOrEmpty(cachedJson))
        {
            var profile = JsonSerializer.Deserialize<DogProfile>(cachedJson);
            _cache.Set($"dog:{dogId}", profile, TimeSpan.FromMinutes(5));
            return profile;
        }
        
        // Database
        var dbProfile = await _repository.GetDogProfileAsync(dogId);
        
        // Cache population
        await _distributedCache.SetStringAsync(
            redisKey, 
            JsonSerializer.Serialize(dbProfile),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        
        _cache.Set($"dog:{dogId}", dbProfile, TimeSpan.FromMinutes(5));
        
        return dbProfile;
    }
}
```

### Photo Loading Optimization
- Lazy loading with intersection observer
- Progressive image loading (blur-up technique)
- CDN with geographic distribution
- WebP format with fallbacks

## Monitoring & Analytics

### Key Metrics
1. **Feature Usage**:
   - Photo uploads per day
   - Medical records created
   - Activity logs per user
   - AI feature usage
2. **Performance Metrics**:
   - Photo upload time
   - Page load times
   - API response times
3. **Health Metrics**:
   - Vaccination compliance rate
   - Activity goal achievement
   - Profile completeness

### Event Tracking
```csharp
_analytics.TrackEvent("DogProfileCreated", new
{
    UserId = userId,
    DogId = dog.Id,
    Breed = dog.Breed,
    HasPhoto = !string.IsNullOrEmpty(dog.ProfileImageUrl),
    CompletionPercentage = CalculateProfileCompleteness(dog)
});
```

## Integration Points

### Veterinary API Integration (Future)
```csharp
public interface IVeterinaryAPIService
{
    Task<VetRecord> FetchMedicalHistory(string microchipNumber);
    Task<bool> VerifyVaccination(string certificateNumber);
    Task<List<VetClinic>> FindNearbyVets(double latitude, double longitude);
}
```

### Calendar Integration
- Export vaccination schedules to calendar
- Activity reminders
- Appointment scheduling with providers

## Testing Strategy

### Unit Tests
```csharp
[TestClass]
public class DogProfileServiceTests
{
    [TestMethod]
    public async Task CreateDogProfile_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var request = new CreateDogProfileRequest
        {
            Name = "Max",
            Breed = "Golden Retriever",
            DateOfBirth = DateTime.Now.AddYears(-2),
            Gender = Gender.Male
        };
        
        // Act
        var result = await _service.CreateDogProfile(request);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Max", result.Name);
    }
}
```

### Integration Tests
- Photo upload with processing
- Medical record workflow
- AI feature integration
- Activity tracking accuracy