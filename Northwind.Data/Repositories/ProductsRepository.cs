using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private ICollection<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Red T-Shirt", Code = "RTS", Description = "Red colored t-shirt."},
            new Product { Id = 2, Name = "Orange T-Shirt", Code = "OTS"},
            new Product { Id = 1, Name = "Yellow T-Shirt", Code = "YTS" },
            new Product { Id = 1, Name = "Green T-Shirt", Code = "GTS" },
            new Product { Id = 1, Name = "Blue T-Shirt", Code = "BTS" },
            new Product { Id = 1, Name = "Indigo T-Shirt", Code = "ITS" },
            new Product { Id = 1, Name = "Violet T-Shirt", Code = "VTS" },
        };

        public void Create(Product product)
        {
            throw new NotImplementedException();
        }

        public ICollection<Product> GetAll()
        {
            return _products;
        }
    }
}
