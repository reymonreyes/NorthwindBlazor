using Northwind.Core.Enums;
using Northwind.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public OrderStatus Status { get; set; }
        public Payment Payment { get; set; }
    }    
}
