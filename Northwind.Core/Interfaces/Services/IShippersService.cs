using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface IShippersService
    {
        public Task<ICollection<ShipperDto>> GetAll();
        public Task<ServiceResult> Create(ShipperDto? shipperDto);
        public Task<ShipperDto?> Get(int shipperId);
        public Task<ServiceResult> Edit(int shipperId, ShipperDto? shipperDto);
    }
}
