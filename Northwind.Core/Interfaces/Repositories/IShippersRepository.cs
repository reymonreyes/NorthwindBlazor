using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface IShippersRepository
    {
        Task<ICollection<Shipper>> GetAll();
        Task Create(Shipper shipper);
        Task<Shipper?> Get(int shipperId);
        Task Update(Shipper shipper);
        Task Delete(int shipperId);
        Task<IEnumerable<Shipper>> FindAsync(string shipperName);
    }
}
