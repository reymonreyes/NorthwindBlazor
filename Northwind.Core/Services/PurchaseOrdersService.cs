﻿using Northwind.Core.Dtos;
using DocumentDtos = Northwind.Core.Dtos.Document;
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
using Northwind.Core.Interfaces.Infrastructure;

namespace Northwind.Core.Services
{
    public class PurchaseOrdersService : IPurchaseOrdersService
    {
        private readonly IPurchaseOrderValidator _poValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentGeneratorService _documentGeneratorService;
        private readonly IEmailService _emailService;
        public PurchaseOrdersService(IPurchaseOrderValidator poValidator, IUnitOfWork unitOfWork, IDocumentGeneratorService documentGeneratorService, IEmailService emailService)
        {
            _poValidator = poValidator;
            _unitOfWork = unitOfWork;
            _documentGeneratorService = documentGeneratorService;
            _emailService = emailService;
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
            {
                await _unitOfWork.Stop();
                throw new ValidationFailedException(validationExceptions);
            }

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
            {
                await _unitOfWork.Stop();
                throw new ValidationFailedException($"Purchase Order already {purchaseOrder.Status}");                
            }
            
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

            if(purchaseOrder.OrderItems == null || !purchaseOrder.OrderItems.Any())
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("Order Items not found.");
            }

            purchaseOrder.Status = Enums.OrderStatus.Submitted;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("PurchaseOrder", purchaseOrder.Status.ToString()) };
        }

        public async Task<ServiceMessageResult> ApproveAsync(int id)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException("id");

            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (purchaseOrder == null)
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("Purchase Order not found.");
            }
            if(purchaseOrder.OrderItems == null || !purchaseOrder.OrderItems.Any())
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("Order Items not found.");
            }

            purchaseOrder.Status = Enums.OrderStatus.Approved;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("PurchaseOrder", purchaseOrder.Status.ToString()) };
        }

        public async Task<ServiceMessageResult> CancelAsync(int id)
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

            if(!(purchaseOrder.Status == Enums.OrderStatus.New || purchaseOrder.Status == Enums.OrderStatus.Submitted))
            {
                await _unitOfWork.Stop();
                throw new ValidationFailedException($"Purchase Order is already {purchaseOrder.Status}");
            }

            purchaseOrder.Status = Enums.OrderStatus.Cancelled;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            return new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("PurchaseOrder", purchaseOrder.Status.ToString()) };
        }

        public async Task<string> GeneratePdfDocument(int id)
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException("id");

            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (purchaseOrder == null)
                throw new DataNotFoundException("Purchase Order not found.");

            var supplier = await _unitOfWork.SuppliersRepository.Get(purchaseOrder.SupplierId);
            if (supplier == null)
                throw new DataNotFoundException("Supplier not found.");

            if (purchaseOrder.OrderItems == null || purchaseOrder.OrderItems.Count == 0)
                throw new DataNotFoundException("Line Items not found.");

            var poModel = new DocumentDtos.PurchaseOrderDto();
            var filename = _documentGeneratorService.CreatePurchaseOrderPdf(poModel);

            return filename;
        }

        public async Task EmailPdfToSupplier(int purchaseOrderId)
        {
            if (purchaseOrderId <= 0) throw new ArgumentOutOfRangeException("Purchase Order Id");

            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(1);
            if(purchaseOrder == null) throw new DataNotFoundException("Purchase Order");

            var supplier = await _unitOfWork.SuppliersRepository.Get(purchaseOrder.SupplierId);
            if (supplier == null) throw new DataNotFoundException("Supplier");

            _emailService.Send("address@mail.com", supplier.Email, $"Purchase Order #{purchaseOrder.Id}", "Please see attached file.");
        }
    }
}
