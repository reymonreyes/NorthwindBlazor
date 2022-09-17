using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface ICustomersRepository
    {
        Task<ICollection<Customer>> GetAll();
        Task<Customer?> Get(string customerId);
        Task Create(Customer customer);
        Task Update(Customer customer);
    }
}
