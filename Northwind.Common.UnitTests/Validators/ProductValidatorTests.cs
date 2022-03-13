using Northwind.Common.Validators;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Common.UnitTests.Validators
{
    public class ProductValidatorTests
    {
        [Fact]
        public void ShouldReturnErrorsOnRequiredFields()
        {
            IProductValidator validator = new ProductValidator();
            var result = validator.Validate(new Core.Dtos.ProductDto());

            Assert.True(result.Count > 0);
        }
    }
}
