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
            ..Roles(),
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

    private List<Permission> Roles()
    {
        return new()
        {
            new Permission { Name = "rol.create", Description = "Crear roles", Module = "Roles" },
            new Permission { Name = "rol.update", Description = "Actualizar roles", Module = "Roles" },
            new Permission { Name = "rol.delete", Description = "Eliminar roles", Module = "Roles" },
            new Permission { Name = "rol.show", Description = "Ver roles", Module = "Roles" }
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
            new Permission { Name = "user.show", Description = "Ver usuario", Module = "Users" },
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
