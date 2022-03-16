using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Migrations
{
    public partial class AddCodeAndDescriptionColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "products",
                type: "character varying",
                nullable: false,
                defaultValue: "NOTSET");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "products",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.DropColumn(
                name: "code",
                table: "products");

            migrationBuilder.DropColumn(
                name: "description",
                table: "products");            
        }
    }
}
