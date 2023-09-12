using Microsoft.EntityFrameworkCore;
using Northwind.Core;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql
{
    public class EfDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Shipper> Shippers => Set<Shipper>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=admin;Database=northwind").EnableDetailedErrors(true);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCategoryEntity(modelBuilder);
            ConfigureCustomerEntity(modelBuilder);
            ConfigureSupplierEntity(modelBuilder);
            ConfigureShipperEntity(modelBuilder);
            ConfigureProductEntity(modelBuilder);

            modelBuilder.Entity<PurchaseOrder>().ToTable("purchaseorders").HasKey(x => x.Id);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.SupplierId).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.Status).HasColumnType("smallint").HasDefaultValue(OrderStatus.New);
            modelBuilder.Entity<PurchaseOrder>().HasMany(x => x.OrderItems).WithOne().HasForeignKey(x => x.PurchaseOrderId).IsRequired();
            modelBuilder.Entity<PurchaseOrder>().OwnsOne(x => x.Payment);

            modelBuilder.Entity<OrderItem>().ToTable("orderitems").HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.ProductId).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.Quantity).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.UnitCost).HasColumnType("decimal").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.PostedToInventory).HasColumnType("boolean").HasDefaultValue(false);
        }

        private void ConfigureProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnType("varchar").HasMaxLength(128).IsRequired(true);
            modelBuilder.Entity<Product>().Property(x => x.Code).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Product>().Property(x => x.Description).HasColumnType("text");
            modelBuilder.Entity<Product>().Property(x => x.StandardCost).HasColumnType("real");
            modelBuilder.Entity<Product>().Property(x => x.ListPrice).HasColumnType("real");
            modelBuilder.Entity<Product>().Property(x => x.ReorderLevel).HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.TargetLevel).HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnType("varchar").HasMaxLength(128);            
            modelBuilder.Entity<Product>().Property(x => x.Discontinued).HasColumnType("boolean");
            modelBuilder.Entity<Product>().Property(x => x.MinimumReorderQuantity).HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.SupplierId).HasColumnType("int");
        }

        private void ConfigureCategoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Category>().HasKey(x => x.Id);
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnType("smallserial").IsRequired(true);
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnType("text");
        }

        private void ConfigureSupplierEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().ToTable("suppliers");
            modelBuilder.Entity<Supplier>().HasKey(x => x.Id);
            modelBuilder.Entity<Supplier>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<Supplier>().Property(x => x.Name).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Supplier>().Property(x => x.Email).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Supplier>().Property(x => x.ContactName).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Supplier>().Property(x => x.ContactTitle).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Supplier>().Property(x => x.Address).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Supplier>().Property(x => x.City).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Supplier>().Property(x => x.State).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Supplier>().Property(x => x.PostalCode).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Supplier>().Property(x => x.Country).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Supplier>().Property(x => x.Phone).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Supplier>().Property(x => x.Fax).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Supplier>().Property(x => x.Website).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Supplier>().Property(x => x.Notes).HasColumnType("text");
        }

        private void ConfigureShipperEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipper>().ToTable("shippers");
            modelBuilder.Entity<Shipper>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<Shipper>().Property(x => x.Name).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Shipper>().Property(x => x.Email).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Shipper>().Property(x => x.ContactName).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Shipper>().Property(x => x.ContactTitle).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Shipper>().Property(x => x.Address).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Shipper>().Property(x => x.City).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Shipper>().Property(x => x.State).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Shipper>().Property(x => x.PostalCode).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Shipper>().Property(x => x.Country).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Shipper>().Property(x => x.Phone).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Shipper>().Property(x => x.Fax).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Shipper>().Property(x => x.Website).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Shipper>().Property(x => x.Notes).HasColumnType("text");
        }

        private void ConfigureCustomerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("customers");
            modelBuilder.Entity<Customer>().HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<Customer>().Property(x => x.Name).HasColumnType("varchar").HasMaxLength(64).IsRequired(true);
            modelBuilder.Entity<Customer>().Property(x => x.Email).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Customer>().Property(x => x.ContactName).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(x => x.ContactTitle).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(x => x.Address).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Customer>().Property(x => x.City).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(x => x.State).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(x => x.PostalCode).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Customer>().Property(x => x.Country).HasColumnType("varchar").HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(x => x.Phone).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Customer>().Property(x => x.Fax).HasColumnType("varchar").HasMaxLength(32);
            modelBuilder.Entity<Customer>().Property(x => x.Website).HasColumnType("varchar").HasMaxLength(128);
            modelBuilder.Entity<Customer>().Property(x => x.Notes).HasColumnType("text");
        }
    }
}
