using Northwind.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Validators
{
    public interface IValidator<T> where T : class
    {
        List<ServiceMessageResult> Validate(T value);
        //List<KeyValuePair<string, string>> Validate(T entity);
    }
}
