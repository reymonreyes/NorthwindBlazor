using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class MapSupplierIdOfProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "products",
                type: "int",
                nullable: false
            );
            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers",
                table: "products",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
