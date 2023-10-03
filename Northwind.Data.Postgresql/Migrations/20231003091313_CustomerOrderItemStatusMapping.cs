using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOrderItemStatusMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "customerorders",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "customerorderitems",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "customerorderitems");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                table: "inventorytransactions",
                type: "serial",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_inventorytransactions_PurchaseOrderId",
                table: "inventorytransactions",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventorytransactions_purchaseorders_PurchaseOrderId",
                table: "inventorytransactions",
                column: "PurchaseOrderId",
                principalTable: "purchaseorders",
                principalColumn: "Id");
        }
    }
}
