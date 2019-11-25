using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Entitites;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCommon.Data.ElasticSearch.Base
{
    /// <summary>
    /// Elastic search base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class ElasticSearchRepositoryBase<TEntity, TPrimaryKey> : IElasticRepositoryBase<TEntity, TPrimaryKey> where TEntity : class, IElasticSearchEntity<TPrimaryKey>
    {
        public IElasticClient ElasticClient { get; set; }

        public IEnumerable<TEntity> GetAll()
        {
            return ElasticClient.Search<TEntity>(s => s.MatchAll()).Documents.ToList();
        }

        public TEntity GetBy(TPrimaryKey id)
        {
            return ElasticClient.Get<TEntity>(id.ToString()).Source;
        }

        public bool Delete(TPrimaryKey id)
        {
            var deleteResponse = ElasticClient.Delete(new DocumentPath<TEntity>(id.ToString()));
            return deleteResponse.Result == Result.Deleted;
        }

        public bool Edit(TEntity entity)
        {
            var updateResponse = ElasticClient.Update<TEntity, TEntity>(new UpdateDescriptor<TEntity, TEntity>(entity));
            return updateResponse.Result == Result.Updated;
        }

        public TEntity Add(TEntity entity)
        {
            var indexResponse = ElasticClient.IndexDocument(entity);
            if (indexResponse.Result == Result.Created)
            {
                return null;
            }
            entity.Id = (TPrimaryKey)Convert.ChangeType(indexResponse.Id, typeof(TPrimaryKey));
            return entity;
        }
    }
}
