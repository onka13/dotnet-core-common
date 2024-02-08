using System;
using System.Collections.Generic;
using System.Text;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class MessageOptions
    {
        public bool AutoCompleteMessages { get; set; }

        public TimeSpan MaxAutoLockRenewalDuration { get; set; }

        public TimeSpan? SessionIdleTimeout { get; set; }

        public int MaxConcurrent { get; set; }

        public int MaxConcurrentCalls { get; set; }

        public bool IsPeekLock { get; set; }
    }
}
