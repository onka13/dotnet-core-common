using CoreCommon.Data.EntityFrameworkBase.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreCommon.Data.EntityFrameworkBase.Components
{
    /// <summary>
    /// A simple Db Context
    /// </summary>
    public class InformationDbContext : EmptyDbContext
    {
        public virtual DbSet<InformationSchemaColumn> InformationSchemaColumns { get; set; }
        public virtual DbSet<InformationSchemaTable> InformationSchemaTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InformationSchemaTable>().HasNoKey();
            modelBuilder.Entity<InformationSchemaColumn>().HasNoKey();
        }

        public static InformationDbContext Init(string provider, string connectionString)
        {
            var context = new InformationDbContext();
            context.Provider = provider;
            context.ConnectionString = connectionString;
            return context;
        }
    }
}
