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
            var supplierDto = new SupplierDto { Name = String.Empty };

            var mock = AutoMock.GetLoose();
            mock.Mock<IUnitOfWork>().Setup(x => x.SuppliersRepository).Returns((mock.Mock<ISuppliersRepository>()).Object);
            ISuppliersService service = mock.Create<SuppliersService>();
            var validatorMock = mock.Mock<ISupplierValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<SupplierDto>())).Returns(new List<ServiceMessageResult>()).Verifiable();
            
            var result = await service.Create(supplierDto);

            validatorMock.Verify(x => x.Validate(supplierDto), Times.Once);
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
        
        [Fact]
        public async Task Edit_ShouldThrowErrorIfIdIsInvalid()
        {
            var mock = AutoMock.GetLoose();
            ISuppliersService service = mock.Create<SuppliersService>();
            var supplierDto = new SupplierDto
            {
                Id = 1,
                Name = "Supplier One"
            };
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.Edit(0, supplierDto));
        }

        [Fact]
        public async Task Edit_ShouldThrowErrorIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            ISuppliersService service = mock.Create<SuppliersService>();            
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.Edit(1, It.IsAny<SupplierDto>()));
        }

        [Fact]
        public async Task Edit_ShouldThrowErrorIfSupplierDoesNotExist()
        {
            var mock = AutoMock.GetLoose();
            var suppliersRepoMock = mock.Mock<ISuppliersRepository>();
            suppliersRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Supplier?)null);
            mock.Mock<IUnitOfWork>().Setup(x => x.SuppliersRepository).Returns(suppliersRepoMock.Object);
            ISuppliersService service = mock.Create<SuppliersService>();

            var result = await Assert.ThrowsAsync<Exception>(() => service.Edit(1, new SupplierDto { Name = "Supplier One" }));
            
            Assert.True(result.Message == "supplier not found");
        }
        
        [Fact]
        public async Task Edit_ShouldReturnErrorsIfInvalidInputFields()
        {
            var supplier = new Supplier
            {
                Name = String.Empty
            };

            var mock = AutoMock.GetLoose();
            var suppliersRepoMock = mock.Mock<ISuppliersRepository>();
            suppliersRepoMock.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(supplier);
            mock.Mock<IUnitOfWork>().Setup(x => x.SuppliersRepository).Returns(suppliersRepoMock.Object);
            ISuppliersService service = mock.Create<SuppliersService>();
            var validatorMock = mock.Mock<ISupplierValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<SupplierDto>())).Returns(new List<ServiceMessageResult> {
                new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>("Name", "Required") }
            });
            var supplierDto = new SupplierDto();

            var result = await service.Edit(1, supplierDto);

            Assert.True(result.Messages != null && result.Messages.Count(x => x.MessageType == Enums.ServiceMessageType.Error) > 0);
        }
        
    }
}
