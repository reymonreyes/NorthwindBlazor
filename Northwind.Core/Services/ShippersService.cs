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
    public class ShippersService : IShippersService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShippersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ICollection<ShipperDto>> GetAll()
        {
            ICollection<ShipperDto> result = new List<ShipperDto>();
            var shippers = await _unitOfWork.ShippersRepository.GetAll();
            return shippers.Select(x => new ShipperDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone
            }).ToList();
            //return new List<ShipperDto>
            //{
            //    new ShipperDto{ Id = 1, Name = "Shipper One", Phone = "1234" },
            //    new ShipperDto{ Id = 2, Name = "Shipper Two", Phone = "2234" },
            //    new ShipperDto{ Id = 3, Name = "Shipper Three", Phone = "3234" },
            //    new ShipperDto{ Id = 4, Name = "Shipper Four", Phone = "4234" },
            //    new ShipperDto{ Id = 5, Name = "Shipper Five", Phone = "5234" }
            //};
        }
    }
}
