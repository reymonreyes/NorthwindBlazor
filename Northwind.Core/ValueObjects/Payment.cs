using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.ValueObjects
{
    public class Payment
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
    }
}
