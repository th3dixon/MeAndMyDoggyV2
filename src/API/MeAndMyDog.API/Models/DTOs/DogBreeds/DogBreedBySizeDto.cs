namespace MeAndMyDog.API.Models.DTOs.DogBreeds;

/// <summary>
/// DTO for dog breeds grouped by size category
/// </summary>
public class DogBreedBySizeDto
{
    public string SizeCategory { get; set; } = string.Empty;
    public List<DogBreedDto> Breeds { get; set; } = new();
}