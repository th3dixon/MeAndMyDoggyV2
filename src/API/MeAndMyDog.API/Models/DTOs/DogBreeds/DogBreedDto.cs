namespace MeAndMyDog.API.Models.DTOs.DogBreeds;

/// <summary>
/// DTO for dog breed information
/// </summary>
public class DogBreedDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SizeCategory { get; set; }
}