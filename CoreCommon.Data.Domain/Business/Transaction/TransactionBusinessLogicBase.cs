using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base crud
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TransactionBusinessLogicBase<TEntity, TRepository> :
        QueryableBusinessLogicBase<TEntity, TRepository>, ITransactionBusinessLogicBase<TEntity>
        where TRepository : ITransactionRepositoryBase<TEntity>
    {
        public void SetRef(string refId)
        {
            Repository.SetRefId(refId);
        }

        public string GetRef()
        {
            return Repository.GetRefId();
        }

        public ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func)
        {
            return Repository.DoTransaction(func);
        }

        public void BeginTransaction(string newRefId = null)
        {
            Repository.BeginTransaction(newRefId);
        }

        public void CommitTransaction(string newRefId = null)
        {
            Repository.CommitTransaction(newRefId);
        }

        public void RollbackTransaction(string newRefId = null)
        {
            Repository.RollbackTransaction(newRefId);
        }


    }
}
