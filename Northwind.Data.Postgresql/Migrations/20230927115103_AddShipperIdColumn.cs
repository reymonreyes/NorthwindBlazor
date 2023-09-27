using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddShipperIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int?>(
                name: "ShipperId",
                table: "customerorders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_customerorders_ShipperId",
                table: "customerorders",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_customerorders_shippers_ShipperId",
                table: "customerorders",
                column: "ShipperId",
                principalTable: "shippers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customerorders_shippers_ShipperId",
                table: "customerorders");

            migrationBuilder.DropIndex(
                name: "IX_customerorders_ShipperId",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "customerorders");
        }
    }
}
