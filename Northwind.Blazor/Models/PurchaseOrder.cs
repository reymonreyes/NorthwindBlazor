namespace Northwind.Blazor.Models
{
    public class PurchaseOrder
    {
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public string ShipTo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Notes { get; set; }
    }
}
