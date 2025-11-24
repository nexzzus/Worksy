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

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Message> Messages { get; set; }

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

            builder.Entity<Service>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            builder.Entity<Conversation>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversation>()
                .HasOne(c => c.Provider)
                .WithMany()
                .HasForeignKey(c => c.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversation>()
                .HasOne(c => c.Service)
                .WithMany()
                .HasForeignKey(c => c.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
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