using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface ICustomerOrdersRepository
    {
        Task CreateAsync(CustomerOrder customerOrder);
        Task<CustomerOrder?> GetAsync(int customerOrderId);
        void Update(CustomerOrder customerOrder);
    }
}
