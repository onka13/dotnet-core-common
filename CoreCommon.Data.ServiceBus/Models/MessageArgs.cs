using CoreCommon.Infra.Helpers;
using System.Text;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class MessageArgs
    {
        public string MessageId { get; set; }
        public byte[] Body { get; set; }

        /// <summary>
        /// Get data as string
        /// </summary>
        /// <returns></returns>
        public string GetDataAsString()
        {
            return Encoding.UTF8.GetString(Body);
        }

        /// <summary>
        /// Convert data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDataAs<T>()
        {
            return ConversionHelper.ByteArrayTo<T>(Body);
        }
    }
}
