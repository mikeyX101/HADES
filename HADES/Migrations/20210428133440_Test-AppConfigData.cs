using Microsoft.EntityFrameworkCore.Migrations;

namespace HADES.Migrations
{
	public partial class TestAppConfigData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ActiveDirectory_ADR",
                columns: new[] { "ADR_id", "ADR_account_dn", "ADR_base_dn", "ADR_connection_filter", "ADR_password_dn", "ADR_port_number", "ADR_root_ou", "ADR_server_address", "ADR_sync_field" },
                values: new object[] { 1, "CN=hades,CN=Users,DC=R991-AD,DC=lan", "CN=Users,DC=R991-AD,DC=lan", "(&(objectClass=user)(objectCategory=person))", "Toto123!", 389, "OU=hades_root,DC=R991-AD,DC=lan", "172.20.48.10", "samaccountName" });

            migrationBuilder.InsertData(
                table: "AppConfig_ACF",
                columns: new[] { "ACF_id", "ACF_ADR_id", "ACF_company_background_file", "ACF_company_logo_file", "ACF_company_name", "ACF_default_language", "ACF_log_delete_frequency", "ACF_log_max_file_size", "ACF_SMTP" },
                values: new object[] { 1, 1, "background.png", "logo.png", "YourCompanyName", "fr-CA", 1, 1, "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppConfig_ACF",
                keyColumn: "ACF_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ActiveDirectory_ADR",
                keyColumn: "ADR_id",
                keyValue: 1);
        }
    }
}
