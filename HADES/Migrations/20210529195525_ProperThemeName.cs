using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
    public partial class ProperThemeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserConfig_UCF",
                keyColumn: "UCF_id",
                keyValue: 1,
                column: "UCF_theme_file",
                value: "site");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserConfig_UCF",
                keyColumn: "UCF_id",
                keyValue: 1,
                column: "UCF_theme_file",
                value: "site.css");
        }
    }
}
