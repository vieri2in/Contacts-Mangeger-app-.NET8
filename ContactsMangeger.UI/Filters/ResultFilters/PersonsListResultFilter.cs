using CRUDapp.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDapp.Filters.ResultFilters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;
        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            //context.HttpContext.Response.Headers["sdsd"] = "dsds";
            // cannot write into response  headers because it's read only
        }
    }
}
