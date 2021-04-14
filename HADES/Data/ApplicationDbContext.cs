using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using HADES.Models;

namespace HADES.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdminGroup>().ToTable("AdminGroup_ADG");
            modelBuilder.Entity<AppConfig>().ToTable("AppConfig_ACF");
            modelBuilder.Entity<DefaultUser>().ToTable("DefaultUser_DUS");
            modelBuilder.Entity<Email>().ToTable("Email_EMA");
            modelBuilder.Entity<OwnerGroup>().ToTable("OwnerGroup_GRP");
            modelBuilder.Entity<Role>().ToTable("Role_ROL");
            modelBuilder.Entity<SuperAdminGroup>().ToTable("SuperAdminGroup_SUG");
            modelBuilder.Entity<User>().ToTable("User_USE");
            modelBuilder.Entity<UserConfig>().ToTable("UserConfig_UCF");


            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = 1,
                Name = "role1",
                AppConfigAccess = true,
                EventLogAccess = true,
                UserListAccess = true,
                DefineOwner = true,
                AdCrudAccess = true
            }
            );
            modelBuilder.Entity<UserConfig>().HasData(new UserConfig
            {
                Id = 1,
                Language = "fr",
                ThemeFile = "dark",
                Notification = true
            },
            new UserConfig
            {
                Id = 2,
                Language = "fr",
                ThemeFile = "light",
                Notification = true
            });

            modelBuilder.Entity<Email>().HasData(new Email
            {
                Id = 1,
                Address = "DefaultUser@google.com",
                ExpirationDate = true,
                GroupCreate = true,
                GroupDelete = true,
                MemberRemoval = true,
                MemberAdd = true,
                UserConfigId = 1
            },
            new Email
            {
                Id = 2,
                Address = "user@google.com",
                ExpirationDate = true,
                GroupCreate = true,
                GroupDelete = true,
                MemberRemoval = true,
                MemberAdd = true,
                UserConfigId = 2
            });

            modelBuilder.Entity<DefaultUser>().HasData(new DefaultUser
            {
                Id = 1,
                UserName = "user1",
                Password = "password",
                RoleId = 1,
                UserConfigId = 1
            });

            modelBuilder.Entity<OwnerGroup>().HasData(new OwnerGroup
            {
                Id = 1,
                SamAccount = "samOwnerGroup"
            });

            modelBuilder.Entity<OwnerGroupUser>().HasData(new OwnerGroupUser
            {
                Id = 1,
                UserId = 1,
                OwnerGroupId = 1
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                SamAccount = "user2",
                RoleId = 1,
                UserConfigId = 2
            });

            modelBuilder.Entity<AppConfig>().HasData(new AppConfig
            {
                Id = 1,
                ActiveDirectory = "",
                CompanyName = null,
                CompanyLogoFile = null,
                CompanyBackgroundFile = null,
                DefaultLanguage = "fr",
                SMTP = null,
                LogDeleteFrequency = 30,
                LogMaxFileSize = 10
            });

            modelBuilder.Entity<SuperAdminGroup>().HasData(new SuperAdminGroup
            {
                Id = 1,
                SamAccount = "samSuperAdmin",
                AppConfigId = 1
            });

            modelBuilder.Entity<AdminGroup>().HasData(new AdminGroup
            {
                Id = 1,
                SamAccount = "samAdmin",
                AppConfigId = 1
            });
        }
    }
}
