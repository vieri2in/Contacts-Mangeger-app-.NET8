using AutoFixture;
using ContactsMangeger.Core.Domain.RepositoryContracts;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTestProject
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonService>>();
            _personService = new PersonService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? request = null;
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task AddCountry_NullPersonName_ToBeArgumentException()
        {
            PersonAddRequest? request = new PersonAddRequest() { PersonName = null };
            Person person = request.ToPerson();
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.AddPerson(request);
            });
        }
        [Fact]
        public async Task AddCountry_FullPersonDetails_ToBeSuccessful()
        {
            PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@gmail.com").Create();
            Person person = request.ToPerson();
            PersonResponse person_response_expected = person
                .ToPersonResponse();
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            PersonResponse response_from_add = await _personService.AddPerson(request);
            person_response_expected.PersonId = response_from_add.PersonId;
            //List<PersonResponse> persons_from_getAllPersons = await _personService.GetAllPersons();
            //Assert.True(response_from_add.PersonId != Guid.Empty);
            response_from_add.PersonId.Should().NotBe(Guid.Empty);
            //Assert.Contains(response_from_add, persons_from_getAllPersons);
            //persons_from_getAllPersons.Should().Contain(persons_from_getAllPersons);
            response_from_add.Should().Be(person_response_expected);
        }
        //[Fact]
        //public void AddCountry_DuplicateCountryName()
        //{
        //    CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
        //    CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };
        //    Assert.Throws<ArgumentException>(() =>
        //    {
        //        _countryService.AddCountry(request1);
        //        _countryService.AddCountry(request2);
        //    });
        //}
        #endregion
        #region AddPersonById
        [Fact]
        public async Task GetPersonById_NullPersonId_ToBeNull()
        {
            Guid? PersonId = Guid.Empty;
            PersonResponse? person_response_from_get = await _personService.GetPersonById(PersonId);
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }
        [Fact]
        public async Task GetPersonById_ValidPersonId_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "abc@gmail.com").With(temp => temp.Country, null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);
            PersonResponse? person_response_from_get = await _personService.GetPersonById(person.PersonId);
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_expected);

        }
        #endregion
        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmpty()
        {
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //
        }
        #endregion
        #region GetFilteredPersons
        // If the search text is empty and search by its PersonName, it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
               };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_search in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_search.ToString());
            }
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        [Fact]
        public async Task GetFilteredPersons_SearchByValidPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
               };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_search in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_search.ToString());
            }
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion
        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersons_SortBy_PersonName_ASC_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
               };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            //Act
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            List<PersonResponse> persons_list_from_sort = _personService.GetSortedPersons(await _personService.GetAllPersons(), nameof(PersonResponse.PersonName), SortOrderOptions.ASC);
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_search in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_search.ToString());
            }
            persons_list_from_sort.Should().BeInAscendingOrder(temp => temp.PersonName);
        }
        #endregion
        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? person_update = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.UpdatePerson(person_update);
            });
        }
        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
        {
            PersonUpdateRequest? person_update = new PersonUpdateRequest() { PersonId = Guid.NewGuid() };
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_EmptyPersonName_ToBeArgumentException()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "sample1@gmail.com")
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Gender, "Male")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponse = person.ToPersonResponse();
            PersonUpdateRequest? person_update = personResponse.ToPersonUpdateRequest();
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                 .With(temp => temp.Email, "sample1@gmail.com")
                 .With(temp => temp.Gender, "Male")
                 .With(temp => temp.Country, null as Country)
                 .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();
            PersonUpdateRequest? person_update = person_response_expected.ToPersonUpdateRequest();
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update);
            person_response_from_update.Should().Be(person_response_expected);
        }
        #endregion
        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidPersonId_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                  .With(temp => temp.Email, "sample1@gmail.com")
                  .With(temp => temp.Gender, "Male")
                  .With(temp => temp.Country, null as Country)
                  .Create();
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            Assert.True(await _personService.DeletePerson(person.PersonId));
        }
        [Fact]
        public async Task DeletePerson_InvalidPersonId_ToBeFalse()
        {
            Assert.False(await _personService.DeletePerson(Guid.NewGuid()));
        }
        #endregion
    }
}
