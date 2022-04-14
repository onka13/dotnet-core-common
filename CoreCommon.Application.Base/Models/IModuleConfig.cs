using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreCommon.Application.Base.Models
{
    /// <summary>
    /// Module Config interface.
    /// </summary>
    public interface IModuleConfig
    {
        void ConfigureServices(IConfiguration configuration, IServiceCollection services);

        void ConfigureContainer(ContainerBuilder builder);
    }
}
