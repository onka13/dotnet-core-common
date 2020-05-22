using System.Collections.Generic;

namespace CoreCommon.Data.Domain.Business
{
    /// <summary>
    /// Business logic base crud
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class CrudBusinessLogicBase<TEntity, TRepository> : ICrudBusinessLogicBase<TEntity>
        where TRepository : ICrudRepositoryBase<TEntity>
    {
        public abstract TRepository Repository { get; set; }

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

        public ServiceResult<int> Edit(TEntity entity)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.Edit(entity);
            if (form > 0)
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
    }
}
