using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HADES.Models;

namespace HADES.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminGroup> AdminGroup { get; set; }
        public DbSet<AppConfig> AppConfig { get; set; }
        public DbSet<DefaultUser> DefaultUser { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<OwnerGroup> OwnerGroup { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<SuperAdminGroup> SuperAdminGroup { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserConfig> UserConfig { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=App_Data/DBHades.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminGroup>().ToTable("AdminGroup_ADG");
            modelBuilder.Entity<AppConfig>().ToTable("AppConfig_ACF");
            modelBuilder.Entity<DefaultUser>().ToTable("DefaultUser_DUS");
            modelBuilder.Entity<Email>().ToTable("Email_EMA");
            modelBuilder.Entity<OwnerGroup>().ToTable("OwnerGroup_GRP");
            modelBuilder.Entity<Role>().ToTable("Role_ROL");
            modelBuilder.Entity<SuperAdminGroup>().ToTable("SuperAdminGroup_SUG");
            modelBuilder.Entity<User>().ToTable("User_USE");
            modelBuilder.Entity<UserConfig>().ToTable("UserConfig_UCF");

            // CREATE ROLES
            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "SuperAdmin", AppConfigAccess = true, AdCrudAccess = true, UserListAccess = true, EventLogAccess = true, DefineOwner = true, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 2, Name = "Admin", AppConfigAccess = false, AdCrudAccess = true, UserListAccess = true, EventLogAccess = true, DefineOwner = true, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 3, Name = "Owner", AppConfigAccess = false, AdCrudAccess = false, UserListAccess = false, EventLogAccess = false, DefineOwner = false, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 4, Name = "inactive", AppConfigAccess = false, AdCrudAccess = false, UserListAccess = false, EventLogAccess = false, DefineOwner = false, HadesAccess = false });

            // CREATE DEFAULT USERCONFIG
            modelBuilder.Entity<UserConfig>().HasData(new UserConfig { Id = 1, Notification = false, Language="fr-CA", ThemeFile="site.css" });

            // ADD DEFAULT USER
            modelBuilder.Entity<DefaultUser>().HasData(new DefaultUser {Id=1, UserName = "admin", Password = "teWqcWW3Ks4yNoq84+Akbx+4feKr/tp+ZVU2CjCbKwI=", RoleId = 1, UserConfigId = 1 });
        }
    }
}
