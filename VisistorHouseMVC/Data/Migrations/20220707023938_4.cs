using Microsoft.EntityFrameworkCore.Migrations;

namespace VisistorHouseMVC.Data.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SavedNews");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SavedNews",
                type: "text",
                nullable: true);
        }
    }
}
