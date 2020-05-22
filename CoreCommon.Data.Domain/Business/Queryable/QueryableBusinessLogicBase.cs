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
    public abstract class QueryableBusinessLogicBase<TEntity, TRepository> : CrudBusinessLogicBase<TEntity, TRepository>, IQueryableBusinessLogicBase<TEntity> 
        where TRepository : IQueryableRepositoryBase<TEntity>
    {
        public ServiceResult<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            var response = ServiceResult<IEnumerable<TEntity>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.FindBy(predicate);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

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

        public ServiceResult<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            var response = ServiceResult<TEntity>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetBy(predicate);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

        public ServiceResult<int> BulkEditOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkUpdateOnly(entities, properties);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> DeleteBy(Expression<Func<TEntity, bool>> predicate)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.DeleteBy(predicate);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Deleted);
            }
            return response;
        }

        public ServiceResult<int> EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var affected = Repository.EditOnly(entity, properties);
            if (affected > 0)
            {
                response.SuccessResult(affected, ServiceResultCode.Updated);
            }
            return response;
        }
    }
}
