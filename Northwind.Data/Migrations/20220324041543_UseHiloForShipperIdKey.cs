using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Migrations
{
    public partial class UseHiloForShipperIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateSequence<int>(
                name: "EFShipperIdHiloSequence",
                startValue: 7,
                incrementBy: 1);            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "EFShipperIdHiloSequence");            
        }
    }
}
