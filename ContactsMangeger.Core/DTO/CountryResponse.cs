using Entities;

namespace ServiceContracts.DTO
{
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(CountryResponse)) { return false; }
            var country_to_compare = (CountryResponse)obj;
            return CountryName == country_to_compare.CountryName && CountryId == country_to_compare.CountryId;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public static class CountryExtentions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
