using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
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
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult UsersTable() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Por favor completa los campos correctamente.");
                return View("Register", model); // <-- antes devolvías "Login"
            }

            // (Opcional pero útil) Validación previa de email duplicado
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Email), "El correo ya está registrado.");
                _notyf.Error("El correo ya está registrado.");
                return View("Register", model);
            }

            // Mapeo manual VM -> DTO (evitamos AutoMapper aquí)
            var userDto = new UserDTO
            {
                FirstName = model.FirstName?.Trim() ?? string.Empty,
                LastName = model.LastName?.Trim() ?? string.Empty,
                Email = model.Email?.Trim() ?? string.Empty,
                Password = model.Password, // Identity se encarga del hash
                PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim(),
                // Si tu UserDTO requiere Address, asegúrate de que no llegue null
                Address = string.IsNullOrWhiteSpace(model.Address) ? string.Empty : model.Address.Trim(),
                Biography = null
            };

            // DTO -> Entidad User (sin AutoMapper)
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.Email, // necesario para Identity
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Bienvenido a Worksy",
                        $"Hola {user.FirstName}, tu cuenta ha sido creada exitosamente."
                    );
                }
                catch (Exception ex)
                {
                    // No interrumpas el flujo por un correo fallido
                    _notyf.Error("No se pudo enviar el correo de bienvenida. Verifica la configuración SMTP.");
                    // (Opcional) log si tienes ILogger<HomeController> disponible en este controller
                    // _logger.LogError(ex, "Fallo enviando email de bienvenida");
                }

                _notyf.Success("Cuenta creada correctamente. ¡Bienvenido!");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }


            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            _notyf.Error("No se pudo registrar el usuario");
            return View("Register", model); // <-- antes devolvías "Login"
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Por favor completa los campos correctamente.");
                return View("Login", model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _notyf.Success("Inicio de sesión exitoso");
                return RedirectToAction("Index", "Home");
            }

            _notyf.Error("Correo o contraseña incorrectos");
            return View("Login", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notyf.Success("Sesión cerrada correctamente.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

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
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete el campo requerido");
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _notyf.Success("Se envió un correo de recuperación al correo indicado");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Users", new
            {
                token,
                email = user.Email
            }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Recuperar contraseña",
                $"Haga clic <a href='{resetLink}'>aquí</a> para restablecer su contraseña");

            _notyf.Success("Se envió un correo de recuperación al correo indicado");
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token is null || email is null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _notyf.Error("Error al restablecer la contraseña");
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (result.Succeeded)
            {
                _notyf.Success("Contraseña restablecida exitosamente");
                return RedirectToAction("Login");
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _notyf.Error("" + error.Description);
            }

            return View(dto);
        }
    }
}