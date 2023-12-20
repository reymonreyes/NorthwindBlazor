using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface ICustomerOrderItemsRepository
    {
        Task CreateAsync(CustomerOrderItem item);
        Task DeleteAsync(int id);
        Task<CustomerOrderItem?> GetAsync(int id);
        Task UpdateAsync(CustomerOrderItem item);
    }
}
