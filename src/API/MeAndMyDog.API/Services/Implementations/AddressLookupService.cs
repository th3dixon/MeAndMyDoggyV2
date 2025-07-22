using Dapper;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.DTOs.Address;
using MeAndMyDog.API.Models;
using MeAndMyDog.API.Services.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MeAndMyDog.API.Services.Implementations
{
    /// <summary>
    /// Service for UK address and postcode lookup functionality
    /// </summary>
    public class AddressLookupService : IAddressLookupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AddressLookupService> _logger;

        public AddressLookupService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AddressLookupService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult<List<AddressSearchResultDto>>> SearchAddressesAsync(string searchTerm, int maxResults = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
                {
                    return ServiceResult<List<AddressSearchResultDto>>.FailureResult("Search term must be at least 3 characters");
                }

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@SearchTerm", searchTerm);
                parameters.Add("@MaxResults", maxResults);
                parameters.Add("@IncludePostcodeOnly", true);

                var results = await connection.QueryAsync<AddressSearchResultDto>(
                    "sp_AddressAutocomplete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResult<List<AddressSearchResultDto>>.SuccessResult(results.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching addresses for term: {SearchTerm}", searchTerm);
                return ServiceResult<List<AddressSearchResultDto>>.FailureResult("An error occurred while searching addresses");
            }
        }

        public async Task<ServiceResult<PostcodeInfoDto>> LookupPostcodeAsync(string postcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(postcode))
                {
                    return ServiceResult<PostcodeInfoDto>.FailureResult("Postcode is required");
                }

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@Postcode", postcode);

                var result = await connection.QuerySingleOrDefaultAsync<PostcodeInfoDto>(
                    "sp_LookupPostcode",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return ServiceResult<PostcodeInfoDto>.FailureResult("Postcode not found");
                }

                // Parse cities from comma-separated string
                if (!string.IsNullOrEmpty(result.Cities?.FirstOrDefault()))
                {
                    result.Cities = result.Cities[0].Split(',').Select(c => c.Trim()).ToList();
                }

                return ServiceResult<PostcodeInfoDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up postcode: {Postcode}", postcode);
                return ServiceResult<PostcodeInfoDto>.FailureResult("An error occurred while looking up the postcode");
            }
        }

        public async Task<ServiceResult<AddressDetailDto>> GetAddressByIdAsync(int addressId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@AddressId", addressId);

                var result = await connection.QuerySingleOrDefaultAsync<AddressDetailDto>(
                    "sp_GetAddressById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return ServiceResult<AddressDetailDto>.FailureResult("Address not found");
                }

                return ServiceResult<AddressDetailDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address by ID: {AddressId}", addressId);
                return ServiceResult<AddressDetailDto>.FailureResult("An error occurred while retrieving the address");
            }
        }

        public async Task<ServiceResult<List<AddressSearchResultDto>>> SearchAddressesNearLocationAsync(
            decimal latitude, 
            decimal longitude, 
            decimal radiusMiles = 5, 
            string? searchTerm = null, 
            int maxResults = 50)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@Latitude", latitude);
                parameters.Add("@Longitude", longitude);
                parameters.Add("@RadiusMiles", radiusMiles);
                parameters.Add("@SearchTerm", searchTerm);
                parameters.Add("@MaxResults", maxResults);

                var results = await connection.QueryAsync<AddressSearchResultDto>(
                    "sp_SearchAddressesNearLocation",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResult<List<AddressSearchResultDto>>.SuccessResult(results.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching addresses near location: {Latitude}, {Longitude}", latitude, longitude);
                return ServiceResult<List<AddressSearchResultDto>>.FailureResult("An error occurred while searching addresses near location");
            }
        }

        public async Task<ServiceResult<List<CitySearchResultDto>>> SearchCitiesAsync(string searchTerm, int? countyId = null, int maxResults = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ServiceResult<List<CitySearchResultDto>>.FailureResult("Search term is required");
                }

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@SearchTerm", searchTerm);
                parameters.Add("@CountyId", countyId);
                parameters.Add("@MaxResults", maxResults);

                var results = await connection.QueryAsync<CitySearchResultDto>(
                    "sp_SearchCities",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResult<List<CitySearchResultDto>>.SuccessResult(results.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cities for term: {SearchTerm}", searchTerm);
                return ServiceResult<List<CitySearchResultDto>>.FailureResult("An error occurred while searching cities");
            }
        }
    }
}