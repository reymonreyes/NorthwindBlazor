using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class CustomerOrderSummaryDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
    }
}
