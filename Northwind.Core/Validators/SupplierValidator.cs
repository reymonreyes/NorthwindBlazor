using FluentValidation;
using Northwind.Core.Dtos;
using Northwind.Core.Enums;
using Northwind.Core.Interfaces.Validators;

namespace Northwind.Core.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierDto>, ISupplierValidator
    {
        public SupplierValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(40);
            RuleFor(x => x.ContactName).MaximumLength(30);
            RuleFor(x => x.ContactTitle).MaximumLength(30);
            RuleFor(x => x.Address).MaximumLength(60);
            RuleFor(x => x.City).MaximumLength(15);
            RuleFor(x => x.State).MaximumLength(15);
            RuleFor(x => x.PostalCode).MaximumLength(15);
            RuleFor(x => x.Country).MaximumLength(15);
            RuleFor(x => x.Phone).MaximumLength(24);
            RuleFor(x => x.Fax).MaximumLength(24);
            RuleFor(x => x.Email).MaximumLength(254).EmailAddress();
        }

        List<ServiceMessageResult>? Core.Interfaces.Validators.IValidator<SupplierDto>.Validate(SupplierDto supplier)
        {
            var validationResult = Validate(supplier);
            return validationResult.Errors.Select(x => new ServiceMessageResult { MessageType = ServiceMessageType.Error, Message =  new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage)}).ToList();
        }
    }
}