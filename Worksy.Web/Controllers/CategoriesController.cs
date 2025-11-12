using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Attributes;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers
{
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
        [CustomAuthorize("category.show", "Categories")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? q = null)
        {
            Response<List<CategoryDTO>> response = await _categoriesService.GetAllAsync();

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(new List<CategoryDTO>());
            }

            var data = response.Result ?? new List<CategoryDTO>();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                data = data.Where(c =>
                    (!string.IsNullOrWhiteSpace(c.Name) && c.Name.ToLower().Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(c.Description) && c.Description.ToLower().Contains(term)) ||
                    (c.Services != null && c.Services.Any(s =>
                        !string.IsNullOrWhiteSpace(s.Title) && s.Title.ToLower().Contains(term)))
                ).ToList();
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = data.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var paged = data
                .OrderBy(c => c.Name)
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

        [CustomAuthorize("category.show", "Categories")]
        public async Task<IActionResult> Details(Guid id)
        {
            Response<CategoryDTO> response = await _categoriesService.GetOneAsync(id);
            if (!response.isSuccess)
            {
                return NotFound();
            }

            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize("category.create", "Categories")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize("category.create", "Categories")]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }

            Response<CategoryDTO> response = await _categoriesService.CreateAsync(dto);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [CustomAuthorize("category.update", "Categories")]
        public async Task<IActionResult> Edit(Guid id)
        {
            Response<CategoryDTO> response = await _categoriesService.GetOneAsync(id);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        [HttpPost]
        [CustomAuthorize("category.update","Categories")]

    public async Task<IActionResult> Edit(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }

            Response<CategoryDTO> response = await _categoriesService.UpdateAsync(dto);

            if (!response.isSuccess)
            {
                _notifyService.Error(response.Message);
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize("category.delete","Categories")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<object> response = await _categoriesService.DeleteAsync(id);

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
