using Northwind.Core.Dtos;

namespace Northwind.Blazor.Models
{
    public class PurchaseOrder
    {
        public SupplierDto? Supplier { get; set; }
        public string ShipTo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Notes { get; set; }
        public List<PurchaseOrderItem> Items { get; set; }
    }

    public class PurchaseOrderItem
    {
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }
    }
}
