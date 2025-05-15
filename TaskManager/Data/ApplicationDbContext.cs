using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskModel> Tasks { get; set; } = default!;
        public DbSet<NotificationSettingsModel> NotificationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskModel>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskModel>()
                .Property(t => t.UserId)
                .HasColumnType("VARCHAR(255)");

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("VARCHAR(255)");
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("VARCHAR(255)");
                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.NormalizedName).HasMaxLength(256);
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("VARCHAR(255)");
            });

            builder.Entity<ApplicationUser>()
            .HasOne(u => u.NotificationSettings)
            .WithOne(ns => ns.User)
            .HasForeignKey<NotificationSettingsModel>(ns => ns.UserId);

            builder.Entity<NotificationSettingsModel>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("VARCHAR(255)");
            });
        }
    }
}