using MailKit.Net.Smtp;
using MimeKit;
using Northwind.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {        
        public void Send(string from, string to, string subject, string body, List<string> attachments = null)
        {
            Func<string, string, string, string, (bool passed, string parameter)> validateFunc = (from, to, subject, body) => 
            {
                if (string.IsNullOrWhiteSpace(from))
                    return (false, nameof(from));
                if (string.IsNullOrWhiteSpace(to))
                    return (false, nameof(to));
                if (string.IsNullOrWhiteSpace(subject))
                    return (false, nameof(subject));
                if (string.IsNullOrWhiteSpace(body))
                    return (false, nameof(body));

                return (true, string.Empty);
            };            

            var validateResult = validateFunc(from, to, subject, body);
            if(!validateResult.passed)
                throw new ArgumentNullException(nameof(validateResult.parameter));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.TextBody = body;

            if(attachments != null && attachments.Count > 0)
                attachments.ForEach(x => builder.Attachments.Add(x));

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                //these settings must from external source
                client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("some@mail.com", "123456789");

                client.Send(message);
                
                client.Disconnect(true);
            }
        }        
    }
}
