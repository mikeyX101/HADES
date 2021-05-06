using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class defaultUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserConfig_UCF",
                columns: new[] { "UCF_id", "UCF_language", "UCF_notification", "UCF_theme_file" },
                values: new object[] { 1, "fr-CA", false, "site.css" });

            migrationBuilder.InsertData(
                table: "DefaultUser_DUS",
                columns: new[] { "DUS_id", "DUS_password_hash", "DUS_ROL_id", "DUS_UCF_id", "DUS_username" },
                values: new object[] { 1, "", 1, 1, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DefaultUser_DUS",
                keyColumn: "DUS_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserConfig_UCF",
                keyColumn: "UCF_id",
                keyValue: 1);
        }
    }
}
