using Worksy.Web.Core;
using Worksy.Web.Core.Pagination;
using Worksy.Web.DTOs;

namespace Worksy.Web.Services.Abstractions;

public interface IRolesService
{
    Task<Response<WorksyRoleDTO>> CreateAync(WorksyRoleDTO dto);
    Task<Response<object>> DeleteAsync(Guid id);
    Task<Response<WorksyRoleDTO>> EditAsync(WorksyRoleDTO dto);
    Task<Response<WorksyRoleDTO>> GetOneAsync(Guid id);
    Task<Response<PaginationResponse<WorksyRoleDTO>>> GetPaginatedListAsync(PaginationRequest request);
    Task<Response<List<PermissionsForRoleDTO>>> GetPermissionsAsync();
}