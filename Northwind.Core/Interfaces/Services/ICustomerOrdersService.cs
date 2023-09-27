using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface ICustomerOrdersService
    {
        Task AddItem(int customerOrderId, CustomerOrderItemDto? customerOrderItemDto);
        Task<ServiceResult> Create(int customerId, DateTime? orderDate, List<CustomerOrderItemDto> orderItems);
        Task CreateInvoice(int orderId);
    }
}
