using Northwind.Core.Dtos;
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
        Task<ServiceMessageResult> ApproveAsync(int id);
        Task<ServiceMessageResult> CancelAsync(int id);
        Task<ServiceResult> Create(PurchaseOrderDto purchaseOrder);
        Task EmailPdfToSupplier(int purchaseOrderId);
        Task<string> GeneratePdfDocument(int id);
        Task PaySupplierAsync(int v, Payment payment);
        Task<List<(int purchaseOrderId, int purchaseOrderItemId, string result)>> ReceiveInventory(List<(int purchaseOrderId, int purchaseOrderItemId)> itemsToReceive);
        Task<ServiceMessageResult> SubmitAsync(int id);
        Task<ServiceResult> UpdateAsync(int id, PurchaseOrderDto purchaseOrderDto);
    }
}
