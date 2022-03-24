using Northwind.Common.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Common.UnitTests.Validators
{
    public class ShipperValidatorTests
    {
        [Fact]
        public void ShouldReturnErrorsOnRequiredFields()
        {
            IShipperValidator shipperValidator = new ShipperValidator();
            var result = shipperValidator.Validate(new ShipperDto());
            Assert.True(result?.Count > 0);
        }
    }
}
