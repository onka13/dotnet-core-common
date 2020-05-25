using CoreCommon.Data.Domain.Business;
using System.Collections.Generic;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public interface IElasticSearchBaseBusinessLogic<TDocument, TPrimaryKey> : ICrudBusinessLogicBase<TDocument>
    {
        ServiceResult<int> DeleteBy(TPrimaryKey id);
        ServiceResult<TDocument> GetBy(TPrimaryKey id);
        ServiceResult<List<TDocument>> GetMany(List<TPrimaryKey> ids);
        ServiceResult<int> EditOnly(TPrimaryKey id, object partialEntity);
    }
}
