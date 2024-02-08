using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    /// <summary>
    /// DbContext Base class.
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        public DbContextBase()
        {
            // System.Diagnostics.Debug.WriteLine("XXX DbContextBase Cons " + Name + " " + RefId);
            RefId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Db Connection.
        /// </summary>
        public System.Data.Common.DbConnection Connection { get; set; }

        /// <summary>
        /// Gets name of the context.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets reference Id.
        /// </summary>
        public string RefId { get; set; }

        /// <summary>
        /// Gets or sets autowired property for getting appsettings.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        public string Provider { get; set; }

        public string ConnectionString { get; set; }

        public string GetConfiguration(string key)
        {
            var value = Configuration[$"{Name}:{key}"];

            if (!string.IsNullOrEmpty(Configuration[$"{Name}_{key}"]))
            {
                value = Configuration[$"{Name}_{key}"];
            }

            return value;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Configure db context.
        /// </summary>
        /// <param name="optionsBuilder">Options Builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // optionsBuilder.UseLazyLoadingProxies(false);
            if (Configuration != null)
            {
                if (string.IsNullOrEmpty(Provider))
                {
                    Provider = GetConfiguration("Provider");
                }

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    ConnectionString = GetConfiguration("ConnectionString");
                }
            }

            Provider = Provider?.ToLower() ?? string.Empty;

            var dataPath = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
            ConnectionString = ConnectionString?.Replace("[DataDirectory]", dataPath);

            if (Provider.Contains("mysql"))
            {
                if (Connection != null)
                {
                    optionsBuilder.UseMySQL(Connection);
                }
                else
                {
                    optionsBuilder.UseMySQL(ConnectionString);
                }
            }
            else if (Provider.Contains("postgres"))
            {
                if (Connection != null)
                {
                    optionsBuilder.UseNpgsql(Connection);
                }
                else
                {
                    optionsBuilder.UseNpgsql(ConnectionString);
                }
            }
            else if (Provider.Contains("cosmos"))
            {
                var endPoint = GetConfiguration("EndPoint") ?? GetConfiguration("DatabaseUrl");
                var accountKey = GetConfiguration("AccountKey") ?? GetConfiguration("AuthKey");
                var databaseName = GetConfiguration("DatabaseName");

                optionsBuilder.UseCosmos(endPoint, accountKey, databaseName);
            }
            else
            {
                if (Connection != null)
                {
                    optionsBuilder.UseSqlServer(Connection);
                }
                else
                {
                    optionsBuilder.UseSqlServer(ConnectionString);
                }
            }

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
