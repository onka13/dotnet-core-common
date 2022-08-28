using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreCommon.Data.EntityFrameworkBase.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CoreCommon.Data.Domain.Business;
using CoreCommon.Data.Domain.Entitites;
using CoreCommon.Data.Domain.Models;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    /// <summary>
    /// EF base repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity.</typeparam>
    /// <typeparam name="TDbContext">DbContext.</typeparam>
    public class EntityFrameworkBaseRepository<TEntity, TDbContext>
        : RepositoryBase<TEntity>, IEntityFrameworkBaseRepository<TEntity>
        where TEntity : class, IEntityBase, new()
        where TDbContext : DbContext
    {
        public EntityFrameworkBaseRepository()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX EFBaseRepo " + typeof(TEntity).Name + " " + RefId);
        }

        public string RefId { get; private set; }

        public DbContextManager Manager { get; set; }

        public void SetRefId(string refId)
        {
            RefId = refId;
        }

        public string GetRefId()
        {
            return RefId;
        }

        public DbContext GetDbContext()
        {
            return Manager.GetDbContext<TDbContext>(RefId);
        }

        public DbSet<TEntity> GetDbSet()
        {
            return GetDbContext().Set<TEntity>();
        }

        public async Task<int> Save(bool throwErrorIfNoResult = false)
        {
            var result = await Manager.Save<TDbContext>(RefId);
            if (throwErrorIfNoResult && result == 0)
            {
                throw new AppException(ServiceResultCode.NoRowsAffected);
            }

            return result;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return GetDbSet();
        }

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return await Any(predicate);
        }
        
        public async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetDbSet().Where(predicate).AnyAsync();
        }

        public async Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return await FindBy(predicate, false);
        }

        public async Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
            {
                return await GetDbSet().Where(predicate).ToListAsync();
            }

            return await GetRelationQueryable().Where(predicate).ToListAsync();
        }

        public async Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take)
        {
            return await FindBy(predicate, skip, take, false);
        }

        public async Task<List<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take, bool includeRelations)
        {
            if (!includeRelations)
            {
                return await GetDbSet().Where(predicate).Skip(skip).Take(take).ToListAsync();
            }

            return await GetRelationQueryable().Where(predicate).Skip(skip).Take(take).ToListAsync();
        }

        public IQueryable<TEntity> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include)
        {
            var inc = GetDbSet().Include(include[0]);
            for (int i = 1; i < include.Length; i++)
            {
                inc = inc.Include(include[i]);
            }

            return inc.Where(predicate);
        }

        public async Task<int> DeleteBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = GetDbSet().Where(predicate);
            GetDbSet().RemoveRange(query);
            return await Save();
        }

        public async Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetBy(predicate, false);
        }

        public async Task<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
            {
                return await GetDbSet().FirstOrDefaultAsync(predicate);
            }

            return await GetRelationQueryable().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            GetDbSet().Add(entity);
            await Save(true);
            return entity;
        }

        public virtual async Task<int> Delete(TEntity entity)
        {
            return await SetState(entity, EntityState.Deleted);
        }

        public virtual async Task<int> Update(TEntity entity)
        {
            int result;

            // We are getting an error when we try use setState for nested objects.
            // TODO: Cosmos bug should be fixed in next releases.
            if (GetDbContext().Database.IsCosmos())
            {
                GetDbSet().Update(entity);
                result = await Save();
            }
            else
            {
                result = await SetState(entity, EntityState.Modified);
            }

            if (result == 0)
            {
                throw new AppException(ServiceResultCode.NoRowsAffected);
            }

            return result;
        }

        public virtual async Task<int> UpdateOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            if (GetDbContext().Database.IsCosmos() || properties == null || properties.Length == 0)
            {
                return await Update(entity);
            }

            var entry = AttachOnly(entity, EntityState.Modified);
            foreach (var property in properties)
            {
                entry.Property(property).IsModified = true;
            }

            return await Save(true);
        }

        public virtual async Task<int> UpdateExcept(TEntity entity, params Expression<Func<TEntity, object>>[] exclude)
        {
            if (GetDbContext().Database.IsCosmos() || exclude == null || exclude.Length == 0)
            {
                return await Update(entity);
            }

            var entry = AttachOnly(entity, EntityState.Modified);
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    continue;
                }

                property.IsModified = true;
            }

            foreach (var property in exclude)
            {
                entry.Property(property).IsModified = false;
            }

            return await Save(true);
        }

        public string GetTableName()
        {
            return typeof(TEntity).Name.Replace("Entity", string.Empty);
        }

        public async Task<int> BulkInsert(List<TEntity> entities)
        {
            if (entities.Count == 0)
            {
                return 0;
            }

            GetDbSet().AddRange(entities);
            return await Save(true);
        }

        public async Task<int> BulkUpdate(List<TEntity> entities)
        {
            if (entities.Count == 0)
            {
                return 0;
            }

            if (GetDbContext().Database.IsCosmos())
            {
                foreach (var entity in entities)
                {
                    GetDbSet().Update(entity);
                }
            }
            else
            {
                foreach (var entity in entities)
                {
                    SetStateOnly(entity, EntityState.Modified);
                }
            }

            return await Save(true);
        }

        public async Task<int> BulkDelete(List<TEntity> entities)
        {
            if (entities.Count == 0)
            {
                return 0;
            }

            foreach (var entity in entities)
            {
                SetStateOnly(entity, EntityState.Deleted);
            }

            return await Save();
        }

        public async Task<int> BulkUpdateOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties)
        {
            if (entities.Count == 0)
            {
                return 0;
            }

            if (properties == null || properties.Length == 0)
            {
                return await BulkUpdate(entities);
            }

            foreach (var entity in entities)
            {
                var entry = AttachOnly(entity, EntityState.Modified);
                foreach (var property in properties)
                {
                    entry.Property(property).IsModified = true;
                }
            }

            return await Save(true);
        }

        public virtual async Task<int> Execute(string query, params object[] parameters)
        {
            return await GetDbContext().Database.ExecuteSqlRawAsync(query, parameters);
        }

        /// <summary>
        /// <inheritdoc cref="RelationalQueryableExtensions.FromSqlRaw"/>
        /// COSMOS:
        /// <inheritdoc cref="CosmosQueryableExtensions.FromSqlRaw"/>.
        /// </summary>
        public virtual IQueryable<TEntity> ExecuteQuery(string query, params object[] parameters)
        {
            if (GetDbContext().Database.IsCosmos())
            {
                return CosmosQueryableExtensions.FromSqlRaw(GetDbSet(), query, parameters);
            }

            return RelationalQueryableExtensions.FromSqlRaw(GetDbSet(), query, parameters);
        }

        public async Task<T1> DoTransactionIfNot<T1>(Func<string, T1> func)
        {
            if (!HasTransaction())
            {
                return await DoTransaction(func);
            }

            return func(GetRefId());
        }

        public async Task<T1> DoTransaction<T1>(Func<string, T1> func)
        {
            var transactionRef = Guid.NewGuid().ToString();
            await BeginTransaction(transactionRef);
            try
            {
                // GetDbContext().Database.UseTransaction(transaction.GetDbTransaction());
                var result = func(transactionRef);
                await CommitTransaction(transactionRef);
                return result;
            }
            catch
            {
                await RollbackTransaction(transactionRef);
                throw;
            }
        }

        public async Task BeginTransaction(string newRefId = null)
        {
            if (newRefId != null)
            {
                SetRefId(newRefId);
            }

            await Manager.BeginTransaction<TDbContext>(newRefId ?? RefId);
        }

        public async Task CommitTransaction(string newRefId = null)
        {
            await Manager.CommitTransaction<TDbContext>(newRefId ?? RefId);
        }

        public async Task RollbackTransaction(string newRefId = null)
        {
            await Manager.RollbackTransaction<TDbContext>(newRefId ?? RefId);
        }

        public bool HasTransaction(string newRefId = null)
        {
            return Manager.HasTransaction<TDbContext>(newRefId ?? RefId);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return GetDbSet();
        }

        public virtual IQueryable<TEntity> GetRelationQueryable()
        {
            return GetDbSet();
        }

        protected async Task<int> SetState(TEntity entity, EntityState state, bool throwErrorIfNoResult = false)
        {
            SetStateOnly(entity, state);
            return await Save(throwErrorIfNoResult);
        }

        protected EntityEntry<T1> AttachOnly<T1>(T1 entity, EntityState state)
            where T1 : class, IEntityBase, new()
        {
            var entry = GetDbContext().Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                entry.State = state;
                GetDbContext().Set<T1>().Attach(entity);
            }

            return entry;
        }

        protected EntityEntry<T1> SetStateOnly<T1>(T1 entity, EntityState state)
            where T1 : class, IEntityBase, new()
        {
            var entry = AttachOnly(entity, state);
            entry.State = state;
            return entry;
        }
    }
}
