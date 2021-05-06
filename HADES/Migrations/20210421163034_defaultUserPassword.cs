using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class defaultUserPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DefaultUser_DUS",
                keyColumn: "DUS_id",
                keyValue: 1,
                column: "DUS_password_hash",
                value: "teWqcWW3Ks4yNoq84+Akbx+4feKr/tp+ZVU2CjCbKwI=");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DefaultUser_DUS",
                keyColumn: "DUS_id",
                keyValue: 1,
                column: "DUS_password_hash",
                value: "");
        }
    }
}
