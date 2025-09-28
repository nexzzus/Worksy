using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data;
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
        
        return builder;
    }

    public static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserService>();
    }
}