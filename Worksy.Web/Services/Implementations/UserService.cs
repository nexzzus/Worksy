using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;
using Worksy.Web.ViewModels;

namespace Worksy.Web.Services.Implementations;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;

    public UserService(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<Response<IdentityResult>> AddUserAsync(UserDTO dto, string password)
    {
        User user = _mapper.Map<User>(dto);
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return new Response<IdentityResult>()
        {
            Result = result,
            isSuccess = result.Succeeded
        };
    }

    public async Task<Response<SignInResult>> LoginAsync(LoginViewModel model)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

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

    public Task<Response<IdentityResult>> UpdateAsync(UserDTO user)
    {
        throw new NotImplementedException();
    }
}