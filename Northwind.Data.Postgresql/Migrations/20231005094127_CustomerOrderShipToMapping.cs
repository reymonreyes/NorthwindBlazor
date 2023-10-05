using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOrderShipToMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipTo_Address",
                table: "customerorders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipTo_Name",
                table: "customerorders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipTo_Address",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "ShipTo_Name",
                table: "customerorders");
        }
    }
}
