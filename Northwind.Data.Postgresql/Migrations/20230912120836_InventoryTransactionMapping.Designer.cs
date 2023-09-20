﻿// <auto-generated />
using System;
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
    [Migration("20230912120836_InventoryTransactionMapping")]
    partial class InventoryTransactionMapping
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

            modelBuilder.Entity("Northwind.Core.Entities.InventoryTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("serial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp");

                    b.Property<int>("ProductId")
                        .HasColumnType("serial");

                    b.Property<int>("PurchaseOrderId")
                        .HasColumnType("serial");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<short>("TransactionType")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("PurchaseOrderId");

                    b.ToTable("inventorytransactions", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("serial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("PostedToInventory")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseOrderId")
                        .HasColumnType("serial");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitCost")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("PurchaseOrderId");

                    b.ToTable("orderitems", (string)null);
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

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.Property<short>("TargetLevel")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.PurchaseOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("serial");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<short>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)10);

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("purchaseorders", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Shipper", b =>
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

                    b.ToTable("shippers", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.Supplier", b =>
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

                    b.ToTable("suppliers", (string)null);
                });

            modelBuilder.Entity("Northwind.Core.Entities.InventoryTransaction", b =>
                {
                    b.HasOne("Northwind.Core.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Northwind.Core.Entities.PurchaseOrder", null)
                        .WithMany()
                        .HasForeignKey("PurchaseOrderId");
                });

            modelBuilder.Entity("Northwind.Core.Entities.OrderItem", b =>
                {
                    b.HasOne("Northwind.Core.Entities.PurchaseOrder", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("PurchaseOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Northwind.Core.Entities.PurchaseOrder", b =>
                {
                    b.OwnsOne("Northwind.Core.ValueObjects.Payment", "Payment", b1 =>
                        {
                            b1.Property<int>("PurchaseOrderId")
                                .HasColumnType("serial");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Method")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("PurchaseOrderId");

                            b1.ToTable("purchaseorders");

                            b1.WithOwner()
                                .HasForeignKey("PurchaseOrderId");
                        });

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("Northwind.Core.Entities.PurchaseOrder", b =>
                {
                    b.Navigation("OrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}