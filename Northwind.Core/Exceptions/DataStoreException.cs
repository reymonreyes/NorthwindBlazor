using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Exceptions
{
    public class DataStoreException :Exception
    {
        public DataStoreException():base()
        {
        }

        public DataStoreException(string message, Exception innerException):base(message, innerException)
        {
        }
    }
}
