using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class SendMessageOptions
    {
        public string MessageId { get; set; }

        public string SessionId { get; set; }

        public string PartitionKey { get; set; }
    }
}
