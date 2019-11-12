using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace CoreCommon.Data.EntityFrameworkBase.Base
{
    public abstract class DbContextBase : DbContext
    {
        public abstract string Name { get; }
        public string RefId { get; set; }

        public IConfiguration Configuration { get; set; }
        private System.Data.Common.DbConnection _connection;

        public DbContextBase()
        {
            RefId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine("XXX DbContextBase Cons " + Name + " " + RefId);
        }

        //public DbContextBase(System.Data.Common.DbConnection connection)
        //{
        //    _connection = connection;
        //    System.Diagnostics.Debug.WriteLine("XXX DbContextBase conn " + Name);
        //}

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
