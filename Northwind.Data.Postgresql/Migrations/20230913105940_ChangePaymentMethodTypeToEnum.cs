using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    public partial class ChangePaymentMethodTypeToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment_Method",
                table: "purchaseorders"
            );

            migrationBuilder.AddColumn<int>(
                name: "Payment_Method",
                table: "purchaseorders",
                type: "smallint"
            );            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Payment_Method",
                table: "purchaseorders",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
