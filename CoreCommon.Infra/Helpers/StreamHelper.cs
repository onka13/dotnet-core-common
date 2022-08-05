using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CoreCommon.Infra.Helpers
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

    public class StreamHelper
    {
        public static async Task<byte[]> DownloadContentAsync(string url)
        {
            var client = new System.Net.Http.HttpClient();
            return await client.GetByteArrayAsync(url);
        }

        public static async Task<byte[]> CreateZipFileContent(IList<ZipFileDetail> files)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = zipArchive.CreateEntry(file.Name);

                        using (var entryStream = entry.Open())
                        {
                            if (file.File != null)
                            {
                                await file.File.CopyToAsync(entryStream);
                            }
                            else
                            {
                                await entryStream.WriteAsync(file.Content, 0, file.Content.Length);
                            }
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
