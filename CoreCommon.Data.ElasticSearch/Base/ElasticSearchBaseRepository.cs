using CoreCommon.Infrastructure.Domain.Business;
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

        public string IndexName { get; }

        /// <summary>
        /// Autowired property for getting appsettings
        /// </summary>
        public IConfiguration Configuration { get; set; }

        private IElasticClient _elasticClient;
        public IElasticClient ElasticClient
        {
            get
            {
                if (_elasticClient == null)
                {
                    var url = Configuration[ConnectionName + ":Url"];

                    if (!string.IsNullOrEmpty(Configuration[ConnectionName + "_Url"]))
                    {
                        url = Configuration[ConnectionName + "_Url"];
                    }

                    var settings = new ConnectionSettings(new Uri(url));
                    settings.DefaultIndex(IndexName);
                    _elasticClient = new ElasticClient(settings);

                    _elasticClient.Indices.Create(IndexName, c => c.Map<TDocument>(m => m.AutoMap<TDocument>()));
                }
                return _elasticClient;
            }
        }

        public ElasticSearchBaseRepository()
        {
            var attribute = (IndexConfigAttribute)typeof(TDocument).GetCustomAttributes(typeof(IndexConfigAttribute), false).FirstOrDefault();
            IndexName = attribute.Name;
        }

        public TDocument Add(TDocument entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            var indexResponse = ElasticClient.Index(entity, x => x.Id(entity.Id));
            if (indexResponse.Result != Result.Created)
            {
                return null;
            }
            return entity;
        }

        public int BulkDelete(List<TDocument> entities)
        {
            var result = ElasticClient.Bulk(b => b.Index<TDocument>().DeleteMany(entities));
            return !result.Errors ? entities.Count : 0;
        }

        public int BulkInsert(List<TDocument> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid().ToString();
            }
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
            var deleteResponse = ElasticClient.Delete<TDocument>(entity.Id);
            return deleteResponse.Result == Result.Deleted ? 1 : 0;
        }

        public int Edit(TDocument entity)
        {
            var updateResponse = ElasticClient.Update<TDocument>(entity.Id, x => x.Doc(entity));
            return updateResponse.Result == Result.Updated ? 1 : 0;
        }

        public int EditOnly(TPrimaryKey id, object partialEntity)
        {
            var updateResponse = ElasticClient.Update<object>(id, x => x.Index(IndexName).Doc(partialEntity));
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
                .Index<TDocument>()
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
