using Autofac;
using System;
using System.Linq;
using System.Reflection;

namespace CoreCommon.Business.Service.Helpers
{
    /// <summary>
    /// Dependency helper
    /// </summary>
    public class DependencyHelper
    {
        /// <summary>
        /// Register common types
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        public static void RegisterCommonTypes(ContainerBuilder builder, params Type[] types)
        {
            var assemblies = types.Select(x => x.GetTypeInfo().Assembly);
            RegisterCommonTypes(builder, assemblies.ToArray());
        }

        /// <summary>
        /// Register common types in assemblies.
        /// Controller, Manager, Filter => asSelf
        /// Repository, BusinessLogic => asImplementedInterfaces
        /// DbContext => asSelf
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
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
                
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("MongoContext"))
                    .AsSelf()
                    .PropertiesAutowired()
                    .SingleInstance();
            }
        }
    }
}
