﻿using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Dtos.Document;
using Northwind.Core.Entities;
using Northwind.Core.Enums;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Infrastructure;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using Northwind.Core.Services;
using Northwind.Core.ValueObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Core.UnitTests.Services
{
    public class PurchaseOrdersServiceTests
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
            var poDto = new Dtos.PurchaseOrderDto { SupplierId = 1 };
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
            var purchaseOrder = new Dtos.PurchaseOrderDto { SupplierId = 1 };
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
        public async Task Create_MustThrowExceptionIfItemQuantityIsLessThanOne()
        {
            var purchaseOrder = new Dtos.PurchaseOrderDto { SupplierId = 1, OrderItems = new List<PurchaseOrderItemDto> { new PurchaseOrderItemDto { ProductId = 1, Quantity = 0 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var exception = await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Item quantity must be greater than 0"));            
        }

        [Fact]
        public async Task Create_MustThrowExceptionIfItemUnitCostIsLessThanZero()
        {
            var purchaseOrder = new Dtos.PurchaseOrderDto { SupplierId = 1, OrderItems = new List<PurchaseOrderItemDto> { new PurchaseOrderItemDto { ProductId = 1, Quantity = 1, UnitPrice = 0 } } };
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
            var purchaseOrderDto = new Dtos.PurchaseOrderDto { SupplierId = 1, OrderItems = new List<PurchaseOrderItemDto> { new PurchaseOrderItemDto { ProductId = 1, Quantity = 1, UnitPrice = 1 } } };
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

            var serviceResult = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await service.UpdateAsync(0, new Dtos.PurchaseOrderDto()); });
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

            var serviceResult = await Assert.ThrowsAsync<DataNotFoundException>(async () => { await service.UpdateAsync(1, new Dtos.PurchaseOrderDto()); });
        }
        
        [Theory]
        [InlineData(Enums.OrderStatus.Approved)]
        [InlineData(Enums.OrderStatus.Completed)]
        [InlineData(Enums.OrderStatus.Cancelled)]
        public async Task Update_ShouldFailStatusIsApprovedOrCompletedOrCancelled(Enums.OrderStatus orderStatus)
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = orderStatus });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await Assert.ThrowsAsync<ValidationFailedException>(async () => { await service.UpdateAsync(1, new Dtos.PurchaseOrderDto()); });
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

            var serviceResult = await service.UpdateAsync(1, new Dtos.PurchaseOrderDto { SupplierId = 1, OrderItems = new List<PurchaseOrderItemDto> { new PurchaseOrderItemDto { ProductId = 1, Quantity = 1, UnitPrice = 1 } } });
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
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, Status = OrderStatus.New, SupplierId = 1, OrderItems = new List<PurchaseOrderItem> { new PurchaseOrderItem { ProductId = 1, Quantity = 1, UnitCost = 1 } } });
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
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, Status = OrderStatus.New, SupplierId = 1, OrderItems = new List<PurchaseOrderItem> { new PurchaseOrderItem { ProductId = 1, Quantity = 1, UnitCost = 1 } } });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await service.ApproveAsync(1);
            Assert.Contains(OrderStatus.Approved.ToString(), result.Message.Value);
        }

        [Fact]
        public async Task Cancel_ShouldThrowExceptionIfInvalidId()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.CancelAsync(0));
        }

        [Fact]
        public async Task Cancel_ShouldThrowExceptionIfPurchaseOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.CancelAsync(1));
        }

        [Theory]
        [InlineData(OrderStatus.Approved)]
        [InlineData(OrderStatus.Invoiced)]
        [InlineData(OrderStatus.Shipped)]
        [InlineData(OrderStatus.Paid)]
        [InlineData(OrderStatus.Completed)]
        [InlineData(OrderStatus.Cancelled)]
        public async Task Cancel_ShouldThrowExceptionIfPurchaseOrderIsNotNewOrSubmitted(OrderStatus orderStatus)
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = orderStatus});
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var exceptionResult = await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.CancelAsync(1));
            Assert.Equal($"Purchase Order is already {orderStatus}", exceptionResult.Message);
        }

        [Fact]
        public async Task Cancel_ShouldReturnCancelledStatus()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var result = await service.CancelAsync(1);

            Assert.Contains(OrderStatus.Cancelled.ToString(), result.Message.Value);
        }

        [Fact]
        public async Task GeneratePdfDocument_ShouldThrowExceptionOnInvalidId()
        {
            var mock = AutoMock.GetLoose();            
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GeneratePdfDocument(0));
        }

        [Fact]
        public async Task GeneratePdfDocument_ShouldThrowExceptionIfPurchaseOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null!);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.GeneratePdfDocument(1));
        }

        [Fact]
        [Trait("TODO", "Return to this after actual implementation.")]
        public async Task GeneratePdfDocument_ShouldReturnFilename()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem> {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1 },
                    new PurchaseOrderItem { Id = 2, ProductId = 2 , UnitCost = 2, Quantity = 1 }
                }
            });
            
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Supplier { Id = 1, Name = "Supplier One" });

            var outputFilename = "purchase-order-1.pdf";
            var docGenService = mock.Mock<IDocumentGeneratorService>();
            docGenService.Setup(x => x.CreatePurchaseOrderPdf(It.IsAny<Dtos.Document.PurchaseOrderDto>())).Returns(outputFilename);

            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var filename = await service.GeneratePdfDocument(1);
            
            Assert.Contains(outputFilename, filename);
        }

        [Fact]
        public async Task GeneratePdfDocument_ShouldThrowExceptionIfSupplierIsNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Supplier)null);
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.GeneratePdfDocument(1));
        }

        [Fact]
        public async Task GeneratePdfDocument_ShouldThrowExceptionIfThereAreNoLineItems()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Supplier { Id = 1, Name = "Supplier One"});
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.GeneratePdfDocument(1));
        }

        [Fact]
        public async Task GeneratePdfDocument_ShouldThrowExceptionIfShippingInformationIsNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Supplier { Id = 1 });
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New, OrderItems = new List<PurchaseOrderItem> { new PurchaseOrderItem { Id = 1 } } });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.GeneratePdfDocument(1));
        }

        //email pdf to supplier
        //purchase order is required
        //supplier is required for email destination
        [Fact]
        public async Task EmailPdfToSupplier_ShouldThrowExceptionInvalidPurchaseOrderId()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.EmailPdfToSupplier(0));
        }

        [Fact]
        public async Task EmailPdfToSupplier_ShouldThrowExceptionPurchaseOrderNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.EmailPdfToSupplier(1));
        }

        [Fact]
        public async Task EmailPdfToSupplier_ShouldThrowExceptionSupplierNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Supplier)null);
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.EmailPdfToSupplier(1));
        }

        [Fact]
        public async Task EmailPdfToSupplier_ShouldThrowExceptionIfPdfDocumentIsNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            var supplierRepo = mock.Mock<ISuppliersRepository>();
            supplierRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Entities.Supplier { Id = 1, Name = "Supplier One" });
            uow.Setup(x => x.SuppliersRepository).Returns(supplierRepo.Object);
            var emailService = mock.Mock<IEmailService>();
            emailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>())).Throws<FileNotFoundException>();

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<FileNotFoundException>(() => service.EmailPdfToSupplier(1));
        }

        [Fact]
        public async Task ReceiveInventory_ShouldReturnSameNumberOfPostedItems()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var repo = mock.Mock<IPurchaseOrdersRepository>();
            repo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem> {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1 },
                    new PurchaseOrderItem { Id = 2, ProductId = 2 , UnitCost = 2, Quantity = 1 }
                }
            });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(repo.Object);
            
            var inventoryTransactionRepo = mock.Mock<IInventoryTransactionsRepository>();
            uow.Setup(x => x.InventoryTransactionsRepository).Returns(inventoryTransactionRepo.Object);

            var itemsToReceive = new List<(int purchaseOrderId, int purchaseOrderItemId)>
            {
                (purchaseOrderId: 1, purchaseOrderItemId: 1),
                (purchaseOrderId: 1, purchaseOrderItemId: 2)
            };
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            
            List<(int purchaseOrderId, int purchaseOrderItemId, string result)> receivedItems = await service.ReceiveInventory(itemsToReceive);

            Assert.Equal(itemsToReceive.Count, receivedItems.Count);
        }

        [Fact]
        public async Task ReceiveInventory_PurchaseOrderNotFoundResult()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            var itemsToReceive = new List<(int purchaseOrderId, int purchaseOrderItemId)>
            {
                (purchaseOrderId: 1, purchaseOrderItemId: 1)
            };
            List<(int purchaseOrderId, int purchaseOrderItemId, string result)> receivedItems = await service.ReceiveInventory(itemsToReceive);
            var itemResult = receivedItems.FirstOrDefault();

            Assert.Equal("Purchase Order not found.", itemResult.result);
        }

        [Fact]
        public async Task ReceiveInventory_PurchaseOrderItemNotFoundResult()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem>()
            });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            var itemsToReceive = new List<(int purchaseOrderId, int purchaseOrderItemId)>
            {
                (purchaseOrderId: 1, purchaseOrderItemId: 1)
            };
            List<(int purchaseOrderId, int purchaseOrderItemId, string result)> receivedItems = await service.ReceiveInventory(itemsToReceive);
            var itemResult = receivedItems.FirstOrDefault();

            Assert.Equal("Purchase Order Item not found.", itemResult.result);
        }

        [Fact]
        public async Task ReceiveInventory_PurchaseOrderItemPostedResult()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            var poData = new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem>()
                {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1, PostedToInventory = false }
                }
            };

            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(poData);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);

            var inventoryTransactionRepo = mock.Mock<IInventoryTransactionsRepository>();
            uow.Setup(x => x.InventoryTransactionsRepository).Returns(inventoryTransactionRepo.Object);

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            var itemsToReceive = new List<(int purchaseOrderId, int purchaseOrderItemId)>
            {
                (purchaseOrderId: 1, purchaseOrderItemId: 1)
            };
            List<(int purchaseOrderId, int purchaseOrderItemId, string result)> receivedItems = await service.ReceiveInventory(itemsToReceive);
            var itemResult = receivedItems.FirstOrDefault();

            Assert.Equal("Purchase Order Item posted.", itemResult.result);            
            Assert.True(poData.OrderItems.First().PostedToInventory);
        }

        [Fact]
        public async Task ReceiveInventory_VerifyInventoryTransactionIsAddedInRepository()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            var poData = new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem>()
                {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1, PostedToInventory = false }
                }
            };
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(poData);

            var inventoryTransactionRepo = mock.Mock<IInventoryTransactionsRepository>();
            uow.Setup(x => x.InventoryTransactionsRepository).Returns(inventoryTransactionRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            var itemsToReceive = new List<(int purchaseOrderId, int purchaseOrderItemId)>
            {
                (purchaseOrderId: 1, purchaseOrderItemId: 1)
            };
            List<(int purchaseOrderId, int purchaseOrderItemId, string result)> receivedItems = await service.ReceiveInventory(itemsToReceive);

            mock.Mock<IInventoryTransactionsRepository>().Verify(x => x.Create(It.IsAny<InventoryTransaction>()));
        }

        [Fact]
        public async Task PaySupplier_ShouldThrowExceptionIfPurchaseOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poData = new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem>()
                {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1, PostedToInventory = false }
                }
            };
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.PaySupplierAsync(1, new Payment { Date = DateTime.Now, Method = PaymentMethodType.Cash, Amount = 10 }));
        }

        [Fact]
        public async Task PaySupplier_ShouldThrowExceptionIfSupplierDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poData = new PurchaseOrder
            {
                Id = 1,
                SupplierId = 1,
                Status = OrderStatus.New,
                OrderItems = new List<PurchaseOrderItem>()
                {
                    new PurchaseOrderItem { Id = 1, ProductId = 1 , UnitCost = 1, Quantity = 1, PostedToInventory = false }
                }
            };
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(poData);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            var suppliersRepo = mock.Mock<ISuppliersRepository>();
            suppliersRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Supplier)null);
            uow.Setup(x => x.SuppliersRepository).Returns(suppliersRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.PaySupplierAsync(1, new Payment { Date = DateTime.Now, Method = PaymentMethodType.Cash, Amount = 10 }));
        }

        [Fact]
        public async Task PaySupplier_ShouldThrowExceptionIfPaymentDataIsNull()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();            
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.PaySupplierAsync(1, null));
        }

        [Fact]
        public async void CompleteOrder_ShouldThrowExceptionIfOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.CompleteOrder(1));
        }

        [Fact]
        public async void CompleteOrder_ShouldSetStatusToCompletedIfSuccessful()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            var order = new PurchaseOrder { Id = 1, SupplierId = 1, Status = OrderStatus.New };
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(order);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await service.CompleteOrder(1);

            Assert.Equal(Enums.OrderStatus.Completed, order.Status);
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfCustomerOrderDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var purchaseOrdersRepo = mock.Mock<IPurchaseOrdersRepository>();
            purchaseOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Entities.PurchaseOrder)null!);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(purchaseOrdersRepo.Object);

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.AddItem(1, (PurchaseOrderItemDto)null!));
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfProductDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var purchaseOrdersRepo = mock.Mock<IPurchaseOrdersRepository>();
            purchaseOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1 });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(purchaseOrdersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Entities.Product)null);
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(async () => await service.AddItem(1, new PurchaseOrderItemDto { ProductId = 1, Quantity = 1, UnitPrice = 1 }));
        }

        [Fact]
        public async Task AddItem_ShouldThrowExceptionIfQuantityIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var purchaseOrdersRepo = mock.Mock<IPurchaseOrdersRepository>();
            purchaseOrdersRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrder { Id = 1, SupplierId = 1 });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(purchaseOrdersRepo.Object);
            var productsRepo = mock.Mock<IProductsRepository>();
            productsRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new Product { Id = 1, Name = "Product One" });
            uow.Setup(x => x.ProductsRepository).Returns(productsRepo.Object);

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(async () => await service.AddItem(1, new PurchaseOrderItemDto { ProductId = 1, Quantity = 0 }));
        }

        [Fact]
        public async Task RemoveItem_ShouldThrowExceptionIfIdIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RemoveItem(0));
        }

        [Fact]
        public async Task RemoveItem_ShouldThrowExceptionIfItemDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var poItemsRepo = mock.Mock<IPurchaseOrderItemsRepository>();
            poItemsRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrderItem?)null);
            var uow = mock.Mock<IUnitOfWork>();
            uow.Setup(x => x.PurchaseOrderItemsRepository).Returns(poItemsRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.RemoveItem(1));
        }

        [Fact]
        public async Task RemoveItem_ShouldThrowExceptionIfPurchaseOrerDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poItemsRepo = mock.Mock<IPurchaseOrderItemsRepository>();
            poItemsRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(new PurchaseOrderItem { Id = 1});
            uow.Setup(x => x.PurchaseOrderItemsRepository).Returns(poItemsRepo.Object);
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder?)null);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);

            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.RemoveItem(1));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfInvalidPurchaseOrderId()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.UpdateItem(0, new PurchaseOrderItemDto { Id = 1}));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderItemIsNull()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null!);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateItem(1, null!));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderIsNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((PurchaseOrder)null!);
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.UpdateItem(1, new PurchaseOrderItemDto { Id = 1, Quantity = 1, UnitPrice = 1}));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderItemIsNotFound()
        {
            var mock = AutoMock.GetLoose();
            var uow = mock.Mock<IUnitOfWork>();
            var poRepo = mock.Mock<IPurchaseOrdersRepository>();
            poRepo.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(
                new PurchaseOrder
                {
                    Id = 1,
                    OrderItems = new List<PurchaseOrderItem>()
                });
            uow.Setup(x => x.PurchaseOrdersRepository).Returns(poRepo.Object);
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.UpdateItem(1, new PurchaseOrderItemDto { Id = 1, Quantity = 1, UnitPrice = 1 }));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderItemQuantityIsInvalid()
        {
            var mock = AutoMock.GetLoose();            
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.UpdateItem(1, new PurchaseOrderItemDto { Id = 1, Quantity = 0 }));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderItemUnitPriceIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.UpdateItem(1, new PurchaseOrderItemDto { Id = 1, Quantity = 1, UnitPrice = 0 }));
        }

        [Fact]
        public async Task UpdateItem_ShouldThrowExceptionIfPurchaseOrderItemUnitPriceIsNotSet()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.UpdateItem(1, new PurchaseOrderItemDto { Id = 1, Quantity = 1 }));
        }
    }
}
