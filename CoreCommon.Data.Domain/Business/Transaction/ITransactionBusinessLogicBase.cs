﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base interface
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface ITransactionBusinessLogicBase<TEntity> : IQueryableBusinessLogicBase<TEntity>
    {
        void SetRef(string refId);
        string GetRef();
        ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func);
        void BeginTransaction(string newRefId = null);
        void CommitTransaction(string newRefId = null);
        void RollbackTransaction(string newRefId = null);
    }
}
