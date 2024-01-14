using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class CustomerOrderItemDto
    {
        public int Id { get; set; }
        public int CustomerOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Discount { get; set; }
        public int Status { get; set; }
        public DateTime DateAllocated { get; set; }
        public int PurchaseOrderId { get; set; }
        public int InventoryTransactionId { get; set; }
    }
}
