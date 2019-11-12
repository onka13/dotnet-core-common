using CoreCommon.Data.EntityFrameworkBase.Managers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using CoreCommon.Data.Domain.Entitites;
using CoreCommon.Data.Domain.Business;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public class EntityFrameworkBaseRepository<T, T2> where T : class, IEntityBase, new() where T2 : DbContext
    {
        public string RefId { get; private set; }

        public DbContextManager Manager { get; set; }

        public EntityFrameworkBaseRepository()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX EFBaseRepo " + typeof(T).Name + " " + RefId);
        }

        public void SetRefId(string _ref)
        {
            System.Diagnostics.Debug.WriteLine("XXX EFBaseRepo REF CHANGED " + typeof(T).Name + " " + RefId + "=>" + _ref);
            RefId = _ref;
        }

        public string GetRefId()
        {
            return RefId;
        }

        public DbContext GetDbContext()
        {
            return Manager.GetDbContext<T2>(RefId);
        }

        public DbSet<T> GetDbSet()
        {
            return GetDbContext().Set<T>();
        }

        public int Save()
        {
            return Manager.Save<T2>(RefId);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return GetDbSet().AsEnumerable();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = GetDbSet().Where(predicate).AsEnumerable();
            return query;
        }

        public IEnumerable<T> FindAndIncludeBy<TProp>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProp>>[] include)
        {
            var inc = GetDbSet().Include(include[0]);
            for (int i = 1; i < include.Length; i++)
            {
                inc = inc.Include(include[i]);
            }
            return inc.Where(predicate).AsEnumerable();
        }

        public int DeleteBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = GetDbSet().Where(predicate);
            GetDbSet().RemoveRange(query);
            return Save();
        }

        public T GetBy(Expression<Func<T, bool>> predicate)
        {
            return GetDbSet().FirstOrDefault(predicate);
        }

        public virtual T Add(T entity)
        {
            GetDbSet().Add(entity);
            Save();
            return entity;
        }

        public virtual int Delete(T entity)
        {
            return SetState(entity, EntityState.Deleted);
        }

        public virtual int Edit(T entity)
        {
            return SetState(entity, EntityState.Modified);
        }

        public virtual int EditOnly(T entity, params Expression<Func<T, object>>[] properties)
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

        public virtual int EditBy(T entity, params string[] properties)
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

        public virtual int BulkEditBy(List<T> entities, params string[] properties)
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

        public List<object> SkipTake(IEnumerable<object> result, int skip, int take, out long total)
        {
            if (take > 0)
            {
                total = result.Count();
                result = result.Skip(skip).Take(take);
            }
            else
            {
                total = 0;
            }
            return result.Select(x => (object)x).ToList();
        }

        protected int SetState(T entity, EntityState state)
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
            return typeof(T).Name.Replace("Entity", "");
        }

        public int BulkInsert(List<T> entityList)
        {
            if (entityList.Count == 0) return 0;

            GetDbSet().AddRange(entityList);
            return Save();
        }

        public int BulkUpdate(List<T> entityList)
        {
            if (entityList.Count == 0) return 0;

            foreach (var entity in entityList)
            {
                SetStateOnly(entity, EntityState.Modified);
            }
            return Save();
        }

        public int BulkDelete(List<T> entityList)
        {
            if (entityList.Count == 0) return 0;

            foreach (var entity in entityList)
            {
                SetStateOnly(entity, EntityState.Deleted);
            }
            return Save();
        }

        public int BulkUpdateOnly(List<T> entityList, params Expression<Func<T, object>>[] properties)
        {
            if (entityList.Count == 0) return 0;
            if (properties == null || properties.Length == 0)
            {
                return BulkUpdate(entityList);
            }

            foreach (var entity in entityList)
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
            Manager.BeginTransaction<T2>(newRefId ?? RefId);
        }

        public void CommitTransaction(string newRefId = null)
        {
            Manager.CommitTransaction<T2>(newRefId ?? RefId);
        }

        public void RollbackTransaction(string newRefId = null)
        {
            Manager.RollbackTransaction<T2>(newRefId ?? RefId);
        }

        public bool HasTransaction(string newRefId = null)
        {
            return Manager.HasTransaction<T2>(newRefId ?? RefId);
        }
    }
}
