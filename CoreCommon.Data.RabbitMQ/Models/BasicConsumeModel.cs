using System.Collections.Generic;

namespace CoreCommon.Data.RabbitMQ.Models
{
    /// <summary>
    /// Basic Consume Model
    /// </summary>
    public class BasicConsumeModel
    {
        public string Queue { get; set; }
        public bool AutoAck { get; set; }
        public string ConsumerTag { get; set; }
        public bool NoLocal { get; set; }
        public bool Exclusive { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        public BasicConsumeModel()
        {
            Arguments = new Dictionary<string, object>();
            ConsumerTag = "";
        }
    }
}
