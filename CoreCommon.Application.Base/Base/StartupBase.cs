using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreCommon.Application.Base.Helpers;
using CoreCommon.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CoreCommon.Application.Base.Base
{
    /// <summary>
    /// Startup base class.
    /// </summary>
    public abstract class StartupBase
    {
        /// <summary>
        /// Gets or sets iConfiguration object.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets Environment.
        /// </summary>
        public IHostEnvironment Environment { get; set; }

        /// <summary>
        /// Gets or sets AutofacContainer.
        /// </summary>
        public ILifetimeScope AutofacContainer { get; set; }

        /// <summary>
        /// Add autofac and default options.
        /// </summary>
        /// <param name="services">IServiceCollection object.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddAutofac();
            services.AddOptions();
            services.AddLogging();

            foreach (var moduleConfig in ModuleHelper.GetModules())
            {
                moduleConfig.ConfigureServices(Configuration, services);
            }
        }

        /// <summary>
        /// Register types and for DbContext.
        /// </summary>
        /// <param name="builder">ContainerBuilder object.</param>
        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            DependencyHelper.RegisterCommonTypes(builder, ModuleHelper.FindAllAssemblies().ToArray());

            foreach (var moduleConfig in ModuleHelper.GetModules())
            {
                moduleConfig.ConfigureContainer(builder);
            }

            builder.RegisterType<LogService>().As<ILogService>().PropertiesAutowired().SingleInstance();
        }

        /// <summary>
        /// Add appsettings files.
        /// </summary>
        /// <param name="config">IConfigurationBuilder object.</param>
        /// <param name="env">IHostEnvironment object.</param>
        public virtual void Configure(IConfigurationBuilder config, IHostEnvironment env)
        {
            Environment = env;
            config.SetBasePath(AppContext.BaseDirectory); // TODO: check Directory.GetCurrentDirectory()
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            config.AddJsonFile($"appsettings.overrides.json", optional: true);
            config.AddJsonFile($"appsettings.Local.json", optional: true);
            config.AddEnvironmentVariables();

            var settings = config.Build();
            config.AddJsonFile($"appsettings.Final.json", optional: true);
        }

        /// <summary>
        /// Add logging.
        /// </summary>
        /// <param name="configuration">IConfiguration object.</param>
        /// <param name="logging">ILoggingBuilder object.</param>
        public virtual void ConfigureLogging(IConfiguration configuration, ILoggingBuilder logging)
        {
            Configuration = configuration;
            logging.AddConfiguration(configuration.GetSection("Logging"));
        }
    }
}
