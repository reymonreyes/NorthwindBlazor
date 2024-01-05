using Northwind.Core.Dtos;
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
using Northwind.Core.ValueObjects;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;

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

            if (purchaseOrder.OrderItems != null && purchaseOrder.OrderItems.Count > 0)           
            {
                if (purchaseOrder.OrderItems!.Any(x => x.Quantity <= 0)) 
                    validationExceptions.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("OrderItems", "Order Item quantity must be greater than 0"), MessageType = Enums.ServiceMessageType.Error });
                if (purchaseOrder.OrderItems!.Any(x => x.UnitPrice <= 0))
                    validationExceptions.Add(new ServiceMessageResult { Message = new KeyValuePair<string, string>("OrderItems", "Order Item unit cost must be greater than 0"), MessageType = Enums.ServiceMessageType.Error });
            }

            if (validationExceptions.Any())
            {
                throw new ValidationFailedException(validationExceptions);
            }

            var newPurchaseOrder = new PurchaseOrder();
            newPurchaseOrder.Status = Enums.OrderStatus.New;
            newPurchaseOrder.SupplierId = purchaseOrder.SupplierId;
            newPurchaseOrder.ShipTo = purchaseOrder.ShipTo;
            newPurchaseOrder.OrderDate = purchaseOrder.OrderDate;
            newPurchaseOrder.Notes = purchaseOrder.Notes;

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

            if(purchaseOrder.Status == Enums.OrderStatus.Approved || purchaseOrder.Status == Enums.OrderStatus.Completed || purchaseOrder.Status == Enums.OrderStatus.Cancelled)
            {
                await _unitOfWork.Stop();
                throw new ValidationFailedException($"Purchase Order already {purchaseOrder.Status}");                
            }
            
            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            purchaseOrder.SupplierId = purchaseOrderDto.SupplierId;
            purchaseOrder.OrderDate = purchaseOrderDto.OrderDate;
            purchaseOrder.ShipTo = purchaseOrderDto.ShipTo;
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
            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (purchaseOrder == null)
                throw new DataNotFoundException("Purchase Order not found.");

            var supplier = await _unitOfWork.SuppliersRepository.Get(purchaseOrder.SupplierId);
            if (supplier == null)
                throw new DataNotFoundException("Supplier not found.");

            if (purchaseOrder.OrderItems == null || purchaseOrder.OrderItems.Count == 0)
                throw new DataNotFoundException("Line Items not found.");

            if(string.IsNullOrWhiteSpace(purchaseOrder.ShipTo))
                throw new DataNotFoundException("Shipping Information not found");

            var poModel = new DocumentDtos.PurchaseOrderDto();
            poModel.PONumber = purchaseOrder.Id.ToString();
            poModel.Date = DateTime.Now;
            poModel.Notes = purchaseOrder.Notes;
            poModel.Company = new DocumentDtos.BasicCompanyInformation { Name = "Northwind Blazor", Address = new DocumentDtos.Address { Street = "1st", City = "Tagum City" } };
            poModel.ShipTo = new DocumentDtos.ShipTo { Name = "Northwind Blazor", Address = new DocumentDtos.Address { Street = purchaseOrder.ShipTo } };
            poModel.Supplier = new DocumentDtos.Supplier { Name = supplier.Name, Address = new DocumentDtos.Address { Street = supplier.Address, City = supplier.City, State = supplier.State, PostalCode = supplier.PostalCode, Phone = supplier.Phone } };
            poModel.LineItems = purchaseOrder.OrderItems.Select(x => new DocumentDtos.LineItem { ItemDescription = x.ProductId.ToString(), Qty = x.Quantity, UnitPrice = x.UnitCost, Total = x.UnitCost * x.Quantity }).ToList();
            //TODO all fields below
            var subtotal = purchaseOrder.OrderItems.Sum(x => x.UnitCost * x.Quantity);
            poModel.Subtotal = subtotal;
            poModel.Tax = 0;
            poModel.ShippingCost = 0;
            poModel.Total = subtotal + poModel.Tax + poModel.ShippingCost;
            poModel.PreparedBy = "Northwind Blazor";
            poModel.ApprovedBy = "Northwind Blazor";
            var filename = _documentGeneratorService.CreatePurchaseOrderPdf(poModel);
            await _unitOfWork.Stop();

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

        public async Task<List<(int purchaseOrderId, int purchaseOrderItemId, string result)>> ReceiveInventory(List<(int purchaseOrderId, int purchaseOrderItemId)> itemsToReceive)
        {
            var result = new List<(int purchaseOrderId, int purchaseOrderItemId, string result)>();

            await _unitOfWork.Start();
            
            foreach (var item in itemsToReceive)
            {
                var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(item.purchaseOrderId);
                if (purchaseOrder == null)
                    result.Add((item.purchaseOrderId, item.purchaseOrderItemId, "Purchase Order not found."));
                else
                {
                    var poItem = purchaseOrder.OrderItems.FirstOrDefault(x => x.Id == item.purchaseOrderItemId);
                    if(poItem == null)
                        result.Add((item.purchaseOrderId, item.purchaseOrderItemId, "Purchase Order Item not found."));
                    else
                    {
                        //todo: add inventory transaction and flag as posted to inventory
                        poItem.PostedToInventory = true;
                        await _unitOfWork.InventoryTransactionsRepository.Create(new InventoryTransaction
                        {
                            ProductId = poItem.ProductId,
                            PurchaseOrderId = purchaseOrder.Id,
                            Quantity = poItem.Quantity,
                            Created = DateTime.UtcNow,
                            TransactionType = Enums.InventoryTransactionType.Purchased
                        });

                        await _unitOfWork.Commit();
                        result.Add((item.purchaseOrderId, item.purchaseOrderItemId, "Purchase Order Item posted."));
                    }
                }
            }

            await _unitOfWork.Stop();

            return result;
        }

        public async Task PaySupplierAsync(int purchaseOrderId, Payment payment)
        {
            if(payment == null) throw new ArgumentNullException(nameof(payment));

            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(purchaseOrderId);
            if (purchaseOrder == null) throw new DataNotFoundException("Purchase Order not found.");
            
            var supplier = await _unitOfWork.SuppliersRepository.Get(purchaseOrder.SupplierId);
            if (supplier == null) throw new DataNotFoundException("Supplier not found.");

            purchaseOrder.Payment = payment;
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task CompleteOrder(int id)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if (order == null) throw new DataNotFoundException("Customer Order not found.");

            order.Status = Enums.OrderStatus.Completed;
            _unitOfWork.PurchaseOrdersRepository.Update(order);

            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task AddItem(int id, PurchaseOrderItemDto purchaseOrderItemDto)
        {
            await _unitOfWork.Start();
            var order = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            await _unitOfWork.Stop();
            if (order == null) throw new DataNotFoundException("Purchase Order not found.");
            
            if (purchaseOrderItemDto != null)
            {
                await _unitOfWork.Start();
                var product = await _unitOfWork.ProductsRepository.Get(purchaseOrderItemDto.ProductId);
                if (product == null) throw new DataNotFoundException("Product not found.");
                if (purchaseOrderItemDto.Quantity <= 0) throw new ValidationFailedException("Quantity must be greater than 0.");

                var orderItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = order.Id,
                    ProductId = purchaseOrderItemDto.ProductId,
                    Quantity = purchaseOrderItemDto.Quantity,
                    UnitCost = purchaseOrderItemDto.UnitPrice ?? product.ListPrice,
                };
                await _unitOfWork.PurchaseOrderItemsRepository.Create(orderItem);                
                await _unitOfWork.Commit();
                await _unitOfWork.Stop();
            }
        }

        public async Task<PurchaseOrderDto?> GetAsync(int id)
        {
            PurchaseOrderDto? result = null;
            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(id);
            if(purchaseOrder != null)
            {
                result = new PurchaseOrderDto
                {
                    SupplierId = purchaseOrder.SupplierId,
                    ShipTo = purchaseOrder.ShipTo,
                    OrderDate = purchaseOrder.OrderDate.Value.ToUniversalTime(),
                    Status = purchaseOrder.Status,
                    OrderItems = purchaseOrder.OrderItems.Select(x => new PurchaseOrderItemDto { Id = x.Id, ProductId = x.ProductId, Quantity = x.Quantity, UnitPrice = x.UnitCost }).ToList()
                };
            }

            await _unitOfWork.Stop();
            return result;
        }

        public async Task RemoveItem(int purchaseOrderItemId)
        {
            if(purchaseOrderItemId <= 0)
                throw new ArgumentOutOfRangeException(nameof(purchaseOrderItemId));

            await _unitOfWork.Start();
            var purchaseOrderItem = await _unitOfWork.PurchaseOrderItemsRepository.GetAsync(purchaseOrderItemId);
            if (purchaseOrderItem is null)
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("PurchaseOrderItem not found");
            }
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(purchaseOrderItem.PurchaseOrderId);
            if (purchaseOrder is null)
            {
                await _unitOfWork.Stop();
                throw new DataNotFoundException("PurchaseOrder not found");
            }

            await _unitOfWork.PurchaseOrderItemsRepository.DeleteAsync(purchaseOrderItemId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task UpdateItem(int purchaseOrderId, PurchaseOrderItemDto purchaseOrderItem)
        {
            if (purchaseOrderId <= 0) throw new ArgumentOutOfRangeException($"{nameof(purchaseOrderId)}");
            if (purchaseOrderItem is null) throw new ArgumentNullException($"{nameof(purchaseOrderItem)}");
            if (purchaseOrderItem.Quantity <= 0) throw new ValidationFailedException("Quantity must be greater than 0");
            if (!purchaseOrderItem.UnitPrice.HasValue) throw new ValidationFailedException("UnitPrice is required");
            if (purchaseOrderItem.UnitPrice <= 0) throw new ValidationFailedException("UnitPrice must be greater than 0");

            await _unitOfWork.Start();
            var purchaseOrder = await _unitOfWork.PurchaseOrdersRepository.GetAsync(purchaseOrderId);
            if (purchaseOrder is null) throw new DataNotFoundException("PurchaseOrder not found");
            var purchaseOrderItemData = purchaseOrder.OrderItems.FirstOrDefault(x => x.Id == purchaseOrderItem.Id);
            if (purchaseOrderItemData is null) throw new DataNotFoundException("PurchaseOrderItem not found");

            purchaseOrderItemData.Quantity = purchaseOrderItem.Quantity;
            purchaseOrderItemData.UnitCost = purchaseOrderItem.UnitPrice.Value;
            _unitOfWork.PurchaseOrdersRepository.Update(purchaseOrder);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<(int TotalRecords, IEnumerable<PurchaseOrderDto> Records)> GetAllAsync(int page = 1, int size = 10)
        {
            await _unitOfWork.Start();
            var orders = await _unitOfWork.PurchaseOrdersRepository.GetAllAsync(page, size);
            var results = orders.Records.Select(x => new PurchaseOrderDto { Id = x.Id, SupplierId = x.SupplierId, Status = x.Status }).ToList();
            await _unitOfWork.Stop();

            return (orders.TotalRecords, results);
        }
    }
}
