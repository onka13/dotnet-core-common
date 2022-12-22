using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Data.Domain.Attributes
{
    /// <summary>
    /// ModelStateIgnoreAttribute.
    /// </summary>
    public class ModelStateIgnoreAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelStateIgnoreAttribute"/> class.
        /// </summary>
        /// <param name="names">Names.</param>
        public ModelStateIgnoreAttribute(string[] names)
        {
            Names = names.ToList();
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public List<string> Names { get; set; }
    }
}
