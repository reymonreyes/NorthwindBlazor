using Microsoft.EntityFrameworkCore;
using Northwind.Common.Extensions;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class ShippersRepository : IShippersRepository
    {
        private readonly EfDbContext _efDbContext;
        public ShippersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public async Task Create(Shipper shipper)
        {
            await _efDbContext.Shippers.AddAsync(shipper);
        }

        public async Task Delete(int shipperId)
        {
            var shipper = await Get(shipperId);
            if (shipper is not null)
                _efDbContext.Shippers.Remove(shipper);
        }

        public async Task<IEnumerable<Shipper>> FindAsync(string shipperName)
        {
            if(shipperName.IsEmptyOrLengthLessThan(2))
                return new List<Shipper>();

            var results = await _efDbContext.Shippers.Where(x => x.Name.ToLower().Contains(shipperName.ToLower())).ToListAsync();
            return results;
        }

        public async Task<Shipper?> Get(int shipperId)
        {
            return await _efDbContext.Shippers.FirstOrDefaultAsync(x => x.Id == shipperId);
        }

        public async Task<ICollection<Shipper>> GetAll()
        {
            return await _efDbContext.Shippers.ToListAsync();
        }

        public Task Update(Shipper shipper)
        {
            _efDbContext.Update(shipper);

            return Task.CompletedTask;
        }
    }
}
