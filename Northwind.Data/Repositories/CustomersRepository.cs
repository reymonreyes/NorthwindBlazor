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
    public class CustomersRepository : ICustomersRepository
    {
        private readonly EfDbContext _dbContext;
        public CustomersRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICollection<Customer>> GetAll()
        {
            return await _dbContext.Customers.ToListAsync();
        }
    }
}
