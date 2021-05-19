using HADES.Models;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<ActiveDirectory> ActiveDirectory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
            if (!options.IsConfigured)
            {
                options.UseSqlite(Settings.AppSettings.SqlLiteConnectionString);
            }
        }

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
            modelBuilder.Entity<Role>().HasData(new Role { Id = (int)RolesID.SuperAdmin, Name = "SuperAdmin", AppConfigAccess = true, AdCrudAccess = true, UserListAccess = true, EventLogAccess = true, DefineOwner = true, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = (int)RolesID.Admin, Name = "Admin", AppConfigAccess = false, AdCrudAccess = true, UserListAccess = true, EventLogAccess = true, DefineOwner = true, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = (int)RolesID.Owner, Name = "Owner", AppConfigAccess = false, AdCrudAccess = false, UserListAccess = false, EventLogAccess = false, DefineOwner = false, HadesAccess = true });
            modelBuilder.Entity<Role>().HasData(new Role { Id = (int)RolesID.Inactive, Name = "inactive", AppConfigAccess = false, AdCrudAccess = false, UserListAccess = false, EventLogAccess = false, DefineOwner = false, HadesAccess = false });

            // CREATE DEFAULT USERCONFIG
            modelBuilder.Entity<UserConfig>().HasData(new UserConfig { Id = 1, Notification = false, Language = "fr-CA", ThemeFile = "site.css" });

            // ADD DEFAULT USER
            modelBuilder.Entity<DefaultUser>().HasData(new DefaultUser {Id=1, UserName = "admin", Password = "teWqcWW3Ks4yNoq84+Akbx+4feKr/tp+ZVU2CjCbKwI=", RoleId = 1, UserConfigId = 1 });

            // --- TESTS TEMPORAIRE (En attendant le Wizard AppConfig) ---

            // ADD DEFAULT ACTIVE DIRECTORY
            modelBuilder.Entity<ActiveDirectory>().HasData(new ActiveDirectory { Id=1, RootOu= "OU=hades_root,DC=R991-AD,DC=lan", PortNumber=389, ServerAddress= "172.20.48.10", ConnectionFilter= "(&(objectClass=user)(objectCategory=person))", BaseDN= "CN=Users,DC=R991-AD,DC=lan", AccountDN= "CN=hades,CN=Users,DC=R991-AD,DC=lan", PasswordDN= "Toto123!", SyncField= "samaccountName" });

            // ADD DEFAULT APP CONFIG
            modelBuilder.Entity<AppConfig>().HasData(new AppConfig { Id=1, CompanyName="YourCompanyName", CompanyBackgroundFile="background.png", CompanyLogoFile="logo.png", DefaultLanguage="fr-CA", SMTPServer="", SMTPPort = 465, SMTPUsername = "", SMTPPassword = "", SMTPFromEmail = "", LogDeleteFrequency=31, LogMaxFileSize=100000000, ActiveDirectoryId=1 });
        }
    }
}
