using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class PurchaseOrdersService : IPurchaseOrdersService
    {
        private readonly IPurchaseOrderValidator _poValidator;
        
        public PurchaseOrdersService(IPurchaseOrderValidator poValidator)
        {
            _poValidator = poValidator;
        }
        public void Create(PurchaseOrderDto purchaseOrder)
        {
            if (purchaseOrder is null)
                throw new ArgumentNullException("purchaseOrder");

            var validationExceptions = new List<ServiceMessageResult>();
            
            validationExceptions.AddRange(_poValidator.Validate(purchaseOrder)!);

            if (purchaseOrder.OrderItems is null || purchaseOrder.OrderItems.Count == 0)
                validationExceptions.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("OrderItems", "Order Items are required"), MessageType = Enums.ServiceMessageType.Error });
            else
            {
                if (purchaseOrder.OrderItems!.Any(x => x.Quantity <= 0)) 
                    validationExceptions.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("OrderItems", "Order Item quantity must be greater than 0"), MessageType = Enums.ServiceMessageType.Error });
                if (purchaseOrder.OrderItems!.Any(x => x.UnitCost <= 0))
                    validationExceptions.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("OrderItems", "Order Item unit cost must be greater than 0"), MessageType = Enums.ServiceMessageType.Error });
            }

            if (validationExceptions.Any())
                throw new ValidationFailedException(validationExceptions);

            //add repo logic

        }
    }
}
