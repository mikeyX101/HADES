using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class SMTPConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ACF_SMTP",
                table: "AppConfig_ACF",
                newName: "ACF_SMTP_username");

            migrationBuilder.AddColumn<string>(
                name: "ACF_SMTP_password",
                table: "AppConfig_ACF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ACF_SMTP_port",
                table: "AppConfig_ACF",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ACF_SMTP_server",
                table: "AppConfig_ACF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                columns: new[] { "ACF_SMTP_server", "ACF_SMTP_password", "ACF_SMTP_port", "ACF_SMTP_server" },
                values: new object[] { "", "", 465, "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACF_SMTP_password",
                table: "AppConfig_ACF");

            migrationBuilder.DropColumn(
                name: "ACF_SMTP_port",
                table: "AppConfig_ACF");

            migrationBuilder.DropColumn(
                name: "ACF_SMTP_server",
                table: "AppConfig_ACF");

            migrationBuilder.RenameColumn(
                name: "ACF_SMTP_username",
                table: "AppConfig_ACF",
                newName: "ACF_SMTP");
        }
    }
}
