using System;
using System.Collections.Generic;
using CoreCommon.Infrastructure.Helpers;
using StackExchange.Redis;

namespace CoreCommon.Data.RedisCache.Components
{
    /// <summary>
    /// Redis Manager
    /// </summary>
    public class RedisCacheManager
    {
        private string ConnectionString { get; set; }
        private static Dictionary<string, Lazy<ConnectionMultiplexer>> lazyConnections = new Dictionary<string, Lazy<ConnectionMultiplexer>>();

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public RedisCacheManager(string connectionString)
        {
            ConnectionString = connectionString;
            if (lazyConnections.ContainsKey(connectionString))
            {
                lazyConnection = lazyConnections[connectionString];
            }
            else
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(ConnectionString);
                });
            }
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public IDatabase Database
        {
            get
            {
                return lazyConnection.Value.GetDatabase();
            }
        }
        
        public ISubscriber Subscriber
        {
            get
            {
                return lazyConnection.Value.GetSubscriber();
            }
        }

        public string Ping()
        {
            return Database.Execute("PING").ToString();
        }

        public string Get(string key)
        {
            return Database.StringGet(new RedisKey(key));
        }

        public void Set(string key, string value)
        {
            Database.StringSet(new RedisKey(key), new RedisValue(value));
        }

        public T GetObject<T>(string key)
        {
            var strContent = Get(key);
            return ConversionHelper.DerializeObject<T>(strContent);
        }

        public void SetObject<T>(string key, T value)
        {
            var strContent = ConversionHelper.SerializeObject(value);
            Set(key, strContent);
        }

        public void Subscribe(string channelName, Action<ChannelMessage> handler)
        {
            Subscriber.Subscribe(channelName).OnMessage(handler);
        }
        
        public void Publish(string channelName, string content, CommandFlags commandFlags = CommandFlags.None)
        {
            Subscriber.Publish(channelName, content, commandFlags);
        }
        
        public void Subscribe<T>(string channelName, Action<T> handler)
        {
            Subscriber.Subscribe(channelName).OnMessage(channelMessage => {
                var data = ConversionHelper.DerializeObject<T>(channelMessage.Message.ToString());
                handler.Invoke(data);
            });
        }
        
        public void Publish<T>(string channelName, T data, CommandFlags commandFlags = CommandFlags.None)
        {
            var content = ConversionHelper.SerializeObject(data);
            Subscriber.Publish(channelName, content, commandFlags);
        }

        public void SetExpire(string key, TimeSpan expiration)
        {
            Database.KeyExpire(key, expiration, flags: CommandFlags.FireAndForget);
        }
    }
}
