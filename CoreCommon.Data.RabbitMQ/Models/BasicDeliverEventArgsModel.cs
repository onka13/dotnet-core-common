using CoreCommon.Infra.Helpers;
using System;
using System.Text;

namespace CoreCommon.Data.RabbitMQ.Models
{
    /// <summary>
    /// Basic Deliver Event Args Model
    /// </summary>
    public class BasicDeliverEventArgsModel
    {
        //
        // Summary:
        //     The message body.
        public ReadOnlyMemory<byte> Body { get; set; }
        //
        // Summary:
        //     The consumer tag of the consumer that the message was delivered to.
        public string ConsumerTag { get; set; }
        //
        // Summary:
        //     The delivery tag for this delivery. See IModel.BasicAck.
        public ulong DeliveryTag { get; set; }
        //
        // Summary:
        //     The exchange the message was originally published to.
        public string Exchange { get; set; }
        //
        // Summary:
        //     The AMQP "redelivered" flag.
        public bool Redelivered { get; set; }
        //
        // Summary:
        //     The routing key used when the message was originally published.
        public string RoutingKey { get; set; }

        /// <summary>
        /// Get data as string
        /// </summary>
        /// <returns></returns>
        public string GetDataAsString()
        {
            return Encoding.UTF8.GetString(Body.ToArray());
        }
        
        /// <summary>
        /// Convert data to <
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDataAs<T>()
        {
            return ConversionHelper.ByteArrayTo<T>(Body.ToArray());
        }
    }
}
