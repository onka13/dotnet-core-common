using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Models
{
    public class SearchResult
    {
        public List<object> Items { get; set; }

        public long Total { get; set; }
    }
}
