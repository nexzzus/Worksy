using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations
{
    public class ServicesService : IServicesService
    {
        private readonly DataContext _context;

        public ServicesService(DataContext context)
        {
            _context = context;
        }
        public async Task<Response<ServiceDTO>> CreateAsync(ServiceDTO dto)
        {
            try
            {
                Service service = new Service
                {
                    ServiceId = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price
                    
                };
                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();
                dto.ServiceId = service.ServiceId;

                return new Response<ServiceDTO>
                {
                    isSuccess = true,
                    Message = "Servicio creado exitosamente.",
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
                Service? service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == ServiceId);

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

        public async Task<Response<List<ServiceDTO>>> GetAllAsync()
        {
            try
            {
                List<Service> services = await _context.Services.ToListAsync();
                List<ServiceDTO> dtos = services.Select(service => new ServiceDTO
                {
                    ServiceId = service.ServiceId,
                    Title = service.Title,
                    Description = service.Description,
                    Price = service.Price
                }).ToList();
                return new Response<List<ServiceDTO>>
                {
                    isSuccess = true,
                    Message = "Servicios obtenidos exitosamente.",
                    Errors = null,
                    Result = dtos
                };
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
            try { 
                Service? service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
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
                    ServiceId = service.ServiceId,
                    Title = service.Title,
                    Description = service.Description,
                    Price = service.Price
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

        public async Task<Response<ServiceDTO>> UpdateAsync(ServiceDTO dto)
        {
            try
            {
                Service? service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == dto.ServiceId);

                if (service == null)
                {
                    return new Response<ServiceDTO>
                    {
                        isSuccess = false,
                        Message = $"El servicio de id '{dto.ServiceId}' no existe.",
                    };
                }
                service.Title = dto.Title;
                service.Description = dto.Description;
                service.Price = dto.Price;
                _context.Services.Update(service);
                await _context.SaveChangesAsync();

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
    }
}
