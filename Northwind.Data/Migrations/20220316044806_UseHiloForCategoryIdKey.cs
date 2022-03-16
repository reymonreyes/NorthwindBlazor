using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Migrations
{
    public partial class UseHiloForCategoryIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateSequence(
                name: "EFCategoryIdHiLoSequence",
                incrementBy: 100,
                startValue: 1);                       
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "EFCategoryIdHiLoSequence");            
        }
    }
}
