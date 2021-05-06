using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class HadesAccessRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ROL_access_hades",
                table: "Role_ROL",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 1,
                column: "ROL_access_hades",
                value: true);

            migrationBuilder.UpdateData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 2,
                column: "ROL_access_hades",
                value: true);

            migrationBuilder.UpdateData(
                table: "Role_ROL",
                keyColumn: "ROL_id",
                keyValue: 3,
                column: "ROL_access_hades",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ROL_access_hades",
                table: "Role_ROL");
        }
    }
}
