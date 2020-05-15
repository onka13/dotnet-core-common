using CoreCommon.Data.ServiceBus.Models;
using CoreCommon.Infra.Helpers;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreCommon.Data.RabbitMQ.Helpers
{
    /// <summary>
    /// Helper class for RabbitMQ
    /// </summary>
    public class ServiceBusHelper
    {
        readonly IQueueClient client;
        public Action<ServiceBus.Models.ServiceBusException> OnException { get; set; }

        public ServiceBusHelper(string connectionString, string entityPath)
        {
            client = new QueueClient(connectionString, entityPath);
        }

        /// <summary>
        /// Get client
        /// </summary>
        /// <returns></returns>
        public IQueueClient GetQueueClient()
        {
            return client;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public async Task CloseConnection()
        {
            await client.CloseAsync();
        }


        /// <summary>
        /// Send message to queue
        /// </summary>
        /// <param name="message"></param>
        public async Task Send(string message)
        {
            await Send(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task Send(object body)
        {
            await Send(ConversionHelper.ObjectToByteArray(body));
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task Send(byte[] body)
        {
            await client.SendAsync(new Message(body));
        }

        /// <summary>
        /// Receive messages
        /// </summary>
        /// <param name="received"></param>
        /// <param name="options"></param>
        public void Receive(Func<MessageArgs, CancellationToken, Task<bool>> received, MessageOptions options = null)
        {
            if (options == null) options = new MessageOptions { MaxConcurrentCalls = 1 };
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = options.MaxConcurrentCalls,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = options.AutoComplete,
                MaxAutoRenewDuration = options.MaxAutoRenewDuration
            };

            // Register the function that processes messages.
            client.RegisterMessageHandler(async (Message message, CancellationToken token) =>
            {
                var result = await received(new MessageArgs
                {
                    Body = message.Body,
                    MessageId = message.MessageId
                }, token);
                if (result)
                {
                    await client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, messageHandlerOptions);
        }

        // Use this handler to examine the exceptions received on the message pump.
        Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            OnException?.Invoke(new ServiceBus.Models.ServiceBusException
            {
                Exception = e.Exception,
                Action = e.ExceptionReceivedContext.Action,
                ClientId = e.ExceptionReceivedContext.ClientId,
                Endpoint = e.ExceptionReceivedContext.Endpoint,
                EntityPath = e.ExceptionReceivedContext.EntityPath
            });
            return Task.CompletedTask;
        }
    }
}
