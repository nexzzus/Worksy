using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoriesService _categoriesService;
    public INotyfService _notifyService { get; }
    
    public CategoriesController(ICategoriesService categoriesService, INotyfService notifyService)
    {
        _categoriesService = categoriesService;
        _notifyService = notifyService;
    }

    [HttpGet("/Categories")]
    public async Task<IActionResult> Index()
    {
        Response<List<CategoryDTO>> response = await _categoriesService.GetAllAsync();
        if (!response.isSuccess)
        {
            _notifyService.Error(response.Message);
            return View(new List<CategoryDTO>());
        }

        var data = response.Result ?? new List<CategoryDTO>();
        return View(data.OrderBy(c => c.Name).ToList());
    }
}