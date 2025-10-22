using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Service> Services { get; set;
        }
    }
}