﻿using Northwind.Core.Entities;
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
    }
}
