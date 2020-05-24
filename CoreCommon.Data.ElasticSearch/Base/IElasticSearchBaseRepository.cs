using CoreCommon.Data.Domain.Business;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public interface IElasticSearchBaseRepository<TDocument, TPrimaryKey> : ICrudRepositoryBase<TDocument> where TDocument : class, new()
    {
        int DeleteBy(TPrimaryKey id);
        TDocument GetBy(TPrimaryKey id);
        List<TDocument> GetMany(List<TPrimaryKey> ids);
        List<TDocument> Search(int from, int size, Func<QueryContainerDescriptor<TDocument>, QueryContainer> query, Expression<Func<TDocument, object>>[] selectFields, Field sortField, bool isAscending, out long _total);
    }
}
