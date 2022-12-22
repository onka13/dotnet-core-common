using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Business.Crud;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public interface IElasticSearchBaseRepository<TDocument, TPrimaryKey> : ICrudRepositoryBase<TDocument> where TDocument : class, new()
    {
        Task<int> DeleteBy(TPrimaryKey id);
        Task<TDocument> GetBy(TPrimaryKey id);
        Task<List<TDocument>> GetMany(List<TPrimaryKey> ids);
        Task<int> EditOnly(TPrimaryKey id, object partialEntity);
        Task<(List<TDocument> data, long total)> Search(int from, int size, Func<QueryContainerDescriptor<TDocument>, QueryContainer> query, Expression<Func<TDocument, object>>[] selectFields, Field sortField, bool isAscending);
    }
}
