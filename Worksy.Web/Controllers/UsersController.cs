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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Complete los campos requeridos");
                return View(dto);
            }

            var user = _mapper.Map<User>(dto);
            user.UserName = dto.Email;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _notyf.Success("Registro exitoso. ¡Bienvenido!");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                var Error = error.Code;

                switch (Error)
                {
                    case "DuplicateEmail":
                        ModelState.AddModelError(nameof(dto.Email), "El correo ya está en uso.");
                        _notyf.Error("El correo ya está en uso.");
                        break;
                    case "PasswordTooShort":
                        ModelState.AddModelError(nameof(dto.Password), "La contraseña es demasiado corta.");
                        _notyf.Error("La contraseña es demasiado corta.");
                        break;
                    case "PasswordRequiresDigit":
                        ModelState.AddModelError(nameof(dto.Password), "La contraseña debe contener al menos un dígito.");
                        _notyf.Error("La contraseña debe contener al menos un dígito.");
                        break;
                    case "PasswordRequiresUpper":
                        ModelState.AddModelError(nameof(dto.Password), "La contraseña debe contener al menos una mayúscula.");
                        _notyf.Error("La contraseña debe contener al menos una mayúscula");
                        break;
                    default:
                        _notyf.Error("" + error.Description);
                        break;
                }
            }
            
            
            return View(dto);
        }

        [HttpGet]
            public IActionResult Login(string? returnUrl = null)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(LoginViewModel viewModel, string? returnUrl = null)
            {
                if (!ModelState.IsValid)
                {
                    _notyf.Error("Complete todos los campos");
                    return View(viewModel);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    viewModel.Email,
                    viewModel.Password,
                    viewModel.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    _notyf.Success("Inicio de sesión exitoso.");
                    return RedirectToAction("Index", "Home");
                }

                _notyf.Error("Credenciales inválidas");
                return View(viewModel);
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
                    return View("Profile", dto);
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