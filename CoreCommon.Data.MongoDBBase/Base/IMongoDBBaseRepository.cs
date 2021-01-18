using CoreCommon.Data.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public interface IMongoDBBaseRepository<TDocument> : IQueryableRepositoryBase<TDocument>
    {
        IEnumerable<TDocument> FindAndIncludeBy<TForeignDocument>(Expression<Func<TDocument, bool>> predicate,
                                                                                    Expression<Func<TDocument, object>> localField,
                                                                                    Expression<Func<TForeignDocument, object>> foreignField,
                                                                                    Expression<Func<TDocument, object>> bindField);
        List<dynamic> RawJsonQuery<T>(string json);
    }
}
