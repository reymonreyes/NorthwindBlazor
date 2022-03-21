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
            //var data = new List<SupplierDto>
            //        {
            //            new SupplierDto{ Id = 1, Name = "Alpha One"},
            //            new SupplierDto{ Id = 2, Name = "Bravo Two" },
            //            new SupplierDto{ Id = 3, Name = "Charlie Three" },
            //            new SupplierDto{ Id = 4, Name = "Delta Four" },
            //            new SupplierDto{ Id = 5, Name = "Eagle Five" }
            //        };

            //return Task.FromResult<ICollection<SupplierDto>>(data);
        }
    }
}
