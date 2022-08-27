using CoreCommon.Infra.Domain.Config;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreCommon.Infra.Helpers
{
    /// <summary>
    /// Email Address Model
    /// </summary>
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

    public class MailAttachment
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public byte[] Data { get; set; }

        public MailAttachment(string fileName, Stream stream)
        {
            FileName = fileName;
            Stream = stream;
        }

        public MailAttachment(string fileName, byte[] byteData)
        {
            FileName = fileName;
            Data = byteData;
        }
    }

    /// <summary>
    /// Email helpers 
    /// </summary>
    public class EmailHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

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
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends an email.
        /// If you want to send email with using gmail, please check the below url;
        /// https://myaccount.google.com/u/0/lesssecureapps
        /// </summary>
        /// <param name="config"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="from"></param>
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
                HtmlBody = body
            };

            if (attachments != null)
            {
                foreach (var attach in attachments)
                {
                    if (attach.Stream != null)
                        bodyBuilder.Attachments.Add(attach.FileName, attach.Stream);
                    else
                        bodyBuilder.Attachments.Add(attach.FileName, attach.Data);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.CheckCertificateRevocation = false;

                client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                await client.ConnectAsync(config.Server, config.Port, config.UseSsl);
                await client.AuthenticateAsync(config.User, config.Pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
