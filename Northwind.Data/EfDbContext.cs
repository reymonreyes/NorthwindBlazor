using Microsoft.EntityFrameworkCore;
using Northwind.Core;
using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data
{
    public class EfDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=admin;Database=northwind").EnableDetailedErrors(true);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("EFProductIdHiLoSequence").IncrementsBy(1);
            modelBuilder.HasSequence<int>("EFCategoryIdHiLoSequence").IncrementsBy(1);
            modelBuilder.HasSequence<int>("EFSupplierIdHiloSequence").IncrementsBy(1);

            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().HasKey(x => x.Id).HasName("pk_products");
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("product_id").HasColumnType("smallint").IsRequired(true).UseHiLo("EFProductIdHiLoSequence");
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("product_name").HasColumnType("character varying").IsRequired(true);            
            modelBuilder.Entity<Product>().Property(x => x.Code).HasColumnName("code").HasColumnType("character varying").IsRequired(true);
            modelBuilder.Entity<Product>().Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("real");
            modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnName("quantity_per_unit").HasColumnType("character varying");
            modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("units_in_stock").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.UnitsInOrder).HasColumnName("units_on_order").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.ReorderLevel).HasColumnName("reorder_level").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.Discontinued).HasColumnName("discontinued").HasColumnType("integer");
            
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Category>().HasKey(x => x.Id).HasName("pk_categories");
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("category_id").HasColumnType("smallint").IsRequired(true).UseHiLo("EFCategoryIdHiLoSequence");
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("category_name").HasColumnType("character varying").IsRequired(true)
                .HasMaxLength(15);
            modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            modelBuilder.Entity<Category>().Ignore(x => x.Picture);

            modelBuilder.Entity<Supplier>().ToTable("suppliers");
            modelBuilder.Entity<Supplier>().HasKey(x => x.Id).HasName("pk_suppliers");
            modelBuilder.Entity<Supplier>().Property(x => x.Id).HasColumnName("supplier_id").HasColumnType("smallint").IsRequired(true).UseHiLo("EFSupplierIdHiloSequence");
            modelBuilder.Entity<Supplier>().Property(x => x.Name).HasColumnName("company_name").HasColumnType("character varying").HasMaxLength(40).IsRequired(true);
            modelBuilder.Entity<Supplier>().Property(x => x.ContactName).HasColumnName("contact_name").HasColumnType("character varying").HasMaxLength(30);
            modelBuilder.Entity<Supplier>().Property(x => x.ContactTitle).HasColumnName("contact_title").HasColumnType("character varying").HasMaxLength(30);
            modelBuilder.Entity<Supplier>().Property(x => x.Address).HasColumnName("address").HasColumnType("character varying").HasMaxLength(60);
            modelBuilder.Entity<Supplier>().Property(x => x.City).HasColumnName("city").HasColumnType("character varying").HasMaxLength(15);
            modelBuilder.Entity<Supplier>().Property(x => x.Region).HasColumnName("region").HasColumnType("character varying").HasMaxLength(15).HasDefaultValue(string.Empty);
            modelBuilder.Entity<Supplier>().Property(x => x.PostalCode).HasColumnName("postal_code").HasColumnType("character varying").HasMaxLength(15);
            modelBuilder.Entity<Supplier>().Property(x => x.Country).HasColumnName("country").HasColumnType("character varying").HasMaxLength(15);
            modelBuilder.Entity<Supplier>().Property(x => x.Phone).HasColumnName("phone").HasColumnType("character varying").HasMaxLength(24);
            modelBuilder.Entity<Supplier>().Property(x => x.Fax).HasColumnName("fax").HasColumnType("character varying").HasMaxLength(24);
            modelBuilder.Entity<Supplier>().Property(x => x.Homepage).HasColumnName("homepage").HasColumnType("text");
            modelBuilder.Entity<Supplier>().Property(x => x.Email).HasColumnName("email").HasColumnType("character varying").HasMaxLength(254);
        }
    }
}
