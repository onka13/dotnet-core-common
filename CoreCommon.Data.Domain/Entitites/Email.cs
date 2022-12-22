using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CoreCommon.Data.Domain.Entitites
{
    /// <summary>
    /// Email Address Model.
    /// </summary>
    public class MailAddress
    {
        public MailAddress(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }

        [Key]
        public string Email { get; set; }
    }

    public class MailAttachment
    {
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

        public string FileName { get; set; }

        public Stream Stream { get; set; }

        public byte[] Data { get; set; }
    }
}
