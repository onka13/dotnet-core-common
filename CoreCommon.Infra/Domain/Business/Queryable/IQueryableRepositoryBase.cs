using CoreCommon.Infra.Domain.Business.Crud;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Infra.Domain.Business.Queryable
{
    /// <summary>
    /// Repository base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IQueryableRepositoryBase<TEntity> : ICrudRepositoryBase<TEntity>
    {
        int BulkUpdateOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations);
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take);
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take, bool includeRelations);
        TEntity GetBy(Expression<Func<TEntity, bool>> predicate);
        TEntity GetBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations);
        int DeleteBy(Expression<Func<TEntity, bool>> predicate);
        int EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        int EditExcept(TEntity entity, params Expression<Func<TEntity, object>>[] exclude);
    }
}
