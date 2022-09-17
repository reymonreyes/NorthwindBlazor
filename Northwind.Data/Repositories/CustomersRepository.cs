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

        public async Task Create(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);            
        }

        public Task<Customer?> Get(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return null!;

            return _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        }

        public async Task<ICollection<Customer>> GetAll()
        {
            return await _dbContext.Customers.ToListAsync();
        }


    }
}
