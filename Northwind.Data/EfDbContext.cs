using Microsoft.EntityFrameworkCore;
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=admin;Database=northwind");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().HasKey(x => x.Id).HasName("pk_products");
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("product_id").HasColumnType("smallint").UseHiLo();
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("product_name").HasColumnType("character varying").IsRequired(true);
            modelBuilder.Entity<Product>().Property(x => x.Code).HasColumnName("code").HasColumnType("character varying").IsRequired(true);
            modelBuilder.Entity<Product>().Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("real");
            modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnName("quantity_per_unit").HasColumnType("character varying");
            modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("units_in_stock").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.UnitsInOrder).HasColumnName("units_on_order").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.ReorderLevel).HasColumnName("reorder_level").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.Discontinued).HasColumnName("discontinued").HasColumnType("integer");
        }
    }
}
