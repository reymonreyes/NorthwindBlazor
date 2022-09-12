using Autofac;
using Autofac.Extras.Moq;
using Moq;
using Northwind.Common.Validators;
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
    public class ShippersServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowErrorIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            IShippersService shippersService = mock.Create<ShippersService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => shippersService.Create(null!));
        }

        [Fact]
        public async Task Create_ShouldThrowValidationExceptionForInvalidFields()
        {
            var validator = new ShipperValidator();
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(validator).As<IShipperValidator>();
            });
            IShippersService service = mock.Create<ShippersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.Create(new ShipperDto()));
        }        

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            IShippersService shippersService = mock.Create<ShippersService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => shippersService.Update(1, null));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfShipperIdIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            IShippersService shippersService = mock.Create<ShippersService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => shippersService.Update(0, new ShipperDto()));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfShipperDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var shippersRepoMock = mock.Mock<IShippersRepository>();
            shippersRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Shipper?)null);
            mock.Mock<IUnitOfWork>().Setup(x => x.ShippersRepository).Returns(shippersRepoMock.Object);
            IShippersService shippersService = mock.Create<ShippersService>();
            var result = await Assert.ThrowsAsync<Exception>(() => shippersService.Update(1, new ShipperDto()));
            Assert.True(result.Message == "shipper not found");
        }

        [Fact]
        public async Task Edit_ShouldThrowValidationFailedExceptionForInvalidFields()
        {
            var validator = new ShipperValidator();
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(validator).As<IShipperValidator>();
            });
            IShippersService service = mock.Create<ShippersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.Update(1, new ShipperDto()));
        }
    }
}
