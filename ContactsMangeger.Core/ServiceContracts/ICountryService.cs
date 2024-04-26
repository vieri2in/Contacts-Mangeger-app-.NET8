using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        Task<List<CountryResponse>> GetAllCountries();
        Task<CountryResponse?> GetCountryById(Guid? CountryId);
    }
}
