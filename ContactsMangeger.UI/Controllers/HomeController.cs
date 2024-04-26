using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDapp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("Error")]
        [HttpGet]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null) {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }
            return View();
        }
    }
}
