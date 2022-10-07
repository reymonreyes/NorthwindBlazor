using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int TargetLevel { get; set; }
        public string? QuantityPerUnit { get; set; }
        public bool Discontinued { get; set; }
        public int MinimumReorderQuantity { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
    }
}
