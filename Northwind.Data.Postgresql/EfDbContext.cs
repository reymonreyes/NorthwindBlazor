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
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=admin;Database=northwind").EnableDetailedErrors(true);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCategoryEntity(modelBuilder);
            ConfigureCustomerEntity(modelBuilder);
            ConfigureSupplierEntity(modelBuilder);
            ConfigureShipperEntity(modelBuilder);
            ConfigureProductEntity(modelBuilder);
            ConfigurePurchaseOrderEntity(modelBuilder);
            ConfigureOrderItemEntity(modelBuilder);
            ConfigureInventoryTransactionEntity(modelBuilder);
            ConfigureCustomerOrderEntity(modelBuilder);
            ConfigureInvoiceEntity(modelBuilder);            
        }

        private void ConfigureInvoiceEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>().ToTable("invoices");
            modelBuilder.Entity<Invoice>().HasKey(x => x.Id);
            modelBuilder.Entity<Invoice>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<Invoice>().Property(x => x.InvoiceDate).HasColumnType("timestamp").IsRequired(true);
            modelBuilder.Entity<Invoice>().Property(x => x.DueDate).HasColumnType("timestamp").IsRequired(false);
            modelBuilder.Entity<Invoice>().Property(x => x.ShippingCost).HasColumnType("money");
            modelBuilder.Entity<CustomerOrder>().HasMany<Invoice>().WithOne().HasForeignKey(x => x.CustomerOrderId);
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
    
        private void ConfigurePurchaseOrderEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>().ToTable("purchaseorders").HasKey(x => x.Id);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.SupplierId).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<PurchaseOrder>().Property(x => x.Status).HasColumnType("smallint").HasDefaultValue(OrderStatus.New);
            modelBuilder.Entity<PurchaseOrder>().HasMany(x => x.OrderItems).WithOne().HasForeignKey(x => x.PurchaseOrderId).IsRequired();
            modelBuilder.Entity<PurchaseOrder>().OwnsOne(x => x.Payment);
        }

        private void ConfigureOrderItemEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().ToTable("orderitems").HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.ProductId).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.Quantity).HasColumnType("int").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.UnitCost).HasColumnType("decimal").IsRequired(true);
            modelBuilder.Entity<OrderItem>().Property(x => x.PostedToInventory).HasColumnType("boolean").HasDefaultValue(false);
        }
    
        private void ConfigureInventoryTransactionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryTransaction>().ToTable("inventorytransactions").HasKey(x => x.Id);
            modelBuilder.Entity<InventoryTransaction>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<InventoryTransaction>().Property(x => x.TransactionType).HasColumnType("smallint");
            modelBuilder.Entity<InventoryTransaction>().Property(x => x.Created).HasColumnType("timestamp");
            modelBuilder.Entity<InventoryTransaction>().Property(x => x.Quantity).HasColumnType("int");
            modelBuilder.Entity<InventoryTransaction>().Property(x => x.Comments).HasColumnType("text");
            modelBuilder.Entity<InventoryTransaction>().HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).IsRequired(false);
            modelBuilder.Entity<InventoryTransaction>().HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).IsRequired(false);
        }
    
        private void ConfigureCustomerOrderEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerOrder>().ToTable("customerorders").HasKey(x => x.Id);
            modelBuilder.Entity<CustomerOrder>().Property(x => x.Id).HasColumnType("serial").IsRequired(true);
            modelBuilder.Entity<CustomerOrder>().Property(x => x.OrderDate).HasColumnType("timestamp").IsRequired(true);
            modelBuilder.Entity<CustomerOrder>().Property(x => x.Notes).HasColumnType("text");
            modelBuilder.Entity<CustomerOrder>().Property(x => x.Status).HasColumnType("smallint");
            modelBuilder.Entity<CustomerOrderItem>().ToTable("customerorderitems").HasKey(x => x.Id);
            modelBuilder.Entity<CustomerOrderItem>().Property(x => x.Quantity).HasColumnType("int");
            modelBuilder.Entity<CustomerOrderItem>().Property(x => x.UnitPrice).HasColumnType("money");
            modelBuilder.Entity<CustomerOrderItem>().Property(x => x.Discount).HasColumnType("int");
            modelBuilder.Entity<CustomerOrderItem>().Property(x => x.DateAllocated).HasColumnType("timestamp").IsRequired(false);
            modelBuilder.Entity<CustomerOrderItem>().Property(x => x.Status).HasColumnType("smallint");

            modelBuilder.Entity<CustomerOrder>().HasMany(x => x.Items).WithOne().HasForeignKey(x => x.CustomerOrderId).IsRequired(true);
            modelBuilder.Entity<CustomerOrderItem>().HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).IsRequired(true);
            modelBuilder.Entity<PurchaseOrder>().HasMany<CustomerOrderItem>().WithOne().HasForeignKey(x => x.PurchaseOrderId).IsRequired(false);
            modelBuilder.Entity<InventoryTransaction>().HasMany<CustomerOrderItem>().WithOne().HasForeignKey(x => x.InventoryTransactionId).IsRequired(false);
            modelBuilder.Entity<Shipper>().HasMany<CustomerOrder>().WithOne().HasForeignKey(x => x.ShipperId).IsRequired(false);
            modelBuilder.Entity<CustomerOrder>().OwnsOne(x => x.ShipTo);
        }
    }
}
