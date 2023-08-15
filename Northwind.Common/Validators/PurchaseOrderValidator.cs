using FluentValidation;
using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEnums = Northwind.Core.Enums;

namespace Northwind.Common.Validators
{
    public class PurchaseOrderValidator : AbstractValidator<PurchaseOrderDto>, IPurchaseOrderValidator
    {
        public PurchaseOrderValidator()
        {
            RuleFor(x => x.SupplierId).NotEmpty().WithName("SupplierId");
        }

        List<ServiceMessageResult>? Core.Interfaces.Validators.IValidator<PurchaseOrderDto>.Validate(PurchaseOrderDto purchaseOrder)
        {
            var result = Validate(purchaseOrder);
            var temp = result.Errors.Select(x => new ServiceMessageResult { MessageType = CoreEnums.ServiceMessageType.Error, Message = new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage) }).ToList();
            return temp;
        }
    }
}
