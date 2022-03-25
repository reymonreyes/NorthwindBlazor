using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => shippersService.Create(null));
        }

        [Fact]
        public async Task Create_ShouldReturnErrorsIfInputRequiredFieldsAreEmpty()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<IShipperValidator>().Setup(x => x.Validate(It.IsAny<ShipperDto>())).Returns(new List<ServiceMessageResult>
            {
                new ServiceMessageResult{ MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("Name", "Required") }
            });
            IShippersService shippersService = mock.Create<ShippersService>();

            var result = await shippersService.Create(new ShipperDto());

            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            IShippersService shippersService = mock.Create<ShippersService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => shippersService.Edit(1, null));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfShipperIdIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            IShippersService shippersService = mock.Create<ShippersService>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => shippersService.Edit(0, new ShipperDto()));
        }

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfShipperDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var shippersRepoMock = mock.Mock<IShippersRepository>();
            shippersRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Shipper?)null);
            mock.Mock<IUnitOfWork>().Setup(x => x.ShippersRepository).Returns(shippersRepoMock.Object);
            IShippersService shippersService = mock.Create<ShippersService>();
            var result = await Assert.ThrowsAsync<Exception>(() => shippersService.Edit(1, new ShipperDto()));
            Assert.True(result.Message == "shipper not found");
        }

        [Fact]
        public async Task Edit_ShouldReturnErrorsIfValidationFailed()
        {
            var mock = AutoMock.GetLoose();
            var validator = mock.Mock<IShipperValidator>();
            validator.Setup(x => x.Validate(It.IsAny<ShipperDto>())).Returns(new List<ServiceMessageResult>
            {
                new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("Name", "Required")}
            });
            IShippersService shippersService = mock.Create<ShippersService>();
            var result = await shippersService.Edit(1, new ShipperDto());
            Assert.True(result.Messages?.Count > 0);
        }
    }
}
