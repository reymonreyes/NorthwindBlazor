using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class CustomerOrderItemsRepository : ICustomerOrderItemsRepository
    {
        private readonly EfDbContext _efDbContext;
        public CustomerOrderItemsRepository(EfDbContext efDbContext) 
        {
            _efDbContext = efDbContext;
        }
        public async Task<CustomerOrderItem?> GetAsync(int id)
        {
            var customerOrder = await _efDbContext.CustomerOrderItems.FindAsync(id);
            return customerOrder;
        }

        public Task UpdateAsync(CustomerOrderItem item)
        {
            _efDbContext.CustomerOrderItems.Update(item);

            return Task.CompletedTask;
        }
    }
}
