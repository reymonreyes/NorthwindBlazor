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
    public class CategoryValidator : AbstractValidator<CategoryDto>, ICategoryValidator
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }

        List<ServiceMessageResult>? Core.Interfaces.Validators.IValidator<CategoryDto>.Validate(CategoryDto categoryDto)
        {
            var validationResult = Validate(categoryDto);
            return validationResult.Errors.Select(x => new ServiceMessageResult
            {
                MessageType = Core.Enums.ServiceMessageType.Error,
                Message = new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage)
            }).ToList();
        }
    }
}
