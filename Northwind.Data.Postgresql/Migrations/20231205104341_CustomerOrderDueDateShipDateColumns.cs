using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOrderDueDateShipDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "customerorders",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipDate",
                table: "customerorders",
                type: "timestamp",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "customerorders");

            migrationBuilder.DropColumn(
                name: "ShipDate",
                table: "customerorders");
        }
    }
}
