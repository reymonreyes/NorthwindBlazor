using Autofac;
using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
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
    public class ProductsServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfInputIsNull()
        {            
            var productsSvc = GetMock();
            ProductDto productDto = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => productsSvc.Create(productDto!));
        }        
        
        [Fact]
        public async Task Create_ShouldReturnIsSuccessfulIfSuccessful()
        {
            var mock = AutoMock.GetLoose();
            var productsRepo = mock.Mock<IProductsRepository>();
            mock.Mock<IUnitOfWork>().Setup(x => x.ProductsRepository).Returns(productsRepo.Object);
            var productsSvc = mock.Create<ProductsService>();
            var result = await productsSvc.Create(new ProductDto { Name = "Aniseed", Code = "ANSD" });
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationFailedExceptionForInvalidFields()
        {
            var validator = new ProductValidator();
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(validator).As<IProductValidator>();
            });
            IProductsService productsService = mock.Create<ProductsService>();
            await Assert.ThrowsAsync<ValidationFailedException>(() => productsService.Create(new ProductDto()));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            var productsService = mock.Create<ProductsService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => productsService.Update(It.IsAny<int>(), It.IsAny<ProductDto>()));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfInvalidId()
        {
            var mock = AutoMock.GetLoose();
            var productsService = mock.Create<ProductsService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => productsService.Update(0, new ProductDto()));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfProductDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var productsRepoMock = mock.Mock<IProductsRepository>();
            productsRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Product?)null);
            var uowMock = mock.Mock<IUnitOfWork>();
            uowMock.Setup(x => x.ProductsRepository).Returns(productsRepoMock.Object);
            var productsService = mock.Create<ProductsService>();
            
            await Assert.ThrowsAsync<DataNotFoundException>(() => productsService.Update(1, new ProductDto()));
        }

        [Fact]
        public async Task Edit_ShouldReturnIsSuccessfulIfUpdateWasSuccessful()
        {
            var productDto = new ProductDto
            {
                Id = 1,
                Name = "Product 1",
                Code = "PRD1"
            };
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Code = "PRD1"
            };

            var mock = AutoMock.GetLoose();
            var productsRepoMock = mock.Mock<IProductsRepository>();
            productsRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(product);
            var uowMock = mock.Mock<IUnitOfWork>();
            uowMock.Setup(x => x.ProductsRepository).Returns(productsRepoMock.Object);
            var productsService = mock.Create<ProductsService>();
            var result = await productsService.Update(1, productDto);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Edit_ShouldThrowValidationFailedExceptionForInvalidFields()
        {
            var validator = new ProductValidator();
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(validator).As<IProductValidator>();
            });
            IProductsService productsService = mock.Create<ProductsService>();
            await Assert.ThrowsAsync<ValidationFailedException>(() => productsService.Update(1, new ProductDto()));
        }

        private IProductsService GetMock()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<IUnitOfWork>().Setup(x => x.ProductsRepository).Returns(mock.Mock<IProductsRepository>().Object);
            return mock.Create<ProductsService>();
        }
    }
}
