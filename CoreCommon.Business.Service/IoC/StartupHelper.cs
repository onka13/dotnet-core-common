using Autofac.Extensions.DependencyInjection;
using CoreCommon.Business.Service.IoC;
using Microsoft.Extensions.Hosting;
using System;

namespace CoreCommon.Business.Service.Helpers
{
    /// <summary>
    /// Startup helpers
    /// </summary>
    public class StartupHelper
    {
        /// <summary>
        /// Creates host.
        /// </summary>
        /// <typeparam name="TStartupBase">Startup class</typeparam>
        /// <param name="args">Arguments</param>
        /// <param name="startup">Startup class instance</param>
        /// <param name="isWeb">If environment is web patform</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder<TStartupBase>(string[] args, TStartupBase startup = null, bool isWeb = false) where TStartupBase : StartupBase
        {
            if (startup == null)
                startup = Activator.CreateInstance<TStartupBase>();

            var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
                    {
                        startup.ConfigureContainer(builder);
                    }))
                    .ConfigureAppConfiguration((hostContext, config) => startup.ConfigureAppConfiguration(config, hostContext.HostingEnvironment))
                    .ConfigureLogging((hostContext, logging) => startup.ConfigureLogging(hostContext.Configuration, logging));
            if (!isWeb)
            {
                host.ConfigureServices((hostContext, services) => startup.ConfigureServices(services));
            }

            return host;
        }
    }
}
