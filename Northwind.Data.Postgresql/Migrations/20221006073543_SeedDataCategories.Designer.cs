﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Northwind.Data.Postgresql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Northwind.Data.Postgresql.Migrations
{
    [DbContext(typeof(EfDbContext))]
    [Migration("20221006073543_SeedDataCategories")]
    partial class SeedDataCategories
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Northwind.Core.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallserial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("serial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasMaxLength(128)
                        .HasColumnType("varchar");

                    b.Property<string>("City")
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("ContactTitle")
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("Country")
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("varchar");

                    b.Property<string>("Fax")
                        .HasMaxLength(32)
                        .HasColumnType("varchar");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("varchar");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(32)
                        .HasColumnType("varchar");

                    b.Property<string>("State")
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("Website")
                        .HasMaxLength(128)
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("customers", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("serial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<short>("CategoryId")
                        .HasColumnType("smallint");

                    b.Property<string>("Code")
                        .HasMaxLength(64)
                        .HasColumnType("varchar");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("Discontinued")
                        .HasColumnType("boolean");

                    b.Property<float>("ListPrice")
                        .HasColumnType("real");

                    b.Property<short>("MinimumReorderQuantity")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar");

                    b.Property<string>("QuantityPerUnit")
                        .HasMaxLength(128)
                        .HasColumnType("varchar");

                    b.Property<short>("ReorderLevel")
                        .HasColumnType("smallint");

                    b.Property<float>("StandardCost")
                        .HasColumnType("real");

                    b.Property<short>("TargetLevel")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Shipper", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContactTitle")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Fax")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("shippers", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContactTitle")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Fax")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("suppliers", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
