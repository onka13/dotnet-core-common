using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace CoreCommon.Data.EntityFrameworkBase.Managers
{
    /// <summary>
    /// Manages all instance of DbContexts.
    /// </summary>
    public class DbContextManager
    {
        /// <summary>
        /// Autofac component to resolve dependencies
        /// </summary>
        public IComponentContext ComponentContext { get; set; }

        /// <summary>
        /// Contains dbcontexts
        /// </summary>
        private Dictionary<string, DbContext> _initializedDbContexts;

        /// <summary>
        /// Contains transactions
        /// </summary>
        private Dictionary<string, IDbContextTransaction> _transactions;

        public DbContextManager()
        {
            System.Diagnostics.Debug.WriteLine("XXX DbContextManager Constructor");
            _initializedDbContexts = new Dictionary<string, DbContext>();
            _transactions = new Dictionary<string, IDbContextTransaction>();
        }

        private string GetKey<T>(string refId)
        {
            return typeof(T).FullName + "." + refId;
        }

        public DbContext GetDbContext<T>(string refId) where T : DbContext
        {
            var key = GetKey<T>(refId);
            if (!_initializedDbContexts.ContainsKey(key))
            {
                System.Diagnostics.Debug.WriteLine("XXX DbContextManager resolved " + refId + " " + typeof(T).Name);
                var dbContext = ComponentContext.Resolve<T>();
                dbContext.ChangeTracker.LazyLoadingEnabled = false;
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                _initializedDbContexts.Add(key, dbContext);
            }
            return _initializedDbContexts[key];
        }

        private void RemoveDbContext<T>(string refId)
        {
            var key = GetKey<T>(refId);
            _initializedDbContexts.Remove(key);
        }

        public int Save<T>(string refId) where T : DbContext
        {
            var res = GetDbContext<T>(refId).SaveChanges();
            if (!HasTransaction<T>(refId))
            {
                GetDbContext<T>(refId).Dispose();
                RemoveDbContext<T>(refId);
            }
            return res;
        }

        public bool HasTransaction<T>(string refId)
        {
            return _transactions.ContainsKey(GetKey<T>(refId));
        }

        public void BeginTransaction<T>(string refId) where T : DbContext
        {
            var key = GetKey<T>(refId);
            var transaction = GetDbContext<T>(refId).Database.BeginTransaction();
            _transactions.Add(key, transaction);
        }

        public void CommitTransaction<T>(string refId) where T : DbContext
        {
            var key = GetKey<T>(refId);
            _transactions[key].Commit();
            _transactions[key].Dispose();
            _transactions.Remove(key);
            GetDbContext<T>(refId).Dispose();
            RemoveDbContext<T>(refId);
        }

        public void RollbackTransaction<T>(string refId) where T : DbContext
        {
            var key = GetKey<T>(refId);
            _transactions[key].Rollback();
            _transactions[key].Dispose();
            _transactions.Remove(key);
            GetDbContext<T>(refId).Dispose();
            RemoveDbContext<T>(refId);
        }
    }
}
