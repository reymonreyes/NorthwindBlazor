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
    public class SuppliersServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            ISuppliersService service = mock.Create<SuppliersService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.Create(null));
        }

        [Fact]
        public async Task Create_ShouldUseValidatorForInputValidation()
        {
            var mock = AutoMock.GetLoose();
            ISuppliersService service = mock.Create<SuppliersService>();
            var validatorMock = mock.Mock<ISupplierValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<SupplierDto>())).Returns(new List<ServiceMessageResult>());
            var supplier = new SupplierDto();
            var result = await service.Create(supplier);
            validatorMock.Verify(x => x.Validate(supplier), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnErrorsIfInvalidInputFields()
        {
            var mock = AutoMock.GetLoose();
            ISuppliersService service = mock.Create<SuppliersService>();
            var validatorMock = mock.Mock<ISupplierValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<SupplierDto>())).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("Name", "Required") }
            });
            var supplier = new SupplierDto();
            var result = await service.Create(supplier);
            Assert.True(result.Messages != null && result.Messages.Count(x => x.MessageType == Enums.ServiceMessageType.Error) > 0);
        }
    }
}
