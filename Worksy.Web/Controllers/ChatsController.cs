using Microsoft.AspNetCore.Mvc;

namespace Worksy.Web.Controllers;

public class ChatsController : Controller
{
    public IActionResult WithUser(Guid prestadorId, Guid serviceId)
    {
        ViewBag.PrestadorId = prestadorId;
        ViewBag.ServiceId = serviceId;
        return View();
    }
}