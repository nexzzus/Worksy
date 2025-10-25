using Microsoft.AspNetCore.Identity;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Data.Seeders;

public class SeedDB
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    public SeedDB(DataContext context, IUserService userService, UserManager<User> userManager)
    {
        _context = context;
        _userService = userService;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await new PermissionSeeder(_context).SeedAsync();
        await new RolesSeeder(_userService, _context, _userManager).SeedAsync();
    }
}