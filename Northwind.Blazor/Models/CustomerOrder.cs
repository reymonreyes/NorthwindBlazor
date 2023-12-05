namespace Northwind.Blazor.Models
{
    public class CustomerOrder
    {
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipperId { get; set; }
        public string? ShipTo { get; set; }
        public string? Notes { get; set; }
        public int Id { get; set; }
    }
}
