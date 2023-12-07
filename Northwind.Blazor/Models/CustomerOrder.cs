using Northwind.Core.Dtos;

namespace Northwind.Blazor.Models
{
    public class CustomerOrder
    {
        public CustomerDto? Customer { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public ShipperDto? Shipper { get; set; }
        public int? ShipperId { get; set; }
        public string? ShipTo { get; set; }
        public string? Notes { get; set; }
        public int Id { get; set; }
    }
}
