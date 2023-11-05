using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseOrderNotesPropertyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "purchaseorders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "purchaseorders");
        }
    }
}
