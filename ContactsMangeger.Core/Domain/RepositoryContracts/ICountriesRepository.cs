using Entities;

namespace ContactsMangeger.Core.Domain.RepositoryContracts
{
    public interface ICountriesRepository
    {
        Task<Country> AddCountry(Country country);
        Task<List<Country>> GetAllCountries();
        Task<Country?> GetCountryByCountryId(Guid countryId);
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
