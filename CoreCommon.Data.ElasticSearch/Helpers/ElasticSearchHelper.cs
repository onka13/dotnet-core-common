using Nest;
using System;
using Microsoft.Extensions.Configuration;
using Autofac;

namespace CoreCommon.Data.ElasticSearch.Helpers
{
    public class ElasticSearchHelper
    {
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
