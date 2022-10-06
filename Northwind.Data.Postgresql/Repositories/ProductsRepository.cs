﻿using Microsoft.EntityFrameworkCore;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Data.Postgresql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postresql.Repositories
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

        public async Task Delete(int productId)
        {
            var product = await Get(productId);
            if (product is not null)
                _dbContext.Products.Remove(product);
        }

        public async Task<Product?> Get(int productId)
        {
            var result = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
            return result;
        }

        public async Task<ICollection<Product>> GetAll()
        {
            var result = await _dbContext.Products.OrderBy(x => x.Name).ToListAsync();
            return result;
        }

        public Task Update(Product product)
        {
            _dbContext.Update(product);
            return Task.CompletedTask;
        }
    }
}
