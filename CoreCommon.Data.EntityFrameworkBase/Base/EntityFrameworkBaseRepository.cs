using CoreCommon.Data.EntityFrameworkBase.Managers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using CoreCommon.Infra.Domain.Business;
using CoreCommon.Infra.Domain.Entitites;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    /// <summary>
    /// EF base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class EntityFrameworkBaseRepository<TEntity, TDbContext> 
        : RepositoryBase<TEntity>, IEntityFrameworkBaseRepository<TEntity> 
        where TEntity : class, IEntityBase, new() where TDbContext : DbContext
    {
        public string RefId { get; private set; }

        public DbContextManager Manager { get; set; }

        public EntityFrameworkBaseRepository()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX EFBaseRepo " + typeof(TEntity).Name + " " + RefId);
        }

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

        public int Save()
        {
            return Manager.Save<TDbContext>(RefId);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetDbSet().AsEnumerable();
        }

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return FindBy(predicate, false);
        }

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return GetDbSet().Where(predicate).AsEnumerable();
            return GetRelationQueryable().Where(predicate).AsEnumerable();
        }

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take)
        {
            return FindBy(predicate, skip, take, false);
        }

        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, int skip, int take, bool includeRelations)
        {
            if (!includeRelations)
                return GetDbSet().Where(predicate).Skip(skip).Take(take).AsEnumerable();
            return GetRelationQueryable().Where(predicate).Skip(skip).Take(take).AsEnumerable();
        }

        public IEnumerable<TEntity> FindAndIncludeBy<TProp>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, TProp>>[] include)
        {
            var inc = GetDbSet().Include(include[0]);
            for (int i = 1; i < include.Length; i++)
            {
                inc = inc.Include(include[i]);
            }
            return inc.Where(predicate).AsEnumerable();
        }

        public int DeleteBy(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> query = GetDbSet().Where(predicate);
            GetDbSet().RemoveRange(query);
            return Save();
        }

        public TEntity GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            return GetBy(predicate, false);
        }

        public TEntity GetBy(Expression<Func<TEntity, bool>> predicate, bool includeRelations)
        {
            if (!includeRelations)
                return GetDbSet().FirstOrDefault(predicate);
            return GetRelationQueryable().FirstOrDefault(predicate);
        }

        public virtual TEntity Add(TEntity entity)
        {
            GetDbSet().Add(entity);
            Save();
            return entity;
        }

        public virtual int Delete(TEntity entity)
        {
            return SetState(entity, EntityState.Deleted);
        }

        public virtual int Edit(TEntity entity)
        {
            return SetState(entity, EntityState.Modified);
        }

        public virtual int EditOnly(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            if (properties == null || properties.Length == 0)
            {
                return Edit(entity);
            }
            var entry = AttachOnly(entity, EntityState.Modified);
            foreach (var property in properties)
            {
                entry.Property(property).IsModified = true;
            }
            return Save();
        }

        public virtual int EditExcept(TEntity entity, params Expression<Func<TEntity, object>>[] exclude)
        {
            if (exclude == null || exclude.Length == 0)
            {
                return Edit(entity);
            }
            var entry = AttachOnly(entity, EntityState.Modified);
            foreach (var property in entry.Properties)
            {
                property.IsModified = true;
            }
            foreach (var property in exclude)
            {
                entry.Property(property).IsModified = true;
            }
            return Save();
        }

        public virtual int EditBy(TEntity entity, params string[] properties)
        {
            if (properties == null || properties.Length == 0)
            {
                return Edit(entity);
            }
            //DbContext.Entry(entity.ErpMainAddressAddress).CurrentValues.SetValues(model.ErpMainAddressAddress);

            //disable detection of changes to improve performance
            //db.Configuration.AutoDetectChangesEnabled = false;
            var entry = AttachOnly(entity, EntityState.Modified);
            
            foreach (var property in properties)
            {
                entry.Property(property).IsModified = true;
            }
            // db.Configuration.ValidateOnSaveEnabled = false
            return Save();
            //Set().AddOrUpdate(entity);
            //return DbContext.SaveChanges();
            //return SetState(entity, EntityState.Modified);
        }

        public virtual int BulkEditBy(List<TEntity> entities, params string[] properties)
        {
            //disable detection of changes to improve performance
            //db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var entity in entities)
            {
                var entry = GetDbContext().Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    GetDbSet().Attach(entity);
                }
                foreach (var property in properties)
                {
                    entry.Property(property).IsModified = true;
                }
            }
            // db.Configuration.ValidateOnSaveEnabled = false
            return Save();
            //Set().AddOrUpdate(entity);
            //return DbContext.SaveChanges();
            //return SetState(entity, EntityState.Modified);
        }

        protected int SetState(TEntity entity, EntityState state)
        {
            SetStateOnly(entity, state);
            return Save();
        }

        protected EntityEntry<T1> AttachOnly<T1>(T1 entity, EntityState state) where T1 : class, IEntityBase, new()
        {
            var entry = GetDbContext().Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                entry.State = state;
                GetDbContext().Set<T1>().Attach(entity);
            }
            return entry;
        }

        protected EntityEntry<T1> SetStateOnly<T1>(T1 entity, EntityState state) where T1 : class, IEntityBase, new()
        {
            var entry = AttachOnly(entity, state);
            entry.State = state;
            return entry;
        }

        public string GetTableName()
        {
            return typeof(TEntity).Name.Replace("Entity", "");
        }

        public int BulkInsert(List<TEntity> entities)
        {
            if (entities.Count == 0) return 0;

            GetDbSet().AddRange(entities);
            return Save();
        }

        public int BulkUpdate(List<TEntity> entities)
        {
            if (entities.Count == 0) return 0;

            foreach (var entity in entities)
            {
                SetStateOnly(entity, EntityState.Modified);
            }
            return Save();
        }

        public int BulkDelete(List<TEntity> entities)
        {
            if (entities.Count == 0) return 0;

            foreach (var entity in entities)
            {
                SetStateOnly(entity, EntityState.Deleted);
            }
            return Save();
        }

        public int BulkUpdateOnly(List<TEntity> entities, params Expression<Func<TEntity, object>>[] properties)
        {
            if (entities.Count == 0) return 0;
            if (properties == null || properties.Length == 0)
            {
                return BulkUpdate(entities);
            }

            foreach (var entity in entities)
            {
                var entry = SetStateOnly(entity, EntityState.Modified);
                foreach (var property in properties)
                {
                    entry.Property(property).IsModified = true;
                }
            }
            return Save();
        }

        public virtual int Execute(string query, params object[] parameters)
        {
            return GetDbContext().Database.ExecuteSqlRaw(query, parameters);
        }

        public ServiceResult<T1> DoTransactionIfNot<T1>(Func<string, T1> func)
        {
            var response = ServiceResult<T1>.Instance.ErrorResult();
            if (!HasTransaction())
            {
                return DoTransaction(func);
            }
            var result = func(GetRefId());
            return response.SuccessResult(result);
        }

        public ServiceResult<T1> DoTransaction<T1>(Func<string, T1> func)
        {
            var response = ServiceResult<T1>.Instance.ErrorResult();
            var transactionRef = Guid.NewGuid().ToString();
            BeginTransaction(transactionRef);
            try
            {
                //GetDbContext().Database.UseTransaction(transaction.GetDbTransaction());
                var result = func(transactionRef);
                CommitTransaction(transactionRef);
                return response.SuccessResult(result);
            }
            catch (Exception ex)
            {
                RollbackTransaction(transactionRef);
                // TODO: log error
                response = response.ErrorResult(ServiceResultCode.ServerError, ex);
            }

            return response;
        }

        public void BeginTransaction(string newRefId = null)
        {
            if (newRefId != null) SetRefId(newRefId);
            Manager.BeginTransaction<TDbContext>(newRefId ?? RefId);
        }

        public void CommitTransaction(string newRefId = null)
        {
            Manager.CommitTransaction<TDbContext>(newRefId ?? RefId);
        }

        public void RollbackTransaction(string newRefId = null)
        {
            Manager.RollbackTransaction<TDbContext>(newRefId ?? RefId);
        }

        public bool HasTransaction(string newRefId = null)
        {
            return Manager.HasTransaction<TDbContext>(newRefId ?? RefId);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return GetDbSet().AsQueryable();
        }

        public virtual IQueryable<TEntity> GetRelationQueryable()
        {
            return GetDbSet();
        }
    }
}
