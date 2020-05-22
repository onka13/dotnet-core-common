using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public interface IElasticSearchBaseRepository<TDocument> : ICrudRepositoryBase<TDocument>
    {
    }
}
