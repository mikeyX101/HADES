using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfig_ACF",
                columns: table => new
                {
                    ACF_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACF_active_directory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_company_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_company_logo_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_company_background_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_default_language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_SMTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACF_log_delete_frequency = table.Column<int>(type: "int", nullable: false),
                    ACF_log_max_file_size = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfig_ACF", x => x.ACF_id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerGroup_GRP",
                columns: table => new
                {
                    GRP_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRP_sam_account_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerGroup_GRP", x => x.GRP_id);
                });

            migrationBuilder.CreateTable(
                name: "Role_ROL",
                columns: table => new
                {
                    ROL_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROL_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ROL_access_app_config = table.Column<bool>(type: "bit", nullable: false),
                    ROL_access_event_log = table.Column<bool>(type: "bit", nullable: false),
                    ROL_access_users_list = table.Column<bool>(type: "bit", nullable: false),
                    ROL_define_owner = table.Column<bool>(type: "bit", nullable: false),
                    ROL_access_ad_crud = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_ROL", x => x.ROL_id);
                });

            migrationBuilder.CreateTable(
                name: "UserConfig_UCF",
                columns: table => new
                {
                    UCF_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UCF_language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UCF_theme_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UCF_notification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfig_UCF", x => x.UCF_id);
                });

            migrationBuilder.CreateTable(
                name: "AdminGroup_ADG",
                columns: table => new
                {
                    ADG_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ADG_sam_account_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADG_ACF_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminGroup_ADG", x => x.ADG_id);
                    table.ForeignKey(
                        name: "FK_AdminGroup_ADG_AppConfig_ACF_ADG_ACF_id",
                        column: x => x.ADG_ACF_id,
                        principalTable: "AppConfig_ACF",
                        principalColumn: "ACF_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuperAdminGroup_SUG",
                columns: table => new
                {
                    SUG_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SUG_sam_account_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SUG_ACF_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdminGroup_SUG", x => x.SUG_id);
                    table.ForeignKey(
                        name: "FK_SuperAdminGroup_SUG_AppConfig_ACF_SUG_ACF_id",
                        column: x => x.SUG_ACF_id,
                        principalTable: "AppConfig_ACF",
                        principalColumn: "ACF_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefaultUser_DUS",
                columns: table => new
                {
                    DUS_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DUS_username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DUS_password_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DUS_ROL_id = table.Column<int>(type: "int", nullable: true),
                    DUS_UCF_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUser_DUS", x => x.DUS_id);
                    table.ForeignKey(
                        name: "FK_DefaultUser_DUS_Role_ROL_DUS_ROL_id",
                        column: x => x.DUS_ROL_id,
                        principalTable: "Role_ROL",
                        principalColumn: "ROL_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultUser_DUS_UserConfig_UCF_DUS_UCF_id",
                        column: x => x.DUS_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Email_EMA",
                columns: table => new
                {
                    EMA_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMA_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMA_expiration_date = table.Column<bool>(type: "bit", nullable: false),
                    EMA_group_create = table.Column<bool>(type: "bit", nullable: false),
                    EMA_group_delete = table.Column<bool>(type: "bit", nullable: false),
                    EMA_member_add = table.Column<bool>(type: "bit", nullable: false),
                    EMA_member_remove = table.Column<bool>(type: "bit", nullable: false),
                    EMA_UCF_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email_EMA", x => x.EMA_id);
                    table.ForeignKey(
                        name: "FK_Email_EMA_UserConfig_UCF_EMA_UCF_id",
                        column: x => x.EMA_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_USE",
                columns: table => new
                {
                    USE_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USE_sam_account_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    USE_ROL_id = table.Column<int>(type: "int", nullable: true),
                    USE_UCF_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_USE", x => x.USE_id);
                    table.ForeignKey(
                        name: "FK_User_USE_Role_ROL_USE_ROL_id",
                        column: x => x.USE_ROL_id,
                        principalTable: "Role_ROL",
                        principalColumn: "ROL_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_USE_UserConfig_UCF_USE_UCF_id",
                        column: x => x.USE_UCF_id,
                        principalTable: "UserConfig_UCF",
                        principalColumn: "UCF_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerGroupUser_GRU",
                columns: table => new
                {
                    GRU_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRU_USE_id = table.Column<int>(type: "int", nullable: true),
                    GRU_GRP_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerGroupUser_GRU", x => x.GRU_id);
                    table.ForeignKey(
                        name: "FK_OwnerGroupUser_GRU_OwnerGroup_GRP_GRU_GRP_id",
                        column: x => x.GRU_GRP_id,
                        principalTable: "OwnerGroup_GRP",
                        principalColumn: "GRP_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerGroupUser_GRU_User_USE_GRU_USE_id",
                        column: x => x.GRU_USE_id,
                        principalTable: "User_USE",
                        principalColumn: "USE_id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                unique: true,
                filter: "[DUS_UCF_id] IS NOT NULL");

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
                unique: true,
                filter: "[USE_UCF_id] IS NOT NULL");
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
