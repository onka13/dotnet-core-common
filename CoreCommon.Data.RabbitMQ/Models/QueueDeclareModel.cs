using System.Collections.Generic;

namespace CoreCommon.Data.RabbitMQ.Models
{
    /// <summary>
    /// Queue Declare Model
    /// </summary>
    public class QueueDeclareModel
    {
        public string Queue { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
