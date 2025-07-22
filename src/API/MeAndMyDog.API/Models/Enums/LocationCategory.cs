namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Categories for location bookmarks
/// </summary>
public enum LocationCategory
{
    /// <summary>
    /// General/uncategorized location
    /// </summary>
    General = 0,
    
    /// <summary>
    /// Home address
    /// </summary>
    Home = 1,
    
    /// <summary>
    /// Work/office location
    /// </summary>
    Work = 2,
    
    /// <summary>
    /// Veterinary clinic
    /// </summary>
    Veterinary = 3,
    
    /// <summary>
    /// Dog park or recreational area
    /// </summary>
    Park = 4,
    
    /// <summary>
    /// Pet store or pet services
    /// </summary>
    PetStore = 5,
    
    /// <summary>
    /// Grooming salon
    /// </summary>
    Grooming = 6,
    
    /// <summary>
    /// Training facility
    /// </summary>
    Training = 7,
    
    /// <summary>
    /// Boarding/daycare facility
    /// </summary>
    Boarding = 8,
    
    /// <summary>
    /// Emergency veterinary hospital
    /// </summary>
    Emergency = 9,
    
    /// <summary>
    /// Restaurant or cafe (dog-friendly)
    /// </summary>
    Restaurant = 10,
    
    /// <summary>
    /// Hotel or accommodation
    /// </summary>
    Hotel = 11,
    
    /// <summary>
    /// Beach or waterfront
    /// </summary>
    Beach = 12,
    
    /// <summary>
    /// Hiking trail or nature area
    /// </summary>
    Trail = 13,
    
    /// <summary>
    /// Friend or family location
    /// </summary>
    Friend = 14,
    
    /// <summary>
    /// Meeting point
    /// </summary>
    MeetingPoint = 15
}