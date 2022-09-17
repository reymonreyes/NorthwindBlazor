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
        Task<CustomerDto?> Get(string customerId);
        Task Create(CustomerDto customer);
        Task Update(string customerId, CustomerDto customerDto);
    }
}
