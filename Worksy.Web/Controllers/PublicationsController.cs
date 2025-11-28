using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Pagination;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;

public class PublicationsController : Controller
{
    private readonly IServicesService _servicesService;
    private readonly INotyfService _notifyService;

    public PublicationsController(INotyfService notifyService, IServicesService servicesService)
    {
        _notifyService = notifyService;
        _servicesService = servicesService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
    {
        // Traer lista paginada de servicios
        Response<PaginationResponse<ServiceDTO>> response =
            await _servicesService.GetPaginatedListAsync(request);

        if (!response.isSuccess)
        {
            _notifyService.Error(response.Message);
            // Devuelves el Index de Services vacío si falla
            return View(new PaginationResponse<ServiceDTO>());
        }

        return View(response.Result);
    }
}