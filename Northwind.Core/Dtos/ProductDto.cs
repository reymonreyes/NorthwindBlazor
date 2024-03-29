﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal ListPrice { get; set; }
        public string? QuantityPerUnit { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsInOrder { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public decimal StandardCost { get; set; }
        public int TargetLevel { get; set; }
        public int MinimumReorderQuantity { get; set; }
        public int CategoryId { get; set; }
    }
}
