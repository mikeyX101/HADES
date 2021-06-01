using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class passwordencrypt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveDirectory_ADR",
                keyColumn: "ADR_id",
                keyValue: 1,
                column: "ADR_password_dn",
                value: "Ncr4Ix+48wVfeAC30A5agpX7PlcS18Zy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveDirectory_ADR",
                keyColumn: "ADR_id",
                keyValue: 1,
                column: "ADR_password_dn",
                value: "Toto123!");
        }
    }
}
