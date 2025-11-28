using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
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

        public ServicesController(
            IServicesService servicesService,
            INotyfService notifyService,
            IUserService userService)
        {
            _servicesService = servicesService;
            _notifyService = notifyService;
            _userService = userService;
        }

        // ==================== LISTA / INDEX ====================
        [HttpGet("/Services")]
        [CustomAuthorize("service.show", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
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

            // Verificar si el usuario actual es ADMIN
            bool isAdmin = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN]);

            if (isAdmin is false)
            {
                // Si es admin -> redirigir al panel de Provider
                return RedirectToAction("Index", "Provider");
            }

            // Si NO es admin -> mostrar la vista Index.cshtml de Services
            return View(response.Result);
        }


        // ==================== DETALLES ====================

        [CustomAuthorize("service.show", "Servicios")]
        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _servicesService.GetOneAsync(id);
            if (!response.isSuccess)
            {
                return NotFound();
            }
            
            var currentRol = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN, Env.ROLE_COLLAB]);
            ViewData["Layout"] = currentRol
                ? "_Dashboard"
                : "_Layout";

            return View(response.Result);
        }

        // ==================== CREAR ====================

        [HttpGet]
        [CustomAuthorize("service.create", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
        public async Task<IActionResult> Create()
        {
            var catsResp = await _servicesService.GetAllCategoriesAsync();
            ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
            return View();
        }

        [HttpPost]
        [CustomAuthorize("service.create", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
        public async Task<IActionResult> Create(ServiceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                var catsResp = await _servicesService.GetAllCategoriesAsync();
                ViewBag.Categories = catsResp.isSuccess ? catsResp.Result : new List<CategoryDTO>();
                return View(dto);
            }

            // ASIGNAR SIEMPRE EL USER ACTUAL
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

            var isAdmin = await _userService.CurrentUserHasRoleAsync(new[] { Env.ROLE_ADMIN });
            if (isAdmin is false)
            {
                // colab / user → a su panel de proveedor
                return RedirectToAction("Index", "Provider");
            }

            // admin → vuelve al panel de servicios
            return RedirectToAction(nameof(Index));
        }

        // ==================== EDITAR ====================

        [HttpGet]
        [CustomAuthorize("service.update", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
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
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
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

        // ==================== ELIMINAR ====================

        [HttpPost]
        [CustomAuthorize("service.delete", "Servicios")]
        [CustomRoleAuthorize([Env.ROLE_ADMIN, Env.ROLE_COLLAB])]
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

            var isAdmin = await _userService.CurrentUserHasRoleAsync([Env.ROLE_ADMIN]);
            if (isAdmin is false)
            {
                return RedirectToAction("Index", "Provider");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}