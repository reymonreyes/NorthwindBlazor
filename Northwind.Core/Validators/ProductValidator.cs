using FluentValidation;
using Northwind.Core.Dtos;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Validators
{
    public class ProductValidator : AbstractValidator<ProductDto>, IProductValidator
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
        List<ServiceMessageResult> Core.Interfaces.Validators.IValidator<ProductDto>.Validate(ProductDto instance)
        {
            var validationResult = Validate(instance);
            return validationResult.Errors.Select(x => new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage) }).ToList();
        }
    }
}
