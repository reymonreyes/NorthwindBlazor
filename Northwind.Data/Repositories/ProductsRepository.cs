using Microsoft.EntityFrameworkCore;
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

        public async Task Create(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public async Task<Product?> Get(int productId)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);            
        }

        public async Task<ICollection<Product>> GetAll()
        {
            return await _dbContext.Products.OrderBy(x => x.Name).ToListAsync();
        }

        public Task Update(Product product)
        {
            _dbContext.Update(product);
            return Task.CompletedTask;
        }
    }
}
