using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface IPurchaseOrderItemsRepository
    {
        Task Create(PurchaseOrderItem purchaseOrderItem);
    }
}
