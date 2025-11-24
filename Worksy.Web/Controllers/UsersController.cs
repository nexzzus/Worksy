using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Worksy.Web.Core;
using Worksy.Web.Core.Abstractions;
using Worksy.Web.Core.Attributes;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Herpers.Abstractions;
using Worksy.Web.Services.Abstractions;
using Worksy.Web.ViewModels;

namespace Worksy.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly ICombosHelper _combosHelper;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper _mapper,
            INotyfService notyf, IEmailSender emailSender, IUserService userService, ICombosHelper combosHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._mapper = _mapper;
            _notyf = notyf;
            _emailSender = emailSender;
            _userService = userService;
            _combosHelper = combosHelper;
        }

        [HttpGet]
        public IActionResult Register(string? type)
        {
            ViewBag.Type = type;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? type)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                return View(model);
            }

            Response<IdentityResult> result;
            if (type == "collab")
            {
                result = await _userService.AddCollabAsync(model, model.Password);
            }
            else
            {
                result = await _userService.AddUserAsync(model, model.Password);
            }


            if (!result.isSuccess)
            {
                _notyf.Error("Ocurrió un error durante el registro, inténtelo nuevamente.");
                return View(model);
            }

            await _emailSender.SendEmailAsync(
                model.Email,
                "Bienvenido a Worksy",
                $"Hola {model.FirstName}, tu cuenta ha sido creada exitosamente."
            );

            _notyf.Success("Registro exitoso. ¡Bienvenido!");
            return RedirectToAction("Login", "Account");
        }

        // ===================== USERS TABLE / CRUD =====================

        // LISTA con búsqueda y paginación
        [HttpGet]
        [CustomAuthorize(permission: "user.showAll", module: "Users")]
        public async Task<IActionResult> UsersTable(int page = 1, int pageSize = 10, string? q = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _userManager.Users
                .Include(u => u.WorksyRole)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(term)) ||
                    (u.Address != null && u.Address.ToLower().Contains(term)) ||
                    (u.WorksyRole.Name != null && u.WorksyRole.Name.ToLower().Contains(term))
                );
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.Q = q;

            return View("Table/UsersTable", users);
        }


        // DETALLE
        [CustomAuthorize("user.show", "Users")]
        [HttpGet]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            return View("Table/UserDetails", user);
        }

        // CREATE
        [CustomAuthorize("user.create", "Users")]
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            return View("Table/CreateUser", new RegisterViewModel()
            {
                Roles = await _combosHelper.GetComboRoles()
            });
        }

        [CustomAuthorize("user.create", "Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                model.Roles = await _combosHelper.GetComboRoles();
                return View("Table/CreateUser", model);
            }

            var result = await _userService.AddUserAsync(model, model.Password);
            if (!result.isSuccess)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }

                _notyf.Error("No se pudo crear el usuario.");
                return View("Table/UsersTable", model);
            }

            _notyf.Success("Usuario creado correctamente.");
            return RedirectToAction(nameof(UsersTable));
        }

        // EDIT
        [CustomAuthorize("user.update", "Usuarios")]
        [HttpGet]
        public async Task<IActionResult> EditUser(Guid id)
        {
            User? user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            UpdateUserAdmin dto = _mapper.Map<UpdateUserAdmin>(user);

            dto.WorksyRoleId = user.WorksyRoleId;
            dto.Roles = await _combosHelper.GetComboRoles();

            return View("Table/EditUser", dto);
        }

        [CustomAuthorize("user.update", "Usuarios")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(
            [Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,Biography,WorksyRoleId")]
            UpdateUserAdmin model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                model.Roles = await _combosHelper.GetComboRoles();
                return View("Table/EditUser", model);
            }

            User? user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.WorksyRole = model.WorksyRole;
            user.WorksyRoleId = model.WorksyRoleId;
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                _notyf.Error("No se pudo guardar el usuario.");
                model.Roles = await _combosHelper.GetComboRoles();
                return View("Table/EditUser", model);
            }

            _notyf.Success("Usuario actualizado.");
            return RedirectToAction(nameof(UsersTable));
        }

        // DELETE
        [CustomAuthorize("user.delete", "Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                _notyf.Error("No se pudo eliminar el usuario.");
                return RedirectToAction(nameof(UsersTable));
            }

            _notyf.Success("Usuario eliminado.");
            return RedirectToAction(nameof(UsersTable));
        }
    }
}