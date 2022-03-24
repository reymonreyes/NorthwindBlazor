using Northwind.Core.Dtos;
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
        public ICollection<ShipperDto> GetAll()
        {
            return new List<ShipperDto>
            {
                new ShipperDto{ Id = 1, Name = "Shipper One", Phone = "1234" },
                new ShipperDto{ Id = 2, Name = "Shipper Two", Phone = "2234" },
                new ShipperDto{ Id = 3, Name = "Shipper Three", Phone = "3234" },
                new ShipperDto{ Id = 4, Name = "Shipper Four", Phone = "4234" },
                new ShipperDto{ Id = 5, Name = "Shipper Five", Phone = "5234" }
            };
        }
    }
}
