using ContactsMangeger.Core.Domain.RepositoryContracts;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountryService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null) { throw new ArgumentNullException(nameof(countryAddRequest)); }
            if (countryAddRequest.CountryName == null) { throw new ArgumentException(nameof(countryAddRequest.CountryName)); }
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null) { throw new ArgumentException("Given country name already exists"); }
            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid();
            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();
        }
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? countryId)
        {
            if (countryId == null) { return null; }
            Country? country_from_list = await _countriesRepository.GetCountryByCountryId(countryId.Value);
            if (country_from_list == null) { return null; }
            return country_from_list.ToCountryResponse();
        }
    }
}
