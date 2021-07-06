using Microsoft.EntityFrameworkCore.Migrations;

namespace ShipWithMeInfrastructure.Migrations
{
    public partial class ResetPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordKeyCreatedAt",
                table: "AspNetUsers",
                type: "nvarchar(48)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetPasswordKeyCreatedAt",
                table: "AspNetUsers");
        }
    }
}
