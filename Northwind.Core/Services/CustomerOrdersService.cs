using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
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

        public async Task<ServiceResult> Create(int customerId, DateTime? orderDate, List<Dtos.CustomerOrderItemDto> orderItems)
        {
            
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
                        await _unitOfWork.CustomerOrdersRepository.CreateAsync(customerOrder);
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

            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
            
            return result;
        }
    }
}
