﻿using Northwind.Core.Dtos;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface IPurchaseOrdersService
    {
        Task AddItem(int id, PurchaseOrderItemDto purchaseOrderItemDto);
        Task<ServiceMessageResult> ApproveAsync(int id);
        Task<ServiceMessageResult> CancelAsync(int id);
        Task CompleteOrder(int id);
        Task<ServiceResult> Create(PurchaseOrderDto purchaseOrder);
        Task EmailPdfToSupplier(int purchaseOrderId);
        Task<string> GeneratePdfDocument(int id);
        Task<(int TotalRecords, IEnumerable<PurchaseOrderSummaryDto> Records)> GetAllAsync(int page = 1, int size = 10);
        Task<PurchaseOrderDto?> GetAsync(int id, bool includeLinkedNames = false);
        Task PaySupplierAsync(int v, Payment payment);
        Task<List<(int purchaseOrderId, int purchaseOrderItemId, string result)>> ReceiveInventory(List<(int purchaseOrderId, int purchaseOrderItemId)> itemsToReceive);
        Task RemoveItem(int purchaseOrderItemId);
        Task<ServiceMessageResult> SubmitAsync(int id);
        Task<ServiceResult> UpdateAsync(int id, PurchaseOrderDto purchaseOrderDto);
        Task UpdateItem(int purchaseOrderId, PurchaseOrderItemDto purchaseOrderItem);
    }
}
