using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
using CoreCommon.Infra.Domain.Business;

namespace CoreCommon.Data.MongoDBBase.Base
{
    /// <summary>
    /// MongoDB base repository
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public abstract class MongoDBBaseRepository<TDocument, TDbContext>
        : RepositoryBase<TDocument>, IMongoDBBaseRepository<TDocument>
        where TDocument : class, IMongoDBBaseEntity<TDocument> where TDbContext : MongoDbContextBase
    {
        public TDbContext DbContext { get; set; }

        protected IMongoCollection<TDocument> Collection
        {
            get
            {
                return DbContext.GetCollection<TDocument>();
            }
        }

        public IQueryable<TDocument> GetQueryable()
        {
            return Collection.AsQueryable();
        }

        public IEnumerable<TDocument> GetAll()
        {
            return Collection.Find(x => true).ToList();
        }

        public TDocument Add(TDocument entity)
        {
            Collection.InsertOne(entity);
            return entity;
        }

        public IEnumerable<TDocument> FindBy(Expression<Func<TDocument, bool>> predicate)
        {
            return FindBy(predicate, false);
        }

        public IEnumerable<TDocument> FindBy(Expression<Func<TDocument, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return Collection.Find(predicate).ToList();
            return GetRelationAggregate().Match(predicate).ToList();
        }

        public IEnumerable<TDocument> FindBy(Expression<Func<TDocument, bool>> predicate, int skip, int take)
        {
            return FindBy(predicate, skip, take, false);
        }

        public IEnumerable<TDocument> FindBy(Expression<Func<TDocument, bool>> predicate, int skip, int take, bool includeRelations)
        {
            if (!includeRelations)
                return Collection.Find(predicate).Skip(skip).Limit(take).ToEnumerable();
            return GetRelationAggregate().Match(predicate).Skip(skip).Limit(take).ToEnumerable();
        }

        public TDocument GetBy(Expression<Func<TDocument, bool>> predicate)
        {
            return GetBy(predicate, false);
        }

        public TDocument GetBy(Expression<Func<TDocument, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return Collection.Find(predicate).FirstOrDefault();
            return GetRelationAggregate().Match(predicate).FirstOrDefault();
        }

        public int Delete(TDocument entity)
        {
            return DeleteBy(entity.PrimaryPredicate());
        }

        public int DeleteBy(Expression<Func<TDocument, bool>> predicate)
        {
            var result = Collection.DeleteOne(predicate);
            return result.IsAcknowledged ? (int)result.DeletedCount : 0;
        }

        public int Edit(TDocument entity)
        {
            var result = Collection.ReplaceOne(entity.PrimaryPredicate(), entity);
            return result.IsAcknowledged ? (int)result.MatchedCount : 0;
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
            var result = Collection.UpdateOne(entity.PrimaryPredicate(), def);
            return result.IsAcknowledged ? (int)result.MatchedCount : 0;
        }

        public int BulkInsert(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = Collection.BulkWrite(entities.Select(x => new InsertOneModel<TDocument>(x)));
            return (int)res.InsertedCount;
        }

        public int BulkUpdate(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = Collection.BulkWrite(entities.Select(x => new ReplaceOneModel<TDocument>(x.PrimaryPredicate(), x) { IsUpsert = true }));
            return (int)res.ModifiedCount;
        }

        public int BulkDelete(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = Collection.BulkWrite(entities.Select(x => new DeleteOneModel<TDocument>(x.PrimaryPredicate())));
            return (int)res.DeletedCount;
        }

        public int BulkUpdateOnly(List<TDocument> entities, params Expression<Func<TDocument, object>>[] properties)
        {
            if (properties.Length == 0 || entities.Count == 0) return 0;

            var res = Collection.BulkWrite(entities.Select(entity =>
            {
                var def = new UpdateDefinitionBuilder<TDocument>().Set(properties[0], properties[0].Compile().Invoke(entity));
                for (int i = 1; i < properties.Length; i++)
                {
                    def = def.Set(properties[i], properties[i].Compile().Invoke(entity));
                }
                return new UpdateManyModel<TDocument>(entity.PrimaryPredicate(), def) { IsUpsert = true };
            }));
            return (int)(res.ModifiedCount + res.InsertedCount);
        }

        public virtual IAggregateFluent<TDocument> GetRelationAggregate()
        {
            return Collection.Aggregate();
        }

        public IEnumerable<TDocument> FindAndIncludeBy<TForeignDocument>(Expression<Func<TDocument, bool>> predicate,
                                                                                    Expression<Func<TDocument, object>> localField,
                                                                                    Expression<Func<TForeignDocument, object>> foreignField,
                                                                                    Expression<Func<TDocument, object>> bindField
                                                                                    )
        {
            var res = Collection.Aggregate();
            res = res.Lookup<TDocument, TForeignDocument, TDocument>(
                        DbContext.GetCollection<TForeignDocument>(),
                        localField,
                        foreignField,
                        bindField
                    ).Unwind(bindField).As<TDocument>();

            return res.Match(predicate).ToEnumerable();
        }

        public List<dynamic> RawJsonQuery<T>(string json)
        {
            //var query = BsonSerializer.Deserialize<List<BsonDocument>>(json);
            //var res = Collection.Aggregate<BsonDocument>(query).ToList();
            var doc = BsonDocument.Parse(json.Trim());
            PipelineDefinition<T, dynamic> pipeline = new BsonDocument[]
            {
                doc
            };

            var res0 = DbContext.GetCollection<T>().Aggregate(pipeline).ToList();

            return res0;
        }

        public List<object> SkipTake<T>(IAggregateFluent<T> result, int skip, int take, out long total)
        {
            if (take > 0)
            {
                //var sortStage = result.Stages.FirstOrDefault(x => x.OperatorName == "$sort");
                //var stages = result.Stages.Where(x => x.OperatorName != "$sort");                
                //var resultForTotal = new PipelineStagePipelineDefinition<TDocument, T>(stages);
                
                total = result.Count().FirstOrDefault()?.Count ?? 0;
                result = result.Skip(skip).Limit(take);
            }
            else
            {
                total = 0;
            }
            var list = result.ToList();

            return list.Cast<object>().ToList();
        }

        protected AggregateUnwindOptions<T> GetAggregateUnwindOptions<T>()
        {
            return new AggregateUnwindOptions<T>()
            {
                PreserveNullAndEmptyArrays = true
            };
        }

        public int EditExcept(TDocument entity, params Expression<Func<TDocument, object>>[] exclude)
        {
            throw new NotImplementedException();
        }
    }
}
