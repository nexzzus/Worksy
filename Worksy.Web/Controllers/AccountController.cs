using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Core.Abstractions;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;
using Worksy.Web.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Worksy.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly INotyfService _notyf;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public AccountController(IUserService userService, IEmailSender emailSender, INotyfService notyf, IMapper mapper)
    {
        _userService = userService;
        _emailSender = emailSender;
        _notyf = notyf;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _notyf.Error("Complete los campos requeridos");
            return View(model);
        }

        Response<IdentityResult> result = await _userService.AddUserAsync(model, model.Password);

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
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            _notyf.Error("Complete los campos requeridos");
            return View(model);
        }

        Response<SignInResult> result = await _userService.LoginAsync(model);

        if (!result.isSuccess)
        {
            _notyf.Error("Credenciales incorrectas. Intente nuevamente.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        _notyf.Success("Inicio de sesión exitoso. ¡Bienvenido de nuevo!");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync();
        _notyf.Success("Cierre de sesión exitoso.");
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _notyf.Error("Complete lo campo requerido");
            return View(model);
        }

        var result = await _userService.ForgotPasswordAsync(model.Email, Url, Request.Scheme);

        if (!result.isSuccess)
        {
            _notyf.Error(result.Message);
            return View();
        }

        _notyf.Error("Se envió un correo de recuperación al correo indicado");
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _notyf.Error("Complete los campos requeridos");
            return View(model);
        }
        
        var result = await _userService.ResetPasswordAsync(model);

        if (!result.isSuccess)
        {
            _notyf.Error(result.Message);
            return RedirectToAction("Login");
        }

        _notyf.Success(result.Message);
        return RedirectToAction(nameof(Login));
    }
    
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        User? user = await _userService.GetByEmailAsync(User.Identity.Name);
        if (user is null)
        {
            return NotFound();
        }

        UpdateProfileDTO dto = _mapper.Map<UpdateProfileDTO>(user);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
    {
        if (ModelState.IsValid)
        {
            Response<UpdateProfileDTO> result = await _userService.UpdateAsync(dto);
            if (result.isSuccess)
            {
                _notyf.Success(result.Message);
                return RedirectToAction("Profile");
            }
            else
            {
                Console.WriteLine("-------------------");
                _notyf.Error(result.Message);
            }
            return RedirectToAction("Index", "Home");
        }
        _notyf.Error("Complete los campos requeridos");
        return View("Profile", dto);
    }
}