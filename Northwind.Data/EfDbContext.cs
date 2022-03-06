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
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=admin;Database=northwind");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("product_id").HasColumnType("smallint");
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("product_name").HasColumnType("character varying");
            modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("real");
            modelBuilder.Entity<Product>().Ignore(x => x.Code);
            modelBuilder.Entity<Product>().Ignore(x => x.Description);
        }
    }
}
