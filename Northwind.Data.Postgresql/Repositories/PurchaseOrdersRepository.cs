using Microsoft.EntityFrameworkCore;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class PurchaseOrdersRepository : IPurchaseOrdersRepository
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

        public async Task<PurchaseOrder?> GetAsync(int id)
        {
            return await _efDbContext.PurchaseOrders.Include("OrderItems").FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(PurchaseOrder purchaseOrder)
        {
            _efDbContext.Update(purchaseOrder);
        }

        public async Task<(int TotalRecords, IEnumerable<PurchaseOrderSummaryDto> Records)> GetAllAsync(int page = 1, int size = 10)
        {
            var query = from order in _efDbContext.PurchaseOrders
                        join supplier in _efDbContext.Suppliers on order.SupplierId equals supplier.Id
                        select new PurchaseOrderSummaryDto
                        {
                            Id = order.Id,
                            SupplierId = supplier.Id,
                            SupplierName = supplier.Name,
                            Total = (from orderItem in _efDbContext.PurchaseOrderItems
                                     where orderItem.PurchaseOrderId == order.Id
                                     select new { Quantity = orderItem.Quantity, UnitPrice = orderItem.UnitCost }).Sum(x => x.Quantity * x.UnitPrice)
                        };

            var results = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var totalRecords = await _efDbContext.PurchaseOrders.CountAsync();

            return (totalRecords, results);
        }                
    }
}
