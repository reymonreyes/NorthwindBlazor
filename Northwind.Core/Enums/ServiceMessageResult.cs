using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Enums
{
    public class ServiceMessageResult
    {
        public ServiceMessageType MessageType { get; set; }
        public KeyValuePair<string, string> Message { get; set; }
    }
}
