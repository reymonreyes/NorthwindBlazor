using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Extensions
{
    public static class ShipperExtension
    {
        public static ShipperDto ToShipperDto(this Shipper shipper)
        {
            if(shipper is null)
                throw new ArgumentNullException(nameof(shipper));

            return new ShipperDto
            {
                Id = shipper.Id,
                Name = shipper.Name,
                Phone = shipper.Phone
            };
        }
    }
}
