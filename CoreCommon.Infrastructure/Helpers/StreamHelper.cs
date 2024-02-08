using CoreCommon.Data.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace CoreCommon.Infrastructure.Helpers
{
    public class StreamHelper
    {
        public static async Task<string> ToStringAsync(Stream stream)
        {
            if (stream?.CanRead != true)
            {
                return null;
            }

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public static string ToString(Stream stream)
        {
            if (!stream.CanRead)
            {
                return null;
            }

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static Stream ToStream(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

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
