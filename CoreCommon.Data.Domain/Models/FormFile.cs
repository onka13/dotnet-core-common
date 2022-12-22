using System.IO;

namespace CoreCommon.Data.Domain.Models
{
    public enum FileType : byte
    {
        Image = 1,
        Pdf = 2,
        Text = 3,
        Excel = 4,
        Other = 5,
        Json = 6,
        Audio = 7,
        Video = 8,
    }

    public class FormFile
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public Stream Stream { get; set; }

        public string Extension { get; set; }

        public FileType FileType { get; set; }
    }
}
