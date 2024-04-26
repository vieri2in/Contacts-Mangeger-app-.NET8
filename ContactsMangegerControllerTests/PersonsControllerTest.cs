using AutoFixture;
using CRUDapp.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTestProject
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personsService;
        private readonly ICountryService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<IPersonService> _personsServiceMock;
        private readonly Mock<ICountryService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly IFixture _fixture;
        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonService>();
            _countriesServiceMock = new Mock<ICountryService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();
            _personsService = _personsServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ToReturnIndexViewAndPersonsList()
        {
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);
            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons_response_list);
            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).Returns(persons_response_list);
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>());
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }
        #endregion
        #region Create
        //[Fact]
        //public async Task Create_IfValidationErrors_ReturnCreateView()
        //{
        //    PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
        //    PersonResponse persons_response = _fixture.Create<PersonResponse>();
        //    List<CountryResponse> countries_response = _fixture.Create<List<CountryResponse>>();
        //    _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries_response);
        //    _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(persons_response);
        //    PersonsController personsController = new PersonsController(_personsService, _countriesService,null);
        //    personsController.ModelState.AddModelError("PersonName", "Person name cannot be blank");
        //    IActionResult result = await personsController.Create(person_add_request);
        //    ViewResult viewResult = Assert.IsType<ViewResult>(result);
        //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        //    viewResult.ViewData.Model.Should().Be(person_add_request);
        //}
        [Fact]
        public async Task Create_IfNoValidationErrors_ReturnRedirectToIndexView()
        {
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse persons_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries_response = _fixture.Create<List<CountryResponse>>();
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries_response);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(persons_response);
            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);
            IActionResult result = await personsController.Create(person_add_request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
