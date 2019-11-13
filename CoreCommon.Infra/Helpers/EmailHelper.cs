using CoreCommon.Data.Domain.Config;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace CoreCommon.Infra.Helpers
{
    public class MailAddress
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public MailAddress(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }

    public class EmailHelper
    {
        // https://myaccount.google.com/u/0/lesssecureapps
        public static async Task SendEmail(SmtpConfig config, string subject, string body, MailAddress from, params MailAddress[] to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from.Name, from.Email));
            foreach (var address in to)
            {
                message.To.Add(new MailboxAddress(address.Name, address.Email));
            }
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody(); 
            /*new TextPart("plain")
            {
                Text = body
            };*/

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(config.Server, config.Port, config.UseSsl);
                await client.AuthenticateAsync(config.User, config.Pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
