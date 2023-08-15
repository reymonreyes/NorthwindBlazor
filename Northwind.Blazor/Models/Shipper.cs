using System.ComponentModel.DataAnnotations;

namespace Northwind.Blazor.Models
{
    public class Shipper
    {
        public int Id { get; set; }
        [Required, MaxLength(40)]
        public string? Name { get; set; }
        [MaxLength(24)]
        public string? Phone { get; set; }
        [Required]
        public string ContactName { get; set; }
    }
}
