using Microsoft.AspNetCore.Components;
using Northwind.Core.Dtos;
using Northwind.Core.Enums;

namespace Northwind.Blazor.Models
{
    public class CustomerOrder
    {
        public CustomerOrder()
        {
            Items = new List<CustomerOrderItem>();
        }
        public int Id { get; set; }
        public CustomerDto? Customer { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public ShipperDto? Shipper { get; set; }
        public int? ShipperId { get; set; }
        public string? ShipTo { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
        public List<CustomerOrderItem> Items { get; set; }
    }

    public class CustomerOrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsInEditMode { get; set; } = false;
        public RenderFragment? QuantityInput { get; set; }
        public RenderFragment? UnitPriceInput { get; set; }
        public RenderFragment? EditItem { get; set; }
        public decimal Total
        {
            get
            {
                return Qty * UnitPrice;
            }
        }
    }
}
