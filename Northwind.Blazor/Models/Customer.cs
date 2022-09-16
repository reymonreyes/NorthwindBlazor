using System.ComponentModel.DataAnnotations;

namespace Northwind.Blazor.Models
{
    public class Customer
    {
        public string? Id { get; set; }
        [Required, MaxLength(40)]
        public string? Name { get; set; }
        [MaxLength(30)]
        public string? ContactName { get; set; }
        [MaxLength(30)]
        public string? ContactTitle { get; set; }
        [MaxLength(60)]
        public string? Address { get; set; }
        [MaxLength(15)]
        public string? City { get; set; }
        [MaxLength(15)]
        public string? Region { get; set; }
        [MaxLength(15)]
        public string? Country { get; set; }
        [MaxLength(10)]
        public string? PostalCode { get; set; }
        [MaxLength(24)]
        public string? Phone { get; set; }
        [MaxLength(24)]
        public string? Fax { get; set; }        
    }
}
