using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Entitites;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCommon.Data.ElasticSearch.Base
{
    public class ElasticSearchRepositoryBase<T, T2> : IElasticRepositoryBase<T, T2> where T : class, IElasticSearchEntity<T2>
    {
        public IElasticClient ElasticClient { get; set; }

        public IEnumerable<T> GetAll()
        {
            return ElasticClient.Search<T>(s => s.MatchAll()).Documents.ToList();
        }

        public T GetBy(T2 id)
        {
            return ElasticClient.Get<T>(id.ToString()).Source;
        }

        public bool Delete(T2 id)
        {
            var deleteResponse = ElasticClient.Delete(new DocumentPath<T>(id.ToString()));
            return deleteResponse.Result == Result.Deleted;
        }

        public bool Edit(T entity)
        {
            var updateResponse = ElasticClient.Update<T, T>(new UpdateDescriptor<T, T>(entity));
            return updateResponse.Result == Result.Updated;
        }

        public T Add(T entity)
        {
            var indexResponse = ElasticClient.IndexDocument(entity);
            if (indexResponse.Result == Result.Created)
            {
                return null;
            }
            entity.Id = (T2)Convert.ChangeType(indexResponse.Id, typeof(T2));
            return entity;
        }
    }
}
