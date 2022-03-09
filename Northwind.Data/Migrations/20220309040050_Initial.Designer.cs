﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Northwind.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Migrations
{
    [DbContext(typeof(EfDbContext))]
    [Migration("20220309040050_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("EntityFrameworkHiLoSequence")
                .IncrementsBy(10);

            modelBuilder.Entity("Northwind.Core.Entities.Product", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("product_id");

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<short>("Id"), "EntityFrameworkHiLoSequence");

                    b.Property<string>("Code")
                        .HasColumnType("character varying")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("Discontinued")
                        .HasColumnType("integer")
                        .HasColumnName("discontinued");

                    b.Property<string>("Name")
                        .HasColumnType("character varying")
                        .HasColumnName("product_name");

                    b.Property<string>("QuantityPerUnit")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("quantity_per_unit");

                    b.Property<short>("ReorderLevel")
                        .HasColumnType("smallint")
                        .HasColumnName("reorder_level");

                    b.Property<float>("UnitPrice")
                        .HasColumnType("real")
                        .HasColumnName("unit_price");

                    b.Property<short>("UnitsInOrder")
                        .HasColumnType("smallint")
                        .HasColumnName("units_on_order");

                    b.Property<short>("UnitsInStock")
                        .HasColumnType("smallint")
                        .HasColumnName("units_in_stock");

                    b.HasKey("Id")
                        .HasName("pk_products");

                    b.ToTable("products", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
