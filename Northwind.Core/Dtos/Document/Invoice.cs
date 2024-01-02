using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos.Document
{
    public class Invoice
    {
        public BasicCompanyInformation Company { get; set; }
        public BasicCompanyInformation ShipTo { get; set; }
        public BasicCompanyInformation BillTo { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public List<LineItem> Items { get; set; }
        public string Notes { get; set; }
    }
}
