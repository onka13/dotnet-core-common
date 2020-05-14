namespace CoreCommon.Data.RabbitMQ.Models
{
    /// <summary>
    /// Basic Qos Model
    /// </summary>
    public class BasicQosModel
    {
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; }
        public bool Global { get; set; }
    }
}
