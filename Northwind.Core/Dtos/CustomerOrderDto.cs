﻿using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class CustomerOrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipperId { get; set; }
        public string ShipperName { get; set; }
        public string Notes { get; set; }
        public DateTime? ShipDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<CustomerOrderItemDto> Items { get; set; }
    }
}
