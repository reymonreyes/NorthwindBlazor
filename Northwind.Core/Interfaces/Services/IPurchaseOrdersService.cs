using Northwind.Core.Dtos;
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
        Task<ServiceMessageResult> SubmitAsync(int id);
        Task<ServiceResult> UpdateAsync(int id, PurchaseOrderDto purchaseOrderDto);
    }
}
