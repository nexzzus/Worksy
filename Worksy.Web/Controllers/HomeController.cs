using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Data.Entities;
using Worksy.Web.Models;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, IUserService userService, UserManager<User> userManager)
    {
        _logger = logger;
        _userService = userService;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Start()
    {
        var user = await _userManager.GetUserAsync(User);

        if (await _userService.CurrentUserIsAuthorizedAsync("access.all", "Acceso completo"))
        {
            return RedirectToAction("Index", "Admin");
        }
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Contact()
    {
        return View();
    }

}