using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class CustomersService : ICustomersService
    {
        public Task<ICollection<CustomerDto>> GetAll()
        {
            ICollection<CustomerDto> result = new List<CustomerDto>
            {
                new CustomerDto { Id = 1, Name = "Alpha One" },
                new CustomerDto { Id = 2, Name = "Bravo Two" },
                new CustomerDto { Id = 3, Name = "Charlie Three" },
                new CustomerDto { Id = 4, Name = "Delta Four" },
                new CustomerDto { Id = 5, Name = "Eagle Five" }
            };

            return Task.FromResult(result);
        }
    }
}
