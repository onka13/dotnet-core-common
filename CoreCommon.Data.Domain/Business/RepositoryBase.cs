using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Data.Domain.Business
{
    public class RepositoryBase<TEntity>
    {
        public List<object> SkipTake(IEnumerable<object> result, int skip, int take, out long total)
        {
            if (take > 0)
            {
                total = result.Count();
                result = result.Skip(skip).Take(take);
            }
            else
            {
                total = 0;
            }
            return result.Select(x => (object)x).ToList();
        }
    }
}
