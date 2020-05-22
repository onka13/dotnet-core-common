using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public interface IElasticSearchBaseBusinessLogic<TEntity> : ICrudBusinessLogicBase<TEntity>
    {
    }
}
