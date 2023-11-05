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
        public List<OrderItemDto> OrderItems { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }        
    }
}
