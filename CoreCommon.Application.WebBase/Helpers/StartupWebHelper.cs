using CoreCommon.Application.Base.Helpers;
using CoreCommon.Application.WebBase.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreCommon.Application.WebBase.Helpers
{
    /// <summary>
    /// Startup Web Helper.
    /// </summary>
    public static class StartupWebHelper
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        public static void Run(string[] args, StartupWebBase startup)
        {
            var hostBuilder = StartupHelper.CreateHostBuilder(args, startup, isWeb: true);
            hostBuilder = ConfigureWebHostDefaults(hostBuilder, startup);
            hostBuilder.Build().Run();
        }

        /// <summary>
        /// Configure Web Host Defaults.
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder.</param>
        /// <param name="startup">StartupWebBase.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureWebHostDefaults(IHostBuilder hostBuilder, StartupWebBase startup)
        {
            return hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder

                // configure services here because we do not use UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    startup.ConfigureServices(services);
                })
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                    var config = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                    startup.Configure(app, env);
                });
            });
        }
    }
}
