using Northwind.Core.Dtos;

namespace Northwind.Blazor.Models
{
    public class PurchaseOrder
    {
        public SupplierDto? Supplier { get; set; }
        public string ShipTo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Notes { get; set; }
    }
}
