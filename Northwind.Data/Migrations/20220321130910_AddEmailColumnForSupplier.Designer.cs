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
    [Migration("20220321130910_AddEmailColumnForSupplier")]
    partial class AddEmailColumnForSupplier
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("EFCategoryIdHiLoSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("EFProductIdHiLoSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("EFSupplierIdHiloSequence")
                .IncrementsBy(10);

            modelBuilder.Entity("Northwind.Core.Entities.Category", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("category_id");

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<short>("Id"), "EFCategoryIdHiLoSequence");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying")
                        .HasColumnName("category_name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Product", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("product_id");

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<short>("Id"), "EFProductIdHiLoSequence");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("Discontinued")
                        .HasColumnType("integer")
                        .HasColumnName("discontinued");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("product_name");

                    b.Property<string>("QuantityPerUnit")
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

            modelBuilder.Entity("Northwind.Core.Entities.Supplier", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("supplier_id");

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<short>("Id"), "EFSupplierIdHiloSequence");

                    b.Property<string>("Address")
                        .HasMaxLength(60)
                        .HasColumnType("character varying")
                        .HasColumnName("address");

                    b.Property<string>("City")
                        .HasMaxLength(15)
                        .HasColumnType("character varying")
                        .HasColumnName("city");

                    b.Property<string>("ContactName")
                        .HasMaxLength(30)
                        .HasColumnType("character varying")
                        .HasColumnName("contact_name");

                    b.Property<string>("ContactTitle")
                        .HasMaxLength(30)
                        .HasColumnType("character varying")
                        .HasColumnName("contact_title");

                    b.Property<string>("Country")
                        .HasMaxLength(15)
                        .HasColumnType("character varying")
                        .HasColumnName("country");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(254)
                        .HasColumnType("character varying")
                        .HasColumnName("email");

                    b.Property<string>("Fax")
                        .HasMaxLength(24)
                        .HasColumnType("character varying")
                        .HasColumnName("fax");

                    b.Property<string>("Homepage")
                        .HasColumnType("text")
                        .HasColumnName("homepage");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying")
                        .HasColumnName("company_name");

                    b.Property<string>("Phone")
                        .HasMaxLength(24)
                        .HasColumnType("character varying")
                        .HasColumnName("phone");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(15)
                        .HasColumnType("character varying")
                        .HasColumnName("postal_code");

                    b.Property<string>("Region")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(15)
                        .HasColumnType("character varying")
                        .HasDefaultValue("")
                        .HasColumnName("region");

                    b.HasKey("Id")
                        .HasName("pk_suppliers");

                    b.ToTable("suppliers", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
