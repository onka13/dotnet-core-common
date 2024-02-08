using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoreCommon.Data.EntityFrameworkBase.Managers
{
    /// <summary>
    /// Manages all instance of DbContexts.
    /// </summary>
    public class DbContextManager
    {
        /// <summary>
        /// Contains dbcontexts.
        /// </summary>
        private Dictionary<string, DbContext> initializedDbContexts;

        /// <summary>
        /// Contains transactions.
        /// </summary>
        private Dictionary<string, IDbContextTransaction> transactions;

        public DbContextManager(IComponentContext componentContext)
        {
            ComponentContext = componentContext;
            System.Diagnostics.Debug.WriteLine("XXX DbContextManager Constructor");
            initializedDbContexts = new Dictionary<string, DbContext>();
            transactions = new Dictionary<string, IDbContextTransaction>();
        }

        /// <summary>
        /// Autofac component to resolve dependencies.
        /// </summary>
        public IComponentContext ComponentContext { get; set; }

        public DbContext GetDbContext<T>(string refId)
            where T : DbContext
        {
            var key = GetKey<T>(refId);
            if (!initializedDbContexts.ContainsKey(key))
            {
                System.Diagnostics.Debug.WriteLine("XXX DbContextManager resolved " + refId + " " + typeof(T).Name);
                var dbContext = ComponentContext.Resolve<T>();
                dbContext.ChangeTracker.LazyLoadingEnabled = false;
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                initializedDbContexts.Add(key, dbContext);
            }

            return initializedDbContexts[key];
        }

        public virtual async Task<int> Save<T>(string refId)
            where T : DbContext
        {
            try
            {
                var res = await GetDbContext<T>(refId).SaveChangesAsync();
                if (!HasTransaction<T>(refId))
                {
                    await RemoveDbContext<T>(refId);
                }

                return res;
            }
            catch
            {
                await RemoveDbContext<T>(refId);
                throw;
            }
        }

        public bool HasTransaction<T>(string refId)
        {
            return transactions.ContainsKey(GetKey<T>(refId));
        }

        public async Task BeginTransaction<T>(string refId)
            where T : DbContext
        {
            var key = GetKey<T>(refId);
            var transaction = await GetDbContext<T>(refId).Database.BeginTransactionAsync();
            transactions.Add(key, transaction);
        }

        public async Task CommitTransaction<T>(string refId)
            where T : DbContext
        {
            var key = GetKey<T>(refId);
            await transactions[key].CommitAsync();
            transactions[key].Dispose();
            transactions.Remove(key);
            await RemoveDbContext<T>(refId);
        }

        public async Task RollbackTransaction<T>(string refId)
            where T : DbContext
        {
            var key = GetKey<T>(refId);
            await transactions[key].RollbackAsync();
            transactions[key].Dispose();
            transactions.Remove(key);
            await RemoveDbContext<T>(refId);
        }

        private string GetKey<T>(string refId)
        {
            return typeof(T).FullName + "." + refId;
        }

        private async Task RemoveDbContext<T>(string refId)
        {
            var key = GetKey<T>(refId);
            if (initializedDbContexts[key] == null)
            {
                return;
            }

            var context = initializedDbContexts[key];

            var undetachedEntriesCopy = context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
            await context.DisposeAsync();
            initializedDbContexts.Remove(key);
        }
    }
}
