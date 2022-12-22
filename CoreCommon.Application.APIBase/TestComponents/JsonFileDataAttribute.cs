using CoreCommon.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace CoreCommon.Application.APIBase.TestComponents
{
    public class JsonFileDataAttribute : DataAttribute
    {
        private readonly string filePath;
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileDataAttribute"/> class.
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        /// <param name="type">Type</param>
        public JsonFileDataAttribute(string filePath, Type type)
        {
            this.filePath = filePath;
            this.type = type;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(filePath)
                ? filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            var content = File.ReadAllText(filePath);
            var data = ConversionHelper.DerializeObject(content, type);

            return new[] { new object[] { data } };
        }
    }
}
