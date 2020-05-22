using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public abstract class MongoDBBaseBusinessLogic<TEntity, TRepository> 
        : QueryableBusinessLogicBase<TEntity, TRepository> 
        where TRepository : IMongoDBBaseRepository<TEntity>
    {

    }
}
