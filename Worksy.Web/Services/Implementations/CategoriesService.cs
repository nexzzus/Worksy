using Worksy.Web.Core;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class CategoriesService : ICategoriesService
{
    public Task<Response<CategoryDTO>> CreateAsync(CategoryDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<Response<object>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<CategoryDTO>> GetOneAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<CategoryDTO>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Response<CategoryDTO>> UpdateAsync(CategoryDTO dto)
    {
        throw new NotImplementedException();
    }
}