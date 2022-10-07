using Microsoft.EntityFrameworkCore.Migrations;
using System.Xml.Linq;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class SeedDataProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string dir = Environment.CurrentDirectory;
            string file = Path.Combine(dir, "SeedData", "northwind.tables.products.xml");
            var xmlDoc = XElement.Load(file);
            var records = xmlDoc.Elements("record");
            if (records != null)
            {
                foreach (var record in records)
                {
                    migrationBuilder.InsertData(
                        table: "products",
                        columns: new string[] { "Name", "Code", "Description", "StandardCost", "ListPrice", "ReorderLevel", "TargetLevel", "QuantityPerUnit", "Discontinued", "MinimumReorderQuantity", "CategoryId", "SupplierId" },
                        values: new object[] {
                            record.Element("Name")?.Value,
                            null,
                            record.Element("Description")?.Value,
                            record.Element("StandardCost")?.Value,
                            record.Element("ListPrice")?.Value,
                            record.Element("ReorderLevel")?.Value,
                            record.Element("TargetLevel")?.Value,
                            record.Element("QuantityPerUnit")?.Value,
                            record.Element("Discontinued")?.Value == "1" ? true : false,
                            record.Element("MinimumReorderQuantity")?.Value,
                            record.Element("CategoryId")?.Value,
                            record.Element("SupplierId")?.Value
                        }
                    );
                }
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
