/*using AutoMapper;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class UserService:CustomQueryableOperationService, IUserService
{
    public UserService(DataContext context, IMapper mapper) : base(context, mapper)
    {
        
    }

    public Task<Response<UserDTO>> CreateAsync(UserDTO user)
    {
        return CreateAsync<User, UserDTO>(user);
    }

    public Task<Response<object>> DeleteAsync(Guid id)
    {
        return DeleteAsync<User>(id);
    }

    public Task<Response<UserDTO>> GetOneAsync(Guid id)
    {
        return GetByIdAsync<User, UserDTO>(id);
    }

    public Task<Response<List<UserDTO>>> GetAllAsync()
    {
        return GetAllAsync<User, UserDTO>();
    }

    public Task<Response<UserDTO>> UpdateAsync(UserDTO user)
    {
        return UpdateAsync<User, UserDTO>(user, user.Id);
    }
}*/