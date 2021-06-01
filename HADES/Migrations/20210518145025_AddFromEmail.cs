using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class AddFromEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ACF_SMTP_from_email",
                table: "AppConfig_ACF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                column: "ACF_SMTP_from_email",
                value: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACF_SMTP_from_email",
                table: "AppConfig_ACF");
        }
    }
}
