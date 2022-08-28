using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Data.Domain.Attributes
{
    /// <summary>
    /// ModelStateFieldsAttribute.
    /// </summary>
    public class ModelStateFieldsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelStateFieldsAttribute"/> class.
        /// </summary>
        /// <param name="names">Names.</param>
        public ModelStateFieldsAttribute(string[] names)
        {
            Names = names.ToList();
        }

        /// <summary>
        /// Gets or sets Names.
        /// </summary>
        public List<string> Names { get; set; }
    }
}
