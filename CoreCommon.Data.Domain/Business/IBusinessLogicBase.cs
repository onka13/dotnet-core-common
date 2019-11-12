using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    public interface IBusinessLogicBase<T>
    {
        void SetRef(string refId);
        string GetRef();
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
        ServiceResult<IEnumerable<T>> GetAll();
        ServiceResult<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate);
        ServiceResult<IEnumerable<T>> FindAndIncludeBy<TProp>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProp>>[] include);
        ServiceResult<T> GetBy(Expression<Func<T, bool>> predicate);
        ServiceResult<T> Add(T entity);
        ServiceResult<T> Delete(T entity);
        ServiceResult<int> DeleteBy(Expression<Func<T, bool>> predicate);
        ServiceResult<int> Edit(T entity, params string[] properties);
        ServiceResult<int> EditOnly(T entity, params Expression<Func<T, object>>[] properties);
        ServiceResult<int> BulkInsert(List<T> entities);
        ServiceResult<int> BulkDelete(List<T> entities);
        ServiceResult<int> BulkEdit(List<T> entities);
        ServiceResult<int> BulkEditOnly(List<T> entities, params Expression<Func<T, object>>[] properties);
    }
}
