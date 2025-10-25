using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Core;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Worksy.Web.Services.Abstractions;

public interface IUserService
{
    public Task<Response<IdentityResult>> AddUserAsync(RegisterViewModel dto, string password);
    public Task<Response<SignInResult>> LoginAsync(LoginViewModel model);
    public Task LogoutAsync();

    public Task<Response<IdentityResult>> ConfirmUserAsync(UserDTO dto, string token);
    public Task<Response<string>> GenerateConfirmationTokenAsync(UserDTO dto);

    public Task<Response<object>> DeleteAsync(Guid id);
    public Task<Response<IdentityResult>> GetOneAsync(Guid id);
    public Task<Response<List<IdentityResult>>> GetAllAsync();
    public Task<Response<UpdateProfileDTO>> UpdateAsync(UpdateProfileDTO dto);
    public Task<User?> GetByEmailAsync(string email);
    public Task<Response<object>> ForgotPasswordAsync(string email, IUrlHelper url, string scheme);
    public Task<Response<object>> ResetPasswordAsync(ResetPasswordViewModel model);
}