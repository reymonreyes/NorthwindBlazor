using Microsoft.EntityFrameworkCore;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Repositories
{
    public class ShippersRepository : IShippersRepository
    {
        private readonly EfDbContext _efDbContext;
        public ShippersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }
        public async Task<ICollection<Shipper>> GetAll()
        {
            return await _efDbContext.Shippers.ToListAsync();
        }
    }
}
