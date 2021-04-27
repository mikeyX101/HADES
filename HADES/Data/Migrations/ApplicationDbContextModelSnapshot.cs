﻿// <auto-generated />
using HADES.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HADES.Data.Migrations
{
    [DbContext(typeof(DbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("HADES.Models.ActiveDirectory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ADR_id");

                    b.Property<string>("AccountDN")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_account_dn");

                    b.Property<string>("BaseDN")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_base_dn");

                    b.Property<string>("ConnectionFilter")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_connection_filter");

                    b.Property<string>("PasswordDN")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_password_dn");

                    b.Property<int>("PortNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ADR_port_number");

                    b.Property<string>("RootOu")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_root_ou");

                    b.Property<string>("ServerAddress")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_server_address");

                    b.Property<string>("SyncField")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADR_sync_field");

                    b.HasKey("Id");

                    b.ToTable("ActiveDirectory_ADR");
                });

            modelBuilder.Entity("HADES.Models.AdminGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ADG_id");

                    b.Property<int>("AppConfigId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ADG_ACF_id");

                    b.Property<string>("SamAccount")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ADG_sam_account_name");

                    b.HasKey("Id");

                    b.HasIndex("AppConfigId");

                    b.ToTable("AdminGroup_ADG");
                });

            modelBuilder.Entity("HADES.Models.AppConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACF_id");

                    b.Property<int>("ActiveDirectoryId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACF_ADR_id");

                    b.Property<string>("CompanyBackgroundFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("ACF_company_background_file");

                    b.Property<string>("CompanyLogoFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("ACF_company_logo_file");

                    b.Property<string>("CompanyName")
                        .HasColumnType("TEXT")
                        .HasColumnName("ACF_company_name");

                    b.Property<string>("DefaultLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ACF_default_language");

                    b.Property<int>("LogDeleteFrequency")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACF_log_delete_frequency");

                    b.Property<int>("LogMaxFileSize")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACF_log_max_file_size");

                    b.Property<string>("SMTP")
                        .HasColumnType("TEXT")
                        .HasColumnName("ACF_SMTP");

                    b.HasKey("Id");

                    b.HasIndex("ActiveDirectoryId")
                        .IsUnique();

                    b.ToTable("AppConfig_ACF");
                });

            modelBuilder.Entity("HADES.Models.DefaultUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("DUS_id");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("DUS_password_hash");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("DUS_ROL_id");

                    b.Property<int>("UserConfigId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("DUS_UCF_id");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("DUS_username");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserConfigId")
                        .IsUnique();

                    b.ToTable("DefaultUser_DUS");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Password = "teWqcWW3Ks4yNoq84+Akbx+4feKr/tp+ZVU2CjCbKwI=",
                            RoleId = 1,
                            UserConfigId = 1,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("HADES.Models.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("EMA_email");

                    b.Property<bool>("ExpirationDate")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_expiration_date");

                    b.Property<bool>("GroupCreate")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_group_create");

                    b.Property<bool>("GroupDelete")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_group_delete");

                    b.Property<bool>("MemberAdd")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_member_add");

                    b.Property<bool>("MemberRemoval")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_member_remove");

                    b.Property<int>("UserConfigId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("EMA_UCF_id");

                    b.HasKey("Id");

                    b.HasIndex("UserConfigId");

                    b.ToTable("Email_EMA");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("GRP_id");

                    b.Property<string>("SamAccount")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("GRP_sam_account_name");

                    b.HasKey("Id");

                    b.ToTable("OwnerGroup_GRP");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroupUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("GRU_id");

                    b.Property<int>("OwnerGroupId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("GRU_GRP_id");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("GRU_USE_id");

                    b.HasKey("Id");

                    b.HasIndex("OwnerGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("OwnerGroupUser_GRU");
                });

            modelBuilder.Entity("HADES.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_id");

                    b.Property<bool>("AdCrudAccess")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_access_ad_crud");

                    b.Property<bool>("AppConfigAccess")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_access_app_config");

                    b.Property<bool>("DefineOwner")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_define_owner");

                    b.Property<bool>("EventLogAccess")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_access_event_log");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ROL_name");

                    b.Property<bool>("UserListAccess")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ROL_access_users_list");

                    b.HasKey("Id");

                    b.ToTable("Role_ROL");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AdCrudAccess = true,
                            AppConfigAccess = true,
                            DefineOwner = true,
                            EventLogAccess = true,
                            Name = "SuperAdmin",
                            UserListAccess = true
                        },
                        new
                        {
                            Id = 2,
                            AdCrudAccess = true,
                            AppConfigAccess = false,
                            DefineOwner = true,
                            EventLogAccess = true,
                            Name = "Admin",
                            UserListAccess = true
                        },
                        new
                        {
                            Id = 3,
                            AdCrudAccess = false,
                            AppConfigAccess = false,
                            DefineOwner = false,
                            EventLogAccess = false,
                            Name = "Owner",
                            UserListAccess = false
                        },
                        new
                        {
                            Id = 4,
                            AdCrudAccess = false,
                            AppConfigAccess = false,
                            DefineOwner = false,
                            EventLogAccess = false,
                            Name = "inactive",
                            UserListAccess = false
                        });
                });

            modelBuilder.Entity("HADES.Models.SuperAdminGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("SUG_id");

                    b.Property<int>("AppConfigId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("SUG_ACF_id");

                    b.Property<string>("SamAccount")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("SUG_sam_account_name");

                    b.HasKey("Id");

                    b.HasIndex("AppConfigId");

                    b.ToTable("SuperAdminGroup_SUG");
                });

            modelBuilder.Entity("HADES.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("USE_id");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("USE_ROL_id");

                    b.Property<string>("SamAccount")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("USE_sam_account_name");

                    b.Property<int>("UserConfigId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("USE_UCF_id");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserConfigId")
                        .IsUnique();

                    b.ToTable("User_USE");
                });

            modelBuilder.Entity("HADES.Models.UserConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("UCF_id");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("UCF_language");

                    b.Property<bool>("Notification")
                        .HasColumnType("INTEGER")
                        .HasColumnName("UCF_notification");

                    b.Property<string>("ThemeFile")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("UCF_theme_file");

                    b.HasKey("Id");

                    b.ToTable("UserConfig_UCF");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Language = "fr-CA",
                            Notification = false,
                            ThemeFile = "site.css"
                        });
                });

            modelBuilder.Entity("HADES.Models.AdminGroup", b =>
                {
                    b.HasOne("HADES.Models.AppConfig", "AppConfig")
                        .WithMany("AdminGroups")
                        .HasForeignKey("AppConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppConfig");
                });

            modelBuilder.Entity("HADES.Models.AppConfig", b =>
                {
                    b.HasOne("HADES.Models.ActiveDirectory", "ActiveDirectory")
                        .WithOne("AppConfig")
                        .HasForeignKey("HADES.Models.AppConfig", "ActiveDirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActiveDirectory");
                });

            modelBuilder.Entity("HADES.Models.DefaultUser", b =>
                {
                    b.HasOne("HADES.Models.Role", "Role")
                        .WithMany("DefaultUsers")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithOne("DefaultUser")
                        .HasForeignKey("HADES.Models.DefaultUser", "UserConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("HADES.Models.Email", b =>
                {
                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithMany("Emails")
                        .HasForeignKey("UserConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroupUser", b =>
                {
                    b.HasOne("HADES.Models.OwnerGroup", "OwnerGroup")
                        .WithMany("OwnerGroupUsers")
                        .HasForeignKey("OwnerGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HADES.Models.User", "User")
                        .WithMany("OwnerGroupUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwnerGroup");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HADES.Models.SuperAdminGroup", b =>
                {
                    b.HasOne("HADES.Models.AppConfig", "AppConfig")
                        .WithMany("SuperAdminGroups")
                        .HasForeignKey("AppConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppConfig");
                });

            modelBuilder.Entity("HADES.Models.User", b =>
                {
                    b.HasOne("HADES.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithOne("User")
                        .HasForeignKey("HADES.Models.User", "UserConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("HADES.Models.ActiveDirectory", b =>
                {
                    b.Navigation("AppConfig");
                });

            modelBuilder.Entity("HADES.Models.AppConfig", b =>
                {
                    b.Navigation("AdminGroups");

                    b.Navigation("SuperAdminGroups");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroup", b =>
                {
                    b.Navigation("OwnerGroupUsers");
                });

            modelBuilder.Entity("HADES.Models.Role", b =>
                {
                    b.Navigation("DefaultUsers");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("HADES.Models.User", b =>
                {
                    b.Navigation("OwnerGroupUsers");
                });

            modelBuilder.Entity("HADES.Models.UserConfig", b =>
                {
                    b.Navigation("DefaultUser");

                    b.Navigation("Emails");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
