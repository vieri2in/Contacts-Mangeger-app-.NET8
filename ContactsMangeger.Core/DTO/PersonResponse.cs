using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewLetters { get; set; }
        public double? Age { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) { return false; }
            var person_to_compare = (PersonResponse)obj;
            return PersonId == person_to_compare.PersonId && PersonName == person_to_compare.PersonName && Email == person_to_compare.Email && DateOfBirth == person_to_compare.DateOfBirth && Gender == person_to_compare.Gender && CountryId == person_to_compare.CountryId && Address == person_to_compare.Address && ReceiveNewLetters == person_to_compare.ReceiveNewLetters;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"Person: PersonId: {PersonId}, PersonName: {PersonName}, Email: {Email}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest() { PersonId = PersonId, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = (GenderOptions)(Enum.Parse(typeof(GenderOptions), Gender!, true)), Address = Address, CountryId = CountryId, ReceiveNewLetters = ReceiveNewLetters };
        }
    }
    public static class PersonExtensions
    {

        public static PersonResponse ToPersonResponse(this Person person)

        {
            Double? age_result = person.DateOfBirth != null ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null;
            return new PersonResponse() { PersonId = person.PersonId, PersonName = person.PersonName, Email = person.Email, DateOfBirth = person.DateOfBirth, Gender = person.Gender, CountryId = person.CountryId, Address = person.Address, ReceiveNewLetters = person.ReceiveNewLetters, Age = age_result, CountryName = person.Country?.CountryName };
            //    return new PersonResponse()
            //    {
            //        PersonID = person.PersonID,
            //        PersonName = person.PersonName,
            //        Email = person.Email,
            //        DateOfBirth = person.DateOfBirth,
            //        ReceiveNewsLetters = person.ReceiveNewsLetters,
            //        Address = person.Address,
            //        CountryID = person.CountryID,
            //        Gender = person.Gender,
            //        Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
            //        Country = person.Country?.CountryName
            //         person_response.CountryName = _countryService.GetCountryById(person.CountryId)?.Result?.CountryName;
            //};
        }
    }
}
