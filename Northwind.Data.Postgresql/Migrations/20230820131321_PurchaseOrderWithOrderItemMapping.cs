using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class PurchaseOrderWithOrderItemMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchaseorders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)10),
                    Payment_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Payment_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Payment_Method = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseorders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orderitems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "serial", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderitems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orderitems_purchaseorders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "purchaseorders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orderitems_PurchaseOrderId",
                table: "orderitems",
                column: "PurchaseOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderitems");

            migrationBuilder.DropTable(
                name: "purchaseorders");
        }
    }
}
