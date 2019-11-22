using Autofac;
using CoreCommon.Data.Domain.Config;
using CoreCommon.Data.EntityFrameworkBase.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace CoreCommon.Business.Service.IoC
{
    public class DependencyHelper
    {
        public static void RegisterCommonTypes(ContainerBuilder builder, params Type[] types)
        {
            var assemblies = types.Select(x => x.GetTypeInfo().Assembly);
            RegisterCommonTypes(builder, assemblies.ToArray());
        }

        public static void RegisterCommonTypes(ContainerBuilder builder, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Controller") || t.Name.EndsWith("Manager") || t.Name.EndsWith("Filter"))
                    .AsSelf()
                    .PropertiesAutowired();

                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("BusinessLogic"))
                    .AsImplementedInterfaces()
                    .PropertiesAutowired();

                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("DbContext"))
                    .AsSelf()
                    .PropertiesAutowired()
                    .InstancePerDependency();
            }
        }
    }
}
