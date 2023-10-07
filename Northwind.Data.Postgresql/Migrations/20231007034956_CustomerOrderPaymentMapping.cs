using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOrderPaymentMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Payment_Amount",
                table: "customerorders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Payment_Date",
                table: "customerorders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Payment_Method",
                table: "customerorders",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment_Amount",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "Payment_Date",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "Payment_Method",
                table: "customerorders");
        }
    }
}
