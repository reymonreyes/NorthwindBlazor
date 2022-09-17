using Autofac;
using Autofac.Extras.Moq;
using Northwind.Common.Validators;
using Northwind.Core.Dtos;
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
        public async Task Create_ShouldThrowValidationFailedException()
        {
            var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterInstance(new CustomerValidator()).As<ICustomerValidator>();
            });
            ICustomersService service = mock.Create<CustomersService>();

            await Assert.ThrowsAsync<ValidationFailedException>(() => service.Create(new CustomerDto()));
        }


    }
}
