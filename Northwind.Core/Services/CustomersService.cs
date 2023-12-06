using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
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
        private readonly ICustomerValidator _customerValidator;

        public CustomersService(IUnitOfWork unitOfWork, ICustomerValidator validator)
        {
            _unitOfWork = unitOfWork;
            _customerValidator = validator;
        }
        public async Task<ICollection<CustomerDto>> GetAll()
        {
            await _unitOfWork.Start();
            var customers = await _unitOfWork.CustomersRepository.GetAll();
            await _unitOfWork.Stop();

            return customers.Select(x => new CustomerDto { Id = x.Id, Name = x.Name, ContactName = x.ContactName, ContactTitle = x.ContactTitle }).ToList();
        }

        public async Task<CustomerDto?> Get(int customerId)
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
                    Email = customer.Email,
                    Address = customer.Address,
                    City = customer.City,                    
                    State = customer.State,
                    Country = customer.Country,
                    PostalCode = customer.PostalCode,
                    Phone = customer.Phone,
                    Fax = customer.Fax,
                    Website = customer.Website,
                    Notes = customer.Notes
                };
            }

            return result;
        }
        public async Task Create(CustomerDto customer)
        {
            if (customer is null)
                throw new ArgumentNullException("customer");
            Validate(customer);
            //await ValidateUniqueId(customer.Id!);

            var customerEntity = new Customer
            {
                Id = customer.Id,
                Name = customer.Name,
                ContactName = customer.ContactName,
                ContactTitle = customer.ContactTitle,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                PostalCode = customer.PostalCode,
                Phone = customer.Phone,
                Fax = customer.Fax,
                Website = customer.Website,
                Notes = customer.Notes
            };
            await _unitOfWork.Start();
            await _unitOfWork.CustomersRepository.Create(customerEntity);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        private void Validate(CustomerDto customer)
        {
            var validationResult = _customerValidator.Validate(customer);
            if (validationResult?.Count > 0)
                throw new ValidationFailedException(validationResult);
        }

        public async Task Update(int customerId, CustomerDto customerDto)
        {            
            if (customerDto is null)
                throw new ArgumentNullException("customerDto");
            Validate(customerDto);

            await _unitOfWork.Start();
            var customer = await _unitOfWork.CustomersRepository.Get(customerId);
            if(customer is null)
                throw new DataNotFoundException("Customer not found.");

            customer.Name = customerDto.Name;
            customer.ContactName = customerDto.ContactName;
            customer.ContactTitle = customerDto.ContactTitle;
            customer.Email = customerDto.Email;
            customer.Address = customerDto.Address;
            customer.City = customerDto.City;
            customer.State = customerDto.State;
            customer.Country = customerDto.Country;
            customer.PostalCode = customerDto.PostalCode;
            customer.Phone = customerDto.Phone;
            customer.Fax = customerDto.Fax;
            customer.Website = customerDto.Website;
            customer.Notes = customerDto.Notes;

            await _unitOfWork.CustomersRepository.Update(customer);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task Delete(int customerId)
        {
            await _unitOfWork.Start();
            await _unitOfWork.CustomersRepository.Delete(customerId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<IEnumerable<CustomerDto>> FindAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName) || (!string.IsNullOrWhiteSpace(customerName) && customerName.Length < 2))
                return new List<CustomerDto>();

            await _unitOfWork.Start();
            var matchedCustomers = await _unitOfWork.CustomersRepository.Find(customerName);
            await _unitOfWork.Stop();
            
            return matchedCustomers.Select(x => new CustomerDto { Id = x.Id, Name = x.Name}).ToList();
        }
    }
}
