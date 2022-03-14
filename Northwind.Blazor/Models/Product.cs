using System.ComponentModel.DataAnnotations;

namespace Northwind.Blazor.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(40)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(16)]
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
