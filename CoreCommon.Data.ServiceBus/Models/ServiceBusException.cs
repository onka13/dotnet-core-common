using System;

namespace CoreCommon.Data.ServiceBus.Models
{
    public class ServiceBusException
    {
        public Exception Exception { get; set; }
        public string Action { get; set; }
        public string Endpoint { get; set; }
        public string EntityPath { get; set; }
        public string ClientId { get; set; }
    }
}
