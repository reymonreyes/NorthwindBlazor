using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        public CustomersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ICollection<CustomerDto>> GetAll()
        {
            await _unitOfWork.Start();
            var customers = await _unitOfWork.CustomersRepository.GetAll();
            await _unitOfWork.Stop();

            return customers.Select(x => new CustomerDto { Id = x.Id, Name = x.Name, ContactName = x.ContactName, ContactTitle = x.ContactTitle }).ToList();
        }

        public async Task<CustomerDto?> Get(string customerId)
        {
            CustomerDto? result = null;
            await _unitOfWork.Start();
            var customer = await _unitOfWork.CustomersRepository.Get(customerId);
            await _unitOfWork.Stop();

            if (customer is not null)
            {
                result = new CustomerDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    ContactName = customer.ContactName,
                    ContactTitle = customer.ContactTitle,
                    Address = customer.Address,
                    City = customer.City,
                    Region = customer.Region,
                    Country = customer.Country,
                    PostalCode = customer.PostalCode,
                    Phone = customer.Phone,
                    Fax = customer.Fax
                };
            }

            return result;
        }
    }
}
