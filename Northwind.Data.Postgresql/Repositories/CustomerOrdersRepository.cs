﻿using Microsoft.EntityFrameworkCore;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.Postgresql.Repositories
{
    public class CustomerOrdersRepository : ICustomerOrdersRepository
    {
        private readonly EfDbContext _efDbContext;

        public CustomerOrdersRepository(EfDbContext efDbContext)
        {
            _efDbContext = efDbContext;
        }
        public async Task CreateAsync(CustomerOrder customerOrder)
        {
            await _efDbContext.CustomerOrders.AddAsync(customerOrder);
        }

        public async Task<(int TotalRecords, IEnumerable<CustomerOrderSummaryDto> Records)> GetAllAsync(int page = 1, int size = 10)
        {
            var query = from order in _efDbContext.CustomerOrders
                    join customer in _efDbContext.Customers on order.CustomerId equals customer.Id
                    select new CustomerOrderSummaryDto
                    {
                        Id = order.Id,
                        CustomerId = customer.Id,
                        CustomerName = customer.Name,
                        Status = order.Status,
                        Total = (from customerOrder in _efDbContext.CustomerOrderItems
                                 where customerOrder.CustomerOrderId == order.Id
                                 select new { Quantity = customerOrder.Quantity, UnitPrice = customerOrder.UnitPrice }).Sum(x => x.Quantity * x.UnitPrice)
                    };
            var results = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var totalRecords = await _efDbContext.CustomerOrders.CountAsync();
            return (totalRecords, results);
        }        

        public Task<CustomerOrder?> GetAsync(int customerOrderId)
        {
            return _efDbContext.CustomerOrders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == customerOrderId);
        }
        public void Update(CustomerOrder customerOrder)
        {
            _efDbContext.CustomerOrders.Update(customerOrder);
        }
    }
}
