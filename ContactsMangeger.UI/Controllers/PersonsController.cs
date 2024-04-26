using CRUDapp.Filters;
using CRUDapp.Filters.ActionFilters;
using CRUDapp.Filters.AuthorizationFilters;
using CRUDapp.Filters.ExceptionFilter;
using CRUDapp.Filters.ResourceFilters;
using CRUDapp.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDapp.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller",3 },Order =3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personsService;
        private readonly ICountryService _countryService;
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(IPersonService personsService, ICountryService countryService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countryService = countryService;
            _logger = logger;

        }
        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter),Order =4)]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] {"X-Custom-Key","Custom-Value",1},Order =1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of the PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");        
            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons);
        }
        private async Task PopulateCoutriesSelectMenu()
        {
            List<CountryResponse> countries = await _countryService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCoutriesSelectMenu();
            return View();
        }
        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        //[TypeFilter(typeof(FeatureDisableResourceFilter))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _personsService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }
        [Route("[action]/{PersonId}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid PersonId)
        {
            PersonResponse? person = await _personsService.GetPersonById(PersonId);
            if (person == null)
            {
                RedirectToAction("Index", "Persons");
            }
            PersonUpdateRequest person_update = person!.ToPersonUpdateRequest();
            await PopulateCoutriesSelectMenu();
            return View(person_update);

        }
        [Route("[action]/{PersonId}")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personRequest.PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            //personRequest.PersonId = Guid.NewGuid();
            await _personsService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }
        [Route("[action]/{PersonId}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid PersonId)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            return View(personResponse);
        }
        [Route("[action]/{PersonId}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {

            PersonResponse? personResponse = await _personsService.GetPersonById(personUpdateRequest.PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            await _personsService.DeletePerson(personResponse.PersonId);
            return RedirectToAction("Index", "Persons");

        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", persons, ViewData);
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }
    }
}
