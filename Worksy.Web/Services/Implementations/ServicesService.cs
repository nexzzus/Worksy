using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Core.Pagination;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations
{
    public class ServicesService : CustomQueryableOperationsService, IServicesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicesService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<List<ServiceDTO>>> GetAllAsync()
        {
            try
            {
                // Incluir categorías para mapearlas en DTO
                List<Service> services = await _context.Services
                    .Include(s=> s.User)
                    .Include(s => s.Categories)
                    .ToListAsync();

                List<ServiceDTO> dtos = _mapper.Map<List<ServiceDTO>>(services);

                return Response<List<ServiceDTO>>.Success(dtos, "Servicios obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return new Response<List<ServiceDTO>>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message },
                    Result = null
                };
            }
        }

        public async Task<Response<ServiceDTO>> GetOneAsync(Guid id)
        {
            try
            {
                Service? service = await _context.Services
                    .Include(s => s.Categories)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (service == null)
                {
                    return new Response<ServiceDTO>
                    {
                        isSuccess = false,
                        Message = $"El servicio de id '{id}' no existe.",
                    };
                }

                ServiceDTO dto = new ServiceDTO
                {
                    Id = service.Id,
                    Title = service.Title,
                    Description = service.Description,
                    Price = service.Price,
                    Categories = service.Categories?.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList(),
                    CategoryIds = service.Categories?.Select(c => c.Id).ToList()
                };

                return new Response<ServiceDTO>
                {
                    isSuccess = true,
                    Message = "Servicio obtenido exitosamente.",
                    Errors = null,
                    Result = dto
                };
            }
            catch (Exception ex)
            {
                return new Response<ServiceDTO>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message },
                    Result = null
                };
            }
        }
        
        public Guid GetCurrentUserId()
        {
            string? id = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return id != null ? Guid.Parse(id) : Guid.Empty;
        }

        public async Task<Response<PaginationResponse<ServiceDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            IQueryable<Service> query = _context.Services
                .Include(s=> s.Categories)
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(r => 
                    r.Title.ToLower().Contains(request.Filter.ToLower()) ||
                    r.Description.ToLower().Contains(request.Filter.ToLower())
                    );
            }

            return await GetPaginationAsync<Service, ServiceDTO>(request, query);
        }

        public async Task<Response<ServiceDTO>> CreateAsync(ServiceDTO dto, Guid providerId)
        {
            try
            {
                
                Service service = _mapper.Map<Service>(dto);
                // Guid userId = GetCurrentUserId();
                // if (userId == Guid.Empty)
                // {
                //     return Response<ServiceDTO>.Failure("No se pudo obtener el ID del usuario actual.");
                // }
                service.UserId = providerId;
                // Service service = new Service
                // {
                //     ServiceId = Guid.NewGuid(),
                //     Title = dto.Title,
                //     Description = dto.Description,
                //     Price = dto.Price,
                //     
                // };

                // Asignar categorías si vienen IDs
                if (dto.CategoryIds != null && dto.CategoryIds.Any())
                {
                    var cats = await _context.Categories
                        .Where(c => dto.CategoryIds.Contains(c.Id))
                        .ToListAsync();

                    service.Categories = cats;
                }
                
                service.Id = Guid.NewGuid();
                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();
                dto.Id = service.Id;

                // devolver categorías mapeadas en DTO
                dto.Categories = service.Categories?.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList();

                return Response<ServiceDTO>.Success(dto, "Servicio creado exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<ServiceDTO>.Failure(ex);
            }
        }

        public async Task<Response<ServiceDTO>> UpdateAsync(ServiceDTO dto)
        {
            try
            {
                Service? service = await _context.Services
                    .Include(s => s.Categories)
                    .FirstOrDefaultAsync(s => s.Id == dto.Id);

                if (service == null)
                {
                    return new Response<ServiceDTO>
                    {
                        isSuccess = false,
                        Message = $"El servicio de id '{dto.Id}' no existe.",
                    };
                }

                service.Title = dto.Title;
                service.Description = dto.Description;
                service.Price = dto.Price;

                // Actualizar categorías: reemplazar asociaciones
                if (dto.CategoryIds != null)
                {
                    var cats = await _context.Categories
                        .Where(c => dto.CategoryIds.Contains(c.Id))
                        .ToListAsync();

                    // Reemplazar colección
                    service.Categories = cats;
                }
                else
                {
                    // Si no se envía nada, limpiar asociaciones
                    service.Categories = new List<Category>();
                }

                _context.Services.Update(service);
                await _context.SaveChangesAsync();

                // Retornar DTO actualizado
                dto.Categories = service.Categories?.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList();

                return new Response<ServiceDTO>
                {
                    isSuccess = true,
                    Message = "Servicio actualizado exitosamente.",
                    Errors = null,
                    Result = dto
                };
            }
            catch (Exception ex)
            {
                return new Response<ServiceDTO>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message },
                    Result = null
                };
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid ServiceId)
        {
            try
            {
                Service? service = await _context.Services.FirstOrDefaultAsync(s => s.Id == ServiceId);

                if (service == null)
                {
                    return new Response<object>
                    {
                        isSuccess = false,
                        Message = $"El servicio de id '{ServiceId}' no existe.",
                    };
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                return new Response<object>
                {
                    isSuccess = true,
                    Message = "Servicio eliminado exitosamente.",
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

        // Nuevo: obtener todas las categorías para poblar formularios
        public async Task<Response<List<CategoryDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var cats = await _context.Categories.ToListAsync();
                var dtos = cats.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList();

                return Response<List<CategoryDTO>>.Success(dtos, "Servicios obtenidos con exito.");
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

        public async Task<Response<IEnumerable<ServiceDTO>>> GetServicesByProviderAsync(Guid providerId)
        {
            var services = await _context.Services
                .Where(s => s.UserId == providerId)
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Price = s.Price,
                    CategoryIds = s.Categories.Select(c => c.Id).ToList()
                })
                .ToListAsync();

            if (!services.Any())
            {
                return Response<IEnumerable<ServiceDTO>>.Failure(
                    "No se encontraron servicios para el proveedor especificado.");
            }

            return Response<IEnumerable<ServiceDTO>>.Success(services, "Servicios obtenidos con éxito.");
        }
    }
}