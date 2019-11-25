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

        public DbContextBase()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX DbContextBase Cons " + Name + " " + RefId);
        }

        /// <summary>
        /// Configure db context
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies(false);
            string provider = Configuration["AppSettings:" + Name + "Provider"];
            string connectionString = Configuration["ConnectionStrings:" + Name];

            if (provider != null && provider.Contains("mysql"))
            {
                if (_connection != null)
                    optionsBuilder.UseMySQL(_connection);
                else
                    optionsBuilder.UseMySQL(connectionString);
            }
            else if (provider != null && provider.Contains("postgresql"))
            {
                if (_connection != null)
                    optionsBuilder.UseNpgsql(_connection);
                else
                    optionsBuilder.UseNpgsql(connectionString);
            }
            else
            {
                if (_connection != null)
                    optionsBuilder.UseSqlServer(_connection);
                else
                    optionsBuilder.UseSqlServer(connectionString);
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
