using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace Testich.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "testichwork@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 587, false);
                await client.AuthenticateAsync("testichwork@mail.ru", "iSt7gCnh7.QqPhJ");
                await client.SendAsync(emailMessage);
                
                await client.DisconnectAsync(true);
            }
        }
    }
}
