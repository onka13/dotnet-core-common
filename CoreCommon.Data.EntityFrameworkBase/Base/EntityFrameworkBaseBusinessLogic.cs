using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public abstract class EntityFrameworkBaseBusinessLogic<TEntity, TRepository>
        : TransactionBusinessLogicBase<TEntity, TRepository> 
        where TRepository : IEntityFrameworkBaseRepository<TEntity>
    {
        public ServiceResult<IEnumerable<TEntity>> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include)
        {
            var response = ServiceResult<IEnumerable<TEntity>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.FindAndIncludeBy(predicate, include);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }
    }
}
