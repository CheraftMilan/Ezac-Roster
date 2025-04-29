using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;
using Microsoft.Extensions.Logging;
using Ezac.Roster.Domain.Interfaces.Services;
namespace Ezac.Roster.Domain.Services
{
    public class EmailService(ILogger<EmailService> logger) : IEmailService
    {
        public async Task SendEmail(MimeMessage email, SmtpClient smtpClient)
        {
			try
			{
				await smtpClient.SendAsync(email);
			}
			catch (Exception ex)
			{
                logger.LogError(ex, $"Het versturen van de email naar {email.To} ging verkeerd!");
			}
        }
        public async Task SendEmails(IEnumerable<EmailToSend> emailsToSend)
        {
            string gmail = "roosteraar.ezac.nl@gmail.com";
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(gmail, "gkqs ohyd vebq liic");

            foreach (var emailToSend in emailsToSend)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(gmail));
                email.To.Add(MailboxAddress.Parse(emailToSend.To));
                email.Subject = emailToSend.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = emailToSend.HtmlText };
                await SendEmail(email, smtp);
            }
            await smtp.DisconnectAsync(true);
        }
        public string MessageWithUniqueLink(string uniqueLink) =>
            $"""Hier heb je de link voor jouw gekozen kalender: <a href="{uniqueLink}">Unieke Link</a>""";
    }
}
public record EmailToSend(string To, string Subject, string HtmlText);
