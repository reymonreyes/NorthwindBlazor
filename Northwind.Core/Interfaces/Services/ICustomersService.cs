using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface ICustomersService
    {
        Task<ICollection<CustomerDto>> GetAll();
        Task<CustomerDto?> Get(int customerId);
        Task Create(CustomerDto customer);
        Task Update(int customerId, CustomerDto customerDto);
        Task Delete(int customerId);
    }
}
