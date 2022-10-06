using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class SeedDataCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Beverages", "Soft drinks, coffees, teas, beers, and ales" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Condiments", "Sweet and savory sauces, relishes, spreads, and seasonings" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Confections", "Desserts, candies, and sweet breads" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Dairy Products", "Cheeses" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Grains/Cereals", "Breads, crackers, pasta, and cereal" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Meat/Poultry", "Prepared meats" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Produce", "Dried fruit and bean curd" });
            migrationBuilder.InsertData(table: "categories",
                columns: new string[] { "Name", "Description" },
                values: new object[] { "Seafood", "Seaweed and fish" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
