using System;
using Autofac.Extensions.DependencyInjection;
using CoreCommon.Application.APIBase.Base;
using Microsoft.Extensions.Hosting;

namespace CoreCommon.Application.APIBase.Helpers
{
    /// <summary>
    /// Startup helpers.
    /// </summary>
    public class StartupHelper
    {
        /// <summary>
        /// Creates host.
        /// </summary>
        /// <typeparam name="TStartupBase">Startup class.</typeparam>
        /// <param name="args">Arguments.</param>
        /// <param name="startup">Startup class instance.</param>
        /// <param name="isWeb">If environment is web patform.</param>
        /// <param name="environment">Environment.</param>
        /// <returns>Host object.</returns>
        public static IHostBuilder CreateHostBuilder<TStartupBase>(string[] args, TStartupBase startup = null, bool isWeb = false, string environment = null)
            where TStartupBase : StartupBase
        {
            if (startup == null)
            {
                startup = Activator.CreateInstance<TStartupBase>();
            }

            var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
                    {
                        startup.ConfigureContainer(builder);
                    }))
                    .ConfigureAppConfiguration((hostContext, config) =>
                    {
                        startup.Configure(config, hostContext.HostingEnvironment);
                    })
                    .ConfigureLogging((hostContext, logging) =>
                    {
                        startup.ConfigureLogging(hostContext.Configuration, logging);
                    });
            if (!isWeb)
            {
                host.ConfigureServices((hostContext, services) =>
                {
                    startup.ConfigureServices(services);
                });
            }

            if (!string.IsNullOrEmpty(environment))
            {
                host = host.UseEnvironment(environment);
            }

            return host;
        }
    }
}
