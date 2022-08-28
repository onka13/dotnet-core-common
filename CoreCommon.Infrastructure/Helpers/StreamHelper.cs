using System.IO;
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
    }
}
