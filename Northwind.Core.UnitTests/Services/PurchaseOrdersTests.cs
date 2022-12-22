﻿using Autofac.Extras.Moq;
using Moq;
using Northwind.Common.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
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
        public void Create_ShouldThrowNullExceptionForNullParameter()
        {
            var mock = AutoMock.GetLoose();
            IPurchaseOrderValidator validator = mock.Create<PurchaseOrderValidator>();
            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            Assert.Throws<ArgumentNullException>(() => service.Create(null));
        }

        [Fact]
        public void Create_ShouldThrowValidationExceptionForInvalidFields()
        {
            var poDto = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } } };            
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(poDto)).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { Message = new KeyValuePair<string, string>("SupplierId", "SupplierId is required"), MessageType = Enums.ServiceMessageType.Error }
            });
;            IPurchaseOrdersService service = mock.Create<PurchaseOrdersService>();
            Assert.Throws<ValidationFailedException>(() => service.Create(poDto));
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
        public void Create_PurchaseOrderMustContainAnItem()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1 };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            var service = mock.Create<PurchaseOrdersService>();

            var exception = Assert.Throws<ValidationFailedException>(() => service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Items are required"));
        }

        [Fact]
        public void Create_PurchaseOrderMustThrowExceptionIfItemQuantityIsLessThanOne()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 0 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            var service = mock.Create<PurchaseOrdersService>();

            var exception = Assert.Throws<ValidationFailedException>(() => service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Item quantity must be greater than 0"));            
        }

        [Fact]
        public void Create_PurchaseOrderMustThrowExceptionIfItemUnitCostIsLessThanOne()
        {
            var purchaseOrder = new PurchaseOrderDto { SupplierId = 1, OrderItems = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1, UnitCost = 0 } } };
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IPurchaseOrderValidator>();
            validator.Setup(x => x.Validate(purchaseOrder)).Returns(new List<ServiceMessageResult>());
            var service = mock.Create<PurchaseOrdersService>();

            var exception = Assert.Throws<ValidationFailedException>(() => service.Create(purchaseOrder));
            Assert.Contains(exception.ValidationErrors, x => x.Message.Value.Contains("Order Item unit cost must be greater than 0"));
        }
    }
}
