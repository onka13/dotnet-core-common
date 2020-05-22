using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Entitites;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreCommon.Data.ElasticSearch.Base
{
    /// <summary>
    /// Elastic search base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class ElasticSearchRepositoryBase<TEntity, TPrimaryKey> 
        : RepositoryBase<TEntity>, IElasticSearchBaseRepository<TEntity> 
        where TEntity : class, IElasticSearchEntity<TPrimaryKey>
    {
        public IElasticClient ElasticClient { get; set; }

        public TEntity Add(TEntity entity)
        {
            var indexResponse = ElasticClient.IndexDocument(entity);
            if (indexResponse.Result != Result.Created)
            {
                return null;
            }
            entity.Id = (TPrimaryKey)Convert.ChangeType(indexResponse.Id, typeof(TPrimaryKey));
            return entity;
        }

        public int BulkDelete(List<TEntity> entities)
        {
            var result = ElasticClient.Bulk(b => b.Index<TEntity>().DeleteMany(entities));
            return !result.Errors ? entities.Count : 0;
        }

        public int BulkInsert(List<TEntity> entities)
        {
            //var result = ElasticClient.IndexMany(entities);
            var result = ElasticClient.Bulk(b => b.Index<TEntity>().IndexMany(entities));
            if (result.Errors)
            {
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    // log, itemWithError.Id, itemWithError.Error
                }
            }
            return entities.Count;
        }

        public int BulkUpdate(List<TEntity> entities)
        {
            var result = ElasticClient.Bulk(b => b.Index<TEntity>().UpdateMany(entities, (bulkUpdateDescriptor, entity) => bulkUpdateDescriptor.Index<TEntity>().Doc(entity)));
            return !result.Errors ? entities.Count : 0;
        }

        public int Delete(TEntity entity)
        {
            var deleteResponse = ElasticClient.Delete<TEntity>(entity);
            return deleteResponse.Result == Result.Deleted ? 1 : 0;
        }

        public int Edit(TEntity entity)
        {
            var updateResponse = ElasticClient.Update(new UpdateDescriptor<TEntity, TEntity>(entity));
            return updateResponse.Result == Result.Updated ? 1 : 0;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return ElasticClient.Search<TEntity>(s => s.MatchAll()).Documents.ToList();
        }

        public int DeleteBy(TPrimaryKey id)
        {
            var deleteResponse = ElasticClient.Delete<TEntity>(id.ToString());
            return deleteResponse.Result == Result.Deleted ? 1 : 0;
        }

        public TEntity GetBy(TPrimaryKey id)
        {
            return ElasticClient.Get<TEntity>(id.ToString()).Source;
        }
    }
}
