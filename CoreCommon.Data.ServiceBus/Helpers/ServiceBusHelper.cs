using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CoreCommon.Data.ServiceBus.Models;

namespace CoreCommon.Data.ServiceBus.Helpers
{
    /// <summary>
    /// Helper class for ServiceBus
    ///
    /// AbandonAsync
    /// Everytime you abandon a message, delivery count will be increased by 1. And when it reaches to max delivery count (which is 10 default), it will be sent to dead queue.
    ///
    /// Session-enabled queue
    /// the number of sessions you write to the queue must match the number of MaxConcurrentSessions - if you want to consume them continously.
    /// Otherwise the MessageWaitTimeout is the time it will run idle, after the timeout it will find a new session id to consume.
    ///
    ///
    /// Important
    /// When partitioning is enabled, MessageId+PartitionKey is used to determine uniqueness.
    /// When partitioning is disabled (default), only MessageId is used to determine uniqueness.
    ///
    /// When sessions are enabled, partition key and session ID must be the same.
    /// A batch must contain messages for only one partition.
    ///
    /// </summary>
    public class ServiceBusHelper
    {
        private ServiceBusClient client;
        private ServiceBusSender sender;
        private string queueName;
        private string connectionString;

        public ServiceBusHelper(string connectionString)
        {
            Init(connectionString, null);
        }

        public ServiceBusHelper(string connectionString, string queueName)
        {
            Init(connectionString, queueName);
        }

        public Action<Models.ServiceBusException> OnException { get; set; }

        public ServiceBusHelper Init(string queueName)
        {
            return Init(null, queueName);
        }

        public ServiceBusHelper Init(string connectionString, string queueName)
        {
            if (connectionString != null)
            {
                this.connectionString = connectionString;
                client = new ServiceBusClient(connectionString);
            }

            if (queueName != null)
            {
                this.queueName = queueName;
                if (client == null)
                {
                    throw new Exception("client is empty");
                }

                sender = client.CreateSender(queueName);
            }

            return this;
        }

        /// <summary>
        /// Get client.
        /// </summary>
        /// <returns></returns>
        public ServiceBusClient GetQueueClient()
        {
            return client;
        }

        public async Task<Dictionary<string, QueueDetail>> GetAllQueueDetails()
        {
            var dict = new Dictionary<string, QueueDetail>();
            var administrationClient = new ServiceBusAdministrationClient(connectionString);
            var queuesRuntimeProperties = administrationClient.GetQueuesRuntimePropertiesAsync().AsPages();
            await foreach (var queuePage in queuesRuntimeProperties)
            {
                foreach (QueueRuntimeProperties currentQueue in queuePage.Values)
                {
                    dict.Add(
                        currentQueue.Name,
                        new QueueDetail
                        {
                            ActiveMessageCount = currentQueue.ActiveMessageCount,
                            TotalMessageCount = currentQueue.TotalMessageCount,
                            DeadLetterMessageCount = currentQueue.DeadLetterMessageCount,
                        });
                }
            }

            return dict;
        }

        public async Task<Dictionary<string, QueueDetail>> GetQueueDetails(params string[] names)
        {
            if (names == null || names.Length == 0)
            {
                throw new ArgumentNullException("names");
            }

            var dict = new Dictionary<string, QueueDetail>();
            var administrationClient = new ServiceBusAdministrationClient(connectionString);
            foreach (var name in names)
            {
                try
                {
                    var runtimeProperty = await administrationClient.GetQueueRuntimePropertiesAsync(name);
                    dict.Add(name, new QueueDetail()
                    {
                        ActiveMessageCount = runtimeProperty.Value.ActiveMessageCount,
                        TotalMessageCount = runtimeProperty.Value.TotalMessageCount,
                        DeadLetterMessageCount = runtimeProperty.Value.DeadLetterMessageCount,
                    });
                }
                catch (Exception ex)
                {
                    dict.Add(name, new QueueDetail
                    {
                        Message = ex.Message,
                    });
                }
            }

            return dict;
        }

        public async Task<QueueDetail> GetQueueDetail(string name)
        {
            var administrationClient = new ServiceBusAdministrationClient(connectionString);
            var runtimeProperty = await administrationClient.GetQueueRuntimePropertiesAsync(name);
            return new QueueDetail()
            {
                ActiveMessageCount = runtimeProperty.Value.ActiveMessageCount,
                TotalMessageCount = runtimeProperty.Value.TotalMessageCount,
                DeadLetterMessageCount = runtimeProperty.Value.DeadLetterMessageCount,
            };
        }

        /// <summary>
        /// Send message to queue.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="options">Options.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Send(string message, SendMessageOptions options = null)
        {
            await Send(BinaryData.FromString(message), options);
        }

        public async Task Send(Dictionary<string, SendMessageOptions> items)
        {
            await Send(items.ToDictionary(x => BinaryData.FromString(x.Key), x => x.Value));
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task Send<T>(T body, SendMessageOptions options = null)
        {
            await Send(BinaryData.FromObjectAsJson(body), options);
        }

        public async Task Send<T>(Dictionary<T, SendMessageOptions> items)
        {
            await Send(items.ToDictionary(x => BinaryData.FromObjectAsJson(x.Key), x => x.Value));
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <returns></returns>
        public virtual async Task Send(BinaryData binaryData, SendMessageOptions options = null)
        {
            await sender.SendMessageAsync(GetServiceBusMessage(binaryData, options));
        }

        public async Task Send(Dictionary<BinaryData, SendMessageOptions> items)
        {
            // await sender.SendMessagesAsync(items.Select(x => GetServiceBusMessage(x.Key, x.Value)));
            var bySessionId = items.GroupBy(s => s.Value?.SessionId);
            foreach (var sessionSet in bySessionId)
            {
                await sender.SendMessagesAsync(sessionSet.Select(x => GetServiceBusMessage(x.Key, x.Value)));
            }
        }

        public async Task Receive(Func<MessageArgs, CancellationToken, Task<bool>> received, MessageOptions options = null, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                options = new MessageOptions
                {
                    MaxConcurrentCalls = 1,
                    SessionIdleTimeout = TimeSpan.FromSeconds(5),
                    MaxAutoLockRenewalDuration = TimeSpan.FromHours(24),
                    AutoCompleteMessages = true,
                    IsPeekLock = false,
                };
            }

            if (client.IsClosed)
            {
                Init(connectionString, queueName);
            }

            var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = options.AutoCompleteMessages,
                MaxAutoLockRenewalDuration = options.MaxAutoLockRenewalDuration,
                MaxConcurrentCalls = options.MaxConcurrentCalls,
                ReceiveMode = options.IsPeekLock ? ServiceBusReceiveMode.PeekLock : ServiceBusReceiveMode.ReceiveAndDelete,
            });
            processor.ProcessMessageAsync += async arg =>
            {
                var result = await received(
                    new MessageArgs
                    {
                        Data = arg.Message.Body,
                        MessageId = arg.Message.MessageId,
                    },
                    arg.CancellationToken);
            };
            processor.ProcessErrorAsync += async arg =>
            {
                await ExceptionReceivedHandler(arg);
            };
            await processor.StartProcessingAsync(cancellationToken);

            cancellationToken.Register(async () =>
            {
                await processor.StopProcessingAsync();
            });
        }

        public async Task ReceiveWithSession(Func<MessageArgs, CancellationToken, Task<bool>> received, MessageSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                options = new MessageSessionOptions
                {
                    MaxConcurrentCallsPerSession = 1,
                    MaxConcurrentSessions = 1,
                    SessionIdleTimeout = TimeSpan.FromSeconds(2),
                    MaxAutoLockRenewalDuration = TimeSpan.FromHours(24),
                    AutoCompleteMessages = false,
                    ReceiveAndDelete = false,
                };
            }

            if (client.IsClosed)
            {
                Init(connectionString, queueName);
            }

            var processor = client.CreateSessionProcessor(queueName, new ServiceBusSessionProcessorOptions
            {
                AutoCompleteMessages = options.AutoCompleteMessages,
                MaxAutoLockRenewalDuration = options.MaxAutoLockRenewalDuration,
                MaxConcurrentCallsPerSession = options.MaxConcurrentCallsPerSession,
                MaxConcurrentSessions = options.MaxConcurrentSessions,
                ReceiveMode = options.ReceiveAndDelete ? ServiceBusReceiveMode.ReceiveAndDelete : ServiceBusReceiveMode.PeekLock,
                SessionIdleTimeout = options.SessionIdleTimeout,
            });
            processor.ProcessMessageAsync += async arg =>
            {
                var result = await received(
                    new MessageArgs
                    {
                        Data = arg.Message.Body,
                        MessageId = arg.Message.MessageId,
                    },
                    arg.CancellationToken);

                if (!options.AutoCompleteMessages)
                {
                    if (result)
                    {
                        await arg.CompleteMessageAsync(arg.Message, arg.CancellationToken);
                    }
                    else
                    {
                        await arg.AbandonMessageAsync(arg.Message, new Dictionary<string, object>
                        {
                            { "DeliveryCount", arg.Message.DeliveryCount + 1 },
                        });
                    }
                }
            };
            processor.ProcessErrorAsync += async arg =>
            {
                await ExceptionReceivedHandler(arg);
            };
            await processor.StartProcessingAsync(cancellationToken);

            cancellationToken.Register(async () =>
            {
                await processor.StopProcessingAsync();
            });
        }

        private ServiceBusMessage GetServiceBusMessage(BinaryData binaryData, SendMessageOptions options)
        {
            var message = new ServiceBusMessage(binaryData)
            {
                SessionId = options?.SessionId,
                PartitionKey = options?.PartitionKey,
            };
            if (!string.IsNullOrWhiteSpace(options?.MessageId))
            {
                message.MessageId = options.MessageId;
            }

            return message;
        }

        // Use this handler to examine the exceptions received on the message pump.
        private Task ExceptionReceivedHandler(ProcessErrorEventArgs e)
        {
            OnException?.Invoke(new Models.ServiceBusException
            {
                Exception = e.Exception,

                // Action = e.ExceptionReceivedContext.Action,
                // ClientId = e.ExceptionReceivedContext.ClientId,
                // Endpoint = e.FullyQualifiedNamespace,
                EntityPath = e.EntityPath,
            });
            return Task.CompletedTask;
        }
    }
}
