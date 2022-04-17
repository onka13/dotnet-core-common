using CoreCommon.Infrastructure.Domain.Business.Queryable;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public interface IMongoDBBaseBusinessLogic<TEntity> : IQueryableBusinessLogicBase<TEntity>
    {

    }
}
