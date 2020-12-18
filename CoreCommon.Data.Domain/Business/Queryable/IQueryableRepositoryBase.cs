using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Repository base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IQueryableRepositoryBase<TEntity> : ICrudRepositoryBase<TEntity>
    {
        int BulkUpdateOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        TEntity GetBy(Expression<Func<TEntity, bool>> predicate);        
        int DeleteBy(Expression<Func<TEntity, bool>> predicate);
        int EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);        
    }
}
