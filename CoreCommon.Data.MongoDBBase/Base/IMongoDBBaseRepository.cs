using CoreCommon.Data.Domain.Business.Queryable;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public interface IMongoDBBaseRepository<TDocument> : IQueryableRepositoryBase<TDocument>
    {
        IEnumerable<TDocument> FindAndIncludeBy<TForeignDocument>(Expression<Func<TDocument, bool>> predicate,
                                                                                    Expression<Func<TDocument, object>> localField,
                                                                                    Expression<Func<TForeignDocument, object>> foreignField,
                                                                                    Expression<Func<TDocument, object>> bindField);
        Task<List<dynamic>> RawJsonQuery<T>(string json);
    }
}
