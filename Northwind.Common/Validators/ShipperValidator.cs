using FluentValidation;
using Northwind.Core.Dtos;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Common.Validators
{
    public class ShipperValidator : AbstractValidator<ShipperDto>, IShipperValidator
    {
        public ShipperValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(40);
            RuleFor(x => x.Phone).MaximumLength(24);
            RuleFor(x => x.ContactName).NotNull();
        }
        List<ServiceMessageResult>? Core.Interfaces.Validators.IValidator<ShipperDto>.Validate(ShipperDto value)
        {
            var result = Validate(value);
            return result.Errors.Select(x =>
                new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message = new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage) }).ToList();
        }
    }
}
