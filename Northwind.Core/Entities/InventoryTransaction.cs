using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Entities
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public  InventoryTransactionType TransactionType { get; set; }
        public DateTime Created { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? CustomerOrderId { get; set; }
        public string Comments { get; set; }
    }
}
