using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Data.Seeders;

public class RolesSeeder
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    public RolesSeeder(IUserService userService, DataContext context, UserManager<User> userManager)
    {
        _userService = userService;
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await RolesAsync();
        await PermissionsAssignedAsync();
        await UserAsync();
    }

    private async Task RolesAsync()
    {
        var roles = new List<string>
        {
            Env.ROLE_ADMIN,
            Env.ROLE_USER,
            Env.ROLE_COLLAB,
            Env.ROLE_ANONYMOUS
        };

        foreach (var roleName in roles)
        {
            bool exists = await _context.WorksyRoles.AnyAsync(r => r.Name == roleName);
            if (!exists)
            {
                WorksyRole role = new()
                {
                    Id = Guid.NewGuid(),
                    Name = roleName
                };

                await _context.WorksyRoles.AddAsync(role);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task PermissionsAssignedAsync()
    {
        List<WorksyRole> roles = await _context.WorksyRoles.ToListAsync();
        List<Permission> permissions = await _context.Permissions.ToListAsync();

        // Admin
        WorksyRole adminRole = roles.First(r => r.Name == Env.ROLE_ADMIN);
        foreach (var permission in permissions)
            await AddRolePermissionIfNotExists(adminRole.Id, permission.Id);

        // Collab
        WorksyRole collabRole = roles.First(r => r.Name == Env.ROLE_COLLAB);
        var collabPermissions = permissions.Where(p =>
            p.Module == "Services" ||
            p.Name== "valoration.show" ||
            p.Name == "category.show"
        );
        foreach (var permission in collabPermissions)
            await AddRolePermissionIfNotExists(collabRole.Id, permission.Id);

        // User
        WorksyRole userRole = roles.First(r => r.Name == Env.ROLE_USER);
        var userPermissions = permissions.Where(p =>
            p.Name == "user.update" ||
            p.Name == "service.show" ||
            p.Name != "valoration.delete"
        );
        foreach (var permission in userPermissions)
            await AddRolePermissionIfNotExists(userRole.Id, permission.Id);

        // Anonymoues
        WorksyRole anonymousRole = roles.First(r => r.Name == Env.ROLE_ANONYMOUS);
        var anonymousPermissions = permissions.Where(p =>
            p.Name == "user.create" ||
            p.Name == "service.show" ||
            p.Name == "valoration.show"
        );
        foreach (var permission in anonymousPermissions)
            await AddRolePermissionIfNotExists(anonymousRole.Id, permission.Id);

        await _context.SaveChangesAsync();
    }

    private async Task AddRolePermissionIfNotExists(Guid roleId, Guid permissionId)
    {
        bool exists = await _context.RolePermissions
            .AnyAsync(rp => rp.WorksyRoleId == roleId && rp.PermissionId == permissionId);

        if (!exists)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                WorksyRoleId = roleId,
                PermissionId = permissionId
            });
        }
    }

    private async Task UserAsync()
    {
        // ADMIN
        string adminEmail = "admin@worksy.com";
        User? adminUser = await _userService.GetByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            WorksyRole adminRole = await _context.WorksyRoles.FirstAsync(r => r.Name == Env.ROLE_ADMIN);
            User admin = new()
            {
                FirstName = "Mateo",
                LastName = "Muñoz",
                Email = adminEmail,
                UserName = adminEmail,
                Address = "Medellín",
                EmailConfirmed = true,
                WorksyRoleId = adminRole.Id
            };

            await _userManager.CreateAsync(admin, "admin");
        }

        // COLLAB
        string collabEmail = "collab@worksy.com";
        User? collabUser = await _userService.GetByEmailAsync(collabEmail);

        if (collabUser == null)
        {
            WorksyRole collabRole = await _context.WorksyRoles.FirstAsync(r => r.Name == Env.ROLE_COLLAB);
            User collab = new()
            {
                FirstName = "Salomé",
                LastName = "Quiceno",
                Email = collabEmail,
                UserName = collabEmail,
                Address = "Medellín",
                EmailConfirmed = true,
                WorksyRoleId = collabRole.Id
            };

            await _userManager.CreateAsync(collab, "collab");
        }
    }
}
