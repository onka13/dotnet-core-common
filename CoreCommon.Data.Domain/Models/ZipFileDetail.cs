using System.IO;

namespace CoreCommon.Data.Domain.Models
{
    public class ZipFileDetail
    {
        public Stream File { get; set; }
        public byte[] Content { get; set; }
        public string Name { get; set; }

        public ZipFileDetail(string name, Stream file)
        {
            File = file;
            Name = name;
        }

        public ZipFileDetail(string name, byte[] content)
        {
            Content = content;
            Name = name;
        }
    }
}
