using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ADR_root_ou",
                table: "ActiveDirectory_ADR",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADR_root_ou",
                table: "ActiveDirectory_ADR");
        }
    }
}
