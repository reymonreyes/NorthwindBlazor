﻿using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Core.UnitTests.Services
{
    public class CustomerOrdersServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfCustomerDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var customersRepo = mock.Mock<ICustomersRepository>();
            customersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Customer)null);
            var uow = mock.Mock<IUnitOfWork>();
            uow.Setup(x => x.CustomersRepository).Returns(customersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.Create(1, null, new List<CustomerOrderItemDto>()));
        }

        [Fact]
        public async Task Create_ShouldReturnErrorMessageIfProductDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var customersRepo = mock.Mock<ICustomersRepository>();
            customersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Customer { Id = 1, Name = "Customer One"});
            var uow = mock.Mock<IUnitOfWork>();
            uow.Setup(x => x.CustomersRepository).Returns(customersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Product)null);
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();
            var items = new List<CustomerOrderItemDto>
            {
                new CustomerOrderItemDto { ProductId = 1 }
            };

            var result = await service.Create(1, null, items);

            Assert.Contains($"Product with ID: 1 not found.", result.Messages?.FirstOrDefault(x => x.MessageType == Enums.ServiceMessageType.Error)?.Message.Value);
        }

        [Fact]
        public async Task Create_ShouldReturnErrorMessagesIfMultipleProductDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var customersRepo = mock.Mock<ICustomersRepository>();
            customersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Customer { Id = 1, Name = "Customer One" });
            var uow = mock.Mock<IUnitOfWork>();
            uow.Setup(x => x.CustomersRepository).Returns(customersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Product)null);
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();
            var items = new List<CustomerOrderItemDto>
            {
                new CustomerOrderItemDto { ProductId = 1, Quantity = 1 },
                new CustomerOrderItemDto { ProductId = 2, Quantity = 1 }
            };

            var result = await service.Create(1, null, items);

            Assert.Equal(2, result?.Messages?.Count(x => x.MessageType == Enums.ServiceMessageType.Error));
        }

        [Fact]
        public async Task Create_ShouldReturnErrorMessageIfItemQuantityIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            var customersRepo = mock.Mock<ICustomersRepository>();
            customersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Customer { Id = 1, Name = "Customer One" });
            var uow = mock.Mock<IUnitOfWork>();
            uow.Setup(x => x.CustomersRepository).Returns(customersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Product { Id = 1, Name = "Product One"});
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();
            var items = new List<CustomerOrderItemDto>
            {
                new CustomerOrderItemDto { ProductId = 1 }
            };

            var result = await service.Create(1, null, items);

            Assert.Contains($"Item quantity for Product[1] must be greater than 0.", result.Messages?.FirstOrDefault(x => x.MessageType == Enums.ServiceMessageType.Error)?.Message.Value);
        }
    }
}
