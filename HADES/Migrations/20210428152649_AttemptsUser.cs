using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace HADES.Migrations
{
	public partial class AttemptsUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "USE_attempts",
                table: "User_USE",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "USE_date",
                table: "User_USE",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "USE_attempts",
                table: "DefaultUser_DUS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "USE_date",
                table: "DefaultUser_DUS",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USE_attempts",
                table: "User_USE");

            migrationBuilder.DropColumn(
                name: "USE_date",
                table: "User_USE");

            migrationBuilder.DropColumn(
                name: "USE_attempts",
                table: "DefaultUser_DUS");

            migrationBuilder.DropColumn(
                name: "USE_date",
                table: "DefaultUser_DUS");
        }
    }
}
