﻿using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name cannot be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be a valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        //[Required(ErrorMessage = "Person gender cannot be blank")]
        public GenderOptions? Gender { get; set; }
        //[Required(ErrorMessage = "Please select a country")]
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewLetters { get; set; }
        public Person ToPerson()
        {
            return new Person() { PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryId = CountryId, Address = Address, ReceiveNewLetters = ReceiveNewLetters };
        }
    }
}
