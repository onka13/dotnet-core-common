using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IBusinessLogicBase<TEntity>
    {
        void SetRef(string refId);
        string GetRef();
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
        ServiceResult<IEnumerable<TEntity>> GetAll();
        ServiceResult<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate);
        ServiceResult<IEnumerable<TEntity>> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include);
        ServiceResult<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate);
        ServiceResult<TEntity> Add(TEntity entity);
        ServiceResult<TEntity> Delete(TEntity entity);
        ServiceResult<int> DeleteBy(Expression<Func<TEntity, bool>> predicate);
        ServiceResult<int> Edit(TEntity entity, params string[] properties);
        ServiceResult<int> EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        ServiceResult<int> BulkInsert(List<TEntity> entities);
        ServiceResult<int> BulkDelete(List<TEntity> entities);
        ServiceResult<int> BulkEdit(List<TEntity> entities);
        ServiceResult<int> BulkEditOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);
    }
}
