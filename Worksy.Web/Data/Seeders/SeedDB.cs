using Microsoft.AspNetCore.Identity;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Data.Seeders;

public class SeedDB
{
    private readonly DataContext _context;
    private readonly IUserService  _userService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    public SeedDB(DataContext context, RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, IUserService userService)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _userService = userService;
    }

    public async Task SeedAsync()
    {
        await new PermissionSeeder(_context).SeedAsync();
        await  new RolesSeeder(_userService, _context, _userManager, _roleManager).SeedAsync();
    }
}