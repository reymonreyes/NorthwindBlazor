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
        private readonly EfDbContext _efDbContext;
        public SuppliersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public async Task<Supplier?> Get(int supplierId)
        {
            return await _efDbContext.Suppliers.FirstOrDefaultAsync(x => x.Id == supplierId);   
        }

        public async Task<ICollection<Supplier>> GetAll()
        {
            return await _efDbContext.Suppliers.OrderBy(x => x.Name).ToListAsync();
        }
    }
}
