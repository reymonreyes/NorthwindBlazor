using Microsoft.EntityFrameworkCore;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class PurchaseOrderItemsRepository : IPurchaseOrderItemsRepository
    {
        private readonly EfDbContext _efDbContext;

        public PurchaseOrderItemsRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }
        public async Task Create(PurchaseOrderItem purchaseOrderItem)
        {
            await _efDbContext.PurchaseOrderItems.AddAsync(purchaseOrderItem);
        }

        public async Task DeleteAsync(int purchaseOrderItemId)
        {
            var purchaseOrderItem = await GetAsync(purchaseOrderItemId);
            if (purchaseOrderItem is not null)
                _efDbContext.PurchaseOrderItems.Remove(purchaseOrderItem);
        }

        public async Task<PurchaseOrderItem?> GetAsync(int purchaseOrderItemId)
        {
            return await _efDbContext.PurchaseOrderItems.FirstOrDefaultAsync(x => x.Id == purchaseOrderItemId);
        }

        public void Update(PurchaseOrderItem purchaseOrderItem)
        {
            _efDbContext.PurchaseOrderItems.Update(purchaseOrderItem);
        }
    }
}
