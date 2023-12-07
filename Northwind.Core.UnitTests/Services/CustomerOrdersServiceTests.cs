using Autofac.Extras.Moq;
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
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);

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
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);

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
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();
            var items = new List<CustomerOrderItemDto>
            {
                new CustomerOrderItemDto { ProductId = 1 }
            };

            var result = await service.Create(1, null, items);

            Assert.Contains($"Item quantity for Product[1] must be greater than 0.", result.Messages?.FirstOrDefault(x => x.MessageType == Enums.ServiceMessageType.Error)?.Message.Value);
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfCustomerOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.CustomerOrder)null!);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.AddItem(1, (CustomerOrderItemDto)null!));
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfProductDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new CustomerOrder { Id = 1, CustomerId = 1 });
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Product)null);
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);
            
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.AddItem(1, new CustomerOrderItemDto { ProductId = 1, Quantity = 1, CustomerOrderid = 1 }));
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfQuantityIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new CustomerOrder { Id = 1, CustomerId = 1 });
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Product { Id = 1, Name = "Product One" });
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.AddItem(1, new CustomerOrderItemDto { ProductId = 1, Quantity = 0, CustomerOrderid = 1 }));
        }

        [Fact]
        public async Task CreateInvoice_ShouldThrowExceptionIfOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.CustomerOrder)null!);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.CreateInvoice(1, null, null, 0));
        }

        [Fact]
        public async Task CreateInvoice_ShouldThrowExceptionIfShippingInformationIsNotSet()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new CustomerOrder { Id = 1, CustomerId = 1, ShipperId = 0 });
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            var shippersRepo = mock.Mock<IShippersRepository>();
            shippersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Shipper)null!);
            uow.Setup(x => x.ShippersRepository).Returns(shippersRepo.Object);
            var invoicesRepo = mock.Mock<IInvoicesRepository>();
            invoicesRepo.Setup(x => x.GetByOrderId(It.IsAny<int>())).ReturnsAsync((Invoice)null);
            uow.Setup(x => x.InvoicesRepository).Returns(invoicesRepo.Object);

            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.CreateInvoice(1, null, null, 0));
        }

        [Fact]
        public async Task ShipOrder_ShouldThrowExceptionIfOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.CustomerOrder)null!);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ShipOrder(1));
        }

        [Fact]
        public async Task ShipOrder_ShouldThrowExceptionIfShippingInformationIsNotSet()
        {
            var mock = AutoMock.GetLoose();
            var order = new CustomerOrder { Id = 1, CustomerId = 1, ShipperId = 0 };
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ShipOrder(1));
        }

        [Fact]
        public async Task ShipOrder_ShouldThrowExceptionIfInvoiceDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var order = new CustomerOrder { Id = 1, CustomerId = 1, ShipperId = 1, ShipTo = new ValueObjects.ShippingInformation { Name = "Customer One", Address = "Somewhere" } };
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            var invoicesRepo = mock.Mock<IInvoicesRepository>();
            invoicesRepo.Setup(x => x.GetByOrderId(It.IsAny<int>())).ReturnsAsync((Invoice)null!);
            uow.Setup(x => x.InvoicesRepository).Returns(invoicesRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ShipOrder(1));
        }

        [Fact]
        public async Task ShipOrder_ShouldSetStatusToShippedIfSuccessful()
        {
            var mock = AutoMock.GetLoose();
            var order = new CustomerOrder
            {
                Id = 1,
                CustomerId = 1,
                ShipperId = 1,
                ShipTo = new ValueObjects.ShippingInformation { Name = "Customer One", Address = "Somewhere" },
                Items = new List<CustomerOrderItem> {
                    new CustomerOrderItem
                    {
                        Id = 1,
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 1
                    }
                }
            };
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            var invoicesRepo = mock.Mock<IInvoicesRepository>();
            invoicesRepo.Setup(x => x.GetByOrderId(It.IsAny<int>())).ReturnsAsync(new Invoice { Id = 1, CustomerOrderId = 1 });
            uow.Setup(x => x.InvoicesRepository).Returns(invoicesRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await service.ShipOrder(1);

            Assert.Equal(Enums.OrderStatus.Shipped, order.Status);
            Assert.All(order.Items, item =>
            {
                Assert.Equal(Enums.OrderStatus.Shipped, item.Status);
            });
        }

        [Fact]
        public async Task ReceivePayment_ShouldThrowExceptionIfOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.CustomerOrder)null!);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ReceivePayment(1, DateTime.UtcNow, 100, Enums.PaymentMethodType.Cash));
        }

        [Fact]
        public async Task ReceivePayment_ShouldThrowExceptionIfOrderStatusIsCompleted()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var order = new CustomerOrder { Id = 1, CustomerId = 1, Status = Enums.OrderStatus.Completed, ShipperId = 1, ShipTo = new ValueObjects.ShippingInformation { Name = "Customer One", Address = "Somewhere" } };
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.ReceivePayment(1, DateTime.UtcNow, 100, Enums.PaymentMethodType.Cash));
        }

        [Fact]
        public async Task ReceivePayment_ShouldSetPaymentInformationIfSuccessful()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var order = new CustomerOrder { Id = 1, CustomerId = 1, Status = Enums.OrderStatus.Shipped, ShipperId = 1, ShipTo = new ValueObjects.ShippingInformation { Name = "Customer One", Address = "Somewhere" } };
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await service.ReceivePayment(1, DateTime.UtcNow, 100, Enums.PaymentMethodType.Cash);

            Assert.NotNull(order.Payment);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowExceptionIfOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.CustomerOrder)null!);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.CompleteOrder(1));
        }

        [Fact]
        public async Task CompleteOrder_ShouldSetStatusToCompletedIfSuccessful()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var order = new CustomerOrder { Id = 1, CustomerId = 1, Status = Enums.OrderStatus.Shipped, ShipperId = 1, ShipTo = new ValueObjects.ShippingInformation { Name = "Customer One", Address = "Somewhere" } };
            var customerOrdersRepo = mock.Mock<ICustomerOrdersRepository>();
            customerOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.CustomerOrdersRepository).Returns(customerOrdersRepo.Object);
            ICustomerOrdersService service = mock.Create<CustomerOrdersService>();

            await service.CompleteOrder(1);

            Assert.Equal(Enums.OrderStatus.Completed, order.Status);
        }
    }
}
