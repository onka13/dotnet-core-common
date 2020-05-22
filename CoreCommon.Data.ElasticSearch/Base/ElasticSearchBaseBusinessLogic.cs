using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public abstract class ElasticSearchBaseBusinessLogic<TEntity, TRepository> 
        : CrudBusinessLogicBase<TEntity, TRepository> 
        where TRepository : IElasticSearchBaseRepository<TEntity>
    {

    }
}
