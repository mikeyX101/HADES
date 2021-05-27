using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
    public partial class stable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                columns: new[] { "ACF_company_background_file", "ACF_company_logo_file" },
                values: new object[] { "", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                columns: new[] { "ACF_company_background_file", "ACF_company_logo_file" },
                values: new object[] { "background.png", "logo.png" });
        }
    }
}
