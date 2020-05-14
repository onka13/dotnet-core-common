using CoreCommon.Data.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreCommon.Data.RabbitMQ.Helpers
{
    /// <summary>
    /// Helper class for RabbitMQ
    /// </summary>
    public class RabbitMQHelper
    {
        readonly ConnectionFactory factory;
        /// <summary>
        /// Connection list already opened
        /// </summary>
        readonly Dictionary<string, IConnection> connections;

        public RabbitMQHelper(string uri)
        {
            factory = new ConnectionFactory
            {
                Uri = new Uri(uri)
            };
            connections = new Dictionary<string, IConnection>();
        }

        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns></returns>
        public ConnectionFactory GetFactory()
        {
            return factory;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="key">connection key</param>
        /// <param name="reasonCode">Reason Code</param>
        /// <param name="reasonText">Reason Text</param>
        public void CloseConnection(string key, ushort reasonCode = 0, string reasonText = "")
        {
            if (!connections.ContainsKey(key)) return;
            connections[key].Close(reasonCode, reasonText);
            connections.Remove(key);
            Console.WriteLine("RabbitMQ: Connection closed.");
        }

        /// <summary>
        /// Declare Queueu
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="model"></param>
        public void QueueDeclare(IModel channel, QueueDeclareModel model)
        {
            if (model == null) return;
            channel.QueueDeclare(queue: model.Queue, durable: model.Durable, exclusive: model.Exclusive, autoDelete: model.AutoDelete, arguments: model.Arguments);
        }

        /// <summary>
        /// Basic Qos
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="model"></param>
        public void BasicQos(IModel channel, BasicQosModel model)
        {
            if (model == null) return;
            channel.BasicQos(prefetchSize: model.PrefetchSize, prefetchCount: model.PrefetchCount, global: model.Global);
        }

        /// <summary>
        /// Create Channel
        /// </summary>
        /// <param name="queue">queue name</param>
        /// <param name="body"></param>
        /// <param name="queueDeclare"></param>
        public void CreateChannel(string queue, byte[] body, QueueDeclareModel queueDeclare = null)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                QueueDeclare(channel, queueDeclare);
                channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: null, body: body);
            }
        }

        /// <summary>
        /// Send message to queue
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="queueDeclare"></param>
        public void Send(string queue, string message, QueueDeclareModel queueDeclare = null)
        {
            CreateChannel(queue, Encoding.UTF8.GetBytes(message), queueDeclare);
        }
        
        public void Send(string queue, byte[] body, QueueDeclareModel queueDeclare = null)
        {
            CreateChannel(queue, body, queueDeclare);
        }

        /// <summary>
        /// Basic Consume
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="received"></param>
        /// <param name="consume"></param>
        /// <param name="qos"></param>
        /// <param name="queueDeclare"></param>
        /// <returns></returns>
        public string BasicConsume(string queue, Func<BasicDeliverEventArgsModel, bool> received, BasicConsumeModel consume = null, BasicQosModel qos = null, QueueDeclareModel queueDeclare = null)
        {
            string connectionKey = Guid.NewGuid().ToString();

            var connection = factory.CreateConnection();            
            var channel = connection.CreateModel();

            connections.Add(connectionKey, connection);

            QueueDeclare(channel, queueDeclare);
            BasicQos(channel, qos);

            Console.WriteLine("RabbitMQ: Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, e) =>
            {
                var res = received(new BasicDeliverEventArgsModel
                {
                    Body = e.Body,
                    ConsumerTag = e.ConsumerTag,
                    DeliveryTag = e.DeliveryTag,
                    Exchange = e.Exchange,
                    Redelivered = e.Redelivered,
                    RoutingKey = e.RoutingKey,
                });
                if (!res) return;
                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: qos?.PrefetchCount > 1);
            };
            if (consume == null) consume = new BasicConsumeModel();
            channel.BasicConsume(queue, consume.AutoAck, consume.ConsumerTag, consume.NoLocal, consume.Exclusive, consume.Arguments, consumer);

            return connectionKey;
        }

        /// <summary>
        /// Dispose connections
        /// </summary>
        public void Dispose()
        {
            foreach (var connection in connections)
            {
                CloseConnection(connection.Key);
            }
        }
    }
}
