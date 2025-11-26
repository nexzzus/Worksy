using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Attributes;
using Worksy.Web.Core.Pagination;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;
        private readonly INotyfService _notifyService;
        private readonly IUserService _userService;

        public ServicesController(IServicesService servicesService, INotyfService notify, IUserService userService)
        {
            _servicesService = servicesService;
            _notifyService = notify;
            _userService = userService;
        }


        [HttpGet("/Services")]
        [CustomAuthorize("service.show", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<ServiceDTO>> response = await _servicesService.GetPaginatedListAsync(request);

            bool isAdmin = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN]);
            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(new PaginationResponse<ServiceDTO>());
            }

            if (isAdmin)
            {
                return View(response.Result);
            }

            return RedirectToAction("Index", "Provider");
        }


        [CustomAuthorize("service.show", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN])]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _servicesService.GetOneAsync(id);
            if (!response.isSuccess)
            {
                return NotFound();
            }

            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize("service.create", "Servicios")]
        public async Task<IActionResult> Create()
        {
            // Cargar categorías para el formulario
            var catsResp = await _servicesService.GetAllCategoriesAsync();
            ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
            return View();
        }

        [HttpPost]
        [CustomAuthorize("service.create", "Servicios")]
        public async Task<IActionResult> Create(ServiceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                var catsResp = await _servicesService.GetAllCategoriesAsync();
                ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
                return View(dto);
            }

            var userId = _servicesService.GetCurrentUserId();
            Response<ServiceDTO> response = await _servicesService.CreateAsync(dto, userId);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                var catsResp = await _servicesService.GetAllCategoriesAsync();
                ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
                return View(dto);
            }

            _notifyService.Success(response.Message);

            var isAdmin = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN]);
            if (isAdmin is false)
            {
                return RedirectToAction("Index", "Provider");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [CustomAuthorize("service.update", "Servicios")]
        public async Task<IActionResult> Edit(Guid id)
        {
            Response<ServiceDTO> response = await _servicesService.GetOneAsync(id);

            if (!response.isSuccess)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return RedirectToAction(nameof(Index));
            }

            var catsResp = await _servicesService.GetAllCategoriesAsync();
            ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();

            return View(response.Result);
        }

        [HttpPost]
        [CustomAuthorize("service.update", "Servicios")]
        public async Task<IActionResult> Edit(ServiceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                var catsResp = await _servicesService.GetAllCategoriesAsync();
                ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
                return View(dto);
            }

            Response<ServiceDTO> response = await _servicesService.UpdateAsync(dto);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                var catsResp = await _servicesService.GetAllCategoriesAsync();
                ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize("service.delete", "Servicios")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<object> response = await _servicesService.DeleteAsync(id);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
            }
            else
            {
                _notifyService.Success(response.Message);
            }

            bool isAdmin = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN]);
            if (isAdmin is false)
            {
                return RedirectToAction("Index", "Provider");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}