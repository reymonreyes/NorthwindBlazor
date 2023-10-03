using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerAndPurchaseOrderIdInventoryTransactionMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerOrderId",
                table: "inventorytransactions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventorytransactions_purchaseorders_PurchaseOrderId",
                table: "inventorytransactions");

            migrationBuilder.DropIndex(
                name: "IX_inventorytransactions_PurchaseOrderId",
                table: "inventorytransactions");

            migrationBuilder.DropColumn(
                name: "CustomerOrderId",
                table: "inventorytransactions");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "inventorytransactions");
        }
    }
}
