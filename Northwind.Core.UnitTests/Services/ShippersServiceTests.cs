using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Dtos;
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
    }
}
