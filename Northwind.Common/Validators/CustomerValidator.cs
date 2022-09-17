using FluentValidation;
using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Common.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDto>, ICustomerValidator
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }

        List<ServiceMessageResult>? Core.Interfaces.Validators.IValidator<CustomerDto>.Validate(CustomerDto customer)
        {
            var validationResult = Validate(customer);
            return validationResult.Errors.Select(x => new ServiceMessageResult
            {
                MessageType = Core.Enums.ServiceMessageType.Error,
                Message = new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage)
            }).ToList();
        }
    }
}
