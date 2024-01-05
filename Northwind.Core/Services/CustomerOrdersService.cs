using Northwind.Core.Dtos;
using Northwind.Core.Dtos.Document;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class CustomerOrdersService : ICustomerOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentGeneratorService _documentGeneratorService;
        public CustomerOrdersService(IUnitOfWork unitOfWork, IDocumentGeneratorService documentGeneratorService)
        {            
            _unitOfWork = unitOfWork;
            _documentGeneratorService = documentGeneratorService;
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

                var orderItem = new CustomerOrderItem
                {
                    CustomerOrderId = customerOrder.Id,
                    Quantity = customerOrderItemDto.Quantity,
                    ProductId = customerOrderItemDto.ProductId,
                    UnitPrice = customerOrderItemDto.UnitPrice ?? product.ListPrice,
                };

                await _unitOfWork.CustomerOrderItemsRepository.CreateAsync(orderItem);
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
                Status = OrderStatus.New,
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
                invoice = new Entities.Invoice();
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
                    DueDate = order.DueDate,
                    ShipDate = order.ShipDate,
                    ShipperId = order.ShipperId,
                    Notes = order.Notes,
                    Status = order.Status,
                    Items = order.Items.Select(x => new CustomerOrderItemDto { Id = x.Id, ProductId = x.ProductId, Quantity = x.Quantity, UnitPrice = x.UnitPrice }).ToList()
                };
            }

            return result;
        }

        public async Task UpdateItem(int customerOrderId, CustomerOrderItemDto customerOrderItem)   
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            if(customerOrderItem is null) throw new ArgumentNullException(nameof(customerOrderItem));

            await _unitOfWork.Start();
            var customerOrder = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (customerOrder is null) throw new DataNotFoundException("CustomerOrder not found");

            var product = await _unitOfWork.ProductsRepository.Get(customerOrderItem.ProductId);
            if (product is null) throw new DataNotFoundException("Product not found");

            var orderItem = await _unitOfWork.CustomerOrderItemsRepository.GetAsync(customerOrderItem.Id);
            if (orderItem is null) throw new DataNotFoundException("Order Item not found");
            
            var validationErrors = new List<ServiceMessageResult>();
            if(customerOrderItem.Quantity <= 0)
                validationErrors.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("Quantity", "invalid"), MessageType = ServiceMessageType.Error });
            if(customerOrderItem.UnitPrice <= 0)
                validationErrors.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("UnitPrice", "invalid"), MessageType = ServiceMessageType.Error });
            if (validationErrors.Count > 0) throw new ValidationFailedException(validationErrors);

            orderItem.Quantity = customerOrderItem.Quantity;
            orderItem.UnitPrice = customerOrderItem.UnitPrice.Value;
            await _unitOfWork.CustomerOrderItemsRepository.UpdateAsync(orderItem);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task RemoveItem(int customerOrderItemId)
        {
            if(customerOrderItemId <= 0) throw new ArgumentException(nameof(customerOrderItemId));

            await _unitOfWork.Start();
            var customerOrderItem = await _unitOfWork.CustomerOrderItemsRepository.GetAsync(customerOrderItemId);
            if (customerOrderItem == null) throw new DataNotFoundException("CustomerOrderItem not found");

            await _unitOfWork.CustomerOrderItemsRepository.DeleteAsync(customerOrderItemId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<ServiceMessageResult> CancelAsync(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (order is null) throw new DataNotFoundException();
            if (!(order.Status == OrderStatus.New)) throw new ValidationFailedException($"Customer Order is already {order.Status}");

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("CustomerOrder", order.Status.ToString()) };
        }

        public async Task<ServiceMessageResult> MarkAsInvoiced(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if(order is null) throw new DataNotFoundException();
            var validationErrors = new List<ServiceMessageResult>();
            if (!(order.Status == OrderStatus.New))
                validationErrors.Add(new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Customer Order is already {order.Status}") });
            if(!order.DueDate.HasValue)
                validationErrors.Add(new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Due Date not set") });
            if(!order.ShipperId.HasValue || !order.ShipDate.HasValue)
                validationErrors.Add(new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Shipping Information not set") });
            if (validationErrors.Any())
                throw new ValidationFailedException(validationErrors);

            order.Status = OrderStatus.Invoiced;
            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("CustomerOrder", order.Status.ToString()) };
        }

        public async Task MarkAsShipped(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if(order is null) throw new DataNotFoundException("Customer Order not found");
            if (!(order.Status == OrderStatus.Paid))
            {
                var validationErrors = new List<ServiceMessageResult> { new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Customer Order is not yet Paid") } };
                throw new ValidationFailedException("Validation failed. Please check ValidationErrors for details.", validationErrors);
            }

            order.Status = OrderStatus.Shipped;
            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<ServiceMessageResult> MarkAsPaid(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (order is null) throw new DataNotFoundException("Customer Order not found");
            if (!(order.Status == OrderStatus.Invoiced))
            {
                var validationErrors = new List<ServiceMessageResult> { new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Customer Order is not yet Invoiced") } };
                throw new ValidationFailedException("Validation failed. Please check ValidationErrors for details.", validationErrors);
            }

            order.Status = OrderStatus.Paid;
            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("CustomerOrder", order.Status.ToString()) };
        }

        public async Task<ServiceMessageResult> MarkAsCompleted(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (order is null) throw new DataNotFoundException("Customer Order not found");
            if (!(order.Status == OrderStatus.Shipped))
            {
                var validationErrors = new List<ServiceMessageResult> { new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>("CustomerOrder", $"Customer Order is not yet Shipped") } };
                throw new ValidationFailedException("Validation failed. Please check ValidationErrors for details.", validationErrors);
            }

            order.Status = OrderStatus.Completed;
            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("CustomerOrder", order.Status.ToString()) };
        }

        public async Task<ServiceResult> Update(int customerOrderId, CustomerOrderDto orderData)
        {
            if(customerOrderId <= 0) throw new ArgumentOutOfRangeException(nameof(customerOrderId));
            if(orderData is null) throw new ArgumentNullException(nameof(orderData));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (order is null) throw new DataNotFoundException("Customer Order not found");
            if(order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Completed)
                throw new ValidationFailedException($"Customer Order is already {order.Status}");

            order.OrderDate = orderData.OrderDate;
            order.ShipDate = orderData.ShipDate;
            order.ShipperId = orderData.ShipperId;
            //order.ShipTo = orderData.ShipTo; TODO
            order.DueDate = orderData.DueDate;
            //order.Payment = orderData.Payment; TODO
            order.Notes = orderData.Notes;

            _unitOfWork.CustomerOrdersRepository.Update(order);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", order.Id.ToString()) });
            return result;
        }

        public async Task<string> GeneratePdfInvoice(int customerOrderId)
        {
            if (customerOrderId <= 0) throw new ArgumentOutOfRangeException(nameof(customerOrderId));
            await _unitOfWork.Start();
            var order = await _unitOfWork.CustomerOrdersRepository.GetAsync(customerOrderId);
            if (order is null) 
                throw new DataNotFoundException("Customer Order not found");
            if (!order.DueDate.HasValue) throw new ValidationFailedException("Due Date is not set");
            if (order.Items is null || !order.Items.Any()) throw new DataNotFoundException("Items not found");
            var customer = await _unitOfWork.CustomersRepository.Get(order.CustomerId);
            if (customer is null) throw new DataNotFoundException("Customer not found");

            var invoiceData = new Dtos.Document.Invoice();
            invoiceData.Company = new BasicCompanyInformation { Name = "Northwind Blazor", Address = new Address { City = "Tagum City" } };
            invoiceData.ShipTo = new BasicCompanyInformation { Name = customer.Name, Address = new Address { City = customer.City } };
            invoiceData.BillTo = invoiceData.Company;
            invoiceData.InvoiceNumber = order.Id.ToString();
            invoiceData.Date = order.OrderDate;
            invoiceData.DueDate = order.DueDate.Value;
            invoiceData.Items = order.Items.Select(x => new LineItem { ItemDescription = x.ProductId.ToString(), Qty = x.Quantity, UnitPrice = x.UnitPrice, Total = x.UnitPrice * x.Quantity }).ToList();
            var subtotal = order.Items.Sum(x => x.UnitPrice * x.Quantity);
            //TODO all fields below
            invoiceData.Subtotal = subtotal;
            invoiceData.Tax = 0;
            invoiceData.ShippingCost = 0;
            invoiceData.Total = subtotal + invoiceData.Tax + invoiceData.ShippingCost;
            invoiceData.Notes = order.Notes;
            invoiceData.PreparedBy = "Northwind Blazor";
            invoiceData.ApprovedBy = "Northwind Blazor";
            var filename = _documentGeneratorService.CreateInvoicePdf(invoiceData);
            await _unitOfWork.Stop();

            return filename;
        }

        public async Task<(int TotalRecords, IEnumerable<CustomerOrderDto> Records)> GetAllAsync(int page = 1, int size = 10)
        {
            await _unitOfWork.Start();
            var orders = await _unitOfWork.CustomerOrdersRepository.GetAllAsync(page, size);
            var results = orders.Records.Select(x => new CustomerOrderDto { Id = x.Id, CustomerId = x.CustomerId, Status = x.Status }).ToList();
            await _unitOfWork.Stop();

            return (orders.TotalRecords, results);
        }
    }
}
