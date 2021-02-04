using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public abstract class MongoDBBaseBusinessLogic<TEntity, TRepository> 
        : QueryableBusinessLogicBase<TEntity, TRepository> 
        where TRepository : IMongoDBBaseRepository<TEntity>
    {
        public ServiceResult<IEnumerable<TEntity>> FindAndIncludeBy<TForeignDocument>(Expression<Func<TEntity, bool>> predicate,
                                                                                    Expression<Func<TEntity, object>> localField,
                                                                                    Expression<Func<TForeignDocument, object>> foreignField,
                                                                                    Expression<Func<TEntity, object>> bindField)
        {
            var response = ServiceResult<IEnumerable<TEntity>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.FindAndIncludeBy(predicate, localField, foreignField, bindField);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }
    }
}
