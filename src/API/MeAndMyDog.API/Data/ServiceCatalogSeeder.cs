using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Data;

/// <summary>
/// Provides seed data for service categories and sub-services
/// </summary>
public static class ServiceCatalogSeeder
{
    /// <summary>
    /// Gets a pre-defined list of service categories with their associated sub-services
    /// </summary>
    /// <returns>List of service categories with sub-services for seeding the database</returns>
    public static List<ServiceCategory> GetServiceCategories()
    {
        var categories = new List<ServiceCategory>();
        
        // 1. Dog Walking
        var dogWalkingId = Guid.NewGuid();
        var dogWalking = new ServiceCategory
        {
            ServiceCategoryId = dogWalkingId,
            Name = "Dog Walking",
            Description = "Professional dog walking services including daily walks, exercise, and fresh air for your furry friend with GPS tracking and photo updates.",
            IconClass = "fas fa-walking",
            ColorCode = "pet-orange",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = dogWalkingId,
                    Name = "Quick Walk (15-30 min)",
                    Description = "Short walks for bathroom breaks and light exercise",
                    DurationMinutes = 30,
                    SuggestedMinPrice = 12.00m,
                    SuggestedMaxPrice = 18.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = dogWalkingId,
                    Name = "Standard Walk (30-60 min)",
                    Description = "Regular walks for exercise and socialization",
                    DurationMinutes = 45,
                    SuggestedMinPrice = 18.00m,
                    SuggestedMaxPrice = 25.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = dogWalkingId,
                    Name = "Extended Walk (60+ min)",
                    Description = "Long walks for high-energy dogs requiring more exercise",
                    DurationMinutes = 75,
                    SuggestedMinPrice = 25.00m,
                    SuggestedMaxPrice = 35.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = dogWalkingId,
                    Name = "Group Walk",
                    Description = "Walks with other friendly dogs for socialization",
                    DurationMinutes = 45,
                    SuggestedMinPrice = 15.00m,
                    SuggestedMaxPrice = 20.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = dogWalkingId,
                    Name = "Solo Walk",
                    Description = "One-on-one walks for dogs that prefer individual attention",
                    DurationMinutes = 45,
                    SuggestedMinPrice = 22.00m,
                    SuggestedMaxPrice = 30.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 5,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(dogWalking);

        // 2. Pet Sitting
        var petSittingId = Guid.NewGuid();
        var petSitting = new ServiceCategory
        {
            ServiceCategoryId = petSittingId,
            Name = "Pet Sitting",
            Description = "Professional in-home pet care while you're away, with regular updates and overnight stays available.",
            IconClass = "fas fa-home",
            ColorCode = "pet-green",
            DisplayOrder = 2,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = petSittingId,
                    Name = "Drop-in Visits",
                    Description = "Short visits to check on pets, feed, and provide companionship",
                    DurationMinutes = 30,
                    SuggestedMinPrice = 18.00m,
                    SuggestedMaxPrice = 25.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = petSittingId,
                    Name = "Half-day Sitting",
                    Description = "4-hour pet sitting sessions in your home",
                    DurationMinutes = 240,
                    SuggestedMinPrice = 35.00m,
                    SuggestedMaxPrice = 50.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = petSittingId,
                    Name = "Full-day Sitting",
                    Description = "8-hour pet sitting sessions with meals, walks, and companionship",
                    DurationMinutes = 480,
                    SuggestedMinPrice = 60.00m,
                    SuggestedMaxPrice = 80.00m,
                    DefaultPricingType = PricingType.PerDay,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = petSittingId,
                    Name = "Overnight Care",
                    Description = "Overnight pet sitting in your home with full care",
                    SuggestedMinPrice = 45.00m,
                    SuggestedMaxPrice = 65.00m,
                    DefaultPricingType = PricingType.PerNight,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = petSittingId,
                    Name = "Holiday Care",
                    Description = "Extended care for multiple days while you're away",
                    SuggestedMinPrice = 40.00m,
                    SuggestedMaxPrice = 60.00m,
                    DefaultPricingType = PricingType.PerDay,
                    DisplayOrder = 5,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(petSitting);

        // 3. Grooming
        var groomingId = Guid.NewGuid();
        var grooming = new ServiceCategory
        {
            ServiceCategoryId = groomingId,
            Name = "Grooming",
            Description = "Professional grooming services including baths, nail trims, and styling at your home or our facility.",
            IconClass = "fas fa-cut",
            ColorCode = "pet-blue",
            DisplayOrder = 3,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = groomingId,
                    Name = "Basic Wash & Dry",
                    Description = "Shampooing, conditioning, and drying",
                    DurationMinutes = 60,
                    SuggestedMinPrice = 25.00m,
                    SuggestedMaxPrice = 40.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = groomingId,
                    Name = "Full Grooming",
                    Description = "Complete grooming including wash, dry, cut, and styling",
                    DurationMinutes = 120,
                    SuggestedMinPrice = 35.00m,
                    SuggestedMaxPrice = 65.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = groomingId,
                    Name = "Nail Trimming",
                    Description = "Professional nail trimming and filing",
                    DurationMinutes = 20,
                    SuggestedMinPrice = 12.00m,
                    SuggestedMaxPrice = 20.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = groomingId,
                    Name = "Ear Cleaning",
                    Description = "Gentle ear cleaning and health check",
                    DurationMinutes = 15,
                    SuggestedMinPrice = 10.00m,
                    SuggestedMaxPrice = 18.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = groomingId,
                    Name = "De-shedding Treatment",
                    Description = "Specialized treatment to reduce shedding",
                    DurationMinutes = 45,
                    SuggestedMinPrice = 20.00m,
                    SuggestedMaxPrice = 35.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 5,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(grooming);

        // 4. Training
        var trainingId = Guid.NewGuid();
        var training = new ServiceCategory
        {
            ServiceCategoryId = trainingId,
            Name = "Training",
            Description = "Professional dog training sessions for behavior, obedience, and special skills development.",
            IconClass = "fas fa-graduation-cap",
            ColorCode = "pet-purple",
            DisplayOrder = 4,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = trainingId,
                    Name = "Basic Obedience",
                    Description = "Fundamental commands: sit, stay, come, down, heel",
                    DurationMinutes = 60,
                    SuggestedMinPrice = 40.00m,
                    SuggestedMaxPrice = 60.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = trainingId,
                    Name = "Behavioral Training",
                    Description = "Addressing specific behavioral issues",
                    DurationMinutes = 90,
                    SuggestedMinPrice = 55.00m,
                    SuggestedMaxPrice = 80.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = trainingId,
                    Name = "Puppy Training",
                    Description = "Specialized training for puppies under 6 months",
                    DurationMinutes = 60,
                    SuggestedMinPrice = 35.00m,
                    SuggestedMaxPrice = 55.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = trainingId,
                    Name = "Advanced Training",
                    Description = "Complex commands and specialized skills training",
                    DurationMinutes = 90,
                    SuggestedMinPrice = 60.00m,
                    SuggestedMaxPrice = 90.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = trainingId,
                    Name = "Agility Training",
                    Description = "Fun agility course training for active dogs",
                    DurationMinutes = 75,
                    SuggestedMinPrice = 45.00m,
                    SuggestedMaxPrice = 70.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 5,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(training);

        // 5. Boarding
        var boardingId = Guid.NewGuid();
        var boarding = new ServiceCategory
        {
            ServiceCategoryId = boardingId,
            Name = "Boarding",
            Description = "Safe and comfortable boarding facilities for your pet when you're away.",
            IconClass = "fas fa-bed",
            ColorCode = "pet-pink",
            DisplayOrder = 5,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = boardingId,
                    Name = "Daycare",
                    Description = "Daytime care and socialization while you're at work",
                    DurationMinutes = 480,
                    SuggestedMinPrice = 30.00m,
                    SuggestedMaxPrice = 45.00m,
                    DefaultPricingType = PricingType.PerDay,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = boardingId,
                    Name = "Overnight Boarding",
                    Description = "Overnight care in a boarding facility",
                    SuggestedMinPrice = 40.00m,
                    SuggestedMaxPrice = 60.00m,
                    DefaultPricingType = PricingType.PerNight,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = boardingId,
                    Name = "Extended Stay",
                    Description = "Multi-day boarding for holidays or business trips",
                    SuggestedMinPrice = 35.00m,
                    SuggestedMaxPrice = 55.00m,
                    DefaultPricingType = PricingType.PerDay,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = boardingId,
                    Name = "Holiday Boarding",
                    Description = "Special holiday care packages with extra attention",
                    SuggestedMinPrice = 45.00m,
                    SuggestedMaxPrice = 70.00m,
                    DefaultPricingType = PricingType.PerDay,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(boarding);

        // 6. Veterinary Services
        var veterinaryId = Guid.NewGuid();
        var veterinary = new ServiceCategory
        {
            ServiceCategoryId = veterinaryId,
            Name = "Veterinary Services",
            Description = "Convenient transport to vet appointments and basic health monitoring services.",
            IconClass = "fas fa-stethoscope",
            ColorCode = "warm-gray",
            DisplayOrder = 6,
            CreatedAt = DateTime.UtcNow,
            SubServices = new List<SubService>
            {
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = veterinaryId,
                    Name = "Vet Transport",
                    Description = "Safe transport to and from veterinary appointments",
                    DurationMinutes = 60,
                    SuggestedMinPrice = 20.00m,
                    SuggestedMaxPrice = 35.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = veterinaryId,
                    Name = "Emergency Transport",
                    Description = "Urgent transport for emergency veterinary care",
                    DurationMinutes = 45,
                    SuggestedMinPrice = 35.00m,
                    SuggestedMaxPrice = 60.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = veterinaryId,
                    Name = "Health Monitoring",
                    Description = "Basic health checks and medication administration",
                    DurationMinutes = 30,
                    SuggestedMinPrice = 25.00m,
                    SuggestedMaxPrice = 40.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new SubService
                {
                    SubServiceId = Guid.NewGuid(),
                    ServiceCategoryId = veterinaryId,
                    Name = "Medication Administration",
                    Description = "Reliable administration of prescribed medications",
                    DurationMinutes = 15,
                    SuggestedMinPrice = 15.00m,
                    SuggestedMaxPrice = 25.00m,
                    DefaultPricingType = PricingType.PerService,
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        categories.Add(veterinary);

        return categories;
    }
}