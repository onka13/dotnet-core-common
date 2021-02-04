using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IQueryableBusinessLogicBase<TEntity> : ICrudBusinessLogicBase<TEntity>
    {
        ServiceResult<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations = false);
        ServiceResult<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take, bool includeRelations = false);
        ServiceResult<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations = false);
        
        ServiceResult<int> DeleteBy(Expression<Func<TEntity, bool>> predicate);
        ServiceResult<int> EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);        
        ServiceResult<int> BulkEditOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);
    }
}
