using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace CoreCommon.Application.APIBase.TestComponents
{
    public class TextFileDataAttribute : DataAttribute
    {
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileDataAttribute"/> class.
        /// Load data from a file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the file to load</param>
        public TextFileDataAttribute(string filePath)
        {
            this.filePath = filePath;
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

            var data = File.ReadAllText(filePath);

            return new[] { new object[] { data } };
        }
    }
}
