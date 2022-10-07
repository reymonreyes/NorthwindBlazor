using Northwind.Common.Validators;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Common.UnitTests.Validators
{
    public class SupplierValidatorTests
    {
        [Fact]
        public void ShouldReturnErrorsOnRequiredFields()
        {
            ISupplierValidator validator = new SupplierValidator();
            var result = validator.Validate(new Core.Dtos.SupplierDto());
            Assert.Contains(result, x => x.MessageType == ServiceMessageType.Error && x.Message.Value.Contains("Name"));
        }

        [Fact]
        public void ShouldNotReturnErrorsIfValidInput()
        {
            ISupplierValidator validator = new SupplierValidator();
            var result = validator.Validate(new Core.Dtos.SupplierDto { Name = "Supplier One" });
            Assert.Empty(result);
        }

        [Fact]
        public void ShouldReturnErrorsForMaxLengthErrors()
        {
            ISupplierValidator validator = new SupplierValidator();
            var result = validator.Validate(new Core.Dtos.SupplierDto
            {
                Name = new string('a', 41),
                ContactName = new string('a', 31),
                ContactTitle = new string('a', 31),
                Address = new string('a', 61),
                City = new string('a', 16),
                State = new string('a', 16),
                PostalCode = new string('a', 16),
                Country = new string('a', 16),
                Phone = new string('a', 25),
                Fax = new string('a', 25),
                Email = new string('a', 255)
            });
            var maxLengthFields = new string[] { "Name", "ContactName", "ContactTitle", "Address", "City", "State", "PostalCode", "Country", "Phone", "Fax", "Email" };

            Assert.All(maxLengthFields, x => Assert.NotNull(result.FirstOrDefault(y => y.Message.Key == x)));
        }

        [Fact]
        public void ShouldReturnErrorIfInvalidEmailAddress()
        {
            ISupplierValidator validator = new SupplierValidator();
            var result = validator.Validate(new Core.Dtos.SupplierDto
            {
                Name = "Test",
                Email = "someinvalidemail"
            });
            Assert.Contains(result, x => x.Message.Key == "Email");
        }
    }
}