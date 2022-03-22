using System.ComponentModel.DataAnnotations;

namespace Northwind.Blazor.Models
{
    public class Supplier
    {        
        public int Id { get; set; }
        [Required,MaxLength(40)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(30)]
        public string ContactName { get; set; } = string.Empty;
        [MaxLength(30)]
        public string ContactTitle { get; set; } = string.Empty;
        [MaxLength(60)]
        public string Address { get; set; } = string.Empty;
        [MaxLength(15)]
        public string City { get; set; } = string.Empty;
        [MaxLength(15)]
        public string Region { get; set; } = string.Empty;
        [MaxLength(15)]
        public string Country { get; set; } = string.Empty;
        [MaxLength(15)]
        public string PostalCode { get; set; } = string.Empty;
        [MaxLength(24)]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(24)]
        public string Fax { get; set; } = string.Empty;
        public string Homepage { get; set; } = string.Empty;
        [EmailAddress,MaxLength(254)]
        public string? Email { get; set; }
    }
}
