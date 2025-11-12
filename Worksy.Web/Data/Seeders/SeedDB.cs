using Microsoft.AspNetCore.Identity;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Data.Seeders;

public class SeedDB
{
    private readonly DataContext _context;
    private readonly IUserService  _userService;
    private readonly UserManager<User> _userManager;
    public SeedDB(DataContext context, UserManager<User> userManager, IUserService userService)
    {
        _context = context;
        _userManager = userManager;
        _userService = userService;
    }

    public async Task SeedAsync()
    {
        await new PermissionSeeder(_context).SeedAsync();
        await  new RolesSeeder(_userService, _context, _userManager).SeedAsync();
    }
}