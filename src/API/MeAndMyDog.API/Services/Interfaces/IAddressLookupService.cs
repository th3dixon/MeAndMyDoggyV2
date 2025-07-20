using MeAndMyDog.API.DTOs.Address;
using MeAndMyDog.API.Models;

namespace MeAndMyDog.API.Services.Interfaces
{
    public interface IAddressLookupService
    {
        Task<ServiceResult<List<AddressSearchResultDto>>> SearchAddressesAsync(string searchTerm, int maxResults = 20);
        Task<ServiceResult<PostcodeInfoDto>> LookupPostcodeAsync(string postcode);
        Task<ServiceResult<AddressDetailDto>> GetAddressByIdAsync(int addressId);
        Task<ServiceResult<List<AddressSearchResultDto>>> SearchAddressesNearLocationAsync(decimal latitude, decimal longitude, decimal radiusMiles = 5, string? searchTerm = null, int maxResults = 50);
        Task<ServiceResult<List<CitySearchResultDto>>> SearchCitiesAsync(string searchTerm, int? countyId = null, int maxResults = 20);
    }
}