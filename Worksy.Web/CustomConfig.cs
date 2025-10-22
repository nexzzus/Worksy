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

        // Identity Access Managment
        AddIAM(builder);

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

    private static void AddIAM(WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequireDigit = false;
                config.Password.RequiredUniqueChars = 0;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequiredLength = 4;
                //config.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "Auth";
            options.ExpireTimeSpan = TimeSpan.FromDays(100);
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Error/AccessDenied";
        });
    }

    public static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();

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