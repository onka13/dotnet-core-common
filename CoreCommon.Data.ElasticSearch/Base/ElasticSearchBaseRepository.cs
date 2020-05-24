using CoreCommon.Data.Domain.Business;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreCommon.Data.ElasticSearch.Base
{
    /// <summary>
    /// Elastic search base repository
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public abstract class ElasticSearchBaseRepository<TDocument, TPrimaryKey>
        : RepositoryBase<TDocument>, IElasticSearchBaseRepository<TDocument, TPrimaryKey>
        where TDocument : class, IElasticSearchBaseEntity<TPrimaryKey>, new()
    {
        /// <summary>
        /// Name of the Connection
        /// </summary>
        public abstract string ConnectionName { get; }

        /// <summary>
        /// Autowired property for getting appsettings
        /// </summary>
        public IConfiguration Configuration { get; set; }

        public IElasticClient ElasticClient { get; set; }

        public ElasticSearchBaseRepository()
        {
            var url = Configuration[ConnectionName + ":Url"];
            var defaultIndex = Configuration[ConnectionName + ":Index"];

            var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(defaultIndex);

            ElasticClient = new ElasticClient(settings);
        }

        public TDocument Add(TDocument entity)
        {
            var indexResponse = ElasticClient.IndexDocument(entity);
            if (indexResponse.Result != Result.Created)
            {
                return null;
            }
            entity.Id = (TPrimaryKey)Convert.ChangeType(indexResponse.Id, typeof(TPrimaryKey));
            return entity;
        }

        public int BulkDelete(List<TDocument> entities)
        {
            var result = ElasticClient.Bulk(b => b.Index<TDocument>().DeleteMany(entities));
            return !result.Errors ? entities.Count : 0;
        }

        public int BulkInsert(List<TDocument> entities)
        {
            //var result = ElasticClient.IndexMany(entities);
            var result = ElasticClient.Bulk(b => b.Index<TDocument>().IndexMany(entities));
            if (result.Errors)
            {
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    // log, itemWithError.Id, itemWithError.Error
                }
            }
            return entities.Count;
        }

        public int BulkUpdate(List<TDocument> entities)
        {
            var result = ElasticClient.Bulk(b => b.Index<TDocument>().UpdateMany(entities, (bulkUpdateDescriptor, entity) => bulkUpdateDescriptor.Index<TDocument>().Doc(entity)));
            return !result.Errors ? entities.Count : 0;
        }

        public int Delete(TDocument entity)
        {
            var deleteResponse = ElasticClient.Delete<TDocument>(entity);
            return deleteResponse.Result == Result.Deleted ? 1 : 0;
        }

        public int Edit(TDocument entity)
        {
            var updateResponse = ElasticClient.Update(new UpdateDescriptor<TDocument, TDocument>(entity));
            return updateResponse.Result == Result.Updated ? 1 : 0;
        }

        public IEnumerable<TDocument> GetAll()
        {
            return ElasticClient.Search<TDocument>(s => s.MatchAll()).Documents.ToList();
        }

        public int DeleteBy(TPrimaryKey id)
        {
            var deleteResponse = ElasticClient.Delete<TDocument>(id.ToString());
            return deleteResponse.Result == Result.Deleted ? 1 : 0;
        }

        public TDocument GetBy(TPrimaryKey id)
        {
            return ElasticClient.Get<TDocument>(id.ToString()).Source;
        }

        public List<TDocument> GetMany(List<TPrimaryKey> ids)
        {
            return ElasticClient.GetMany<TDocument>(ids.Select(x => x.ToString())).Select(x => x.Source).ToList();
        }

        public List<TDocument> Search(int from, int size, Func<QueryContainerDescriptor<TDocument>, QueryContainer> query, Expression<Func<TDocument, object>>[] selectFields, Field sortField, bool isAscending, out long _total)
        {
            var result = ElasticClient.Search<TDocument>(s => s
                .From(from)
                .Size(size)
                .Query(query)
                .Sort(x => x.Field(sortField, isAscending ? SortOrder.Ascending : SortOrder.Descending))
                .Source(x => x.Includes(y => y.Fields(selectFields)))
            );
            _total = result.Total;
            return result.Documents.ToList();
        }
    }
}
