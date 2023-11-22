using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Enums
{
    public enum OrderStatus
    {
        New = 10,
        Invoiced = 20,
        Shipped = 30,
        Paid = 40,
        Submitted = 50,
        Approved = 60,
        Completed = 70,
        Cancelled = 80
    }
}
