using CRUDapp.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDapp.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController) context.Controller;
            personsController.ViewBag.SearchFields = new Dictionary<string, string>() { { nameof(PersonResponse.PersonName), "Person Name" }, { nameof(PersonResponse.Email), "Email" }, { nameof(PersonResponse.DateOfBirth), "Date of Birth" }, { nameof(PersonResponse.Gender), "Gender" }, { nameof(PersonResponse.CountryName), "Country Name" }, { nameof(PersonResponse.Address), "Adress" } };
            IDictionary<string,object?>? parameters = (IDictionary<string, object?>?) context.HttpContext.Items["arguments"];
            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }
                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }
                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuting));
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchOptions = new List<string>() {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryId),
                        nameof(PersonResponse.Address),
                    };
                    if (searchOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation($"searchBy actual value {searchBy}");
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation($"searchBy updated value {context.ActionArguments["searchBy"]}");
                    }
                }
            }
        }
    }
}
