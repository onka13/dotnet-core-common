using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base class
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BusinessLogicBase<TEntity, TRepository> : IBusinessLogicBase<TEntity> where TRepository : IRepositoryBase<TEntity>
    {
        public abstract TRepository Repository { get; set; }

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

        public ServiceResult<IEnumerable<TEntity>> GetAll()
        {
            var response = ServiceResult<IEnumerable<TEntity>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetAll();
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

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

        public ServiceResult<TEntity> Add(TEntity entity)
        {
            var response = ServiceResult<TEntity>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.Add(entity);
            if (form != null)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkInsert(List<TEntity> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkInsert(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkEdit(List<TEntity> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkUpdate(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkDelete(List<TEntity> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkDelete(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
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

        public ServiceResult<TEntity> Delete(TEntity entity)
        {
            var response = ServiceResult<TEntity>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.Delete(entity);
            if (form > 0)
            {
                response.SuccessResult(entity, ServiceResultCode.Deleted);
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

        public ServiceResult<int> Edit(TEntity entity, params string[] properties)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var affected = Repository.EditBy(entity, properties);
            if (affected > 0)
            {
                response.SuccessResult(affected, ServiceResultCode.Updated);
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
