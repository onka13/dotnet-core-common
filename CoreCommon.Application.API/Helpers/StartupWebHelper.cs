using CoreCommon.Application.API.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreCommon.Application.API.Helpers
{
    public static class StartupWebHelper
    {
        public static IHostBuilder ConfigureWebHostDefaults(IHostBuilder hostBuilder, StartupWebBase startup)
        {
            return hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                // configure services here because of that we do not use UseStartup<Startup>()
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
