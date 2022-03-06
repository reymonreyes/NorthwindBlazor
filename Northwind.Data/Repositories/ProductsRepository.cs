using Microsoft.EntityFrameworkCore;
using Northwind.Common.Helpers;
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
        private readonly EfDbContext _dbContext;
        public ProductsRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
            
        }

        public void Create(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDto?> Get(int productId)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
            return ObjectMapperHelper.ToProductDto(product);
        }

        public ICollection<Product> GetAll()
        {
            return _dbContext.Products.ToList();
        }
    }
}
