using Northwind.Core.Interfaces.Repositories;
using Northwind.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly EfDbContext _dbContext;
        public EfUnitOfWork(EfDbContext efDbContext)
        {
            _dbContext = efDbContext;
        }

        public IProductsRepository ProductsRepository => new ProductsRepository(_dbContext);

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
