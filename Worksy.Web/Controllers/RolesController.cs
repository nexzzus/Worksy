using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Attributes;
using Worksy.Web.Core.Pagination;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Controllers;
public class RolesController : Controller
{
    private readonly IRolesService _rolesService;
    private readonly INotyfService _notyfService;

    public RolesController(IRolesService rolesService, INotyfService notyfService)
    {
        _rolesService = rolesService;
        _notyfService = notyfService;
    }
    
    [HttpGet]
    [CustomAuthorize(permission:"rol.show",module: "Roles")]
    public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
    {
        Response<PaginationResponse<WorksyRoleDTO>> response = await _rolesService.GetPaginatedListAsync(request);

        if (!response.isSuccess)
        {
            _notyfService.Error(response.Message);
            return View(response);
        }
        
        return View(response.Result);
    }

    [HttpGet]
    [CustomAuthorize("rol.create", "Roles")]
    public async Task<IActionResult> Create()
    {
        Response<List<PermissionsForRoleDTO>> PermissionResponse = await _rolesService.GetPermissionsAsync();

        if (!PermissionResponse.isSuccess)
        {
            _notyfService.Error(PermissionResponse.Message);
            return RedirectToAction(nameof(Index));
        }

        WorksyRoleDTO dto = new WorksyRoleDTO()
        {
            Permissions = PermissionResponse.Result
        };
        return View(dto);
    }

    [HttpPost]
    [CustomAuthorize("rol.create", "Roles")]
    public async Task<IActionResult> Create(WorksyRoleDTO dto)
    {
        if (!ModelState.IsValid)
        {
            _notyfService.Error("Complete los campos requeridos");
            
            Response<List<PermissionsForRoleDTO>> permissionsResponse = await _rolesService.GetPermissionsAsync();
            if (!permissionsResponse.isSuccess)
            {
                _notyfService.Error(permissionsResponse.Message);
                return RedirectToAction(nameof(Index));
            }
            dto.Permissions = permissionsResponse.Result;
            return View(dto);
        }

        Response<WorksyRoleDTO> result = await _rolesService.CreateAync(dto);
        if (result.isSuccess)
        {
            _notyfService.Success(result.Message);
            return RedirectToAction(nameof(Index));
        }
        
        _notyfService.Error(result.Message);
        
        Response<List<PermissionsForRoleDTO>> permissionsResponse2 = await _rolesService.GetPermissionsAsync();
        if (!permissionsResponse2.isSuccess)
        {
            _notyfService.Error(permissionsResponse2.Message);
            return RedirectToAction(nameof(Index));
        }
        dto.Permissions = permissionsResponse2.Result;
        return View(dto);
    }

    [HttpGet]
    [CustomAuthorize("rol.update", "Roles")]
    public async Task<IActionResult> Edit(Guid id)
    {
        Response<WorksyRoleDTO> response = await _rolesService.GetOneAsync(id);

        if (!response.isSuccess)
        {
            _notyfService.Error(response.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(response.Result);
    }

    [HttpPost]
    [CustomAuthorize("rol.update", "Roles")]
    public async Task<IActionResult> Edit(WorksyRoleDTO dto)
    {
        if (!ModelState.IsValid)
        {
            _notyfService.Error("Complete los campos requeridos");
            
            Response<List<PermissionsForRoleDTO>> permissionsResponse = await _rolesService.GetPermissionsAsync();
            if (!permissionsResponse.isSuccess)
            {
                _notyfService.Error(permissionsResponse.Message);
                return RedirectToAction(nameof(Index));
            }
            dto.Permissions = permissionsResponse.Result;
            return View(dto);
        }

        Response<WorksyRoleDTO> result = await _rolesService.EditAsync(dto);
        if (result.isSuccess)
        {
            _notyfService.Success(result.Message);
            return RedirectToAction(nameof(Index));
        }
        
        _notyfService.Error(result.Message);
        
        Response<List<PermissionsForRoleDTO>> permissionsResponse2 = await _rolesService.GetPermissionsAsync();
        if (!permissionsResponse2.isSuccess)
        {
            _notyfService.Error(permissionsResponse2.Message);
            return RedirectToAction(nameof(Index));
        }
        dto.Permissions = permissionsResponse2.Result;
        return View(dto);
    }
}