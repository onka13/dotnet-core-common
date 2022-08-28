using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Config;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System.Threading.Tasks;
using CoreCommon.Data.Domain.Business.Queryable;
using CoreCommon.Data.Domain.Business.Crud;
using CoreCommon.Data.Domain.Models;
using Microsoft.EntityFrameworkCore;

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

        public IQueryable<TDocument> GetAll()
        {
            return GetQueryable();
        }

        public async Task<TDocument> Add(TDocument entity)
        {
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<List<TDocument>> FindBy(Expression<Func<TDocument, bool>> predicate)
        {
            return await FindBy(predicate, false);
        }

        public async Task<List<TDocument>> FindBy(Expression<Func<TDocument, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return await GetQueryable().Where(predicate).ToListAsync();
            return await GetRelationAggregate().Match(predicate).ToListAsync();
        }

        public async Task<List<TDocument>> FindBy(Expression<Func<TDocument, bool>> predicate, int skip, int take)
        {
            return await FindBy(predicate, skip, take, false);
        }

        public async Task<List<TDocument>> FindBy(Expression<Func<TDocument, bool>> predicate, int skip, int take, bool includeRelations)
        {
            if (!includeRelations)
                return await GetQueryable().Where(predicate).Skip(skip).Take(take).ToListAsync();
            return await GetRelationAggregate().Match(predicate).Skip(skip).Limit(take).ToListAsync();
        }

        public async Task<TDocument> GetBy(Expression<Func<TDocument, bool>> predicate)
        {
            return await GetBy(predicate, false);
        }

        public async Task<TDocument> GetBy(Expression<Func<TDocument, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return await Collection.Find(predicate).FirstOrDefaultAsync();
            return await GetRelationAggregate().Match(predicate).FirstOrDefaultAsync();
        }

        public async Task<int> Delete(TDocument entity)
        {
            return await DeleteBy(entity.PrimaryPredicate());
        }

        public async Task<int> DeleteBy(Expression<Func<TDocument, bool>> predicate)
        {
            var result = await Collection.DeleteOneAsync(predicate);
            return result.IsAcknowledged ? (int)result.DeletedCount : 0;
        }

        public async Task<int> Update(TDocument entity)
        {
            var result = await Collection.ReplaceOneAsync(entity.PrimaryPredicate(), entity);
            return result.IsAcknowledged ? (int)result.MatchedCount : 0;
        }

        public async Task<int> UpdateOnly(TDocument entity, params Expression<Func<TDocument, object>>[] properties)
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
            var result = await Collection.UpdateOneAsync(entity.PrimaryPredicate(), def);
            return result.IsAcknowledged ? (int)result.MatchedCount : 0;
        }

        public async Task<int> BulkInsert(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = await Collection.BulkWriteAsync(entities.Select(x => new InsertOneModel<TDocument>(x)));
            return (int)res.InsertedCount;
        }

        public async Task<int> BulkUpdate(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = await Collection.BulkWriteAsync(entities.Select(x => new ReplaceOneModel<TDocument>(x.PrimaryPredicate(), x) { IsUpsert = true }));
            return (int)res.ModifiedCount;
        }

        public async Task<int> BulkDelete(List<TDocument> entities)
        {
            if (entities.Count == 0) return 0;
            var res = await Collection.BulkWriteAsync(entities.Select(x => new DeleteOneModel<TDocument>(x.PrimaryPredicate())));
            return (int)res.DeletedCount;
        }

        public async Task<int> BulkUpdateOnly(List<TDocument> entities, params Expression<Func<TDocument, object>>[] properties)
        {
            if (properties.Length == 0 || entities.Count == 0) return 0;

            var res = await Collection.BulkWriteAsync(entities.Select(entity =>
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

        public async Task<List<dynamic>> RawJsonQuery<T>(string json)
        {
            //var query = BsonSerializer.Deserialize<List<BsonDocument>>(json);
            //var res = Collection.Aggregate<BsonDocument>(query).ToList();
            var doc = BsonDocument.Parse(json.Trim());
            PipelineDefinition<T, dynamic> pipeline = new BsonDocument[]
            {
                doc
            };

            var res0 = await DbContext.GetCollection<T>().Aggregate(pipeline).ToListAsync();

            return res0;
        }

        public async Task<SearchResult> SkipTake<T>(IAggregateFluent<T> query, int skip, int take)
        {
            var result = new SearchResult();
            if (take > 0)
            {
                //var sortStage = result.Stages.FirstOrDefault(x => x.OperatorName == "$sort");
                //var stages = result.Stages.Where(x => x.OperatorName != "$sort");                
                //var resultForTotal = new PipelineStagePipelineDefinition<TDocument, T>(stages);

                result.Total = query.Count().FirstOrDefault()?.Count ?? 0;
                query = query.Skip(skip).Limit(take);
            }
            else
            {
                result.Total = 0;
            }

            result.Items = (await query.ToListAsync()).Cast<object>().ToList();
            return result;
        }

        protected AggregateUnwindOptions<T> GetAggregateUnwindOptions<T>()
        {
            return new AggregateUnwindOptions<T>()
            {
                PreserveNullAndEmptyArrays = true
            };
        }

        public async Task<bool> Exists(Expression<Func<TDocument, bool>> predicate)
        {
            return await Any(predicate);
        }
        
        public async Task<bool> Any(Expression<Func<TDocument, bool>> predicate)
        {
            return await Collection.Find(predicate).AnyAsync();
        }

        Task<int> IQueryableRepositoryBase<TDocument>.UpdateExcept(TDocument entity, params Expression<Func<TDocument, object>>[] exclude)
        {
            throw new NotImplementedException();
        }
    }
}
