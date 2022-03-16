using Northwind.Core;
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
    }
}
