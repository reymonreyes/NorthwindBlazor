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
    public class PurchaseOrderValidatorTests
    {
        [Fact]
        public void ShouldReturnErrors()
        {
            IPurchaseOrderValidator validator = new PurchaseOrderValidator();
            var result = validator.Validate(new Core.Dtos.PurchaseOrderDto());
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ShouldReturnEmptyIfValid()
        {
            IPurchaseOrderValidator validator = new PurchaseOrderValidator();
            var result = validator.Validate(new Core.Dtos.PurchaseOrderDto { SupplierId = 1 });
            Assert.Empty(result);
        }
    }
}
