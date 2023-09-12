using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class InventoryTransactionsRepository : IInventoryTransactionsRepository
    {
        private readonly EfDbContext _dbContext;
        public InventoryTransactionsRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(InventoryTransaction inventoryTransaction)
        {
            await _dbContext.InventoryTransactions.AddAsync(inventoryTransaction);
        }
    }
}
