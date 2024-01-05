using Northwind.Core.Dtos;
using Northwind.Core.Enums;
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
        Task<int> CreateInvoice(int orderId, DateTime? dueDate, DateTime? invoiceDate, decimal shippingCost);
        Task ShipOrder(int orderId);
        Task ReceivePayment(int orderId, DateTime paymenDate, decimal amount, PaymentMethodType paymentType);
        Task CompleteOrder(int orderId);
        Task<CustomerOrderDto?> GetAsync(int orderId);
        Task UpdateItem(int customerOrderId, CustomerOrderItemDto customerOrderItem);
        Task RemoveItem(int customerOrderItemId);
        Task<ServiceMessageResult> CancelAsync(int customerOrderId);
        Task<ServiceMessageResult> MarkAsInvoiced(int customerOrderId);
        Task MarkAsShipped(int customerOrderId);
        Task<ServiceMessageResult> MarkAsPaid(int customerOrderId);
        Task<ServiceMessageResult> MarkAsCompleted(int customerOrderId);
        Task<ServiceResult> Update(int customerOrderId, CustomerOrderDto orderData);
        Task<string> GeneratePdfInvoice(int customerOrderId);
        Task<(int TotalRecords, IEnumerable<CustomerOrderDto> Records)> GetAllAsync(int page = 1, int size = 10);
    }
}
