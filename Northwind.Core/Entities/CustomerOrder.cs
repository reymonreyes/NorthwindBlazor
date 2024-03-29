﻿using Northwind.Core.Dtos.Document;
using Northwind.Core.Enums;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Entities
{
    public class CustomerOrder
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? ShipperId { get; set; }
        public DateTime? ShipDate { get; set; }
        //public int EmployeeId { get; set; } TODO: Implement as identity user
        public DateTime OrderDate { get; set; }
        public string Notes { get; set; }
        public OrderStatus Status { get; set; }
        public List<CustomerOrderItem> Items { get; set; }
        public ShippingInformation?  ShipTo { get; set; }
        public Payment? Payment { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class CustomerOrderItem
    {
        public int Id { get; set; }
        public int CustomerOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int Discount { get; set; }
        public DateTime? DateAllocated { get; set; }
        //public int? PurchaseOrderId { get; set; } TODO
        //public int? InventoryTransactionId { get; set; } TODO
        public OrderStatus Status { get; set; }
    }
}
