using Microsoft.EntityFrameworkCore;
using Northwind.Core.Dtos;
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
            var customer = await Get(customerId);
            if (customer is not null)
                _dbContext.Customers.Remove(customer);
        }

        public async Task<IEnumerable<Customer>> Find(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName) || (!string.IsNullOrWhiteSpace(customerName) && customerName.Length < 2))
                return new List<Customer>();

            var results = new List<Customer>();
            results = await _dbContext.Customers.Where(x => x.Name.ToLower().Contains(customerName.ToLower())).ToListAsync();

            return results;
        }

        public Task<Customer?> Get(int customerId)
        {
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
