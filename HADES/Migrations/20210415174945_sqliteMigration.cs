using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
    public partial class sqliteMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfig_ACF",
                columns: table => new
                {
                    ACF_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ACF_active_directory = table.Column<string>(type: "TEXT", nullable: false),
                    ACF_company_name = table.Column<string>(type: "TEXT", nullable: true),
                    ACF_company_logo_file = table.Column<string>(type: "TEXT", nullable: true),
                    ACF_company_background_file = table.Column<string>(type: "TEXT", nullable: true),
                    ACF_default_language = table.Column<string>(type: "TEXT", nullable: false),
                    ACF_SMTP = table.Column<string>(type: "TEXT", nullable: true),
                    ACF_log_delete_frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    ACF_log_max_file_size = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfig_ACF", x => x.ACF_id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerGroup_GRP",
                columns: table => new
                {
                    GRP_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GRP_sam_account_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerGroup_GRP", x => x.GRP_id);
                });

            migrationBuilder.CreateTable(
                name: "Role_ROL",
                columns: table => new
                {
                    ROL_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ROL_name = table.Column<string>(type: "TEXT", nullable: false),
                    ROL_access_app_config = table.Column<bool>(type: "INTEGER", nullable: false),
                    ROL_access_event_log = table.Column<bool>(type: "INTEGER", nullable: false),
                    ROL_access_users_list = table.Column<bool>(type: "INTEGER", nullable: false),
                    ROL_define_owner = table.Column<bool>(type: "INTEGER", nullable: false),
                    ROL_access_ad_crud = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_ROL", x => x.ROL_id);
                });

            migrationBuilder.CreateTable(
                name: "UserConfig_UCF",
                columns: table => new
                {
                    UCF_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UCF_language = table.Column<string>(type: "TEXT", nullable: false),
                    UCF_theme_file = table.Column<string>(type: "TEXT", nullable: false),
                    UCF_notification = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfig_UCF", x => x.UCF_id);
                });

            migrationBuilder.CreateTable(
                name: "AdminGroup_ADG",
                columns: table => new
                {
                    ADG_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ADG_sam_account_name = table.Column<string>(type: "TEXT", nullable: false),
                    ADG_ACF_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminGroup_ADG", x => x.ADG_id);
                    table.ForeignKey(
                        name: "FK_AdminGroup_ADG_AppConfig_ACF_ADG_ACF_id",
                        column: x => x.ADG_ACF_id,
                        principalTable: "AppConfig_ACF",
                        principalColumn: "ACF_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuperAdminGroup_SUG",
                columns: table => new
                {
                    SUG_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SUG_sam_account_name = table.Column<string>(type: "TEXT", nullable: false),
                    SUG_ACF_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdminGroup_SUG", x => x.SUG_id);
                    table.ForeignKey(
                        name: "FK_SuperAdminGroup_SUG_AppConfig_ACF_SUG_ACF_id",
                        column: x => x.SUG_ACF_id,
                        principalTable: "AppConfig_ACF",
                        principalColumn: "ACF_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultUser_DUS",
                columns: table => new
                {
                    DUS_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DUS_username = table.Column<string>(type: "TEXT", nullable: false),
                    DUS_password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    DUS_ROL_id = table.Column<int>(type: "INTEGER", nullable: false),
                    DUS_UCF_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUser_DUS", x => x.DUS_id);
                    table.ForeignKey(
                        name: "FK_DefaultUser_DUS_Role_ROL_DUS_ROL_id",
                        column: x => x.DUS_ROL_id,
                        principalTable: "Role_ROL",
                        principalColumn: "ROL_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultUser_DUS_UserConfig_UCF_DUS_UCF_id",
                        column: x => x.DUS_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Email_EMA",
                columns: table => new
                {
                    EMA_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EMA_email = table.Column<string>(type: "TEXT", nullable: false),
                    EMA_expiration_date = table.Column<bool>(type: "INTEGER", nullable: false),
                    EMA_group_create = table.Column<bool>(type: "INTEGER", nullable: false),
                    EMA_group_delete = table.Column<bool>(type: "INTEGER", nullable: false),
                    EMA_member_add = table.Column<bool>(type: "INTEGER", nullable: false),
                    EMA_member_remove = table.Column<bool>(type: "INTEGER", nullable: false),
                    EMA_UCF_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email_EMA", x => x.EMA_id);
                    table.ForeignKey(
                        name: "FK_Email_EMA_UserConfig_UCF_EMA_UCF_id",
                        column: x => x.EMA_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_USE",
                columns: table => new
                {
                    USE_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    USE_sam_account_name = table.Column<string>(type: "TEXT", nullable: false),
                    USE_ROL_id = table.Column<int>(type: "INTEGER", nullable: false),
                    USE_UCF_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_USE", x => x.USE_id);
                    table.ForeignKey(
                        name: "FK_User_USE_Role_ROL_USE_ROL_id",
                        column: x => x.USE_ROL_id,
                        principalTable: "Role_ROL",
                        principalColumn: "ROL_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_USE_UserConfig_UCF_USE_UCF_id",
                        column: x => x.USE_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnerGroupUser_GRU",
                columns: table => new
                {
                    GRU_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GRU_USE_id = table.Column<int>(type: "INTEGER", nullable: false),
                    GRU_GRP_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerGroupUser_GRU", x => x.GRU_id);
                    table.ForeignKey(
                        name: "FK_OwnerGroupUser_GRU_OwnerGroup_GRP_GRU_GRP_id",
                        column: x => x.GRU_GRP_id,
                        principalTable: "OwnerGroup_GRP",
                        principalColumn: "GRP_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnerGroupUser_GRU_User_USE_GRU_USE_id",
                        column: x => x.GRU_USE_id,
                        principalTable: "User_USE",
                        principalColumn: "USE_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppConfig_ACF",
                columns: new[] { "ACF_id", "ACF_active_directory", "ACF_company_background_file", "ACF_company_logo_file", "ACF_company_name", "ACF_default_language", "ACF_log_delete_frequency", "ACF_log_max_file_size", "ACF_SMTP" },
                values: new object[] { 1, "", null, null, null, "fr", 30, 10, null });

            migrationBuilder.InsertData(
                table: "OwnerGroup_GRP",
                columns: new[] { "GRP_id", "GRP_sam_account_name" },
                values: new object[] { 1, "samOwnerGroup" });

            migrationBuilder.InsertData(
                table: "Role_ROL",
                columns: new[] { "ROL_id", "ROL_access_ad_crud", "ROL_access_app_config", "ROL_define_owner", "ROL_access_event_log", "ROL_name", "ROL_access_users_list" },
                values: new object[] { 1, true, true, true, true, "role1", true });

            migrationBuilder.InsertData(
                table: "UserConfig_UCF",
                columns: new[] { "UCF_id", "UCF_language", "UCF_notification", "UCF_theme_file" },
                values: new object[] { 1, "fr", true, "dark" });

            migrationBuilder.InsertData(
                table: "UserConfig_UCF",
                columns: new[] { "UCF_id", "UCF_language", "UCF_notification", "UCF_theme_file" },
                values: new object[] { 2, "fr", true, "light" });

            migrationBuilder.InsertData(
                table: "AdminGroup_ADG",
                columns: new[] { "ADG_id", "ADG_ACF_id", "ADG_sam_account_name" },
                values: new object[] { 1, 1, "samAdmin" });

            migrationBuilder.InsertData(
                table: "DefaultUser_DUS",
                columns: new[] { "DUS_id", "DUS_password_hash", "DUS_ROL_id", "DUS_UCF_id", "DUS_username" },
                values: new object[] { 1, "password", 1, 1, "user1" });

            migrationBuilder.InsertData(
                table: "Email_EMA",
                columns: new[] { "EMA_id", "EMA_email", "EMA_expiration_date", "EMA_group_create", "EMA_group_delete", "EMA_member_add", "EMA_member_remove", "EMA_UCF_id" },
                values: new object[] { 1, "DefaultUser@google.com", true, true, true, true, true, 1 });

            migrationBuilder.InsertData(
                table: "Email_EMA",
                columns: new[] { "EMA_id", "EMA_email", "EMA_expiration_date", "EMA_group_create", "EMA_group_delete", "EMA_member_add", "EMA_member_remove", "EMA_UCF_id" },
                values: new object[] { 2, "user@google.com", true, true, true, true, true, 2 });

            migrationBuilder.InsertData(
                table: "SuperAdminGroup_SUG",
                columns: new[] { "SUG_id", "SUG_ACF_id", "SUG_sam_account_name" },
                values: new object[] { 1, 1, "samSuperAdmin" });

            migrationBuilder.InsertData(
                table: "User_USE",
                columns: new[] { "USE_id", "USE_ROL_id", "USE_sam_account_name", "USE_UCF_id" },
                values: new object[] { 1, 1, "user2", 2 });

            migrationBuilder.InsertData(
                table: "OwnerGroupUser_GRU",
                columns: new[] { "GRU_id", "GRU_GRP_id", "GRU_USE_id" },
                values: new object[] { 1, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AdminGroup_ADG_ADG_ACF_id",
                table: "AdminGroup_ADG",
                column: "ADG_ACF_id");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUser_DUS_DUS_ROL_id",
                table: "DefaultUser_DUS",
                column: "DUS_ROL_id");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUser_DUS_DUS_UCF_id",
                table: "DefaultUser_DUS",
                column: "DUS_UCF_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Email_EMA_EMA_UCF_id",
                table: "Email_EMA",
                column: "EMA_UCF_id");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGroupUser_GRU_GRU_GRP_id",
                table: "OwnerGroupUser_GRU",
                column: "GRU_GRP_id");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGroupUser_GRU_GRU_USE_id",
                table: "OwnerGroupUser_GRU",
                column: "GRU_USE_id");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminGroup_SUG_SUG_ACF_id",
                table: "SuperAdminGroup_SUG",
                column: "SUG_ACF_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_USE_USE_ROL_id",
                table: "User_USE",
                column: "USE_ROL_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_USE_USE_UCF_id",
                table: "User_USE",
                column: "USE_UCF_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminGroup_ADG");

            migrationBuilder.DropTable(
                name: "DefaultUser_DUS");

            migrationBuilder.DropTable(
                name: "Email_EMA");

            migrationBuilder.DropTable(
                name: "OwnerGroupUser_GRU");

            migrationBuilder.DropTable(
                name: "SuperAdminGroup_SUG");

            migrationBuilder.DropTable(
                name: "OwnerGroup_GRP");

            migrationBuilder.DropTable(
                name: "User_USE");

            migrationBuilder.DropTable(
                name: "AppConfig_ACF");

            migrationBuilder.DropTable(
                name: "Role_ROL");

            migrationBuilder.DropTable(
                name: "UserConfig_UCF");
        }
    }
}
