using Autofac;
using CoreCommon.Data.Domain.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreCommon.Application.WebAPIBase
{
    public class ModuleConfig : IModuleConfig
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
        }
    }
}
