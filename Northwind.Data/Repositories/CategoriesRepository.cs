using Microsoft.EntityFrameworkCore;
using Northwind.Core;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly EfDbContext _dbContext;
        public CategoriesRepository(EfDbContext efDbContext)
        {
            _dbContext = efDbContext;
        }
        public async Task Create(Category category)
        {
            await _dbContext.Categories.AddAsync(category);            
        }

        public async Task Delete(int categoryId)
        {
            var category = await Get(categoryId);
            if (category is null)
                throw new DataNotFoundException("Category data not found.");

            _dbContext.Categories.Remove(category);
        }

        public async Task<Category?> Get(int categoryId)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);            
        }

        public async Task<ICollection<Category>> GetAll()
        {
            return await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        public Task Update(Category category)
        {
            _dbContext.Update(category);
            return Task.CompletedTask;
        }
    }
}
