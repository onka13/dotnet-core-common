using System.Collections.Generic;

namespace CoreCommon.Infrastructure.Domain.Models
{
    public class IdModel
    {
        public int Id { get; set; }
    }

    public class ApiRequestListModel<T>
    {
        public Pagination Pagination { get; set; }
        public Sort Sort { get; set; }
        public T Filter { get; set; }

        public ApiRequestListModel()
        {
            Pagination = new Pagination { Page = 1, PerPage = 30 };
            Sort = new Sort();
            Filter = default;
        }
    }

    public class Pagination
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
    }

    public class Sort
    {
        public string Field { get; set; }
        public string Order { get; set; }
    }

    public class ArrayRequest<T>
    {
        public List<T> Ids { get; set; }
    }
}
