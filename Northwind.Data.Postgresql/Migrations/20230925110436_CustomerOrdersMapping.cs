using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOrdersMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customerorders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerorders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customerorderitems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CustomerOrderId = table.Column<int>(type: "serial", nullable: false),
                    ProductId = table.Column<int>(type: "serial", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    DateAllocated = table.Column<DateTime>(type: "timestamp", nullable: true),
                    PurchaseOrderId = table.Column<int?>(type: "int", nullable: true),
                    InventoryTransactionId = table.Column<int?>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerorderitems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customerorderitems_customerorders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "customerorders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customerorderitems_inventorytransactions_InventoryTransaction",
                        column: x => x.InventoryTransactionId,
                        principalTable: "inventorytransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_customerorderitems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customerorderitems_purchaseorders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "purchaseorders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_customerorderitems_CustomerOrderId",
                table: "customerorderitems",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_customerorderitems_InventoryTransactionId",
                table: "customerorderitems",
                column: "InventoryTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_customerorderitems_ProductId",
                table: "customerorderitems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_customerorderitems_PurchaseOrderId",
                table: "customerorderitems",
                column: "PurchaseOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customerorderitems");

            migrationBuilder.DropTable(
                name: "customerorders");
        }
    }
}
