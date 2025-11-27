using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Attributes;

namespace Worksy.Web.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}