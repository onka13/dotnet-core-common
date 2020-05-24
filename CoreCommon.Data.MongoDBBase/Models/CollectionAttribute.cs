using System;
using System.Collections.Generic;
using System.Text;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public class CollectionAttribute : Attribute
    {
        public string Name { get; set; }

        public CollectionAttribute(string name)
        {
            Name = name;
        }
    }
}
