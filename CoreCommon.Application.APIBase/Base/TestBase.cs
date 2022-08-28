using System;
using Autofac.Extensions.DependencyInjection;
using CoreCommon.Application.APIBase.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CoreCommon.Application.APIBase.Base
{
    /// <summary>
    /// Base class for all Tests.
    /// </summary>
    public class TestBase<TStartUp>
        where TStartUp : StartupBase
    {
        private TStartUp startup;

        public TestBase()
        {
        }

        public static AutofacServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Initialize IoC and other settings.
        /// </summary>
        public void Init()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

#if DEBUG
            environment = "Development";
#endif

            if (ServiceProvider != null)
            {
                return;
            }

            startup = Activator.CreateInstance<TStartUp>();
            var host = StartupHelper.CreateHostBuilder(null, startup: startup, environment: environment).Build();
            ServiceProvider = (AutofacServiceProvider)host.Services;

            // var container = Startup.ContainerBuilder.Build();
            // var p = new AutofacServiceProvider(container);
        }

        /// <summary>
        /// Get service of type T from the ServiceProvider.
        /// </summary>
        /// <returns></returns>
        public T Resolve<T>()
        {
            if (ServiceProvider == null)
            {
                Init();
            }

            return ServiceProvider.GetService<T>();
        }
    }
}
