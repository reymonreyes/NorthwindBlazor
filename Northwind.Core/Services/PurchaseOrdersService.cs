using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseOrdersService(IPurchaseOrderValidator poValidator, IUnitOfWork unitOfWork)
        {
            _poValidator = poValidator;            
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult> Create(PurchaseOrderDto purchaseOrder)
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

            var newPurchaseOrder = new PurchaseOrder();
            newPurchaseOrder.Status = Enums.OrderStatus.New;
            newPurchaseOrder.SupplierId = purchaseOrder.SupplierId;

            await _unitOfWork.Start();
            await _unitOfWork.PurchaseOrdersRepository.CreateAsync(newPurchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", newPurchaseOrder.Id.ToString()) });

            return result;
        }

        public async Task<ServiceResult> UpdateAsync(int id, PurchaseOrderDto purchaseOrderDto)
        {
            if(purchaseOrderDto == null)
                throw new ArgumentNullException("product");
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id");

            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (purchaseOrder == null)
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("Purchase Order not found");
            }

            if(purchaseOrder.Status == Enums.OrderStatus.Approved || purchaseOrder.Status == Enums.OrderStatus.Closed || purchaseOrder.Status == Enums.OrderStatus.Cancelled)
                throw new ValidationFailedException($"Purchase Order already {purchaseOrder.Status}");                
            
            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            purchaseOrder.SupplierId = purchaseOrderDto.SupplierId;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", purchaseOrder.Id.ToString()) });
            return result;
        }

        public async Task<ServiceMessageResult> SubmitAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id");
            
            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (purchaseOrder == null)
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("Purchase Order not found.");
            }

            purchaseOrder.Status = Enums.OrderStatus.Submitted;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("PurchaseOrder", purchaseOrder.Status.ToString()) };
        }
    }
}
