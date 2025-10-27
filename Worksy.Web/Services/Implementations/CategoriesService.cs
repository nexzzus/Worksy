using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class CategoriesService : ICategoriesService
{
    private readonly DataContext _context;
    
    public CategoriesService(DataContext context)
    {
        _context = context;
    }
    
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

    public async Task<Response<List<CategoryDTO>>> GetAllAsync()
    {
        try
        {
            var dtos = await _context.Categories
                .Include(c => c.Services)
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    Services = c.Services != null
                        ? c.Services.Select(s => new ServiceDTO
                        {
                            ServiceId = s.ServiceId,
                            Title = s.Title,
                            Description = s.Description,
                            Price = s.Price
                        }).ToList()
                        : new List<ServiceDTO>()
                })
                .ToListAsync();

            return new Response<List<CategoryDTO>>
            {
                isSuccess = true,
                Message = "Categorías obtenidas exitosamente.",
                Errors = null,
                Result = dtos
            };
        }
        catch (Exception ex)
        {
            return new Response<List<CategoryDTO>>
            {
                isSuccess = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
                Result = null
            };
        }
    }

    public Task<Response<CategoryDTO>> UpdateAsync(CategoryDTO dto)
    {
        throw new NotImplementedException();
    }
}