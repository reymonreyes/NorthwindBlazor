using Northwind.Core.Enums;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public OrderStatus Status { get; set; }
        public Payment? Payment { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string ShipTo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Notes { get; set; }
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public bool PostedToInventory { get; set; }
    }
}
