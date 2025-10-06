using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Core.Abstractions;
using Worksy.Web.Data;
using Worksy.Web.Data.Entities;
using Worksy.Web.Services.Abstractions;
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
        // AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
        
        // Services
        AddServices(builder);
        
        // Cookies
        AddCookies(builder);
        
        // Toast Notification
        builder.Services.AddNotyf(config =>
        {
            config.DurationInSeconds = 5;
            config.IsDismissable = true;
            config.Position = NotyfPosition.BottomRight;
        });
        return builder;
    }

    public static void AddServices(WebApplicationBuilder builder)
    {
        //builder.Services.AddScoped<UserService>();
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        builder.Services.AddScoped<IServicesService, ServicesService>();

    }

    public static void AddCookies(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Users/Login";
            options.AccessDeniedPath = "/Users/AccessDenied";
        });
    }

    public static WebApplication AddCustomAppConfig(this WebApplication app)
    {
        app.UseNotyf();
        
        return app;
    }
}