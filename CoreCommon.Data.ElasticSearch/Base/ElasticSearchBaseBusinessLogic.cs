using CoreCommon.Data.Domain.Business.Crud;
using System.Collections.Generic;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public abstract class ElasticSearchBaseBusinessLogic<TDocument, TPrimaryKey, TRepository> 
        : CrudBusinessLogicBase<TDocument, TRepository>,
        IElasticSearchBaseBusinessLogic<TDocument, TPrimaryKey> 
        where TRepository : IElasticSearchBaseRepository<TDocument, TPrimaryKey>
        where TDocument : class, new()
    {
        public ServiceResult<int> DeleteBy(TPrimaryKey id)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.DeleteBy(id);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Deleted);
            }
            return response;
        }

        public ServiceResult<int> EditOnly(TPrimaryKey id, object partialEntity)
        {
            var response = ServiceResult<int>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.EditOnly(id, partialEntity);
            if (form > 0)
            {
                response.SuccessResult(form, ServiceResultCode.Updated);
            }
            return response;
        }

        public ServiceResult<TDocument> GetBy(TPrimaryKey id)
        {
            var response = ServiceResult<TDocument>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetBy(id);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }
        
        public ServiceResult<List<TDocument>> GetMany(List<TPrimaryKey> ids)
        {
            var response = ServiceResult<List<TDocument>>.Instance.ErrorResult(ServiceResultCode.Error);
            var form = Repository.GetMany(ids);
            if (form != null)
            {
                response.SuccessResult(form);
            }
            return response;
        }
    }
}
