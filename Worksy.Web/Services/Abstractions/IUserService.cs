using Worksy.Web.Core;
using Worksy.Web.DTOs;

namespace Worksy.Web.Services.Abstractions;

public interface IUserService
{
    public Task<Response<UserDTO>> CreateAsync(UserDTO user);
    public Task<Response<object>> DeleteAsync(Guid id);
    public Task<Response<UserDTO>> GetOneAsync(Guid id);
    public Task<Response<List<UserDTO>>> GetAllAsync();
    public Task<Response<UserDTO>> UpdateAsync(UserDTO user);
}