using System;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class MessageSessionOptions
    {
        public bool AutoCompleteMessages { get; set; }

        public TimeSpan MaxAutoLockRenewalDuration { get; set; }

        public TimeSpan? SessionIdleTimeout { get; set; }

        public int MaxConcurrentSessions { get; set; }

        public int MaxConcurrentCallsPerSession { get; set; }

        public bool ReceiveAndDelete { get; set; }
    }
}
