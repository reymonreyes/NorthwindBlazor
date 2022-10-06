using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "smallserial", nullable: false),
                    Name = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    Name = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    ContactName = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    ContactTitle = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "varchar", maxLength: 128, nullable: true),
                    Address = table.Column<string>(type: "varchar", maxLength: 128, nullable: true),
                    City = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    State = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    PostalCode = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    Country = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    Fax = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    Website = table.Column<string>(type: "varchar", maxLength: 128, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    Name = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StandardCost = table.Column<float>(type: "real", nullable: false),
                    ListPrice = table.Column<float>(type: "real", nullable: false),
                    ReorderLevel = table.Column<short>(type: "smallint", nullable: false),
                    TargetLevel = table.Column<short>(type: "smallint", nullable: false),
                    QuantityPerUnit = table.Column<string>(type: "varchar", maxLength: 128, nullable: true),
                    Discontinued = table.Column<bool>(type: "boolean", nullable: false),
                    MinimumReorderQuantity = table.Column<short>(type: "smallint", nullable: false),
                    CategoryId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shippers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactTitle = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shippers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "serial", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactTitle = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "shippers");

            migrationBuilder.DropTable(
                name: "suppliers");
        }
    }
}
