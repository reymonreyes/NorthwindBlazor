using Northwind.Core.Dtos;
using Northwind.Core.Entities;
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
    public class SuppliersService : ISuppliersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierValidator _supplierValidator;
        public SuppliersService(IUnitOfWork unitOfWork, ISupplierValidator supplierValidator)
        {
            _unitOfWork = unitOfWork;
            _supplierValidator = supplierValidator;
        }
        public async Task<ICollection<SupplierDto>> GetAll()
        {
            var result = new List<SupplierDto>();
            var suppliers = await _unitOfWork.SuppliersRepository.GetAll();

            if(suppliers is not null)
            {
                result = suppliers.Select(x => new SupplierDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ContactName = x.ContactName,
                    ContactTitle = x.ContactTitle,
                    Phone = x.Phone
                }).ToList();
            }

            return result;
        }

        public async Task<SupplierDto?> Get(int supplierId)
        {
            SupplierDto? result = null;
            var supplier = await _unitOfWork.SuppliersRepository.Get(supplierId);
            if(supplier is not null)
            {
                result = new SupplierDto
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    ContactName = supplier.ContactName,
                    ContactTitle = supplier.ContactTitle,
                    Phone = supplier.Phone
                };
            }

            return result;
        }

        public async Task<ServiceResult> Create(SupplierDto? supplierDto)
        {
            if(supplierDto == null)
                throw new ArgumentNullException("supplier");

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            var validationResult = _supplierValidator.Validate(supplierDto);
            if(validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                result.Messages.AddRange(validationResult);
            }

            if (!result.IsSuccessful)
                return result;

            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Address = supplierDto.Address,
                City = supplierDto.City,
                Region = supplierDto.Region,
                Country = supplierDto.Country,
                PostalCode = supplierDto.PostalCode,
                Phone = supplierDto.Phone,
                Fax = supplierDto.Fax,
                Email = supplierDto.Email,
                Homepage = supplierDto.Homepage
            };

            await _unitOfWork.SuppliersRepository.Create(supplier);
            await _unitOfWork.Commit();
            result.Messages.Clear();
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("supplier", supplier.Id.ToString()) });

            return result;
        }
    }
}
