using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        void Send(string from, string to, string subject, string body, List<string> attachments = null);
    }
}
