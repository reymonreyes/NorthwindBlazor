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

        public Task<CustomerOrder?> GetAsync(int customerOrderId)
        {
            return _efDbContext.CustomerOrders.FirstOrDefaultAsync(x => x.Id == customerOrderId);
        }
        public void Update(CustomerOrder customerOrder)
        {
            _efDbContext.CustomerOrders.Update(customerOrder);
        }
    }
}
