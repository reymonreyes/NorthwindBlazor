using Northwind.Core.Exceptions;
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
        private EfDbContext _dbContext;
        public EfUnitOfWork()
        {
            _dbContext = null!;
        }
        public IProductsRepository ProductsRepository => new ProductsRepository(_dbContext);
        public ICategoriesRepository CategoriesRepository => new CategoriesRepository(_dbContext);        
        public ISuppliersRepository SuppliersRepository => new SuppliersRepository(_dbContext);
        public IShippersRepository ShippersRepository => new ShippersRepository(_dbContext);
        public ICustomersRepository CustomersRepository => new CustomersRepository(_dbContext);

        public async Task Commit()
        {            
            await _dbContext.SaveChangesAsync();
        }

        public Task Start()
        {
            _dbContext = new EfDbContext();
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
