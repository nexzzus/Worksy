using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;

namespace Worksy.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // GET: /Users/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Users/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Users/Register
        [HttpPost]
        public async Task<IActionResult> Register(UserDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = _mapper.Map<User>(dto);
            user.UserName = dto.Email; // Identity necesita UserName

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(dto);
        }



        // GET: /Users/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Users/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas");
            return View(dto);
        }

        // POST: /Users/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Users/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
