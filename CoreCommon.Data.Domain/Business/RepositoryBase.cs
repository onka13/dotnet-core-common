using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    public class RepositoryBase<TEntity>
    {
        public virtual List<object> SkipTake(IEnumerable<object> result, int skip, int take, out long total)
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

        public Expression<Func<TEntity, T>> Projection<T>(Expression<Func<TEntity, T>> projection)
        {
            return projection;
        }

        public Expression<Func<TEntity, object>> SortField(string orderBy, params Expression<Func<TEntity, object>>[] fields)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                foreach (var field in fields)
                {
                    string name;
                    if (field.Body is MemberExpression)
                    {
                        name = (field.Body as MemberExpression).Member.Name;
                    }
                    else
                    {
                        try
                        {
                            name = (field.Body as dynamic).Operand.Member.Name;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    
                    
                    if (name.Equals(orderBy, StringComparison.InvariantCultureIgnoreCase)) return field;
                }
            }
            return null;
        }
    }
}
