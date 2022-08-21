using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    /// <summary>
    /// DbContext Base class.
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        /// <summary>
        /// Name of the context
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Reference Id
        /// </summary>
        public string RefId { get; set; }

        /// <summary>
        /// Autowired property for getting appsettings
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Db Connection
        /// </summary>
        private System.Data.Common.DbConnection _connection;

        public string Provider { get; set; }
        public string ConnectionString { get; set; }

        public DbContextBase()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX DbContextBase Cons " + Name + " " + RefId);
        }

        public string GetConfiguration(string key)
        {
            var value = Configuration[$"{Name}:{key}"];

            if (!string.IsNullOrEmpty(Configuration[$"{Name}_{key}"]))
                value = Configuration[$"{Name}_{key}"];

            return value;
        }

        /// <summary>
        /// Configure db context
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies(false);
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

            Provider = Provider?.ToLower() ?? "";

            //System.Console.WriteLine("Provider " + Provider);
            //System.Console.WriteLine("ConnectionString " + ConnectionString);

            var dataPath = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
            ConnectionString = ConnectionString?.Replace("[DataDirectory]", dataPath);

            if (Provider.Contains("mysql"))
            {
                if (_connection != null)
                    optionsBuilder.UseMySQL(_connection);
                else
                    optionsBuilder.UseMySQL(ConnectionString);
            }
            else if (Provider.Contains("postgres"))
            {
                if (_connection != null)
                    optionsBuilder.UseNpgsql(_connection);
                else
                    optionsBuilder.UseNpgsql(ConnectionString);
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
                if (_connection != null)
                    optionsBuilder.UseSqlServer(_connection);
                else
                    optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("XXX DbContextBase Disposed" + Name + " " + RefId);
            base.Dispose();
        }
    }
}
