using Microsoft.EntityFrameworkCore.Migrations;
using System.Xml.Linq;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class SeedDataCustomers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string dir = Environment.CurrentDirectory;
            string file = Path.Combine(dir, "SeedData", "northwind.tables.customers.xml");
            var xmlDoc = XElement.Load(file);
            var records = xmlDoc.Elements("record");
            if (records != null)
            {
                foreach (var record in records)
                {
                    migrationBuilder.InsertData(
                        table: "customers",
                        columns: new string[] { "Name", "ContactName", "ContactTitle", "Address", "City", "State", "PostalCode", "Country", "Phone", "Fax", "Website", "Email" },
                        values: new object[] {
                            record.Element("Name")?.Value,
                            record.Element("ContactName")?.Value,
                            record.Element("ContactTitle")?.Value,
                            record.Element("Address")?.Value,
                            record.Element("City")?.Value,
                            record.Element("State").Value == "NULL" ? null : record.Element("State").Value,
                            record.Element("PostalCode")?.Value,
                            record.Element("Country")?.Value,
                            record.Element("Phone")?.Value,
                            record.Element("Fax").Value == "NULL" ? null : record.Element("Fax").Value,
                            record.Element("Website")?.Value,
                            record.Element("Email")?.Value,
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
