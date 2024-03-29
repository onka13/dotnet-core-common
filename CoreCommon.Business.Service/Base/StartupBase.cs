﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreCommon.Business.Service.Helpers;
using CoreCommon.Data.Domain.Config;
using CoreCommon.Data.EntityFrameworkBase.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CoreCommon.Business.Service.Base
{
    /// <summary>
    /// Startup base class
    /// </summary>
    public abstract class StartupBase
    {
        public IConfiguration Configuration { get; set; }
        public IHostEnvironment Environment { get; set; }
        public ILifetimeScope AutofacContainer { get; set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddAutofac();
            services.AddOptions();
            services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));
            services.Configure<AppSettingsConfig>(Configuration.GetSection("AppSettings"));
        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            DependencyHelper.RegisterCommonTypes(builder, typeof(CoreCommon.Data.EntityFrameworkBase.Components.EmptyDbContext));
            builder.RegisterType<DbContextManager>()
                   .AsSelf()
                   .PropertiesAutowired()
                   .InstancePerLifetimeScope();
        }

        public virtual void ConfigureAppConfiguration(IConfigurationBuilder config, IHostEnvironment env)
        {
            Environment = env;

            config.SetBasePath(AppContext.BaseDirectory); // TODO: check Directory.GetCurrentDirectory()
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            config.AddJsonFile($"appsettings.overrides.json", optional: true);
            config.AddJsonFile($"appsettings.Local.json", optional: true);
            config.AddEnvironmentVariables();
        }

        public virtual void ConfigureLogging(IConfiguration configuration, ILoggingBuilder logging)
        {
            Configuration = configuration;

            logging.AddConfiguration(configuration.GetSection("Logging"));
            logging.AddConsole(); // TODO: logging
        }
    }
}
