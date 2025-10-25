using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
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

        // ===================== AUTH / PROFILE EXISTENTES =====================

        // [HttpGet]
        // public IActionResult Register() => View();

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Por favor completa los campos correctamente.");
                return View("Register", model);
            }

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Email), "El correo ya está registrado.");
                _notyf.Error("El correo ya está registrado.");
                return View("Register", model);
            }
            

            
            var userDto = new UserDTO
            {
                FirstName   = model.FirstName?.Trim() ?? string.Empty,
                LastName    = model.LastName?.Trim() ?? string.Empty,
                Email       = model.Email?.Trim() ?? string.Empty,
                Password    = model.Password,
                PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim(),
                Address     = string.IsNullOrWhiteSpace(model.Address) ? string.Empty : model.Address.Trim(),
                Biography   = null
            };

            var user = new User
            {
                FirstName   = userDto.FirstName, // required -> nunca null
                LastName    = userDto.LastName,  // required -> nunca null
                Address     = userDto.Address,   // required -> nunca null
                Email       = userDto.Email,
                UserName    = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
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
                catch
                {
                    _notyf.Error("No se pudo enviar el correo de bienvenida. Verifica la configuración SMTP.");
                }

                _notyf.Success("Cuenta creada correctamente. ¡Bienvenido!");
                await _signInManager.SignInAsync(user, isPersistent: false);
                _notyf.Success("Registro exitoso. ¡Bienvenido!");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            _notyf.Error("No se pudo registrar el usuario");
            return View("Register", model);
        }
*/
   /*

    [HttpGet]
    public IActionResult Login() => View();
    
*/
   /*
        [HttpPost]
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
*/
   
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Logout()
        // {
        //     await _signInManager.SignOutAsync();
        //     _notyf.Success("Sesión cerrada correctamente.");
        //     return RedirectToAction("Index", "Home");
        // }

        // [HttpGet]
        // public IActionResult AccessDenied() => View();

        /*
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
        }*/

        /*
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
        }*/


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
        

        // [HttpGet]
        // public IActionResult ForgotPassword() => View();

        /*
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
        */

        // [HttpGet]
        // public IActionResult ResetPassword(string token, string email)
        // {
        //     if (token is null || email is null)
        //     {
        //         return RedirectToAction("Login");
        //     }
        //
        //     var model = new ResetPasswordViewModel { Token = token, Email = email };
        //     return View(model);
        // }

        /*
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
        */
        // ===================== USERS TABLE / CRUD =====================

        // LISTA con búsqueda y paginación
        [HttpGet]
        public async Task<IActionResult> UsersTable(int page = 1, int pageSize = 10, string? q = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(u =>
                    (u.Email != null       && u.Email.ToLower().Contains(term)) ||
                    (u.UserName != null    && u.UserName.ToLower().Contains(term)) ||
                    (u.FirstName != null   && u.FirstName.ToLower().Contains(term)) ||
                    (u.LastName != null    && u.LastName.ToLower().Contains(term)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(term)) ||
                    (u.Address != null     && u.Address.ToLower().Contains(term))
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
        [HttpGet]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            return View("Table/UserDetails", user);
        }

        // CREATE
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View("Table/CreateUser", new User()
            {
                FirstName = string.Empty,
                LastName  = string.Empty,
                Address   = string.Empty,
                WorksyRoleId = Guid.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User formModel, string password)
        {
            if (string.IsNullOrWhiteSpace(formModel.Email))
                ModelState.AddModelError(nameof(formModel.Email), "El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                ModelState.AddModelError("Password", "La contraseña es obligatoria y debe tener al menos 6 caracteres.");

            if (!ModelState.IsValid)
            {
                // asegúrate de no devolver null en required
                formModel.FirstName ??= string.Empty;
                formModel.LastName  ??= string.Empty;
                formModel.Address   ??= string.Empty;
                return View(formModel);
            }

            var existing = await _userManager.FindByEmailAsync(formModel.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(formModel.Email), "El correo ya está registrado.");
                _notyf?.Error("El correo ya está registrado.");
                formModel.FirstName ??= string.Empty;
                formModel.LastName  ??= string.Empty;
                formModel.Address   ??= string.Empty;
                return View(formModel);
            }

            var user = new User
            {
                FirstName   = (formModel.FirstName ?? string.Empty).Trim(),
                LastName    = (formModel.LastName  ?? string.Empty).Trim(),
                Address     = (formModel.Address   ?? string.Empty).Trim(),

                Email       = formModel.Email?.Trim(),
                UserName    = formModel.Email?.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(formModel.PhoneNumber) ? null : formModel.PhoneNumber.Trim(),
                Biography   = string.IsNullOrWhiteSpace(formModel.Biography)   ? null : formModel.Biography.Trim(),
                
                WorksyRoleId = Guid.Empty
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                _notyf?.Error("No se pudo crear el usuario.");
                // devuelve con required llenos
                user.FirstName ??= string.Empty;
                user.LastName  ??= string.Empty;
                user.Address   ??= string.Empty;
                return View("Table/UsersTable",user);
            }

            _notyf?.Success("Usuario creado correctamente.");
            return RedirectToAction(nameof(UsersTable));
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> EditUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            return View("Table/EditUser",user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Guid id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,Biography")] User model)
        {
            if (id != model.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View("Table/EditUser",model);

            // required no null
            user.FirstName = (model.FirstName ?? string.Empty).Trim();
            user.LastName  = (model.LastName  ?? string.Empty).Trim();
            user.Address   = (model.Address   ?? string.Empty).Trim();

            // si cambia el correo, alinear UserName/Normalized*
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = model.Email?.Trim();
                user.UserName = model.Email?.Trim();
                user.NormalizedEmail = user.Email?.ToUpperInvariant();
                user.NormalizedUserName = user.UserName?.ToUpperInvariant();
            }

            user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();
            user.Biography   = string.IsNullOrWhiteSpace(model.Biography)   ? null : model.Biography.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                _notyf?.Error("No se pudo guardar el usuario.");
                return View("Table/EditUser",model);
            }

            _notyf?.Success("Usuario actualizado.");
            return RedirectToAction(nameof(UsersTable));
        }

        // DELETE
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

                _notyf?.Error("No se pudo eliminar el usuario.");
                return RedirectToAction(nameof(UsersTable));
            }

            _notyf?.Success("Usuario eliminado.");
            return RedirectToAction(nameof(UsersTable));
        }
    }
}
