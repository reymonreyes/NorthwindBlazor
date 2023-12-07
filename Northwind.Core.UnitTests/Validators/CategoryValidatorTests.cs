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
    public class CategoryValidatorTests
    {
        [Fact]
        public void ShouldReturnErrorsOnRequiredFields()
        {
            ICategoryValidator validator = new CategoryValidator();
            var result = validator.Validate(new Core.Dtos.CategoryDto());

            Assert.Contains(result, x => x.MessageType == ServiceMessageType.Error && x.Message.Value.Contains("Name"));
        }
    }
}
