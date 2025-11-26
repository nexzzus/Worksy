using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core.Attributes;

namespace Worksy.Web.Controllers;

public class AdminController : Controller
{
    [CustomAuthorize("all.access", "Todos")]
    public IActionResult Index()
    {
        return View();
    }
}