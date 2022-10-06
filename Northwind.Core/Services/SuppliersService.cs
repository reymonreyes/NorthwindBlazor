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
            await _unitOfWork.Start();
            var suppliers = await _unitOfWork.SuppliersRepository.GetAll();
            await _unitOfWork.Stop();

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
            await _unitOfWork.Start();
            var supplier = await _unitOfWork.SuppliersRepository.Get(supplierId);
            await _unitOfWork.Stop();

            if(supplier is not null)
            {
                result = new SupplierDto
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    ContactName = supplier.ContactName,
                    ContactTitle = supplier.ContactTitle,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    City = supplier.City,
                    State = supplier.State,
                    Country = supplier.Country,
                    PostalCode = supplier.PostalCode,
                    Phone = supplier.Phone,
                    Fax = supplier.Fax,                    
                    Website = supplier.Website,
                    Notes = supplier.Notes
                };
            }

            return result;
        }

        public async Task<ServiceResult> Create(SupplierDto supplierDto)
        {
            if (supplierDto == null)
                throw new ArgumentNullException("supplier");
            Validate(supplierDto);

            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Email = supplierDto.Email,
                Address = supplierDto.Address,
                City = supplierDto.City,
                State = supplierDto.State,
                Country = supplierDto.Country,
                PostalCode = supplierDto.PostalCode,
                Phone = supplierDto.Phone,
                Fax = supplierDto.Fax,
                Website = supplierDto.Website
            };

            await _unitOfWork.Start();
            await _unitOfWork.SuppliersRepository.Create(supplier);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("supplier", supplier.Id.ToString()) });

            return result;
        }

        public async Task<ServiceResult> Update(int supplierId, SupplierDto supplierDto)
        {
            if (supplierId <= 0 || supplierId == int.MinValue)
                throw new ArgumentOutOfRangeException("supplierId");
            if (supplierDto == null)
                throw new ArgumentNullException("supplier");
            Validate(supplierDto);            

            await _unitOfWork.Start();
            var supplierEntity = await _unitOfWork.SuppliersRepository.Get(supplierId);
            if (supplierEntity == null)
                throw new DataNotFoundException("Supplier not found.");
            
            supplierEntity.Name = supplierDto.Name;
            supplierEntity.ContactName = supplierDto.ContactName;
            supplierEntity.ContactTitle = supplierDto.ContactTitle;
            supplierEntity.Address = supplierDto.Address;
            supplierEntity.City = supplierDto.City;
            supplierEntity.State = supplierDto.State;
            supplierEntity.Country = supplierDto.Country;
            supplierEntity.PostalCode = supplierDto.PostalCode;
            supplierEntity.Phone = supplierDto.Phone;
            supplierEntity.Fax = supplierDto.Fax;
            supplierEntity.Email = supplierDto.Email;
            supplierEntity.Website = supplierDto.Website;
            supplierEntity.Notes = supplierDto.Notes;

            await _unitOfWork.SuppliersRepository.Update(supplierEntity);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", supplierEntity.Id.ToString()) });

            return result;
        }

        public async Task Delete(int supplierId)
        {
            await _unitOfWork.Start();
            await _unitOfWork.SuppliersRepository.Delete(supplierId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        private void Validate(SupplierDto supplier)
        {
            var validationResult = _supplierValidator.Validate(supplier);
            if (validationResult?.Count > 0)
                throw new ValidationFailedException(validationResult);            
        }
    }
}
