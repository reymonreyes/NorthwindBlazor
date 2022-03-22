using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
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
        public SuppliersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
