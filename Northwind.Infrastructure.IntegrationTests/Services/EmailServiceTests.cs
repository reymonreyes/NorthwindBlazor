using Northwind.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.IntegrationTests.Services
{
    public class EmailServiceTests
    {
        [Theory]
        [InlineData("", "destination@mail.com", "Subject", "Body")]
        [InlineData("source@mail.com", "", "Subject", "Body")]
        [InlineData("source@mail.com", "destination@mail.com", "", "Body")]
        [InlineData("source@mail.com", "destination@mail.com", "Subject", "")]
        public void Send_ShouldThrowExceptionOnRequiredParameters(string sourceEmail, string destinationEmail, string subject, string body)
        {
            var emailService = new SmtpEmailService();
            
            Assert.Throws<ArgumentNullException>(() => emailService.Send(sourceEmail, destinationEmail, subject, body));
        }
    }
}
