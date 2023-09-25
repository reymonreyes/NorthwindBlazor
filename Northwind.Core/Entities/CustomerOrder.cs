using Northwind.Core.Enums;
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
        //public int EmployeeId { get; set; } TODO: Implement as identity user
        public DateTime OrderDate { get; set; }
        public string Notes { get; set; }

        public List<CustomerOrderItem> Items { get; set; }

        /* TODO: Implement as value objects
        public DateTime ShippedDate { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperState { get; set; }
        public string ShipperPostalCode { get; set; }
        public string Country { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Taxes { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal TaxRate { get; set; }
        public int TaxStatus { get; set; }
        public int StatusId { get; set; }
        */
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
        public int? PurchaseOrderId { get; set; }
        public int? InventoryTransactionId { get; set; }
    }
}
