using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;
        public INotyfService _notifyService { get; }

        public ServicesController(IServicesService servicesService, INotyfService notifyService)
        {
            _servicesService = servicesService;
            _notifyService = notifyService;
        }
        
        
        [HttpGet("/Services")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? q = null)
        {
            Response<List<ServiceDTO>> response = await _servicesService.GetAllAsync();

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(new List<ServiceDTO>());
            }

            var data = response.Result ?? new List<ServiceDTO>();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                data = data.Where(s =>
                    (!string.IsNullOrWhiteSpace(s.Title)       && s.Title.ToLower().Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(s.Description) && s.Description.ToLower().Contains(term)) ||
                    (s.Categories != null && s.Categories.Any(c => !string.IsNullOrWhiteSpace(c.Name) && c.Name.ToLower().Contains(term)))
                ).ToList();
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = data.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var paged = data
                .OrderBy(s => s.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.Q = q;

            return View(paged);
        }


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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceDTO dto)
        {

            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }

            Response<ServiceDTO> response = await _servicesService.CreateAsync(dto);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Response<ServiceDTO> response = await _servicesService.GetOneAsync(id);

            if (!response.isSuccess)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }

            Response<ServiceDTO> response = await _servicesService.UpdateAsync(dto);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]

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

            return RedirectToAction(nameof(Index));
        }
    }
}
