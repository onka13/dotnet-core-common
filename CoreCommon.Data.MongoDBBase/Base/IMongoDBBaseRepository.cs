using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public interface IMongoDBBaseRepository<TDocument> : IQueryableRepositoryBase<TDocument>
    {
    }
}
