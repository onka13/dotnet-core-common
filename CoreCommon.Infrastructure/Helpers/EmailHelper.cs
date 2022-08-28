using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreCommon.Data.Domain.Config;
using CoreCommon.Data.Domain.Entitites;
using MailKit.Net.Smtp;
using MimeKit;

namespace CoreCommon.Infrastructure.Helpers
{
    /// <summary>
    /// Email helpers.
    ///
    /// https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0#remarks
    /// .
    /// </summary>
    public class EmailHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// <summary>
        /// Sends an email.
        /// If you want to send email with using gmail, please check the below url;
        /// https://myaccount.google.com/u/0/lesssecureapps.
        /// </summary>
        /// </summary>
        /// <param name="config"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="from"></param>
        /// <param name="attachments"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static async Task SendEmail(SmtpConfig config, string subject, string body, MailAddress from, List<MailAttachment> attachments, List<MailAddress> cc, List<MailAddress> bcc, params MailAddress[] to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from.Name, from.Email));
            foreach (var address in to)
            {
                message.To.Add(new MailboxAddress(address.Name, address.Email));
            }

            if (cc != null)
            {
                foreach (var address in cc)
                {
                    message.Cc.Add(new MailboxAddress(address.Name, address.Email));
                }
            }

            if (bcc != null)
            {
                foreach (var address in bcc)
                {
                    message.Bcc.Add(new MailboxAddress(address.Name, address.Email));
                }
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
            };

            if (attachments != null)
            {
                foreach (var attach in attachments)
                {
                    if (attach.Stream != null)
                    {
                        bodyBuilder.Attachments.Add(attach.FileName, attach.Stream);
                    }
                    else
                    {
                        bodyBuilder.Attachments.Add(attach.FileName, attach.Data);
                    }
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.CheckCertificateRevocation = false;

                client.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                await client.ConnectAsync(config.Server, config.Port, config.UseSsl);
                await client.AuthenticateAsync(config.User, config.Pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
