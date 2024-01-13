using Microsoft.AspNetCore.Components;
using Northwind.Core.Dtos;
using Northwind.Core.Enums;

namespace Northwind.Blazor.Models
{
    public class PurchaseOrder
    {
        public SupplierDto? Supplier { get; set; }
        public string ShipTo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Notes { get; set; }
        public OrderStatus Status { get; set; }
        public List<PurchaseOrderItem> Items { get; set; }
        public int Id { get; set; }
        public decimal Total { get; set; }
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }
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
        public bool IsInEditMode { get; set; } = false;
        public RenderFragment? QuantityInput { get; set; }
        public RenderFragment? UnitPriceInput { get; set; }
        public RenderFragment? EditItem { get; set; }
    }
}
