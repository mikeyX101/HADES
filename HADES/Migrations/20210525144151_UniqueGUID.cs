using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class UniqueGUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                columns: new[] { "ACF_log_delete_frequency", "ACF_log_max_file_size" },
                values: new object[] { 31, 100000000 });

            migrationBuilder.CreateIndex(
                name: "IX_User_USE_USE_guid",
                table: "User_USE",
                column: "USE_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminGroup_SUG_SUG_guid",
                table: "SuperAdminGroup_SUG",
                column: "SUG_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGroup_GRP_GRP_guid",
                table: "OwnerGroup_GRP",
                column: "GRP_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminGroup_ADG_ADG_guid",
                table: "AdminGroup_ADG",
                column: "ADG_guid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_USE_USE_guid",
                table: "User_USE");

            migrationBuilder.DropIndex(
                name: "IX_SuperAdminGroup_SUG_SUG_guid",
                table: "SuperAdminGroup_SUG");

            migrationBuilder.DropIndex(
                name: "IX_OwnerGroup_GRP_GRP_guid",
                table: "OwnerGroup_GRP");

            migrationBuilder.DropIndex(
                name: "IX_AdminGroup_ADG_ADG_guid",
                table: "AdminGroup_ADG");

            migrationBuilder.UpdateData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1,
                columns: new[] { "ACF_log_delete_frequency", "ACF_log_max_file_size" },
                values: new object[] { 1, 1 });
        }
    }
}
