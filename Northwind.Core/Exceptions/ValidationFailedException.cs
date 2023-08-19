using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Exceptions
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException():base()
        {
        }

        public ValidationFailedException(string message):base(message)
        {
        }

        public ValidationFailedException(string message, Exception innerException) : base(message, innerException)
        {            
        }
        public ValidationFailedException(IList<ServiceMessageResult> validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public IList<ServiceMessageResult> ValidationErrors { get; private set; } = new List<ServiceMessageResult>();
    }
}
