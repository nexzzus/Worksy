using Microsoft.AspNetCore.Identity;
using Worksy.Web.Core;
using Worksy.Web.DTOs;
using Worksy.Web.ViewModels;

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
    public Task<Response<IdentityResult>> UpdateAsync(UserDTO user);
}