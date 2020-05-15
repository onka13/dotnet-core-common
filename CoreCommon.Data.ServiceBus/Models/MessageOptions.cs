using System;
using System.Collections.Generic;
using System.Text;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class MessageOptions
    {
        public int MaxConcurrentCalls { get; set; }
        public bool AutoComplete { get; set; }
        public TimeSpan MaxAutoRenewDuration { get; set; }
    }
}
