using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class CustomerOrdersService : ICustomerOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerOrdersService(IUnitOfWork unitOfWork)
        {            
            _unitOfWork = unitOfWork;
        }

        public async Task AddItem(int customerOrderId, CustomerOrderItemDto? customerOrderItemDto)
        {
            await _unitOfWork.Start();
            var customerOrder = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if(customerOrder == null) throw new DataNotFoundException("Customer Order not found.");

            if (customerOrderItemDto != null)
            {
                var product = await _unitOfWork.ProductsRepository.Get(customerOrderItemDto.ProductId);
                if (product == null) throw new DataNotFoundException("Product not found.");
                if (customerOrderItemDto.Quantity <= 0) throw new ValidationFailedException("Quantity must be greater than 0.");

                customerOrder.Items.Add(new CustomerOrderItem
                {
                    CustomerOrderId = customerOrderItemDto.ProductId,
                    Quantity = customerOrderItemDto.Quantity,
                    ProductId = customerOrderItemDto.ProductId,
                    UnitPrice = customerOrderItemDto.UnitPrice ?? product.ListPrice,
                });
                _unitOfWork.CustomerOrdersRepository.Update(customerOrder);

                await _unitOfWork.Commit();
            }

            await _unitOfWork.Stop();
        }

        public async Task CompleteOrder(int orderId)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(orderId);
            if (order == null) throw new DataNotFoundException("Customer Order not found.");

            order.Status = OrderStatus.Completed;
            await _unitOfWork.Commit();

            await _unitOfWork.Stop();
        }

        public async Task<ServiceResult> Create(int customerId, DateTime? orderDate, List<Dtos.CustomerOrderItemDto> orderItems)
        {
            await _unitOfWork.Start();
            var customer = await _unitOfWork.CustomersRepository.Get(customerId);
            if (customer == null) throw new DataNotFoundException("Customer not found.");

            if (orderDate == null || orderDate == DateTime.MinValue)
                orderDate = DateTime.UtcNow;

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            var customerOrder = new CustomerOrder
            {
                CustomerId = customerId,
                OrderDate = orderDate.Value,
                Items = new List<CustomerOrderItem>()
            };

            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    var product = await _unitOfWork.ProductsRepository.Get(orderItem.ProductId);

                    if (product != null && orderItem.Quantity > 0)
                    {
                        customerOrder.Items.Add(new CustomerOrderItem
                        {
                            CustomerOrderId = orderItem.ProductId,
                            Quantity = orderItem.Quantity,
                            ProductId = orderItem.ProductId,
                            UnitPrice = orderItem.UnitPrice ?? product.ListPrice,
                        });
                    }
                    else
                    {
                        if (product == null)
                            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("OrderItem.Product", $"Product with ID: {orderItem.ProductId} not found.") });
                        if (orderItem.Quantity <= 0)
                            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("OrderItem.Quantity", $"Item quantity for Product[{orderItem.ProductId}] must be greater than 0.") });
                    }                    
                }
            }

            await _unitOfWork.CustomerOrdersRepository.CreateAsync(customerOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", customerOrder.Id.ToString()) });

            return result;
        }

        public async Task<int> CreateInvoice(int orderId, DateTime? dueDate, DateTime? invoiceDate, decimal shippingCost)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(orderId);
            if (order == null) throw new DataNotFoundException("Customer Order not found.");
            var invoice = await _unitOfWork.InvoicesRepository.GetByOrderId(order.Id);
            if (invoice != null) throw new Exception("Invoice already exist.");
            if(order.ShipperId == null) throw new DataNotFoundException("Shipper not found.");
            var shipper = await _unitOfWork.ShippersRepository.Get(order.ShipperId!.Value);
            if (shipper == null) throw new DataNotFoundException("Shipper not found.");

            int id = 0;
            if (invoice == null)
            {
                invoice = new Invoice();
                invoice.CustomerOrderId = order.Id;
                invoice.InvoiceDate = invoiceDate ?? DateTime.UtcNow;
                invoice.DueDate = dueDate;
                invoice.ShippingCost = shippingCost;

                await _unitOfWork.InvoicesRepository.Create(invoice);

                

                //set status to Invoiced
                order.Status = Enums.OrderStatus.Invoiced;

                foreach (var item in order.Items)
                {
                    item.Status = Enums.OrderStatus.Invoiced;
                    //create inventory transaction as Sold
                    var inventoryTransaction = new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        Created = DateTime.UtcNow,
                        CustomerOrderId = order.Id,
                        Quantity = item.Quantity,
                        TransactionType = Enums.InventoryTransactionType.Sold
                    };
                    await _unitOfWork.InventoryTransactionsRepository.Create(inventoryTransaction);
                }

                _unitOfWork.CustomerOrdersRepository.Update(order);
                await _unitOfWork.Commit();

                id = invoice.Id;
            }

            await _unitOfWork.Stop();
            return id;
        }

        public async Task ReceivePayment(int orderId, DateTime paymentDate, decimal amount, PaymentMethodType paymentType)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(orderId);
            if (order == null) throw new DataNotFoundException("Customer Order not found.");
            if (order.Status == OrderStatus.Completed) throw new ValidationFailedException("Customer Order Status closed.");

            var payment = new Payment
            {
                Date = paymentDate,
                Amount = amount,
                Method = paymentType
            };

            order.Payment = payment;
            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task ShipOrder(int orderId)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(orderId);
            if (order == null) throw new DataNotFoundException("Order is not found.");
            if(order.ShipTo == null) throw new DataNotFoundException("Shipping information not found.");
            var invoice = await _unitOfWork.InvoicesRepository.GetByOrderId(order.Id);
            if (invoice == null) throw new DataNotFoundException("Invoice not found.");

            order.Status = Enums.OrderStatus.Shipped;
            foreach (var item in order.Items)
                item.Status = Enums.OrderStatus.Shipped;

            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();

            await _unitOfWork.Stop();
        }

        public async Task<CustomerOrderDto?> GetAsync(int orderId)
        {
            CustomerOrderDto? result = null;

            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(orderId);
            await _unitOfWork.Stop();
            if (order is not null)
            {
                result = new CustomerOrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    //DueDate = order.DueDate, TODO
                    //ShippedDate = order.ShippedDate TODO
                    ShipperId = order.ShipperId,
                    Notes = order.Notes
                };
            }

            return result;
        }
    }
}
