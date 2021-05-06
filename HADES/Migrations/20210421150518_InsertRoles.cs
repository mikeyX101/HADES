using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class InsertRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role_ROL",
                columns: new[] { "ROL_id", "ROL_access_ad_crud", "ROL_access_app_config", "ROL_define_owner", "ROL_access_event_log", "ROL_name", "ROL_access_users_list" },
                values: new object[] { 1, true, true, true, true, "SuperAdmin", true });

            migrationBuilder.InsertData(
                table: "Role_ROL",
                columns: new[] { "ROL_id", "ROL_access_ad_crud", "ROL_access_app_config", "ROL_define_owner", "ROL_access_event_log", "ROL_name", "ROL_access_users_list" },
                values: new object[] { 2, true, false, true, true, "Admin", true });

            migrationBuilder.InsertData(
                table: "Role_ROL",
                columns: new[] { "ROL_id", "ROL_access_ad_crud", "ROL_access_app_config", "ROL_define_owner", "ROL_access_event_log", "ROL_name", "ROL_access_users_list" },
                values: new object[] { 3, false, false, false, false, "Owner", false });

            migrationBuilder.InsertData(
                table: "Role_ROL",
                columns: new[] { "ROL_id", "ROL_access_ad_crud", "ROL_access_app_config", "ROL_define_owner", "ROL_access_event_log", "ROL_name", "ROL_access_users_list" },
                values: new object[] { 4, false, false, false, false, "inactive", false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 4);
        }
    }
}
