using Microsoft.AspNetCore.Mvc;

namespace Worksy.Web.Controllers
{
    public class TodosDemoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}