using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class AddPostedToInventoryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PostedToInventory",
                table: "orderitems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostedToInventory",
                table: "orderitems");
        }
    }
}
