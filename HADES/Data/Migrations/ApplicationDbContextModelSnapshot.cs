﻿// <auto-generated />
using System;
using HADES.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HADES.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HADES.Models.AdminGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ADG_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ADG_ACF_id")
                        .HasColumnType("int");

                    b.Property<string>("SamAccount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ADG_sam_account_name");

                    b.HasKey("Id");

                    b.HasIndex("ADG_ACF_id");

                    b.ToTable("AdminGroup_ADG");
                });

            modelBuilder.Entity("HADES.Models.AppConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ACF_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActiveDirectory")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_active_directory");

                    b.Property<string>("CompanyBackgroundFile")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_company_background_file");

                    b.Property<string>("CompanyLogoFile")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_company_logo_file");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_company_name");

                    b.Property<string>("DefaultLanguage")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_default_language");

                    b.Property<int>("LogDeleteFrequency")
                        .HasColumnType("int")
                        .HasColumnName("ACF_log_delete_frequency");

                    b.Property<int>("LogMaxFileSize")
                        .HasColumnType("int")
                        .HasColumnName("ACF_log_max_file_size");

                    b.Property<string>("SMTP")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ACF_SMTP");

                    b.HasKey("Id");

                    b.ToTable("AppConfig_ACF");
                });

            modelBuilder.Entity("HADES.Models.DefaultUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("DUS_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DUS_ROL_id")
                        .HasColumnType("int");

                    b.Property<int?>("DUS_UCF_id")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DUS_password_hash");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DUS_username");

                    b.HasKey("Id");

                    b.HasIndex("DUS_ROL_id");

                    b.HasIndex("DUS_UCF_id")
                        .IsUnique()
                        .HasFilter("[DUS_UCF_id] IS NOT NULL");

                    b.ToTable("DefaultUser_DUS");
                });

            modelBuilder.Entity("HADES.Models.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EMA_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EMA_email");

                    b.Property<int?>("EMA_UCF_id")
                        .HasColumnType("int");

                    b.Property<bool>("ExpirationDate")
                        .HasColumnType("bit")
                        .HasColumnName("EMA_expiration_date");

                    b.Property<bool>("GroupAdd")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit")
                        .HasColumnName("EMA_member_add");

                    b.Property<bool>("GroupCreate")
                        .HasColumnType("bit")
                        .HasColumnName("EMA_group_create");

                    b.Property<bool>("GroupDelete")
                        .HasColumnType("bit")
                        .HasColumnName("EMA_group_delete");

                    b.Property<bool>("MemberAdd")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit")
                        .HasColumnName("EMA_member_add");

                    b.Property<bool>("MemberRemoval")
                        .HasColumnType("bit")
                        .HasColumnName("EMA_member_remove");

                    b.HasKey("Id");

                    b.HasIndex("EMA_UCF_id");

                    b.ToTable("Email_EMA");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("GRP_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SamAccount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("GRP_sam_account_name");

                    b.HasKey("Id");

                    b.ToTable("OwnerGroup_GRP");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroupUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("GRU_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GRU_GRP_id")
                        .HasColumnType("int");

                    b.Property<int?>("GRU_USE_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GRU_GRP_id");

                    b.HasIndex("GRU_USE_id");

                    b.ToTable("OwnerGroupUser_GRU");
                });

            modelBuilder.Entity("HADES.Models.Role", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ROL_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AdCrudAccess")
                        .HasColumnType("bit")
                        .HasColumnName("ROL_access_ad_crud");

                    b.Property<bool>("AppConfigAccess")
                        .HasColumnType("bit")
                        .HasColumnName("ROL_access_app_config");

                    b.Property<bool>("DefineOwner")
                        .HasColumnType("bit")
                        .HasColumnName("ROL_define_owner");

                    b.Property<bool>("EventLogAccess")
                        .HasColumnType("bit")
                        .HasColumnName("ROL_access_event_log");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ROL_name");

                    b.Property<bool>("UserListAccess")
                        .HasColumnType("bit")
                        .HasColumnName("ROL_access_users_list");

                    b.HasKey("ID");

                    b.ToTable("Role_ROL");
                });

            modelBuilder.Entity("HADES.Models.SuperAdminGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SUG_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("SUG_ACF_id")
                        .HasColumnType("int");

                    b.Property<string>("SamAccount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SUG_sam_account_name");

                    b.HasKey("Id");

                    b.HasIndex("SUG_ACF_id");

                    b.ToTable("SuperAdminGroup_SUG");
                });

            modelBuilder.Entity("HADES.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("USE_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SamAccount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("USE_sam_account_name");

                    b.Property<int?>("USE_ROL_id")
                        .HasColumnType("int");

                    b.Property<int?>("USE_UCF_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("USE_ROL_id");

                    b.HasIndex("USE_UCF_id")
                        .IsUnique()
                        .HasFilter("[USE_UCF_id] IS NOT NULL");

                    b.ToTable("User_USE");
                });

            modelBuilder.Entity("HADES.Models.UserConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UCF_id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UCF_language");

                    b.Property<bool>("Notification")
                        .HasColumnType("bit")
                        .HasColumnName("UCF_notification");

                    b.Property<string>("ThemeFile")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UCF_theme_file");

                    b.HasKey("Id");

                    b.ToTable("UserConfig_UCF");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("HADES.Models.AdminGroup", b =>
                {
                    b.HasOne("HADES.Models.AppConfig", "AppConfig")
                        .WithMany("AdminGroups")
                        .HasForeignKey("ADG_ACF_id");

                    b.Navigation("AppConfig");
                });

            modelBuilder.Entity("HADES.Models.DefaultUser", b =>
                {
                    b.HasOne("HADES.Models.Role", "Role")
                        .WithMany("DefaultUsers")
                        .HasForeignKey("DUS_ROL_id");

                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithOne("DefaultUser")
                        .HasForeignKey("HADES.Models.DefaultUser", "DUS_UCF_id");

                    b.Navigation("Role");

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("HADES.Models.Email", b =>
                {
                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithMany("Emails")
                        .HasForeignKey("EMA_UCF_id");

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("HADES.Models.OwnerGroupUser", b =>
                {
                    b.HasOne("HADES.Models.OwnerGroup", "OwnerGroup")
                        .WithMany("OwnerGroupUsers")
                        .HasForeignKey("GRU_GRP_id");

                    b.HasOne("HADES.Models.User", "User")
                        .WithMany("OwnerGroupUsers")
                        .HasForeignKey("GRU_USE_id");

                    b.Navigation("OwnerGroup");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HADES.Models.SuperAdminGroup", b =>
                {
                    b.HasOne("HADES.Models.AppConfig", "AppConfig")
                        .WithMany("SuperAdminGroups")
                        .HasForeignKey("SUG_ACF_id");

                    b.Navigation("AppConfig");
                });

            modelBuilder.Entity("HADES.Models.User", b =>
                {
                    b.HasOne("HADES.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("USE_ROL_id");

                    b.HasOne("HADES.Models.UserConfig", "UserConfig")
                        .WithOne("User")
                        .HasForeignKey("HADES.Models.User", "USE_UCF_id");

                    b.Navigation("Role");

                    b.Navigation("UserConfig");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
