using System;
using System.Linq;
using System.Reflection;
using Autofac;

namespace CoreCommon.Application.Base.Helpers
{
    /// <summary>
    /// Dependency helper.
    /// </summary>
    public class DependencyHelper
    {
        /// <summary>
        /// Register common types.
        /// </summary>
        /// <param name="builder">ContainerBuilder object.</param>
        /// <param name="types">Types.</param>
        public static void RegisterCommonTypes(ContainerBuilder builder, params Type[] types)
        {
            var assemblies = types.Select(x => x.GetTypeInfo().Assembly);
            RegisterCommonTypes(builder, assemblies.ToArray());
        }

        /// <summary>
        /// Register common types in assemblies.
        /// Controller, Manager, Filter => asSelf
        /// Repository, BusinessLogic => asImplementedInterfaces
        /// DbContext => asSelf.
        /// </summary>
        /// <param name="builder">ContainerBuilder.</param>
        /// <param name="assemblies">Assemblies.</param>
        public static void RegisterCommonTypes(ContainerBuilder builder, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Controller") || t.Name.EndsWith("Manager") || t.Name.EndsWith("Filter"))
                    .AsSelf()
                    .PropertiesAutowired();

                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("BusinessLogic") || t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .AsSelf()
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
