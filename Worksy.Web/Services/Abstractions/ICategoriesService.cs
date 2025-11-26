using Worksy.Web.Core;
using Worksy.Web.DTOs;

namespace Worksy.Web.Services.Abstractions;

public interface ICategoriesService
{
    public Task<Response<CategoryDTO>> CreateAsync(CategoryDTO dto);
    public Task<Response<object>> DeleteAsync(Guid id);
    public Task<Response<CategoryDTO>> GetOneAsync(Guid id);
    public Task<Response<List<CategoryDTO>>> GetAllAsync();
    public Task<Response<CategoryDTO>> UpdateAsync(CategoryDTO dto);

}