using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Repositories
{
    public class PurchaseOrdersRepository //IPurchaseOrdersRepository todo: remove
    {
        private readonly EfDbContext _efDbContext;

        public PurchaseOrdersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public async Task CreateAsync(PurchaseOrder purchaseOrder)
        {
            await _efDbContext.AddAsync(purchaseOrder);
        }
    }
}
