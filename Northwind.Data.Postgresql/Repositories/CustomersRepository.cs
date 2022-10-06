using Microsoft.EntityFrameworkCore;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
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

        public async Task Delete(int customerId)
        {
            //if (!string.IsNullOrWhiteSpace(customerId))-revisit
            //{
                var customer = await Get(customerId);
                if (customer is not null)
                    _dbContext.Customers.Remove(customer);
            //}
        }

        public Task<Customer?> Get(int customerId)
        {
            //if (string.IsNullOrEmpty(customerId))-revisit
            //    return null!;

            return _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        }

        public async Task<ICollection<Customer>> GetAll()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public Task Update(Customer customer)
        {
            _dbContext.Update(customer);
            return Task.CompletedTask;
        }
    }
}
