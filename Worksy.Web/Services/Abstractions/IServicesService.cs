using Worksy.Web.Core;
using Worksy.Web.DTOs;

namespace Worksy.Web.Services.Abstractions
{
    public interface IServicesService
    {
        public Task<Response<ServiceDTO>> CreateAsync(ServiceDTO dto);
        public Task<Response<object>> DeleteAsync(Guid id);
        public Task<Response<ServiceDTO>> GetOneAsync(Guid id);
        public Task<Response<List<ServiceDTO>>> GetAllAsync();
        public Task<Response<ServiceDTO>> UpdateAsync(ServiceDTO dto);
    }
}
