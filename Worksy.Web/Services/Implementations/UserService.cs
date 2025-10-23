using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Core.Abstractions;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;
using Worksy.Web.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Worksy.Web.Services.Implementations;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;

    public UserService(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager,
        IMapper mapper, IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _emailSender = emailSender;
    }

    public async Task<Response<IdentityResult>> AddUserAsync(RegisterViewModel model, string password)
    {
        User user = _mapper.Map<User>(model);
        user.UserName = model.Email;

        IdentityResult result = await _userManager.CreateAsync(user, password);

        return new Response<IdentityResult>
        {
            Result = result,
            isSuccess = result.Succeeded,
            IErrors = result.Errors
        };
    }


    public async Task<Response<SignInResult>> LoginAsync(LoginViewModel model)
    {
        SignInResult result =
            await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        return new Response<SignInResult>
        {
            Result = result,
            isSuccess = result.Succeeded
        };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public Task<Response<IdentityResult>> ConfirmUserAsync(UserDTO dto, string token)
    {
        throw new NotImplementedException();
    }

    public Task<Response<string>> GenerateConfirmationTokenAsync(UserDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<Response<object>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<IdentityResult>> GetOneAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<IdentityResult>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Response<UpdateProfileDTO>> UpdateAsync(UpdateProfileDTO dto)
    {
        try
        {
            User user = await GetUserAsync(dto.Id);
            user.PhoneNumber = dto.PhoneNumber;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Address = dto.Address;
            user.Biography = dto.Biography;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            
            return Response<UpdateProfileDTO>.Success(dto, "Datos actualizados correctamente");
        }
        catch (Exception e)
        {
            return Response<UpdateProfileDTO>.Failure(e);
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Response<object>> ForgotPasswordAsync(string email, IUrlHelper url, string scheme)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Response<object>.Failure("Usuario no encontrado");
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = url.Action("ResetPassword", "Account", new { token, email = user.Email }, scheme);
        await _emailSender.SendEmailAsync(
            user.Email,
            "Restablecer contraseña",
            $"Haga clic <a href='{resetLink}'>aquí</a> para restablecer su contraseña"
        );
        
        return Response<object>.Success(null, "Se envió un correo de recuperación al correo indicado");
    }

    public async Task<Response<object>> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        User? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            return Response<object>.Failure("Ocurrió un error al restablecer la contraseña.");
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            return Response<object>.Failure("Ocurrió un error al restablecer la contraseña.");
        }
        return Response<object>.Success(null, "Contraseña restablecida exitosamente.");
    }


    public async Task<User> GetUserAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

}