﻿using Autofac.Extras.Moq;
using Moq;
using Northwind.Common.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using Northwind.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Core.UnitTests.Services
{
    public class PurchaseOrdersTests
    {        
        [Fact]
        public async Task Create_ShouldThrowNullExceptionForNullParameter()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrderValidator validator = mock.Create<PurchaseOrderValidator>();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Create(null));
        }

        [Fact]
        public async Task Create_ShouldThrowValidationExceptionForInvalidFields()
        {
            var poDto = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } } };            
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(poDto)).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { Message = new KeyValuePair<string, string>("SupplierId", "SupplierId is required"), MessageType = Enums.ServiceMessageType.Error }
            });
;            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.Create(poDto));
        }

        [Fact]
        public void Create_ShouldUseValidatorForValidation()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1, UnitCost = 1 } } };
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { Message = new KeyValuePair<string, string>("SupplierId", "SupplierId is required"), MessageType = Enums.ServiceMessageType.Error }
            });
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            service.Create(purchaseOrder);
            validator.Verify(x => x.Validate(purchaseOrder), Times.Once);
        }

        [Fact]
        public async Task Create_PurchaseOrderMustContainAnItem()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1 };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var exception = await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Items are required"));
        }

        [Fact]
        public async Task Create_PurchaseOrderMustThrowExceptionIfItemQuantityIsLessThanOne()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 0 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var exception = await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Item quantity must be greater than 0"));            
        }

        [Fact]
        public async Task Create_PurchaseOrderMustThrowExceptionIfItemUnitCostIsLessThanZero()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1, UnitCost = 0 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var exception = await Assert.ThrowsAsync<ValidationFailedException>(async () => { await service.Create(purchaseOrder); });
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Item unit cost must be greater than 0"));
        }

        [Fact]
        public async Task Create_ShouldReturnIsSuccessfulOnValidData()
        {
            var purchaseOrderDto = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1, UnitCost = 1 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrderDto)).Returns(new List<ServiceMessageResult>());
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            var unitOfWork = mock.Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var serviceResult = await service.Create(purchaseOrderDto);

            Assert.True(serviceResult.IsSuccessful);
        }

        [Fact]
        public async Task Update_ShouldThrowExceptionIfPurchaseOrderDtoIsNull()
        {
            PurchaseOrder? purchaseOrder = null;
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(purchaseOrder);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var serviceResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => { await service.UpdateAsync(1, null); });
        }

        [Fact]
        public async Task Update_ShouldThrowExceptionIfPurchaseOrderIdIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder());
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var serviceResult = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await service.UpdateAsync(0, new PurchaseOrderDto()); });
        }

        [Fact]
        public async Task Update_ShouldThrowExceptionIfPurchaseOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var serviceResult = await Assert.ThrowsAsync<DataNotFoundException>(async () => { await service.UpdateAsync(1, new PurchaseOrderDto()); });
        }
        
        [Theory]
        [InlineData(Enums.OrderStatus.Approved)]
        [InlineData(Enums.OrderStatus.Closed)]
        [InlineData(Enums.OrderStatus.Cancelled)]
        public async Task Update_ShouldFailStatusIsApprovedOrClosedOrCancelled(Enums.OrderStatus orderStatus)
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = orderStatus });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await Assert.ThrowsAsync<ValidationFailedException>(async () => { await service.UpdateAsync(1, new PurchaseOrderDto()); });
            Assert.Contains($"Purchase Order already {orderStatus}", result.Message);            
        }

        [Fact]
        public async Task Update_ShouldSucceedWhenNoErrors()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var serviceResult = await service.UpdateAsync(1, new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1, UnitCost = 1 } } });
            Assert.True(serviceResult.IsSuccessful);
        }

        [Fact]
        public async Task Submit_ShouldThrowExceptionOnInvalidIdParameter()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.SubmitAsync(0));
        }        

        [Fact]
        public async Task Submit_ShouldThrowExceptionIfItDoesNotExist()
        {            
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder?)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            
            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.SubmitAsync(1));
        }

        [Fact]
        public async Task Submit_ShouldReturnSubmittedStatus()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, Status = OrderStatus.New, SupplierId = 1, OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1, UnitCost = 1 } } });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await service.SubmitAsync(1);
            Assert.Contains(OrderStatus.Submitted.ToString(), result.Message.Value);
        }

        [Fact]
        public async Task Submit_ShouldThrowExceptionIfItemsAreNullOrEmpty()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1 });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(() => repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.SubmitAsync(1));
        }

        [Fact]
        public async Task Approve_ShouldThrowExceptionIfInvalidId()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.ApproveAsync(0));
        }

        [Fact]
        public async Task Approve_ShouldThrowExceptionIfPurchaseOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder?)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ApproveAsync(1));
        }

        [Fact]
        public async Task Approve_ShouldThrowExceptionIfOrderItemsDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1 });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.ApproveAsync(1));
        }

        [Fact]
        public async Task Approve_ShouldReturnApprovedStatus()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, Status = OrderStatus.New, SupplierId = 1, OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1, UnitCost = 1 } } });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await service.ApproveAsync(1);
            Assert.Contains(OrderStatus.Approved.ToString(), result.Message.Value);
        }
    }
}
