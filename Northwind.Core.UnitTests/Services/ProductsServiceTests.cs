using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Dtos;
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
    public class ProductsServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfInputIsNull()
        {            
            var productsSvc = GetMock();
            ProductDto? productDto = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() => productsSvc.Create(productDto));
        }

        [Fact]
        public async Task Create_ShouldFailIfInvalidInputs()
        {
            var (validatorMock, productsService) = GetValidatorMock();
            var product = new ProductDto();
            var result = await productsService.Create(product);

            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Create_ShouldUseValidatorForInputValidation()
        {
            var (validatorMock, productsService) = GetValidatorMock();
            var product = new ProductDto();
            var result = await productsService.Create(product);

            validatorMock.Verify(x => x.Validate(product), Times.Once);
        }

        private IProductsService GetMock()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<IUnitOfWork>().Setup(x => x.ProductsRepository).Returns(mock.Mock<IProductsRepository>().Object);
            return mock.Create<ProductsService>();
        }

        private (Mock<IProductValidator> mock, ProductsService productsService) GetValidatorMock()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<IUnitOfWork>().Setup(x => x.ProductsRepository).Returns(mock.Mock<IProductsRepository>().Object);
            var validatorMock = mock.Mock<IProductValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<ProductDto>())).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("Name", "Required") }
            }).Verifiable();
            var productsService = mock.Create<ProductsService>();

            return (validatorMock, productsService);
        }
    }
}
