using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    public interface IRepositoryBase<T>
    {
        void SetRefId(string refId);
        string GetRefId();
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
        bool HasTransaction(string newRefId = null);
        IEnumerable<T> GetAll();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetBy(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        int Delete(T entity);
        int DeleteBy(Expression<Func<T, bool>> predicate);
        int Edit(T entity);
        int EditBy(T entity, params string[] properties);
        int EditOnly(T entity, params Expression<Func<T, object>>[] properties);
        int BulkInsert(List<T> entityList);
        int BulkUpdate(List<T> entityList);
        int BulkDelete(List<T> entityList);
        int BulkUpdateOnly(List<T> entityList, params Expression<Func<T, object>>[] properties);
        int Execute(string query, params object[] parameters);
        int BulkEditBy(List<T> entities, params string[] properties);
        IEnumerable<T> FindAndIncludeBy<TProp>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProp>>[] include);
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        ServiceResult<T1> DoTransactionIfNot<T1>(Func<string, T1> func);
    }
}
