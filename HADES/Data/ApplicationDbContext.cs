using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using testUML.Models;

namespace HADES.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AdminGroup> AdminGroups { get; set; }
        public DbSet<AppConfig> AppConfigs { get; set; }
        public DbSet<DefaultUser> DefaultUsers { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<OwnerGroup> OwnerGroups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SuperAdminGroup> SuperAdminGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }

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

        }
    }
}
