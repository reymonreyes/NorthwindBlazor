using Northwind.Core.Validators;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Core.UnitTests.Validators
{
    public class ProductValidatorTests
    {
        [Fact]
        public void ShouldReturnErrorsOnRequiredFields()
        {
            IProductValidator validator = new ProductValidator();
            var result = validator.Validate(new Core.Dtos.ProductDto());

            Assert.Contains(result, x => x.MessageType == ServiceMessageType.Error && x.Message.Value.Contains("Name"));
            Assert.Contains(result, x => x.MessageType == ServiceMessageType.Error && x.Message.Value.Contains("Code"));
        }
    }
}
