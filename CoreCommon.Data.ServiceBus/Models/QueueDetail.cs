namespace CoreCommon.Data.ServiceBus.Models
{
    public class QueueDetail
    {
        public string Message { get; set; }

        public long TotalMessageCount { get; set; }

        public long ActiveMessageCount { get; set; }

        public long DeadLetterMessageCount { get; set; }
    }
}
