using CoreCommon.Data.Domain.Business.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreCommon.Data.Domain.Business.Queryable
{
    /// <summary>
    /// Repository base interface.
    /// </summary>
    /// <typeparam name="TEntity">Entity Type.</typeparam>
    public interface IQueryableRepositoryBase<TEntity> : ICrudRepositoryBase<TEntity>
    {
        Task<int> BulkUpdateOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);

        Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);

        Task<bool> Any(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations);

        Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take);

        Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take, bool includeRelations);

        Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations);

        Task<int> DeleteBy(Expression<Func<TEntity, bool>> predicate);

        Task<int> UpdateOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);

        Task<int> UpdateExcept(TEntity entity, params Expression<Func<TEntity, object>>[] exclude);

        IQueryable<TEntity> GetQueryable();
    }
}
