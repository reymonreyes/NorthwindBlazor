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
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly EfDbContext _efDbContext;
        public InvoicesRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public async Task Create(Invoice invoice)
        {
            var addResult = await _efDbContext.Invoices.AddAsync(invoice);            
        }

        public async Task<Invoice?> GetByOrderId(int orderId)
        {
            return await _efDbContext.Invoices.FirstOrDefaultAsync(x => x.CustomerOrderId == orderId);
        }
    }
}
