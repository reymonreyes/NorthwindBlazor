using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos.Document
{
    public class PurchaseOrderDto
    {
        public BasicCompanyInformation Company { get; set; }
        public Supplier Supplier { get; set; }
        public ShipTo ShipTo { get; set; }
        public string PONumber { get; set; }
        public DateTime Date { get; set; }
        public List<LineItem> LineItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Notes { get; set; }
    }

    public class LineItem
    {
        public string ItemDescription { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class BasicCompanyInformation
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    public class Supplier : BasicCompanyInformation
    {
    }

    public class ShipTo : BasicCompanyInformation
    {
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
    }
}
