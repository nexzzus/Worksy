using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core.Abstractions;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
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

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper,
            INotyfService notyf, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _notyf = notyf;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult UsersTable() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                return View(dto);
            }

            var user = await _userManager.GetUserAsync(User);
            _mapper.Map(dto, user);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _notyf.Success("Perfil actualizado exitosamente.");
                return RedirectToAction("Profile");
            }

            _notyf.Error("Error al actualizar el perfil. Intente nuevamente.");
            return View("Profile", dto);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                return View(dto);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _notyf.Success("Contraseña actualizada exitosamente.");
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code.Equals("PasswordMismatch"))
                {
                    ModelState.AddModelError(nameof(dto.OldPassword), "La contraseña actual es incorrecta.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _notyf.Error("" + error.Description);
                }
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login");
            }

            var dto = _mapper.Map<UpdateProfileDTO>(user);
            return View(dto);
        }
    }
}