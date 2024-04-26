using ContactsMangeger.Core.Domain.Exceptions;
using ContactsMangeger.Core.Domain.RepositoryContracts;
using CsvHelper;
using Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonService(IPersonsRepository personsRepository, ILogger<PersonService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null) { throw new ArgumentNullException(nameof(personAddRequest)); }
            ValidationHelper.ModelValidation(personAddRequest);
            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();
            await _personsRepository.AddPerson(person);
            return person.ToPersonResponse();
        }

        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{
        //    PersonResponse person_response = person.ToPersonResponse();
        //    person_response.CountryName = _countryService.GetCountryById(person.CountryId)?.Result?.CountryName;
        //    return person_response;
        //}

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonById(Guid? personId)
        {
            if (personId == null) { return null; }
            Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);
            if (person == null) { return null; };
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");
            List<Person> matchingPersons;
            using (Operation.Time("Time for filter Persons from database"))
            {
                matchingPersons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),
                    nameof(PersonResponse.Email) => await _personsRepository.GetFilteredPersons(temp => temp.Email.Contains(searchString)),
                    nameof(PersonResponse.DateOfBirth) =>
                       await _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),
                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFilteredPersons(temp => temp.Gender.Contains(searchString)),
                    nameof(PersonResponse.CountryName) =>
                        await _personsRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),
                    nameof(PersonResponse.Address) =>
                       await _personsRepository.GetFilteredPersons(temp => temp.Address.Contains(searchString)),
                    _ => await _personsRepository.GetAllPersons()
                };
            }
            _diagnosticContext.Set("Persons", matchingPersons);
            return matchingPersons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation("GetSortedPersons of PersonsService");
            if (sortBy == null) return allPersons;
            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address).ToList(),
                (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.CountryName).ToList(),
                (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.CountryName).ToList(),
                (nameof(PersonResponse.ReceiveNewLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewLetters).ToList(),
                _ => allPersons
            };
            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) { throw new ArgumentNullException(nameof(PersonAddRequest)); }
            ValidationHelper.ModelValidation(personUpdateRequest);
            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);
            if (matchingPerson == null)
            {
                throw new InvalidPersonIdException("Given person does not exist");
            }
            matchingPerson.PersonId = personUpdateRequest.PersonId;
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewLetters = personUpdateRequest.ReceiveNewLetters;
            await _personsRepository.UpdatePerson(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null) { throw new ArgumentNullException(nameof(personId)); }
            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personId.Value);
            if (matchingPerson == null)
            {
                return false;
            }
            await _personsRepository.DeletePersonByPersonId(personId.Value);
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);
            csvWriter.WriteHeader<PersonResponse>();
            csvWriter.NextRecord();
            List<PersonResponse> persons = await GetAllPersons();
            await csvWriter.WriteRecordsAsync(persons);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public Task<MemoryStream> GetPersonsExcel()
        {
            throw new NotImplementedException();
        }
    }
}
