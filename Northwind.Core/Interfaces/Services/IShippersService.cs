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
        public Task<ServiceResult> Create(ShipperDto shipperDto);
        public Task<ShipperDto?> Get(int shipperId);
        public Task<ServiceResult> Update(int shipperId, ShipperDto shipperDto);
        public Task Delete(int shipperId);
        Task<IEnumerable<ShipperDto>> FindAsync(string shipperName);
    }
}
