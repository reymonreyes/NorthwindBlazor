using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class ProductsService : IProductsService
    {
        private ICollection<ProductDto> _products = new List<ProductDto>
        {
            new ProductDto{ Id = 1, Name = "Red T-Shirt", Code = "RTS", Description = "Red colored t-shirt."},
            new ProductDto{ Id = 2, Name = "Orange T-Shirt", Code = "OTS"},
            new ProductDto { Id = 1, Name = "Yellow T-Shirt", Code = "YTS" },
            new ProductDto { Id = 1, Name = "Green T-Shirt", Code = "GTS" },
            new ProductDto { Id = 1, Name = "Blue T-Shirt", Code = "BTS" },
            new ProductDto { Id = 1, Name = "Indigo T-Shirt", Code = "ITS" },
            new ProductDto { Id = 1, Name = "Violet T-Shirt", Code = "VTS" },
        };

        public ProductDto Get()
        {
            throw new NotImplementedException();
        }

        public ICollection<ProductDto> GetAll()
        {            
            return _products;
        }
    }
}
