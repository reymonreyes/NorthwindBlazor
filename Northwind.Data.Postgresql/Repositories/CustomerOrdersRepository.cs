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
    public class CustomerOrdersRepository : ICustomerOrdersRepository
    {
        private readonly EfDbContext _efDbContext;

        public CustomerOrdersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }
        public async Task CreateAsync(CustomerOrder customerOrder)
        {
            await _efDbContext.CustomerOrders.AddAsync(customerOrder);
        }

        public async Task<(int TotalRecords, IEnumerable<CustomerOrder> Records)> GetAllAsync(int page = 1, int size = 10)
        {
            var results = await _efDbContext.CustomerOrders.Skip((page - 1) * size).Take(size).ToListAsync();
            var totalRecords = await _efDbContext.CustomerOrders.CountAsync();
            return (totalRecords, results);
        }

        public Task<CustomerOrder?> GetAsync(int customerOrderId)
        {
            return _efDbContext.CustomerOrders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == customerOrderId);
        }
        public void Update(CustomerOrder customerOrder)
        {
            _efDbContext.CustomerOrders.Update(customerOrder);
        }
    }
}
