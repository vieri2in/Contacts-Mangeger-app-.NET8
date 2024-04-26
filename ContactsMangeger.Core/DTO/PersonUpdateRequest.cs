using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required]
        public Guid PersonId { get; set; }
        [Required(ErrorMessage = "Person name cannot be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be a valid email")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //[Required(ErrorMessage = "Person gender cannot be blank")]
        public GenderOptions? Gender { get; set; }
        //[Required(ErrorMessage = "Please select a country")]
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewLetters { get; set; }
        public Person ToPerson()
        {
            return new Person() { PersonId = PersonId, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, Address = Address, ReceiveNewLetters = ReceiveNewLetters };
        }
    }
}
