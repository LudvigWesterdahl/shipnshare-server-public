using Microsoft.EntityFrameworkCore.Migrations;

namespace ShipWithMeInfrastructure.Migrations
{
    public partial class Rpw2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResetPasswordKeyCreatedAt",
                table: "AspNetUsers",
                type: "nvarchar(48)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResetPasswordKeyCreatedAt",
                table: "AspNetUsers",
                type: "nvarchar(48)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(48)",
                oldNullable: true);
        }
    }
}
