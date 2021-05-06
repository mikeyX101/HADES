using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class guid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "USE_sam_account_name",
                table: "User_USE",
                newName: "USE_guid");

            migrationBuilder.RenameColumn(
                name: "SUG_sam_account_name",
                table: "SuperAdminGroup_SUG",
                newName: "SUG_guid");

            migrationBuilder.RenameColumn(
                name: "GRP_sam_account_name",
                table: "OwnerGroup_GRP",
                newName: "GRP_guid");

            migrationBuilder.RenameColumn(
                name: "ADG_sam_account_name",
                table: "AdminGroup_ADG",
                newName: "ADG_guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "USE_guid",
                table: "User_USE",
                newName: "USE_sam_account_name");

            migrationBuilder.RenameColumn(
                name: "SUG_guid",
                table: "SuperAdminGroup_SUG",
                newName: "SUG_sam_account_name");

            migrationBuilder.RenameColumn(
                name: "GRP_guid",
                table: "OwnerGroup_GRP",
                newName: "GRP_sam_account_name");

            migrationBuilder.RenameColumn(
                name: "ADG_guid",
                table: "AdminGroup_ADG",
                newName: "ADG_sam_account_name");
        }
    }
}
