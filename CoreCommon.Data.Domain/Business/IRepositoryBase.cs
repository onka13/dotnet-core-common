using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Repository base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IRepositoryBase<TEntity>
    {
        void SetRefId(string refId);
        string GetRefId();
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
        bool HasTransaction(string newRefId = null);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        TEntity GetBy(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
        int Delete(TEntity entity);
        int DeleteBy(Expression<Func<TEntity, bool>> predicate);
        int Edit(TEntity entity);
        int EditBy(TEntity entity, params string[] properties);
        int EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        int BulkInsert(List<TEntity> entityList);
        int BulkUpdate(List<TEntity> entityList);
        int BulkDelete(List<TEntity> entityList);
        int BulkUpdateOnly(List<TEntity> entityList, params Expression<Func<TEntity, object>>[] properties);
        int Execute(string query, params object[] parameters);
        int BulkEditBy(List<TEntity> entities, params string[] properties);
        IEnumerable<TEntity> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include);
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        ServiceResult<T1> DoTransactionIfNot<T1>(Func<string, T1> func);
    }
}
