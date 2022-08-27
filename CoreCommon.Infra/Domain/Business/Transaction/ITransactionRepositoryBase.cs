using CoreCommon.Infra.Domain.Business.Queryable;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Infra.Domain.Business.Transaction
{
    /// <summary>
    /// Repository base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface ITransactionRepositoryBase<TEntity> : IQueryableRepositoryBase<TEntity>
    {
        void SetRefId(string refId);
        string GetRefId();
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
        bool HasTransaction(string newRefId = null);
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        ServiceResult<T1> DoTransactionIfNot<T1>(Func<string, T1> func);
    }
}
