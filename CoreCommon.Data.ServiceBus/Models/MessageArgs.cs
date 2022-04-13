using System;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class MessageArgs
    {
        public string MessageId { get; set; }

        public BinaryData Data { get; set; }

        /// <summary>
        /// Get data as string.
        /// </summary>
        /// <returns></returns>
        public string GetDataAsString()
        {
            return Data.ToString();
        }

        /// <summary>
        /// Convert data.
        /// </summary>
        /// <returns></returns>
        public T GetDataAs<T>()
        {
            return Data.ToObjectFromJson<T>();
        }
    }
}
