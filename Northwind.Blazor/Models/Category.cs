using System.ComponentModel.DataAnnotations;

namespace Northwind.Blazor.Models
{
    public class Category
    {
        public int Id { get; set; }
        //[Required,MaxLength(15)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
    }
}
