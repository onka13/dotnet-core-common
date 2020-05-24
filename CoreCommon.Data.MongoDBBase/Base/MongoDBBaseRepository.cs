using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Config;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace CoreCommon.Data.MongoDBBase.Base
{
    /// <summary>
    /// MongoDB base repository
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public abstract class MongoDBBaseRepository<TDocument> 
        : RepositoryBase<TDocument>, IMongoDBBaseRepository<TDocument> 
        where TDocument : class, IMongoDBBaseEntity<TDocument>
    {
        private readonly IMongoCollection<TDocument> collection;
        public string CollectionName { get; }

        /// <summary>
        /// Name of the context
        /// </summary>
        public abstract string ConnectionName { get; }
        public IConfiguration Configuration { get; set; }

        public MongoDBBaseRepository()
        {
            var client = new MongoClient(Configuration[ConnectionName + ":ConnectionString"]);
            var database = client.GetDatabase(Configuration[ConnectionName + ":DatabaseName"]);
            var collectionAttribute = (CollectionAttribute)typeof(TDocument).GetCustomAttributes(typeof(CollectionAttribute), false).FirstOrDefault();
            CollectionName = collectionAttribute.Name;
            collection = database.GetCollection<TDocument>(CollectionName);
        }

        public IQueryable<TDocument> GetQueryable()
        {
            return collection.AsQueryable();
        }

        public IEnumerable<TDocument> GetAll()
        {
            return collection.Find(x => true).ToList();
        }

        public TDocument Get(Expression<Func<TDocument, bool>> filter)
        {
            return collection.Find(filter).FirstOrDefault();
        }

        public TDocument Add(TDocument entity)
        {
            collection.InsertOne(entity);
            return entity;
        }

        public IEnumerable<TDocument> FindBy(Expression<Func<TDocument, bool>> predicate)
        {
            return collection.Find(predicate).ToList();
        }

        public TDocument GetBy(Expression<Func<TDocument, bool>> predicate)
        {
            return collection.Find(predicate).FirstOrDefault();
        }

        public int Delete(TDocument entity)
        {
            return DeleteBy(entity.PrimaryPredicate);
        }

        public int DeleteBy(Expression<Func<TDocument, bool>> predicate)
        {
            var result = collection.DeleteOne(predicate);
            return result.IsAcknowledged ? (int)result.DeletedCount : 0;
        }

        public int Edit(TDocument entity)
        {
            var result = collection.ReplaceOne(entity.PrimaryPredicate, entity);
            return result.IsAcknowledged ? (int)result.ModifiedCount : 0;
        }

        public int EditOnly(TDocument entity, params Expression<Func<TDocument, object>>[] properties)
        {
            //var filter = Builders<TDocument>.Filter.In(x => x.PrimaryPredicate, entities.Select(x => x.PrimaryPredicate));
            //var update = Builders<Profile>.Update.Set(x => x.IsDeleted, true);
            //await collection.UpdateManyAsync(filter, update);

            if (properties.Length == 0) return 0;

            var def = new UpdateDefinitionBuilder<TDocument>().Set(properties[0], properties[0].Compile().Invoke(entity));
            for (int i = 1; i < properties.Length; i++)
            {
                def = def.Set(properties[i], properties[i].Compile().Invoke(entity));
            }
            var result = collection.UpdateOne(entity.PrimaryPredicate, def);
            return result.IsAcknowledged ? (int)result.ModifiedCount : 0;
        }

        public int BulkInsert(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = collection.BulkWrite(entities.Select(x => new InsertOneModel<TDocument>(x)));
            return (int)res.InsertedCount;
        }

        public int BulkUpdate(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = collection.BulkWrite(entities.Select(x => new ReplaceOneModel<TDocument>(x.PrimaryPredicate, x) { IsUpsert = true }));
            return (int)res.ModifiedCount;
        }

        public int BulkDelete(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = collection.BulkWrite(entities.Select(x => new DeleteOneModel<TDocument>(x.PrimaryPredicate)));
            return (int)res.DeletedCount;
        }

        public int BulkUpdateOnly(List<TDocument> entities, params Expression<Func<TDocument, object>>[] properties)
        {
            if (properties.Length == 0) return 0;

            var res = collection.BulkWrite(entities.Select(entity =>
            {
                var def = new UpdateDefinitionBuilder<TDocument>().Set(properties[0], properties[0].Compile().Invoke(entity));
                for (int i = 1; i < properties.Length; i++)
                {
                    def = def.Set(properties[i], properties[i].Compile().Invoke(entity));
                }
                return new UpdateManyModel<TDocument>(entity.PrimaryPredicate, def) { IsUpsert = true };
            }));
            return (int)(res.ModifiedCount + res.InsertedCount);
        }

        public IEnumerable<TDocument> FindAndIncludeBy<TProp>(Expression<Func<TDocument, bool>> predicate, params Expression<Func<TDocument, TProp>>[] include)
        {
            throw new NotImplementedException();
        }
    }
}
