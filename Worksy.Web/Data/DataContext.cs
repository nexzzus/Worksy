using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<WorksyRole> WorksyRoles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureIndex(builder);
            ConfigureKeys(builder);

            base.OnModelCreating(builder);
        }

        private void ConfigureKeys(ModelBuilder builder)
        {
            // RolePermission
            builder.Entity<RolePermission>().HasKey(rp => new { rp.PermissionId, rp.WorksyRoleId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.WorksyRole)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.WorksyRoleId);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        }

        private void ConfigureIndex(ModelBuilder builder)
        {
            // Roles
            builder.Entity<WorksyRole>().HasIndex(r => r.Name).IsUnique();
            // Permission
            builder.Entity<WorksyRole>().HasIndex(r => r.Name).IsUnique();
        }
    }
}