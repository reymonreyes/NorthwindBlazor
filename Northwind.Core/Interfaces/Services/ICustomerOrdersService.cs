﻿using Northwind.Core.Dtos;
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
    }
}
