using CoreCommon.Application.Base.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoreCommon.Application.Base.Helpers
{
    /// <summary>
    /// Contains helper methods for modules.
    /// </summary>
    public class ModuleHelper
    {
        /// <summary>
        /// List of the defined modules.
        /// </summary>
        private static List<IModuleConfig> moduleConfigs;

        public static string SolutionName { get; set; }

        /// <summary>
        /// Finds all modules in the system.
        /// </summary>
        public static void FindModules()
        {
            moduleConfigs = FindAllAssemblies()
                        .SelectMany(x => x.DefinedTypes)
                        .Where(type => type.IsClass && typeof(IModuleConfig).GetTypeInfo().IsAssignableFrom(type.AsType()))
                        .Select(type => (IModuleConfig)Activator.CreateInstance(type))
                        .ToList();
        }

        /// <summary>
        /// Gets module list.
        /// </summary>
        /// <returns></returns>
        public static List<IModuleConfig> GetModules()
        {
            if (moduleConfigs == null)
            {
                FindModules();
            }

            return moduleConfigs;
        }

        public static List<Assembly> FindAllAssemblies()
        {
            return Directory.GetFiles(AppContext.BaseDirectory, $"{SolutionName}*.dll")
                .Select(Assembly.LoadFrom)
                .ToList();
        }
    }
}
