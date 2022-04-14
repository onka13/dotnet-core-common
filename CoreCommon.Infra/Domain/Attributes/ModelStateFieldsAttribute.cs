using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Infrastructure.Domain.Attributes
{
    public class ModelStateFieldsAttribute : Attribute
    {
        public List<string> Names { get; set; }
        public ModelStateFieldsAttribute(string[] _names)
        {
            Names = _names.ToList();
        }
    }
}
