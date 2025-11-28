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

    public async Task<Response<CategoryDTO>> CreateAsync(CategoryDTO dto)
    {
        try
        {
            Category category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            dto.Id = category.Id;

            return new Response<CategoryDTO>
            {
                isSuccess = true,
                Message = "Categoría creada exitosamente.",
                Errors = null,
                Result = dto
            };
        }
        catch (Exception ex)
        {
            return new Response<CategoryDTO>
            {
                isSuccess = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
                Result = null
            };
        }
    }

    public async Task<Response<object>> DeleteAsync(Guid id)
    {
        try
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return new Response<object>
                {
                    isSuccess = false,
                    Message = "Categoría no encontrada.",
                    Errors = new List<string> { "Categoría no encontrada." },
                    Result = null
                };
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return new Response<object>
            {
                isSuccess = true,
                Message = "Categoría eliminada exitosamente.",
                Errors = null,
                Result = null
            };
        }
        catch (Exception ex)
        {
            return new Response<object>
            {
                isSuccess = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
                Result = null
            };
        }
    }

    public async Task<Response<CategoryDTO>> GetOneAsync(Guid id)
    {
        try
        {
            Category? category = await _context.Categories
                .Include(c => c.Services)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
            {
                return Response<CategoryDTO>.Failure("Categoría no encontrada.");
            }

            CategoryDTO dto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Services = category.Services != null
                ? category.Services.Select(s => new ServiceDTO
                {
                   Id = s.Id,
                   Title = s.Title,
                   Description = s.Description,
                   Price = s.Price
                }).ToList()
                : new List<ServiceDTO>()
            };

            return Response<CategoryDTO>.Success(dto, "Categoría obtenida exitosamente.");
        }
        catch (Exception ex)
        {
            return Response<CategoryDTO>.Failure(ex, "No se pudo obtener la categoría.");
        }
    }

    public async Task<Response<List<CategoryDTO>>> GetAllAsync()
    {
        try
        {
            var dtos = await _context.Categories
                .Include(c => c.Services)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Services = c.Services != null
                        ? c.Services.Select(s => new ServiceDTO
                        {
                            Id = s.Id,
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

    public async Task<Response<CategoryDTO>> UpdateAsync(CategoryDTO dto)
    {
        try
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (category == null)
            {
                return new Response<CategoryDTO>
                {
                    isSuccess = false,
                    Message = "Categoría no encontrada.",
                };
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new Response<CategoryDTO>
            {
                isSuccess = true,
                Message = "Categoría actualizada exitosamente.",
                Errors = null,
                Result = dto
            };
        }
        catch (Exception ex)
        {
            return new Response<CategoryDTO>
            {
                isSuccess = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
                Result = null
            };
        }
    }
}