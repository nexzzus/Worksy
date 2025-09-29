using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Implementations;

namespace Worksy.Web;

public static class CustomConfig
{
    public static WebApplicationBuilder AddCustomConfig(this WebApplicationBuilder builder)
    {
        // Data context
        builder.Services.AddDbContext<DataContext>(option =>
        {
            option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        // Services
        AddServices(builder);
        
        // Cookies
        AddCoockies(builder);

        return builder;
    }

    public static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserService>();
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddCoockies(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });
    }
}