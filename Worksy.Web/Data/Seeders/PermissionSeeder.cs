using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Data.Seeders;

public class PermissionSeeder
{
    private readonly DataContext _context;

    public PermissionSeeder(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        List<Permission> permissions = [
            ..RolesPermissions(),
            ..Users(),
            ..Services(),
            ..Categories(),
            ..Valorations()
        ];

        foreach (var permission in permissions)
        {
            bool exists = await _context.Permissions.AnyAsync(p => p.Name == permission.Name);
            if (!exists)
            {
                await _context.Permissions.AddAsync(permission);
            }
        }

        await _context.SaveChangesAsync();
    }

    private List<Permission> RolesPermissions()
    {
        return new()
        {
            new Permission { Name = "rolesPermissions.create", Description = "Crear roles y permisos", Module = "RolesPermissions" },
            new Permission { Name = "rolesPermissions.update", Description = "Actualizar roles y permisos", Module = "RolesPermissions" },
            new Permission { Name = "rolesPermissions.delete", Description = "Eliminar roles y permisos", Module = "RolesPermissions" },
            new Permission { Name = "rolesPermissions.show", Description = "Ver roles y permisos", Module = "RolesPermissions" }
        };
    }

    private List<Permission> Services()
    {
        return new()
        {
            new Permission { Name = "service.show", Description = "Ver servicios", Module = "Services" },
            new Permission { Name = "service.showAll", Description = "Ver lista de servicios", Module = "Services" },
            new Permission { Name = "service.create", Description = "Crear servicios", Module = "Services" },
            new Permission { Name = "service.update", Description = "Actualizar servicios", Module = "Services" },
            new Permission { Name = "service.delete", Description = "Eliminar servicios", Module = "Services" }
        };
    }

    private List<Permission> Categories()
    {
        return new()
        {
            new Permission { Name = "category.show", Description = "Ver categorías", Module = "Categories" },
            new Permission { Name = "category.create", Description = "Crear categorías", Module = "Categories" },
            new Permission { Name = "category.update", Description = "Actualizar categorías", Module = "Categories" },
            new Permission { Name = "category.delete", Description = "Eliminar categorías", Module = "Categories" }
        };
    }

    private List<Permission> Users()
    {
        return new()
        {
            new Permission { Name = "user.showAll", Description = "Ver usuarios", Module = "Users" },
            new Permission { Name = "user.create", Description = "Crear usuarios", Module = "Users" },
            new Permission { Name = "user.update", Description = "Actualizar usuarios", Module = "Users" },
            new Permission { Name = "user.delete", Description = "Eliminar usuarios", Module = "Users" }
        };
    }

    private List<Permission> Valorations()
    {
        return new()
        {
            new Permission { Name = "valoration.show", Description = "Ver valoraciones", Module = "Valorations" },
            new Permission { Name = "valoration.create", Description = "Crear valoraciones", Module = "Valorations" },
            new Permission { Name = "valoration.delete", Description = "Eliminar valoraciones", Module = "Valorations" }
        };
    }
}
