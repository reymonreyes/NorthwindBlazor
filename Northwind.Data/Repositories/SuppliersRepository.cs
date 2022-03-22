using Microsoft.EntityFrameworkCore;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Repositories
{
    public class SuppliersRepository : ISuppliersRepository
    {
        private readonly EfDbContext _dbContext;
        public SuppliersRepository(EfDbContext efDbContext)
        {
            _dbContext = efDbContext;
        }

        public async Task Create(Supplier supplier)
        {
            await _dbContext.Suppliers.AddAsync(supplier);
        }

        public async Task<Supplier?> Get(int supplierId)
        {
            return await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);   
        }

        public async Task<ICollection<Supplier>> GetAll()
        {
            return await _dbContext.Suppliers.OrderBy(x => x.Name).ToListAsync();
        }
    }
}
