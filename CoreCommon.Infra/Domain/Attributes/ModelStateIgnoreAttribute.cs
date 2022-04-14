using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Infrastructure.Domain.Attributes
{
    public class ModelStateIgnoreAttribute : Attribute
    {
        public List<string> Names { get; set; }
        public ModelStateIgnoreAttribute(string[] _names)
        {
            Names = _names.ToList();
        }
    }
}
