using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class InventoryTransactionMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventorytransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    TransactionType = table.Column<short>(type: "smallint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ProductId = table.Column<int>(type: "serial", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "serial", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventorytransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inventorytransactions_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_inventorytransactions_purchaseorders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "purchaseorders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventorytransactions_ProductId",
                table: "inventorytransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inventorytransactions_PurchaseOrderId",
                table: "inventorytransactions",
                column: "PurchaseOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventorytransactions");
        }
    }
}
