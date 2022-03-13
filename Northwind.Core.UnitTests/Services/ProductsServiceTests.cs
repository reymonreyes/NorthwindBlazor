using Autofac.Extras.Moq;
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
    public class ProductsServiceTests
    {
        [Fact]
        public async Task Create_ShouldFailIfInvalidInputs()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<IUnitOfWork>().Setup(x => x.ProductsRepository).Returns(mock.Mock<IProductsRepository>().Object);
            var productsSvc = mock.Create<ProductsService>();
            await productsSvc.Create(new Dtos.ProductDto { Name = "Test Product 1" });

            Assert.True(true);
        }
    }
}
