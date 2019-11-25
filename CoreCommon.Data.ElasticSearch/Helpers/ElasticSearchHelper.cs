using Nest;
using System;
using Microsoft.Extensions.Configuration;
using Autofac;

namespace CoreCommon.Data.ElasticSearch.Helpers
{
    /// <summary>
    /// Elastic search helpers
    /// </summary>
    public class ElasticSearchHelper
    {
        /// <summary>
        /// Rgister elastic search client to IoC
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="builder"></param>
        public static void RegisterClient(IConfiguration configuration, ContainerBuilder builder)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(defaultIndex);

            var client = new ElasticClient(settings);

            builder.Register<IElasticClient>(c =>
            {
                return client;
            }).InstancePerLifetimeScope();
        }
    }
}
