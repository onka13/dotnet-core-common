using CoreCommon.Data.Domain.Business.Queryable;
using System;
using System.Threading.Tasks;

namespace CoreCommon.Data.Domain.Business.Transaction
{
    /// <summary>
    /// Repository base interface.
    /// </summary>
    /// <typeparam name="TEntity">Entity Type.</typeparam>
    public interface ITransactionRepositoryBase<TEntity> : IQueryableRepositoryBase<TEntity>
    {
        void SetRefId(string refId);

        string GetRefId();

        Task BeginTransaction(string newRefId = null);

        Task CommitTransaction(string newRefId = null);

        Task RollbackTransaction(string newRefId = null);

        bool HasTransaction(string newRefId = null);

        Task<T1> DoTransaction<T1>(Func<string, T1> func);

        Task<T1> DoTransactionIfNot<T1>(Func<string, T1> func);
    }
}
