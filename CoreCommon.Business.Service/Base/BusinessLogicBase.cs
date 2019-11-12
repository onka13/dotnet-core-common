using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Business.Service.Base
{
    public class BusinessLogicBase<T> : IBusinessLogicBase<T>
    {
        public IRepositoryBase<T> Repository { get; set; }

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

        public ServiceResult<IEnumerable<T>> GetAll()
        {
            var response = ServiceResult<IEnumerable<T>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetAll();
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

        public ServiceResult<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate)
        {
            var response = ServiceResult<IEnumerable<T>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.FindBy(predicate);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

        public ServiceResult<IEnumerable<T>> FindAndIncludeBy<TProp>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProp>>[] include)
        {
            var response = ServiceResult<IEnumerable<T>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.FindAndIncludeBy(predicate, include);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

        public ServiceResult<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            var response = ServiceResult<T>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetBy(predicate);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }

        public ServiceResult<T> Add(T entity)
        {
            var response = ServiceResult<T>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.Add(entity);
            if (form != null)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkInsert(List<T> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkInsert(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkEdit(List<T> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkUpdate(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkDelete(List<T> entities)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkDelete(entities);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<int> BulkEditOnly(List<T> entities, params Expression<Func<T, object>>[] properties)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.BulkUpdateOnly(entities, properties);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Created);
            }
            return response;
        }

        public ServiceResult<T> Delete(T entity)
        {
            var response = ServiceResult<T>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.Delete(entity);
            if (form > 0)
            {
                response.SuccessResult(entity, ServiceResultCode.Deleted);
            }
            return response;
        }

        public ServiceResult<int> DeleteBy(Expression<Func<T, bool>> predicate)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.DeleteBy(predicate);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Deleted);
            }
            return response;
        }

        public ServiceResult<int> Edit(T entity, params string[] properties)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var affected = Repository.EditBy(entity, properties);
            if (affected > 0)
            {
                response.SuccessResult(affected, ServiceResultCode.Updated);
            }
            return response;
        }

        public ServiceResult<int> EditOnly(T entity, params Expression<Func<T, object>>[] properties)
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
