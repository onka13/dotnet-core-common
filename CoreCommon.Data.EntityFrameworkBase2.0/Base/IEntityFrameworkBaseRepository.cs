using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public interface IEntityFrameworkBaseRepository<TEntity> : ITransactionRepositoryBase<TEntity>
    {
        IEnumerable<TEntity> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include);
    }
}
