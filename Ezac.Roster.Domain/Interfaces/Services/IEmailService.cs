using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        public Task SendEmail(MimeMessage email, SmtpClient smtpClient);
        public Task SendEmails(IEnumerable<EmailToSend> emailsToSend);
        public string MessageWithUniqueLink(string uniqueLink);
    }
}
