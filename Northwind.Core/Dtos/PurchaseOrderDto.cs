using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class PurchaseOrderDto
    {
        public int SupplierId { get; set; }
        public string Notes { get; set; }
        public string ShipTo { get; set; }
        public List<PurchaseOrderItemDto> OrderItems { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public int Id { get; set; }
    }
}
