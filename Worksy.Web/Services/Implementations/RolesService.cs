using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Worksy.Web.Core;
using Worksy.Web.Core.Pagination;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Services.Implementations;

public class RolesService : CustomQueryableOperationsService, IRolesService
{
    public RolesService(DataContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Response<WorksyRoleDTO>> CreateAync(WorksyRoleDTO dto)
    {
        using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                Guid newRoleId = Guid.NewGuid();

                // Role
                WorksyRole role = _mapper.Map<WorksyRole>(dto);

                await _context.WorksyRoles.AddAsync(role);

                await _context.SaveChangesAsync();

                // Permissions
                List<Guid> permissionsIds = new();

                if (!string.IsNullOrEmpty(dto.PermissionsIds))
                {
                    permissionsIds = JsonConvert.DeserializeObject<List<Guid>>(dto.PermissionsIds);
                }

                foreach (Guid permissionId in permissionsIds)
                {
                    RolePermission rolePermission = new RolePermission
                    {
                        WorksyRoleId = role.Id,
                        PermissionId = permissionId
                    };
                    await _context.RolePermissions.AddAsync(rolePermission);
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return Response<WorksyRoleDTO>.Success(dto, "Rol creado con éxito");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Response<WorksyRoleDTO>.Failure(e);
            }
        }
    }

    public async Task<Response<object>> DeleteAsync(Guid id)
    {
        return await DeleteAsync<WorksyRole>(id);
    }

    public async Task<Response<WorksyRoleDTO>> EditAsync(WorksyRoleDTO dto)
    {
        try
        {
            if (dto.Name == Env.ROLE_ADMIN)
            {
                return Response<WorksyRoleDTO>.Failure($"El rol '{Env.ROLE_ADMIN}' no puede ser editado");
            }
            

            // Role
            WorksyRole role = _mapper.Map<WorksyRole>(dto);

            _context.WorksyRoles.Update(role);

            await _context.SaveChangesAsync();

            // Permissions
            List<Guid> permissionsIds = new();

            if (!string.IsNullOrEmpty(dto.PermissionsIds))
            {
                permissionsIds = JsonConvert.DeserializeObject<List<Guid>>(dto.PermissionsIds);
            }

            // Delete old
            List<RolePermission> oldRolePermission =
                await _context.RolePermissions.Where(rp => rp.WorksyRoleId == dto.Id).ToListAsync();
            _context.RolePermissions.RemoveRange(oldRolePermission);
            
            // Create new
            foreach (Guid permissionId in permissionsIds)
            {
                RolePermission rolePermission = new RolePermission()
                {
                    WorksyRoleId = role.Id,
                    PermissionId = permissionId
                };
                await _context.RolePermissions.AddAsync(rolePermission);
            }

            await _context.SaveChangesAsync();

            return Response<WorksyRoleDTO>.Success(dto, "Rol actualizado con éxito");
        }
        catch (Exception e)
        {
            return Response<WorksyRoleDTO>.Failure(e);
        }
    }

    public async Task<Response<WorksyRoleDTO>> GetOneAsync(Guid id)
    {
        Response<WorksyRoleDTO> respose = await GetOneAsync<WorksyRole, WorksyRoleDTO>(id);
        if (!respose.isSuccess)
        {
            return respose;
        }

        WorksyRoleDTO dto = respose.Result;

        List<PermissionsForRoleDTO> permissions = await _context.Permissions.Select(p =>
            new PermissionsForRoleDTO()
            {
                Id = p.Id,
                Description = p.Description,
                Module = p.Module,
                Selected = _context.RolePermissions.Any(rp =>
                    rp.PermissionId == p.Id && rp.WorksyRoleId == respose.Result.Id)
            }).ToListAsync();

        dto.Permissions = permissions;
        return Response<WorksyRoleDTO>.Success(dto, "Rol obtenido con éxito");
    }

    public async Task<Response<PaginationResponse<WorksyRoleDTO>>> GetPaginatedListAsync(PaginationRequest request)
    {
        IQueryable<WorksyRole> query = _context.WorksyRoles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Filter))
        {
            query = query.Where(r => r.Name.ToLower().Contains(request.Filter.ToLower()));
        }

        return await GetPaginationAsync<WorksyRole, WorksyRoleDTO>(request, query);
    }

    public async Task<Response<List<PermissionsForRoleDTO>>> GetPermissionsAsync()
    {
        Response<List<PermissionDTO>> permissions = await GetAllAsync<Permission, PermissionDTO>();

        if (!permissions.isSuccess)
        {
            return Response<List<PermissionsForRoleDTO>>.Failure(permissions.Message);
        }

        List<PermissionsForRoleDTO> dto = permissions.Result.Select(p => new PermissionsForRoleDTO()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Module = p.Module,
            Selected = false
        }).ToList();
        return Response<List<PermissionsForRoleDTO>>.Success(dto);
    }
}