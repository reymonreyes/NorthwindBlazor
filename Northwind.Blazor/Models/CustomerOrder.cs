using Northwind.Core.Dtos;

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
        public List<CustomerOrderItem> Items { get; set; }
    }

    public class CustomerOrderItem
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
