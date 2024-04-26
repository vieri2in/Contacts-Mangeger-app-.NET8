using CRUDapp.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDapp.Filters.ActionFilters
{
    public class PersonCreateAndEditActionFilter : IAsyncActionFilter
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        public PersonCreateAndEditActionFilter(ICountryService countryService, ILogger<ResponseHeaderActionFilter> logger)
        {
            _countryService = countryService;
            _logger = logger;

        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(PersonCreateAndEditActionFilter), nameof(OnActionExecutionAsync));
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countryService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    context.Result = personsController.View(context.ActionArguments["personRequest"]);
                }
                else
                {
                    await next();
                    _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(PersonCreateAndEditActionFilter), nameof(OnActionExecutionAsync));
                }
            }
            else
            {
                await next();
                _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(PersonCreateAndEditActionFilter), nameof(OnActionExecutionAsync));
            }
            
        }
    }
}
