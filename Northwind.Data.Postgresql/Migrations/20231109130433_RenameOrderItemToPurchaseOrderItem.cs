using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderItemToPurchaseOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.RenameTable("orderitems", newName: "purchaseorderitems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("purchaseorderitems", newName: "orderitems");
        }
    }
}
