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
    public class CustomersServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfArgumentIsNull()
        {
            var mock = AutoMock.GetLoose();
            ICustomersService service = mock.Create<CustomersService>();
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.Create(null!));
        }

        [Fact]
        public async Task Create_ShouldThrowValidationFailedExceptionForInvalidFields()
        {
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(new CustomerValidator()).As<ICustomerValidator>();
            });
            ICustomersService service = mock.Create<CustomersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.Create(new CustomerDto()));
        }        

        [Fact]
        public async Task Edit_ShouldThrowExceptionIfArgumentIsNull()
        {
            var mock = AutoMock.GetLoose();
            ICustomersService service = mock.Create<CustomersService>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.Update(1, null!));
        }

        [Fact]
        public async Task Edit_ShouldThrowValidationFailedExceptionForInvalidFields()
        {
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(new CustomerValidator()).As<ICustomerValidator>();
            });
            ICustomersService service = mock.Create<CustomersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.Update(1, new CustomerDto()));
        }

        [Fact]
        public async Task Edit_ShouldThrowDataNotFoundExceptionIfCustomerIsNotFound()
        {
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(new CustomerValidator()).As<ICustomerValidator>();
            });
            var customersRepository = mock.Mock<ICustomersRepository>();
            customersRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Customer?)null);
            var unitOfWork = mock.Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.CustomersRepository).Returns(customersRepository.Object);
            var service = mock.Create<CustomersService>();

            await Assert.ThrowsAsync<DataNotFoundException>(() => service.Update(1, new CustomerDto { Id = 1, Name = "AAAA" }));
        }        
    }
}
